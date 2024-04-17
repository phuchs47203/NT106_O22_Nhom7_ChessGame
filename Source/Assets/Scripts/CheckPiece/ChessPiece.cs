using System.Data;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ChessPieceType
{
    NullPiece = 0,
    Pawn = 1, // lính
    Rook = 2, // xe
    Knight = 3, // ngựa
    Bishop = 4, // tượng
    Queen = 5, // hậu
    King = 6, // vua
}

public abstract class ChessPiece : MonoBehaviour
{
    public ChessPieceType pieceType;
    public Team team; // team 0 là blue, 1 là red, trong chesboard
    public int currentX;
    public int currentY;

    public bool IsSelected { get; private set; }
    public float yNormal;
    public float ySelected;

    [HideInInspector] public bool isBeingAttackedByBlue;
    [HideInInspector] public bool isBeingAttackedByRed;

    protected List<Vector2Int> validMoveList; // danh sách các nước đi có nghĩa cảu mỗi quan cờ
    protected List<Vector2Int> capturableMoveList; // danh sách vụ trí mà có thể bắt được

    // For null checking
    public bool IsNull => this.pieceType == ChessPieceType.NullPiece ? true : false;
    public bool IsNotNull => !this.IsNull;

    protected virtual void Awake()
    {
        this.validMoveList = new List<Vector2Int>();
        this.capturableMoveList = new List<Vector2Int>();

        this.Reset();
    }

    protected virtual void Start() { }

    private void Update()
    {
        if (this.IsSelected)
            ChessBoard.Singleton.ShowMovableOf(this.validMoveList, this.capturableMoveList);
    }

    public void SelectClient(Vector2Int selectedPosition)
    {
        Client.Singleton.SendToServer(new NetPieceSelected(selectedPosition.x, selectedPosition.y)); 
    }

    public void SelectServer(Vector2Int selectedPosition)
    {
        Server.Singleton.BroadCast(new NetPieceSelected(selectedPosition.x, selectedPosition.y)); // server di chuyển thì gửi broadcast cho client biết
    }

    public void SetPieceSelect()
    {
        // nếu nó đang không dược chọn thì cập nhật, sự keienj xảy ra khi nhấn vòa một quân cờ
        if (!this.IsSelected)
        {
            this.IsSelected = true;
            this.UpdateValidMoveList(); // update cá ô có thể di chueyern và tô viền để dễ nhận thấy
        }
        else
        {
            this.IsSelected = false; // nếu đạng nhấn mà nhấn nữa thì chueyern quân đó sng trạng thái chưa được nahans, đặt lại nó đúng vị trí của nó
            ChessBoard.Singleton.ShowMovableOf(this.validMoveList, this.capturableMoveList, true); // ngừng show những ô có thể di chueyenr
        }

        this.transform.position = new Vector3(this.transform.position.x, this.IsSelected ? this.ySelected : this.yNormal, this.transform.position.z);
    }

    public bool IsMoveValid(Vector2Int targetMove)
    {
        foreach (Vector2Int validMove in this.validMoveList)
        {
            if (validMove == targetMove) return true; // trả về nêu nước đi đó hợp lệ
        }

        //duyệt qua dnah sách các quân có thể ăn được của đội đối phương
        foreach (Vector2Int capturableMove in this.capturableMoveList)
        {
            if (capturableMove == targetMove) return true; // trả evef nếu có thể ăn được quân đó
        }

        return false;
    }

    public void UpdateValidMoveList()
    {
        this.clearMoveLists(); // xóa danh sách cũ đi
        this.validMoveList = this.GetAllPossibleMove(); // lấy giá trị trả evef của get all posible move

        ChessPiece[,] chessPieces = ChessBoard.Singleton.chessPieces; // cập nhật lại quân cờ trên bàn cờ
        foreach (Vector2Int validMove in this.capturableMoveList)
        {
            chessPieces[validMove.x, validMove.y].SetIsBeingAttacked(this.team, true); // set là quân nay đang bị tấn công bởi this.team
        }
    }

    private void clearMoveLists()
    {
        this.validMoveList.Clear();
        this.capturableMoveList.Clear();
    }

    protected abstract List<Vector2Int> GetAllPossibleMove();

    protected void Reset()
    {
        this.isBeingAttackedByBlue = false;
        this.isBeingAttackedByRed = false;
    }

