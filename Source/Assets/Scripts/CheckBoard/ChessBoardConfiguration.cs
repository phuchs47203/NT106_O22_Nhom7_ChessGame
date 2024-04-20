using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChessBoardConfiguration : MonoBehaviour
{
    //Singleton
    public static ChessBoardConfiguration Singleton { get; private set; }

    [HideInInspector] public readonly int TILE_COUNT_X = 8; // 8 dòng
    [HideInInspector] public readonly int TILE_COUNT_Y = 8; // 8 cột
    public int smoothTime = 35; // 35 frame
    public float tileSize = 1f; // kích thước 1 ô trong bàn cờ
    public float yOffset = 0.525f; //Độ lệch theo trục y của mỗi ô trên bàn cờ, được sử dụng để điều chỉnh độ cao của quân cờ so với mặt bàn cờ khi nó được nahacs lên.

    private void Awake()
    {
        Singleton = this;
    }
}
