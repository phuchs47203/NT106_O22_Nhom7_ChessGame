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
        List<Vector2Int> all_Possible_Move_List = new List<Vector2Int>();

        for (int x = this.currX - 1; x <= this.currX + 1; x++)
        {
            for (int y = this.currY - 1; y <= this.currY + 1; y++)
            {
                if (x == this.currX && y == this.currY) 
                {
                    continue;
                }
                Vector2Int nextMove = new Vector2Int(x, y);
                Vector2Int moveDir = nextMove - new Vector2Int(this.currX, this.currY);

                if (moveDir.x == moveDir.y || moveDir.x + moveDir.y == 0)
                {
                    continue;
                } 

                this.Add_Move_Recursivelly(ref all_Possible_Move_List, ref this.capturableMoveList, nextMove, moveDir);
            }
        }

        return all_Possible_Move_List;
    }
}