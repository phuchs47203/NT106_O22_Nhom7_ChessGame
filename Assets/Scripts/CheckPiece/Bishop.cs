using System.Collections.Generic;
using UnityEngine;

public class Bishop : ChessPiece
{
    protected override List<Vector2Int> GetAllPossibleMove()
    {
        List<Vector2Int> all_Possible_Move_List = new List<Vector2Int>();

        for (int x = this.currX - 1; x <= this.currX + 1; x++)
        {
            for (int y = this.currY - 1; y <= this.currY + 1; y++)
            {
                if (x == this.currX && y == this.currY) continue;

                Vector2Int next_Move = new Vector2Int(x, y);
                Vector2Int move_Dir = next_Move - new Vector2Int(this.currX, this.currY);

                if (move_Dir.x + move_Dir.y == 0 || move_Dir.x == move_Dir.y)
                    this.Add_Move_Recursivelly(ref all_Possible_Move_List, ref this.capturableMoveList, next_Move, move_Dir);
            }
        }

        return all_Possible_Move_List;
    }
}