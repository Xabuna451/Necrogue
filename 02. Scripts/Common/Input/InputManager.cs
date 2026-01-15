using UnityEngine;

public class InputManager : MonoBehaviour
{

    [Header("현재 입력 (읽기 전용)")]
    public Vector2 Move { get; private set; }
    public KeyCode confirmKey = KeyCode.Z;
    public bool confirm { get; private set; }

    public bool MovePressed { get; private set; }       // H & V
    public float H { get; private set; }                // Horizontal Move
    public float V { get; private set; }                // Vertical Move

    public bool AnyKey { get; private set; }            // 어떤 키라도 눌렸는지 확인

    public bool RightClick { get; private set; }         // 우클릭 눌림 (GetMouseButtonDown)
    public bool RightClickHeld { get; private set; }    // 우클릭 유지

    public bool ESC { get; private set; }                // ESC키 (옵션창)

    void Update()
    {
        H = Input.GetAxisRaw("Horizontal");  // A/D + Left/RightArrow
        V = Input.GetAxisRaw("Vertical");    // W/S + Up/DownArrow

        // 대각선 보정 (normalized 적용)
        Move = new Vector2(H, V).normalized;

        confirm = Input.GetKeyDown(confirmKey);
        AnyKey = Input.anyKey;

        MovePressed = (H != 0f) || (V != 0f);

        RightClick = Input.GetMouseButtonDown(1);        // 눌린 순간
        RightClickHeld = Input.GetMouseButton(1);        // 유지 중

        ESC = Input.GetKeyDown(KeyCode.Escape);
    }
}
