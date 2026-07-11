using UnityEngine;

public class GamepadInputSource : MonoBehaviour, IPlayerInputSource
{
    public enum DPadMode
    {
        Axis,
        Buttons
    }

    [Header("Joystick")]
    [Tooltip("Número del joystick en el Input Manager clásico (1 a 8).")]
    [Range(1, 8)]
    [SerializeField] private int joystickNumber = 1;

    [Header("Movimiento: L1 / R1")]
    [Tooltip("Número de botón de L1.")]
    [Range(0, 19)]
    [SerializeField] private int moveLeftButton = 4;
    [Tooltip("Número de botón de R1.")]
    [Range(0, 19)]
    [SerializeField] private int moveRightButton = 5;

    [Header("Poses por botón")]
    [Tooltip("Pose A: Triángulo.")]
    [Range(0, 19)]
    [SerializeField] private int poseAButton = 3;
    [Tooltip("Pose C: Equis/Cruz.")]
    [Range(0, 19)]
    [SerializeField] private int poseCButton = 0;

    [Header("Poses B/D: D-Pad")]
    [SerializeField] private DPadMode dPadMode = DPadMode.Axis;

    [Tooltip("Nombre exacto del eje creado en Project Settings > Input Manager.")]
    [SerializeField] private string dPadHorizontalAxis = "P2_DPadHorizontal";
    [Range(0.1f, 0.95f)]
    [SerializeField] private float dPadDeadZone = 0.5f;
    [SerializeField] private bool invertDPadAxis;

    [Tooltip("Sólo se usa si D Pad Mode está en Buttons.")]
    [Range(0, 19)]
    [SerializeField] private int dPadLeftButton = 13;
    [Tooltip("Sólo se usa si D Pad Mode está en Buttons.")]
    [Range(0, 19)]
    [SerializeField] private int dPadRightButton = 14;

    public bool MoveLeft() => ReadButton(moveLeftButton);
    public bool MoveRight() => ReadButton(moveRightButton);

    public bool PoseA() => ReadButton(poseAButton);
    public bool PoseC() => ReadButton(poseCButton);

    public bool PoseB()
    {
        if (dPadMode == DPadMode.Buttons)
            return ReadButton(dPadLeftButton);

        return ReadDPadAxis() < -dPadDeadZone;
    }

    public bool PoseD()
    {
        if (dPadMode == DPadMode.Buttons)
            return ReadButton(dPadRightButton);

        return ReadDPadAxis() > dPadDeadZone;
    }

    private float ReadDPadAxis()
    {
        if (string.IsNullOrWhiteSpace(dPadHorizontalAxis))
            return 0f;

        float value = Input.GetAxisRaw(dPadHorizontalAxis);
        return invertDPadAxis ? -value : value;
    }

    private bool ReadButton(int buttonNumber)
    {
        return Input.GetKey(GetJoystickButtonKeyCode(buttonNumber));
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

    private void OnValidate()
    {
        joystickNumber = Mathf.Clamp(joystickNumber, 1, 8);
        dPadDeadZone = Mathf.Clamp(dPadDeadZone, 0.1f, 0.95f);
    }
}
