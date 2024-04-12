using System.Collections.Generic;
using UnityEngine;

public class Bishop : ChessPiece
{
    // trienr khai hàm đó từ lớp cha
    protected override List<Vector2Int> GetAllPossibleMove()
    {
        List<Vector2Int> allPossibleMoveList = new List<Vector2Int>();

        // lặp qua 6 cái ô xung quanh, xau đo tơi ô tiếp theo thì lặp tiếp
        for (int x = this.currentX - 1; x <= this.currentX + 1; x++)
        {
            for (int y = this.currentY - 1; y <= this.currentY + 1; y++)
            {
                if (x == this.currentX && y == this.currentY) continue; // bishop không có đi học hoặc ngang nên sẽ bỏ qua nhwungx x y trung với current

                Vector2Int nextMove = new Vector2Int(x, y);
                Vector2Int moveDir = nextMove - new Vector2Int(this.currentX, this.currentY); // tính lại khaongr cách giữa vị trí tiếp theo và hiện tại đẻ truyền vào hàm, 

                // xét các nước đi chéo tiến, chéo lùi
                if (moveDir.x == moveDir.y || moveDir.x + moveDir.y == 0)
                    this.AddedMoveRecursivelly(ref allPossibleMoveList, ref this.capturableMoveList, nextMove, moveDir); // lặp qua hàm đẹ quy bên kia để tìm tiếp
            }
        }

        return allPossibleMoveList;
    }
}