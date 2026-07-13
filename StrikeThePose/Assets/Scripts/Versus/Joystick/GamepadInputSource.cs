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

    [Header("Botones de poses")]
    [Tooltip("Pose A: Triángulo.")]
    [Range(0, 19)]
    [SerializeField] private int poseAButton = 3;

    [Tooltip("Pose B: Cuadrado.")]
    [Range(0, 19)]
    [SerializeField] private int poseBButton = 2;

    [Tooltip("Pose C: Equis/Cruz.")]
    [Range(0, 19)]
    [SerializeField] private int poseCButton = 0;

    [Tooltip("Pose D: Círculo.")]
    [Range(0, 19)]
    [SerializeField] private int poseDButton = 1;

    [Header("D-Pad")]
    [SerializeField] private DPadMode dPadMode = DPadMode.Axis;

    [Header("D-Pad como ejes")]
    [Tooltip("Eje horizontal: izquierda/derecha.")]
    [SerializeField] private string dPadHorizontalAxis = "P2_DPadHorizontal";
    [Tooltip("Eje vertical: arriba/abajo.")]
    [SerializeField] private string dPadVerticalAxis = "P2_DPadVertical";
    [Range(0.1f, 0.95f)]
    [SerializeField] private float dPadDeadZone = 0.5f;
    [SerializeField] private bool invertDPadHorizontalAxis;
    [SerializeField] private bool invertDPadVerticalAxis;

    [Header("D-Pad como botones")]
    [Tooltip("Sólo se usa si D Pad Mode está en Buttons.")]
    [Range(0, 19)]
    [SerializeField] private int dPadUpButton = 12;
    [Tooltip("Sólo se usa si D Pad Mode está en Buttons.")]
    [Range(0, 19)]
    [SerializeField] private int dPadLeftButton = 13;
    [Tooltip("Sólo se usa si D Pad Mode está en Buttons.")]
    [Range(0, 19)]
    [SerializeField] private int dPadRightButton = 14;
    [Tooltip("Sólo se usa si D Pad Mode está en Buttons.")]
    [Range(0, 19)]
    [SerializeField] private int dPadDownButton = 15;

    public bool MoveLeft() => ReadButton(moveLeftButton);
    public bool MoveRight() => ReadButton(moveRightButton);

    // Pose A = Triángulo O D-Pad arriba.
    public bool PoseA()
    {
        return ReadButton(poseAButton) || ReadDPadUp();
    }

    // Pose B = Cuadrado O D-Pad izquierda.
    public bool PoseB()
    {
        return ReadButton(poseBButton) || ReadDPadLeft();
    }

    // Pose C = Equis O D-Pad abajo.
    public bool PoseC()
    {
        return ReadButton(poseCButton) || ReadDPadDown();
    }

    // Pose D = Círculo O D-Pad derecha.
    public bool PoseD()
    {
        return ReadButton(poseDButton) || ReadDPadRight();
    }

    private bool ReadDPadUp()
    {
        if (dPadMode == DPadMode.Buttons)
            return ReadButton(dPadUpButton);

        return ReadDPadVerticalAxis() > dPadDeadZone;
    }

    private bool ReadDPadDown()
    {
        if (dPadMode == DPadMode.Buttons)
            return ReadButton(dPadDownButton);

        return ReadDPadVerticalAxis() < -dPadDeadZone;
    }

    private bool ReadDPadLeft()
    {
        if (dPadMode == DPadMode.Buttons)
            return ReadButton(dPadLeftButton);

        return ReadDPadHorizontalAxis() < -dPadDeadZone;
    }

    private bool ReadDPadRight()
    {
        if (dPadMode == DPadMode.Buttons)
            return ReadButton(dPadRightButton);

        return ReadDPadHorizontalAxis() > dPadDeadZone;
    }

    private float ReadDPadHorizontalAxis()
    {
        return ReadAxis(dPadHorizontalAxis, invertDPadHorizontalAxis);
    }

    private float ReadDPadVerticalAxis()
    {
        return ReadAxis(dPadVerticalAxis, invertDPadVerticalAxis);
    }

    private float ReadAxis(string axisName, bool invert)
    {
        if (string.IsNullOrWhiteSpace(axisName))
            return 0f;

        float value = Input.GetAxisRaw(axisName);
        return invert ? -value : value;
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
