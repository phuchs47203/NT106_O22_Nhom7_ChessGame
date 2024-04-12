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

        this.hasMadeFirstMove = false; // kiểm tra coi đo có phải lần di chueyern đàu tiên không, vì đàu tiên sẽ được đi 2 ô, nhuwgnx lần còn lại chỉ có 1 ô
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
}