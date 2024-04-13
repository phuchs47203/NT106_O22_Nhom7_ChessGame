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