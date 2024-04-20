using System.Data.Common;
using System.Collections.Generic;
using UnityEngine;

public class Pawn : ChessPiece
{
    [HideInInspector] public bool hasMadeFirstMove = false; // chưa di chuyển lần đầu
    [HideInInspector] public bool enPassant = false;

    protected override void Awake()
    {
        base.Awake();

        this.hasMadeFirstMove = false; // kiểm tra coi đó có phải lần di chueyern đàu tiên không, vì đàu tiên sẽ được đi 2 ô, nhuwgnx lần còn lại chỉ có 1 ô
    }

    public override void MoveTo(Vector2Int targetMove, bool force = false)
    {
        base.MoveTo(targetMove, force);

        if (force) return;

        if (!this.hasMadeFirstMove) this.hasMadeFirstMove = true; // nếu di chueyern lần đầu thì set lại để những lần sau biết là con này no chỉ đi được 1 ô thôi
    }

    protected override List<Vector2Int> GetAllPossibleMove()
    {
        List<Vector2Int> allPossibleMoveList = new List<Vector2Int>();

        int teamForward = this.team == Team.Blue ? 1 : -1; // đội xanh thì hướng di chueyenr elen, đội đỏ thì hướng di chueyenr xuống -y

        Vector2Int forward1, forward2, forwardLeft, forwardRight;
        if (!this.hasMadeFirstMove)
        {
            forward2 = new Vector2Int(this.currentX, this.currentY + 2 * teamForward); // phải naha teamfowrad vì ai beiets đó là quan của đỏ hay xanh
            if (!this.IsBeingBlockedAt(forward2))
                allPossibleMoveList.Add(forward2);
        }

        forward1 = new Vector2Int(this.currentX, this.currentY + 1 * teamForward); // tạo vector cho vị trí đi ellen 1 ô
        if (!this.IsBeingBlockedAt(forward1))
            allPossibleMoveList.Add(forward1);

        forwardLeft = new Vector2Int(this.currentX - 1, this.currentY + 1 * teamForward); // // tạo vector cho vị trí trái 1 ô
        if (this.IsInsideTheBoard(forwardLeft))
            if (this.IsBeingBlockedByOtherTeamAt(forwardLeft)) // nếu có quanad đội kahcs chặn ô này thì add vào danh sách có thể ăn
                this.capturableMoveList.Add(forwardLeft);

        forwardRight = new Vector2Int(this.currentX + 1, this.currentY + 1 * teamForward); // // tạo vector cho vị trí phải 1 ô
        if (this.IsInsideTheBoard(forwardRight))
            if (this.IsBeingBlockedByOtherTeamAt(forwardRight))
                this.capturableMoveList.Add(forwardRight);

        return allPossibleMoveList;
    }
    private void UpdateMoveList_Possibel()
    {
        ChessPiece[,] chess = ChessBoard.Singleton.chessPieces;
        foreach (Vector2Int validMove in this.capturableMoveList)
        {
            if (this.IsMoveValid(validMove))
            {
                continue;
            }
            currentX = validMove.x;
            currentY = validMove.y;
        }
    }
    private void Move_To_Position(Vector2Int trg, bool force = true)
    {
        this.currentX = trg.x;
        this.currentY = trg.y;

        Vector3 tile_Center = ChessBoard.Singleton.GetTileCenter(trg);

        if (force)
            this.transform.position = tile_Center;
    }



    private void Find_Move_Recursive(ref List<Vector2Int> lisstposible, ref List<Vector2Int> capturableMoveList, Vector2Int tesst, Vector2Int increament)
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
    private bool Check_Outside_Board(Vector2Int trg)
    {
        if (trg.x >= ChessBoardConfiguration.Singleton.TILE_COUNT_X || trg.x < 0
        || trg.y >= ChessBoardConfiguration.Singleton.TILE_COUNT_Y || trg.y < 0)
            return false;

        return true;
    }
    private bool Check_Inside_Boad(Vector2Int trg)
    {
        return !this.IsOutsideTheBoard(trg);
    }

    private bool Check_Being_Blocked(Vector2Int trg)
    {
        if (this.IsOutsideTheBoard(trg)) return true;

        return ChessBoard.Singleton.chessPieces[trg.x, trg.y].IsNotNull ? true : false;
    }

    private bool Cehck_Being_Blocked_By_TeamAt(Vector2Int trg)
    {
        if (this.IsOutsideTheBoard(trg)) return true;

        return ChessBoard.Singleton.chessPieces[trg.x, trg.y].IsNotNull ?
            (ChessBoard.Singleton.chessPieces[trg.x, trg.y].team == this.team ? false : true)
                : true;
    }
}