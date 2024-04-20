using System.Collections.Generic;
using UnityEngine;

public class Rook : ChessPiece
{
    [HideInInspector] public bool hasMadeFirstMove = false;

    protected override void Awake()
    {
        base.Awake();

        this.hasMadeFirstMove = false;
    }

    public override void MoveTo(Vector2Int targetMove, bool force = false)
    {
        base.MoveTo(targetMove, force);

        if (force) return;

        if (!this.hasMadeFirstMove) this.hasMadeFirstMove = true;
    }

    protected override List<Vector2Int> GetAllPossibleMove()
    {
        List<Vector2Int> allPossibleMoveList = new List<Vector2Int>();

        for (int x = this.currentX - 1; x <= this.currentX + 1; x++)
        {
            for (int y = this.currentY - 1; y <= this.currentY + 1; y++)
            {
                if (x == this.currentX && y == this.currentY) continue; // đảm bảo không trùng current

                Vector2Int nextMove = new Vector2Int(x, y);
                Vector2Int moveDir = nextMove - new Vector2Int(this.currentX, this.currentY);

                if (moveDir.x == moveDir.y || moveDir.x + moveDir.y == 0) continue; // đảm bảo con xe không đi xéo

                this.AddedMoveRecursivelly(ref allPossibleMoveList, ref this.capturableMoveList, nextMove, moveDir);
            }
        }

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