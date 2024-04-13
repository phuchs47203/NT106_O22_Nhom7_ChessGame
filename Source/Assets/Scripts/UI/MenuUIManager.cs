using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MenuUIManager : MonoBehaviour
{
    [SerializeField] private Animator menuUIAnim;
    [SerializeField] private TMP_InputField addressInput;

    public Server server;
    public Client client;

    public static MenuUIManager Singleton { get; private set; }

    private void Awake()
    {
        Singleton = this;

        this.registerEvents(true);
    }

    private void OnDestroy()
    {
        this.registerEvents(false);
    }

    private void onGameStart(Team team)
    {
        menuUIAnim.SetTrigger("InGameUI");
    }

    public void OnLocalGameBtnClicked()
    {
        Debug.Log("Local Game");
        menuUIAnim.SetTrigger("InGameUI"); // khi click vào thì chueyern sang giao diện InGameUI

        server.Init(8007);
        client.Init("127.0.0.1", 8007);
    }
    public void OnOnlineGameBtnClicked()
    {
        Debug.Log("Online Game");
        menuUIAnim.SetTrigger("UI-2"); // khi click vào thì chueyern sang giao diện UI2
    }
    public void OnHostBtnClicked()
    {
        Debug.Log("Host");
        menuUIAnim.SetTrigger("UI-3"); // khi click vào thì chueyern sang giao diện UI3

        server.Init(8007);
        client.Init("127.0.0.1", 8007);
    }
    public void OnConnectBtnClicked()
    {
        Debug.Log("Connect");

        client.Init(this.addressInput.text, 8007);
    }
    public void OnBackUI2BtnClicked()
    {
        Debug.Log("Back UI 2");
        menuUIAnim.SetTrigger("UI-1"); // khi click vào thì chueyern sang giao diện UI1
    }

    public void OnBackUI3BtnClicked()
    {
        Debug.Log("Back UI 3");
        menuUIAnim.SetTrigger("UI-2"); // // khi click vào thì chueyern sang giao diện UI2

        server.Shutdown(); // hủy 
        client.Shutdown(); // hủy
    }

    private void registerEvents(bool confirm)
    {
        if (confirm)
        {
            ChessBoard.Singleton.onGameStart += onGameStart;
        }
        else
        {
            ChessBoard.Singleton.onGameStart -= onGameStart;
        }
    }
}
