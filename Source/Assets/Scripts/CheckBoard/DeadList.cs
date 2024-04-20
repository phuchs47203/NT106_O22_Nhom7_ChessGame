using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeadList : MonoBehaviour
{
    private int smoothTime;
    private List<ChessPiece> blueDeadList; // danh sách die của team blue
    private List<ChessPiece> redDeadList; // dah sách die của teme red

    private Vector3 blueDeadListPosition; // vị trí die quân blue
    private Vector3 redDeadListPosition; // vị trí die

    private float deadTileSize;
    private float deadSizeMultiplier = 0.6f;

    //hướng di chuyển của quân cờ của 2 đội trên bàn cờ.
    private Vector3 bluePieceForward;
    private Vector3 redPieceForward;

    private void Start()
    {
        this.smoothTime = ChessBoardConfiguration.Singleton.smoothTime;
        GameStateManager.Singleton.OnGameStateChanged += OnGameStateChanged;
    }

    private void OnDestroy()
    {
        GameStateManager.Singleton.OnGameStateChanged -= OnGameStateChanged;
    }

    private void OnGameStateChanged(GameState state, Turn turn)
    {
        if (state == GameState.Reset)
            this.HandleResetState();
    }

    private void HandleResetState()
    {
        this.ResetDeadList(ref this.blueDeadList);
        this.ResetDeadList(ref this.redDeadList);
    }

    private void ResetDeadList(ref List<ChessPiece> deadList)
    {
        deadList = new List<ChessPiece>();
    }

    public void SetupDeadList(Vector3 blueDeadListPosition, Vector3 redDeadListPosition, float tileSize, Vector3 chessBoardForward)
    {
        this.blueDeadList = new List<ChessPiece>();
        this.redDeadList = new List<ChessPiece>();

        this.blueDeadListPosition = blueDeadListPosition; // vị trí đã chết
        this.redDeadListPosition = redDeadListPosition;
        this.deadTileSize = tileSize * this.deadSizeMultiplier;
        this.bluePieceForward = chessBoardForward; // huogns di chueyern cảu quân cờ
        this.redPieceForward = -this.bluePieceForward;
    }

    // thêm mới 1 quân cờ vauwf chết vào list
    public void AddPieceToDeadList(ChessPiece deadPiece)
    {
        if (deadPiece.team == Team.Blue)
        {
            this.blueDeadList.Add(deadPiece);
        }
        else
        {
            this.redDeadList.Add(deadPiece);
        }

        this.UpdateDeadListPosition(deadPiece); // cập nhật cập nahjat ví trí,chuyể nó tới một nơi khác
    }

    private void UpdateDeadListPosition(ChessPiece deadPiece)
    {
        deadPiece.transform.localScale = deadPiece.transform.localScale * this.deadSizeMultiplier;

        // tìm vị trí mục tiêu 
        //kích thước của ô trên bàn cờ cho quân cờ đã chết, được nhân với số lượng quân cờ đã chết của mỗi đội để tính toán khoảng cách di chuyển.
        Vector3 targetPos = ((deadPiece.team == Team.Blue) ?
            this.blueDeadListPosition + this.bluePieceForward * (this.blueDeadList.Count - 1) * this.deadTileSize :
            this.redDeadListPosition + this.redPieceForward * (this.redDeadList.Count - 1) * this.deadTileSize);

        StartCoroutine(this.SmoothUpdateDeadListPosition(deadPiece, targetPos)); // gọi hàm để di chuyển
    }

    private IEnumerator SmoothUpdateDeadListPosition(ChessPiece deadPiece, Vector3 targetPos)
    {
        for (float i = 0; i <= this.smoothTime; i++)
        {
            deadPiece.transform.position = Vector3.Lerp(deadPiece.transform.position, targetPos, i / this.smoothTime);
            yield return null;
        }
    }
}
