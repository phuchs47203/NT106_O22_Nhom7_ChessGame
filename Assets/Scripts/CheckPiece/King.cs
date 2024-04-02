using System.Collections.Generic;
using UnityEngine;

public class King : ChessPiece
{
    [HideInInspector] public bool hasMadeFirstMove = false;
    [HideInInspector] public bool isBeingChecked = false;

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

        for (int x = this.currX - 1; x <= this.currX + 1; x++)
        {
            for (int y = this.currY - 1; y <= this.currY + 1; y++)
            {
                if (x == this.currX && y == this.currY) continue;

                Vector2Int nextMove = new Vector2Int(x, y);

                if (this.Check_Outside_The_Board(nextMove)) continue;

                if (this.Check_Being_Blocked_By_TeamAt(nextMove)) continue;

                if (this.Check_Is_BlockedByOtherTeamAt(nextMove))
                {
                    this.capturableMoveList.Add(nextMove);
                    continue;
                }

                all_Possible_Move_List.Add(nextMove);
            }
        }
        return all_Possible_Move_List;
    }
}