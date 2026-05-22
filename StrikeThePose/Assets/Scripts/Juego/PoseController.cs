using UnityEngine;

public enum PoseType
{
    Idle,
    PoseA,
    PoseB,
    PoseC,
    PoseD
}

public class PoseController : MonoBehaviour
{
    [Header("Animator")]
    [SerializeField] private Animator animator;

    public PoseType CurrentPose { get; private set; } = PoseType.Idle;
    public float LastInputTime { get; private set; } = -999f;
    public PoseType LastInputPose { get; private set; } = PoseType.Idle;

    private void Start()
    {
        if (animator != null)
            animator.applyRootMotion = false;
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.W))
            SetPose(PoseType.PoseA);
        else if (Input.GetKeyDown(KeyCode.A))
            SetPose(PoseType.PoseB);
        else if (Input.GetKeyDown(KeyCode.S))
            SetPose(PoseType.PoseC);
        else if (Input.GetKeyDown(KeyCode.D))
            SetPose(PoseType.PoseD);
    }

    public void SetPose(PoseType pose)
    {
        CurrentPose = pose;
        LastInputPose = pose;
        LastInputTime = Time.time;

        if (animator != null)
        {
            // Resetear todos los triggers para evitar acumulación
            animator.ResetTrigger("PoseA");
            animator.ResetTrigger("PoseB");
            animator.ResetTrigger("PoseC");
            animator.ResetTrigger("PoseD");

            // Activar solo el trigger correspondiente
            animator.SetTrigger(pose.ToString());
        }

        Debug.Log($"[Player] Pose: {pose} en t={Time.time:F2}");
    }

    public bool MatchesPose(PoseType required)
    {
        return CurrentPose == required;
    }
}