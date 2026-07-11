using UnityEngine;

public class KeyboardInputSource : MonoBehaviour, IPlayerInputSource
{
    [Header("Movimiento")]
    [SerializeField] private KeyCode moveLeftKey = KeyCode.J;
    [SerializeField] private KeyCode moveRightKey = KeyCode.K;

    [Header("Poses")]
    [SerializeField] private KeyCode poseAKey = KeyCode.W;
    [SerializeField] private KeyCode poseBKey = KeyCode.A;
    [SerializeField] private KeyCode poseCKey = KeyCode.S;
    [SerializeField] private KeyCode poseDKey = KeyCode.D;

    public bool MoveLeft() => Input.GetKey(moveLeftKey);
    public bool MoveRight() => Input.GetKey(moveRightKey);

    public bool PoseA() => Input.GetKey(poseAKey);
    public bool PoseB() => Input.GetKey(poseBKey);
    public bool PoseC() => Input.GetKey(poseCKey);
    public bool PoseD() => Input.GetKey(poseDKey);
}