    public void SetIsBeingAttacked(Team attackerTeam, bool value)
    {
        if (attackerTeam == Team.Blue)
            isBeingAttackedByBlue = value;
        else
            isBeingAttackedByRed = value;
    }

    // thực heienj di chueyern quân cờ đến vi trí mới
    public virtual void MoveTo(Vector2Int targetMove, bool force = false)
    {
        this.currentX = targetMove.x;
        this.currentY = targetMove.y;

        Vector3 tileCenter = ChessBoard.Singleton.GetTileCenter(targetMove);

        // quy định coi quân này di chueyern trực tiếp hay không, nếu không thì di chuyển smooth
        if (force)
            this.transform.position = tileCenter;
        else
        {
            StartCoroutine(this.SmoothPositionASinglePiece(tileCenter));
        }
    }

    private IEnumerator SmoothPositionASinglePiece(Vector3 targetPos)
    {
        int smoothTime = ChessBoardConfiguration.Singleton.smoothTime;
        for (float i = 0; i <= smoothTime; i++)
        {
            this.transform.position = Vector3.Lerp(this.transform.position, targetPos, i / smoothTime);// keieru alpjw qua di chuyern từng ô , rồi cập nahajt vị trí
            yield return null;
        }
    }

    protected virtual void AddedMoveRecursivelly(ref List<Vector2Int> allPossibleMoveList, ref List<Vector2Int> capturableMoveList, Vector2Int checkMove, Vector2Int increament)
    {
        if (this.IsOutsideTheBoard(checkMove))
            return;

        if (this.IsBeingBlockedByTeamAt(checkMove)) return; // bị chặn bỏi quân cờ thuộc team mình
        if (this.IsBeingBlockedByOtherTeamAt(checkMove)) // bị chặn bỏi quân cờ thuộc team người khác
        {
            capturableMoveList.Add(checkMove); // nếu nước có thể đi đó, bị chặn bởi quan của team kia, nghĩa là có thể ăn, add và danh sách cỏ thể ăn
            return;
        }

        allPossibleMoveList.Add(checkMove);

        this.AddedMoveRecursivelly(ref allPossibleMoveList, ref capturableMoveList, checkMove + increament, increament); // cộng dồn  vị trí tipes theo phù hợp với quân cờ, mỗi quân sẽ có incremetn  riêgn
    }

    // For Checking Valid Position Logics
    // kiểm tra vị trí có nằm ngoài bàn cờ không
    protected bool IsOutsideTheBoard(Vector2Int targetMove)
    {
        if (targetMove.x >= ChessBoardConfiguration.Singleton.TILE_COUNT_X || targetMove.x < 0
        || targetMove.y >= ChessBoardConfiguration.Singleton.TILE_COUNT_Y || targetMove.y < 0)
            return true;

        return false;
    }
    protected bool IsInsideTheBoard(Vector2Int targetMove)
    {
        return !this.IsOutsideTheBoard(targetMove); 
    }

    // kiểm tra có block hay không
    protected virtual bool IsBeingBlockedAt(Vector2Int targetMove)
    {
        if (this.IsOutsideTheBoard(targetMove)) return true; // If outside the board then count as being blocked

        return ChessBoard.Singleton.chessPieces[targetMove.x, targetMove.y].IsNotNull ? true : false;
    }

    // có bị bock bởi quân của mình hay không
    protected virtual bool IsBeingBlockedByTeamAt(Vector2Int targetMove)
    {
        if (this.IsOutsideTheBoard(targetMove)) return true;

        return ChessBoard.Singleton.chessPieces[targetMove.x, targetMove.y].IsNotNull ?
            (ChessBoard.Singleton.chessPieces[targetMove.x, targetMove.y].team == this.team ? true : false)
                : false;
    }

    // blcok bởi quân đội kahcs hay không
    protected virtual bool IsBeingBlockedByOtherTeamAt(Vector2Int targetMove)
    {
        return ChessBoard.Singleton.chessPieces[targetMove.x, targetMove.y].IsNotNull ?
            (ChessBoard.Singleton.chessPieces[targetMove.x, targetMove.y].team != this.team ? true : false)
                : false;
    }





