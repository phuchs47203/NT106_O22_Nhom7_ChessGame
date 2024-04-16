using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Networking.Transport;
using UnityEngine;
using UnityEngine.UI;
using static Unity.Burst.Intrinsics.X86.Avx;

//Ở trong In Game UI của unity (hyerarchy)
public class UIManager : MonoBehaviour
{
    public static UIManager Singleton { get; private set; }
    [SerializeField] private Image currentTurnUI; // hình ảnh hiển thị màu của lượt đi hiện tại
    [SerializeField] private GameObject endGameCanvasUI; // Khi mà có người chơi vào thì tạo một gameobject
    [SerializeField] private Toggle[] toggles; // cờ hiệu đẻ biết cá nhận sẵn sàng hay chưa
    [SerializeField] private Button resetButton; // nút hởi động lại trò chơi
    [SerializeField] private Material blueTeamMaterial; // list các material của team xanh
    [SerializeField] private Material redTeamMaterial; // list các material của team đỏ

    private void Awake()
    {
        Singleton = this; // gán instance của UIManager cho Singleton khi lớp được khởi tạo
    }

    private void Start()
    {
        this.registerEvents(true); // 
    }

    private void OnDestroy()
    {
        this.registerEvents(false); // sự kiện hủy được gọi, gọi hàm đươc viêt bên dưới, truyền đối số false và loại bỏ các thành pahafn đã thêm vào
    }

    private void OnTurnSwitched(Team turn)
    {
        this.currentTurnUI.color = turn == Team.Blue ? this.blueTeamMaterial.color : this.redTeamMaterial.color;
    } // sự kiện cho lượt đi, khi nhận thấy đổi lượt đi thì sẽ đổi màu cho button CCurrentTurn dó

    private void OnGameStateChanged(GameState state, Turn turn)
    {
        switch (state)
        {
            case GameState.Victory:
                this.OnGameVictoryState(turn);
                break;

            case GameState.Reset:
                this.OnGameResetState();
                break;
        }
    }

    private void OnGameVictoryState(Turn turn)
    {
        InputEventManager.Singleton.onSpacePressDown += OnSpaceButtonPressDown; // kích hoạt sự kiện khi người chơi nhấn nút space, nếu 2 bên đều nhấn thì chơi tiếp tục

        this.endGameCanvasUI.SetActive(true); // kích họat sự kiện kết thúc game, hiện UI
        this.endGameCanvasUI.transform.GetChild((int)turn)?.gameObject.SetActive(true);
    }

    private void OnGameResetState()
    {
        // lấy giao diện hiện tại, lấy con của nó và set là 0, nghĩa là ẩn nó, 
        this.endGameCanvasUI.transform.GetChild(0)?.gameObject.SetActive(false); // ẩm giao diện kết thúc, vì nó không cần, trò chơi sẽ hiện lại giao diện ban đầu
        this.endGameCanvasUI.transform.GetChild(1)?.gameObject.SetActive(false);
        this.endGameCanvasUI.SetActive(false);
    }

    public void OnSpaceButtonPressDown()
    {
        Debug.Log("Press");

        Client.Singleton.SendToServer(new NetReady(ChessBoard.Singleton.playerTeam)); // sự kiện chơi tiếp, thông báo ra cho máy chủ bắt sự keienj này
        // Xử lý khi người chơi nhấn phím Space, gửi thông điệp cho máy chủ để thông báo rằng người chơi đã sẵn sàng.
    }

    private void registerEvents(bool confirm)
    {
        // thêm mưới sự kiện hoạc bỏ id sự keienj đó
        // khi game bát đầu sẽ được gọi
        if (confirm)
        {
            ChessBoard.Singleton.onTurnSwitched += OnTurnSwitched;
            GameStateManager.Singleton.OnGameStateChanged += OnGameStateChanged;

            NetUtility.S_READY += onNetReadyServer;
            NetUtility.S_REMATCH += onNetRematchServer;

            NetUtility.C_READY += onNetReadyClient;
            NetUtility.C_REMATCH += onNetRematchClient;
        }
        else
        {
            ChessBoard.Singleton.onTurnSwitched -= OnTurnSwitched; // xử lí trnee bàn cờ, khi nhẫn vào quan cờ, sau đó không nhấn nữa
            GameStateManager.Singleton.OnGameStateChanged -= OnGameStateChanged; // xử lí trạng thái game, dể server beiets mf cập nahajt trnajg thái
            InputEventManager.Singleton.onSpacePressDown -= OnSpaceButtonPressDown; // xử lí hủy không chơi tiếp

            // loại bỏ các sự kiện eben dưới tương ứng khi thêm vào
            NetUtility.S_READY -= onNetReadyServer;
            NetUtility.S_REMATCH -= onNetRematchServer;

            NetUtility.C_READY -= onNetReadyClient;
            NetUtility.C_REMATCH -= onNetRematchClient;
        }
    }

    // Server
    // máy chủ nhận được một thông điệp từ một máy khách rằng người chơi đã sẵn sàng(thông điệp NetReady),
    // nó sẽ broadcast thông điệp này cho tất cả các máy khách khác.
    //==> đồng bộ hóa trạng thái của tất cả các máy khách trong trò chơi.
    private void onNetReadyServer(NetMessage netMessage, NetworkConnection sender)
    {
        NetReady netReady = netMessage as NetReady;

        Server.Singleton.BroadCast(netReady);
    }

    //máy chủ nhận được một thông điệp từ một máy khách yêu cầu chơi lại trò chơi(thông điệp NetRematch),
    //   nó sẽ gửi một thông điệp NetSwitchTeam cho máy khách gửi yêu cầu,
    //   sau đó broadcast thông điệp NetRematch cho tất cả các máy khách khác.

    //nếu cả 2 đồng ý chơi lại thì cái này mưới được gọi
    private void onNetRematchServer(NetMessage netMessage, NetworkConnection sender)
    {
        NetRematch netRematch = netMessage as NetRematch;

        Server.Singleton.SendToClient(sender, new NetSwitchTeam());
        Server.Singleton.BroadCast(netRematch); // gửi goi tin từ thitest bị này đến mọi thiết bị trong cùng mạng LAN
    }

    // Client
    private void onNetReadyClient(NetMessage netMessage)
    {
        NetReady netReady = netMessage as NetReady;

        //khi máy khách nhận được một thông điệp từ máy chủ rằng một người chơi đã sẵn sàng(thông điệp NetReady)
        //   nó sẽ cập nhật trạng thái của toggle tương ứng với đội của người chơi đó.
        this.toggles[(int)netReady.ReadyTeam].isOn = !this.toggles[(int)netReady.ReadyTeam].isOn;


        bool resetConfirm = true;
        //Nếu tất cả các toggle đã được chọn (các người chơi đã sẵn sàng)
        foreach (Toggle toggle in this.toggles)
        {
            if (!toggle.isOn) resetConfirm = false;
        }

        if (resetConfirm)
        {
            Client.Singleton.SendToServer(new NetRematch()); // gửi deense server yêu cầu chơi lại
        }
    }

    private void onNetRematchClient(NetMessage netMessage)
    {
        // Nhận thông điệp khởi động lại
        // cập nhật -> trnajg thái reset
        GameStateManager.Singleton.UpdateGameState(GameState.Reset, null);
        foreach (Toggle toggle in this.toggles)
        {
            toggle.isOn = false; // set lại các toggle là false
        }
        InputEventManager.Singleton.onSpacePressDown -= OnSpaceButtonPressDown;
    }
}
