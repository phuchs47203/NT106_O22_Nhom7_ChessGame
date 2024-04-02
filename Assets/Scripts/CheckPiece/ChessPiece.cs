using System.Data;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Chess_Piece_Type
{
    NullPiece = 0,
    Pawn = 1,
    Rook = 2,
    Knight = 3,
    Bishop = 4,
    Queen = 5,
    King = 6,
}

public abstract class ChessPiece : MonoBehaviour
{
    public Chess_Piece_Type piece_Type;
    public Team team;
    public int currX;
    public int currY;

    public bool Is_Selected 
    { 
        get;
        private set; 
    }
    public float yNormal;
    public float ySelected;

    [HideInInspector] public bool isBeingAttackedByBlue;
    [HideInInspector] public bool isBeingAttackedByRed;

    protected List<Vector2Int> validMoveList;
    protected List<Vector2Int> capturableMoveList;

    // For null checking
    public bool IsNull => this.piece_Type == Chess_Piece_Type.NullPiece ? true : false;
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
        if (this.Is_Selected)
            ChessBoard.Singleton.ShowMovableOf(this.validMoveList, this.capturableMoveList);
    }

    public void SelectClient(Vector2Int selectedPosition)
    {
        Client.Singleton.SendToServer(new NetPieceSelected(selectedPosition.x, selectedPosition.y));
    }

    public void SelectServer(Vector2Int selectedPosition)
    {
        Server.Singleton.BroadCast(new NetPieceSelected(selectedPosition.x, selectedPosition.y));
    }

    public void SetPieceSelect()
    {
        if (!this.Is_Selected)
        {
            this.Is_Selected = true;
            this.UpdateValidMoveList();
        }
        else
        {
            this.Is_Selected = false;
            ChessBoard.Singleton.ShowMovableOf(this.validMoveList, this.capturableMoveList, true);
        }

        this.transform.position = new Vector3(this.transform.position.x, this.Is_Selected ? this.ySelected : this.yNormal, this.transform.position.z);
    }

    public bool IsMoveValid(Vector2Int targetMove)
    {
        foreach (Vector2Int validMove in this.validMoveList)
        {
            if (validMove == targetMove) return true;
        }

        foreach (Vector2Int capturableMove in this.capturableMoveList)
        {
            if (capturableMove == targetMove) return true;
        }

        return false;
    }

    public void UpdateValidMoveList()
    {
        this.clearMoveLists();
        this.validMoveList = this.GetAllPossibleMove();

        ChessPiece[,] chessPieces = ChessBoard.Singleton.chessPieces;
        foreach (Vector2Int i in this.capturableMoveList)
        {
            chessPieces[i.x, i.y].Set_Is_Attacked(this.team, true);
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

    public void Set_Is_Attacked(Team attackerTeam, bool value)
    {
        if (attackerTeam == Team.Blue)
            isBeingAttackedByBlue = value;
        else
            isBeingAttackedByRed = value;
    }

    public virtual void MoveTo(Vector2Int targ_Move, bool force = false)
    {
        this.currX = targ_Move.x;
        this.currY = targ_Move.y;

        Vector3 tileCenter = ChessBoard.Singleton.GetTileCenter(targ_Move);

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
            this.transform.position = Vector3.Lerp(this.transform.position, targetPos, i / smoothTime);
            yield return null;
        }
    }

    protected virtual void Add_Move_Recursivelly(ref List<Vector2Int> allPossibleMoveList, ref List<Vector2Int> capturableMoveList, Vector2Int checkMove, Vector2Int increament)
    {
        if (this.Check_Outside_The_Board(checkMove))
        {
            return;
        }

        if (this.Check_Being_Blocked_By_TeamAt(checkMove))
        {
            return;
        }
        if (this.Check_Is_BlockedByOtherTeamAt(checkMove))
        {
            capturableMoveList.Add(checkMove);
            return;
        }

        allPossibleMoveList.Add(checkMove);

        this.Add_Move_Recursivelly(ref allPossibleMoveList, ref capturableMoveList, checkMove + increament, increament);
    }

    // For Checking Valid Position Logics
    protected bool Check_Outside_The_Board(Vector2Int targ_Move)
    {
        if (targ_Move.x >= ChessBoardConfiguration.Singleton.TILE_COUNT_X || targ_Move.x < 0
        || targ_Move.y >= ChessBoardConfiguration.Singleton.TILE_COUNT_Y || targ_Move.y < 0)
        {
            return true;
        }

        return false;
    }
    protected bool IsInsideTheBoard(Vector2Int targ_Move)
    {
        return !this.Check_Outside_The_Board(targ_Move);
    }
    protected virtual bool Check_Being_Blocked_At(Vector2Int targ_Move)
    {
        // If outside the board then count as being blocked
        if (this.Check_Outside_The_Board(targ_Move))
        {
            return true;
        }

        return ChessBoard.Singleton.chessPieces[targ_Move.x, targ_Move.y].IsNotNull ? true : false;
    }
    protected virtual bool Check_Being_Blocked_By_TeamAt(Vector2Int targ_Move)
    {
        if (this.Check_Outside_The_Board(targ_Move)) return true;

        return ChessBoard.Singleton.chessPieces[targ_Move.x, targ_Move.y].IsNotNull ?
            (ChessBoard.Singleton.chessPieces[targ_Move.x, targ_Move.y].team == this.team ? true : false)
                : false;
    }
    protected virtual bool Check_Is_BlockedByOtherTeamAt(Vector2Int targ_Move)
    {
        return ChessBoard.Singleton.chessPieces[targ_Move.x, targ_Move.y].IsNotNull ?
            (ChessBoard.Singleton.chessPieces[targ_Move.x, targ_Move.y].team != this.team ? true : false)
                : false;
    }
}
