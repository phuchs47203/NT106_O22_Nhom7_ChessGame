using System;
using UnityEngine;

public class InputEventManager : MonoBehaviour
{
    public static InputEventManager Singleton { get; private set; }
    private void Awake()
    {
        Singleton = this;
    }

    // quản lí các sự kiện chuột trên bàn cờ
    public event Action onLeftMouseButtonDown;
    public event Action onLeftMouseButtonUp; // thả/nhả chuột
    public event Action onLeftMouseButtonHold;
    public event Action onRightMouseButtonDown; // nhấn chuột
    public event Action onRightMouseButtonUp;
    public event Action onRightMouseButtonHold;

    // Keyboard
    public event Action onSpacePressDown;
    public event Action onSpacePressUp;

    private void Update()
    {
        //khi nút chuột trái được nhấn xuống(Input.GetMouseButtonDown(0)),
        //sự kiện onLeftMouseButtonDown sẽ được kích hoạt bằng cách gọi Invoke(), hàm đó được thực hiện bên chesboard
        if (Input.GetMouseButtonDown(0)) // chuột trái
            this.onLeftMouseButtonDown?.Invoke();

        if (Input.GetMouseButtonUp(0))
            this.onLeftMouseButtonUp?.Invoke();

        if (Input.GetMouseButton(0))
            this.onLeftMouseButtonHold?.Invoke();

        if (Input.GetMouseButtonDown(1))
            this.onRightMouseButtonDown?.Invoke();

        if (Input.GetMouseButtonUp(1)) // chuột phải
            this.onRightMouseButtonUp?.Invoke();

        if (Input.GetMouseButton(1))
            this.onRightMouseButtonHold?.Invoke();

        // sự kiện thứ 2 dược triển khai
        if (Input.GetKeyDown(KeyCode.Space))
        {
            this.onSpacePressDown?.Invoke();
        }

        if (Input.GetKeyUp(KeyCode.Space))
        {
            this.onSpacePressUp?.Invoke();
        }
    }
}