using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameState
{
    Play,
    Victory,
    Reset
}

public enum Turn
{
    Player,
    Other
}

//thuộc volumn GameStateManager
public class GameStateManager : MonoBehaviour
{
    public static GameStateManager Singleton { get; private set; }

    public event Action<GameState, Turn> OnGameStateChanged; // sự kiện trạng thái game thay đổi
    public event Action<Turn> OnSwitchTurn; // sự kiện chuyển lượt đi
    private GameState currentState; // lưu trữ trạng thái hiện tại là đang chơi, thắng, hay là reset
    private Turn currentTurn; // lưu trữ lượt đi hiện tịa của một client là của chính họ hay là người chơi khác


    private void Awake()
    {
        Singleton = this;
    }

    private void Start()
    {
        this.UpdateGameState(GameState.Play, Turn.Player);
    }

    // update lượt chơi, đối số là lượt chơi player hay other
    private void UpdateCurrentTurn(Turn turn)
    {
        this.currentTurn = turn;

        this.OnSwitchTurn?.Invoke(this.currentTurn);
    }

    // hàm cập nhật trạng thái của game liên tục, sẽ được gộ mỗi khi có sự kiện
    public void UpdateGameState(GameState nextState, Turn? turn)
    {
        this.currentState = nextState; //

        if (turn != null)
            this.UpdateCurrentTurn((Turn)turn); // nếu lượt chơi khác null thì gọi cập nhật

        switch (this.currentState)
        {
            case GameState.Play:
                this.HandlePlayingState();
                break; // nếu play thì gọi hàm xử lí play

            case GameState.Victory:
                this.HandleWinningState();
                break; // nếu thnawgs thì gọi hàm xử lí thắng

            case GameState.Reset:
                this.HandleResetState();
                break; // nếu rết thì rseset game
        }

        this.OnGameStateChanged?.Invoke(this.currentState, this.currentTurn); // giúp thông bóa là trạng thái được cập nhật cho ác thành phần khác, thông báo trnanjg thái tò chơi, thông báo lượt đi thuộc về bên nào
    }

    private void HandlePlayingState()
    {
        Debug.Log("Playing");
    }

    private void HandleWinningState()
    {
        Debug.Log("Victory");
    }

    private void HandleResetState()
    {
        Debug.Log("Reset");
    }
}
