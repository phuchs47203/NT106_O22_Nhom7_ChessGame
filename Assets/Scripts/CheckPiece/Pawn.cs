using System.Data.Common;
using System.Collections.Generic;
using UnityEngine;

public class Pawn : ChessPiece
{
    [HideInInspector] public bool hasMadeFirstMove = false;
    [HideInInspector] public bool enPassant = false;

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
        List<Vector2Int> all_Possible_Move_List = new List<Vector2Int>();

        int teamForward = this.team == Team.Blue ? 1 : -1;

        Vector2Int forward1, forward2, forwardLeft, forwardRight;
        if (!this.hasMadeFirstMove)
        {
            forward2 = new Vector2Int(this.currX, this.currY + 2 * teamForward);
            if (!this.Check_Being_Blocked_At(forward2))
                all_Possible_Move_List.Add(forward2);
        }

        forward1 = new Vector2Int(this.currX, this.currY + 1 * teamForward);
        if (!this.Check_Being_Blocked_At(forward1))
            all_Possible_Move_List.Add(forward1);

        forwardLeft = new Vector2Int(this.currX - 1, this.currY + 1 * teamForward);
        if (this.IsInsideTheBoard(forwardLeft))
            if (this.Check_Is_BlockedByOtherTeamAt(forwardLeft))
                this.capturableMoveList.Add(forwardLeft);

        forwardRight = new Vector2Int(this.currX + 1, this.currY + 1 * teamForward);
        if (this.IsInsideTheBoard(forwardRight))
            if (this.Check_Is_BlockedByOtherTeamAt(forwardRight))
                this.capturableMoveList.Add(forwardRight);

        return all_Possible_Move_List;
    }
}