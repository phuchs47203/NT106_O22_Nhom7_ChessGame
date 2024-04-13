using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Những cái này được cấu hình cho 3 camera
public class CamController : MonoBehaviour
{
    public static CamController Singleton { get; private set; }
    [SerializeField] private GameObject[] cameras;
    private void Awake()
    {
        Singleton = this;
    }

    private void Start()
    {
        this.registerEvents(true); // gọi hàm đăng kí sự kiện khi thành pahanf này bắt đâu
    }

    private void OnDestroy()
    {
        this.registerEvents(false); // gọi hàm hủy sự kiện khi thành pahanf này kt
    }

    private void registerEvents(bool confirm)
    {
        if (confirm)
        {
            ChessBoard.Singleton.onGameStart += onGameStart; // add hàm onGameStart
            NetUtility.C_VICTORY_CLAIM += onVictoryClaimClient;
            NetUtility.C_REMATCH += onNetRematchClient;
        }
        else
        {
            ChessBoard.Singleton.onGameStart -= onGameStart;
            NetUtility.C_VICTORY_CLAIM -= onVictoryClaimClient;
            NetUtility.C_REMATCH -= onNetRematchClient;
        }
    }

    private void onNetRematchClient(NetMessage netMessage)
    {
        this.onGameStart(ChessBoard.Singleton.playerTeam);
    }

    private void onGameStart(Team team)
    {
        foreach (GameObject camera in cameras)
        {
            camera.SetActive(false);
        }

        cameras[(int)team + 1].SetActive(true); // kích haojt cacmera, để nhìn trong giao diện 2D, team 1 là camera 2, team2 là camera 3
    }

    private void onVictoryClaimClient(NetMessage obj)
    {
        foreach (GameObject camera in cameras)
        {
            camera.SetActive(false);
        }

        cameras[0].SetActive(true); // ngừng kích haotj mọi câera kích hoạt camera hiện vào màn hình chiến thắng
    }
}
