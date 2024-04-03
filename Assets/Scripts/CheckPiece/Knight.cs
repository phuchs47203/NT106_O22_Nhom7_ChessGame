using System.Collections.Generic;
using UnityEngine;

public class Knight : ChessPiece
{
    protected override List<Vector2Int> GetAllPossibleMove()
    {
        List<Vector2Int> all_Possible_Move_List = new List<Vector2Int>();

        for (int x = this.currX - 2; x <= this.currX + 2; x++)
        {
            for (int y = this.currY - 2; y <= this.currY + 2; y++)
            {
                if (x == this.currX || y == this.currY)
                {
                    continue;
                }
                Vector2Int next_Move = new Vector2Int(x, y);
                Vector2Int move_Dir = next_Move - new Vector2Int(this.currX, this.currY);

                if (this.Check_Outside_The_Board(next_Move))
                {
                    continue;
                }
                if (this.Check_Being_Blocked_By_TeamAt(next_Move))
                {
                    continue;
                }

                if (move_Dir.x == move_Dir.y || move_Dir.x + move_Dir.y == 0)
                {
                    continue;
                }

                if (this.Check_Is_BlockedByOtherTeamAt(next_Move))
                {
                    this.capturableMoveList.Add(next_Move);
                    continue;
                }

                all_Possible_Move_List.Add(next_Move);
            }
        }

        return all_Possible_Move_List;
    }
}