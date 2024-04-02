using System.Collections.Generic;
using UnityEngine;

public class Queen : ChessPiece
{
    // trả về các nước đi thuộc về quân Queen / nữ hoàng
    protected override List<Vector2Int> GetAllPossibleMove()
    {
        // khởi tạo một list iểu vctor các định vị trí phù hợp
        List<Vector2Int> all_Possible_Move_List = new List<Vector2Int>();

        int x = this.currX - 1;
        while (x <= this.currX + 1 )
        {
            int y = this.currY - 1;
            while ( y <= this.currY + 1)
            {
                if (x == this.currX && y == this.currY)
                    continue;

                Vector2Int next_Move = new Vector2Int(x, y);
                Vector2Int move_Dir = next_Move - new Vector2Int(this.currX, this.currY);

                this.Add_Move_Recursivelly(ref all_Possible_Move_List, ref this.capturableMoveList, next_Move, move_Dir);
                y++;
            }
            x++;
        }

        return all_Possible_Move_List;
    }
}