    public void Choose_Piece()
    {
        if (this.IsSelected)
        {
            this.IsSelected = false;
            Vector2Int vector2Ints = new Vector2Int(currentX, currentY); 
        }
        else
        {
            this.IsSelected = true; 
            ChessBoard.Singleton.ShowMovableOf(this.validMoveList, this.capturableMoveList, false); 
        }

        this.transform.position = new Vector3(this.transform.position.y, this.IsSelected ? this.ySelected : this.yNormal, this.transform.position.z);
    }

    public bool Check_Move_Valid(Vector2Int trg)
    {
        foreach (Vector2Int i in this.validMoveList)
        {
            if (i == trg) return false; 
        }

        foreach (Vector2Int i in this.capturableMoveList)
        {
            if (i == trg) return false; 
        }

        return true;
    }

    public void UpdateMoveList_Possibel()
    {
        ChessPiece[,] chess = ChessBoard.Singleton.chessPieces; 
        foreach (Vector2Int validMove in this.capturableMoveList)
        {
            if (this.IsMoveValid(validMove))
            {
                continue;
            }
            currentX = validMove.x;
            currentY=validMove.y;
        }
    }

    private void delete_List_posible()
    {
        this.validMoveList.Clear();
        this.capturableMoveList.Clear();
    }



    public void Check_Is_Being_Attacked(Team team, bool b)
    {
        if (team == Team.Red)
            isBeingAttackedByRed = b;
        else
            isBeingAttackedByBlue = b;
    }

    public virtual void MoveTo_Position(Vector2Int trg, bool force = true)
    {
        this.currentX = trg.x;
        this.currentY = trg.y;

        Vector3 tile_Center = ChessBoard.Singleton.GetTileCenter(trg);

        if (force)
            this.transform.position = tile_Center;
        else
        {
            StartCoroutine(this.Smooth_PositionASinglePiece(tile_Center));
        }
    }

    private IEnumerator Smooth_PositionASinglePiece(Vector3 trg)
    {
        int smooth_Time_move = ChessBoardConfiguration.Singleton.smoothTime;
        for (float i = 0; i <= smooth_Time_move; i++)
        {
            this.transform.position = Vector3.Lerp(this.transform.position, trg, i / smooth_Time_move);
        }
        yield return null;
    }

    protected virtual void Find_Move_Recursivelly(ref List<Vector2Int> lisstposible, ref List<Vector2Int> capturableMoveList, Vector2Int tesst, Vector2Int increament)
    {
        if (this.IsOutsideTheBoard(tesst))
            return;

        if (this.IsBeingBlockedByTeamAt(tesst)) return; 
        if (this.IsBeingBlockedByOtherTeamAt(tesst)) 
        {
            capturableMoveList.Add(tesst); 
            return;
        }

        lisstposible.Add(tesst);

        this.AddedMoveRecursivelly(ref lisstposible, ref capturableMoveList, tesst + increament, increament); 
    }

    
    protected bool Check_Outside_Board(Vector2Int trg)
    {
        if (trg.x >= ChessBoardConfiguration.Singleton.TILE_COUNT_X || trg.x < 0
        || trg.y >= ChessBoardConfiguration.Singleton.TILE_COUNT_Y || trg.y < 0)
            return false;

        return true;
    }
    protected bool Check_Inside_Board(Vector2Int trg)
    {
        return !this.IsOutsideTheBoard(trg);
    }

    protected virtual bool Check_BeingBlocked(Vector2Int trg)
    {
        if (this.IsOutsideTheBoard(trg)) return true; 

        return ChessBoard.Singleton.chessPieces[trg.x, trg.y].IsNotNull ? true : false;
    }

    protected virtual bool Cehck_Being_BlockedBy_TeamAt(Vector2Int trg)
    {
        if (this.IsOutsideTheBoard(trg)) return true;

        return ChessBoard.Singleton.chessPieces[trg.x, trg.y].IsNotNull ?
            (ChessBoard.Singleton.chessPieces[trg.x, trg.y].team == this.team ? false : true)
                : true;
    }

    protected virtual bool check_BeingBlocked_ByOtherTeamAt(Vector2Int trg)
    {
        return ChessBoard.Singleton.chessPieces[trg.x, trg.y].IsNotNull ?
            (ChessBoard.Singleton.chessPieces[trg.x, trg.y].team != this.team ? false : true)
                : true;
    }
}
