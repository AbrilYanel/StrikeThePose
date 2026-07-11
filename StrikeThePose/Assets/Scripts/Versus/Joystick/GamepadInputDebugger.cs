using UnityEngine;


public class GamepadInputDebugger : MonoBehaviour
{
    [Range(1, 8)]
    [SerializeField] private int joystickNumber = 1;
    [SerializeField] private string dPadHorizontalAxis = "P2_DPadHorizontal";
    [SerializeField] private bool logDPadAxis = true;

    private float _lastAxisValue;

    private void Start()
    {
        string[] joystickNames = Input.GetJoystickNames();

        if (joystickNames.Length == 0)
        {
            Debug.LogWarning("[GamepadDebugger] Unity no detectó ningún joystick.");
            return;
        }

        for (int i = 0; i < joystickNames.Length; i++)
        {
            Debug.Log(
                $"[GamepadDebugger] Joystick {i + 1}: '{joystickNames[i]}'"
            );
        }
    }

    private void Update()
    {
        for (int button = 0; button < 20; button++)
        {
            KeyCode keyCode = GetJoystickButtonKeyCode(button);

            if (Input.GetKeyDown(keyCode))
            {
                Debug.Log(
                    $"[GamepadDebugger] Joystick {joystickNumber} - Button {button}"
                );
            }
        }

        if (!logDPadAxis || string.IsNullOrWhiteSpace(dPadHorizontalAxis))
            return;

        float axisValue = Input.GetAxisRaw(dPadHorizontalAxis);

        if (Mathf.Abs(axisValue - _lastAxisValue) >= 0.05f)
        {
            Debug.Log(
                $"[GamepadDebugger] {dPadHorizontalAxis}: {axisValue:F2}"
            );
            _lastAxisValue = axisValue;
        }
    }

    private KeyCode GetJoystickButtonKeyCode(int buttonNumber)
    {
        int safeJoystick = Mathf.Clamp(joystickNumber, 1, 8);
        int safeButton = Mathf.Clamp(buttonNumber, 0, 19);

        int keyCodeValue =
            (int)KeyCode.Joystick1Button0 +
            ((safeJoystick - 1) * 20) +
            safeButton;

        return (KeyCode)keyCodeValue;
    }
}
