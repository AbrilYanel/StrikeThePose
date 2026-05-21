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
    [Header("Animator (opcional)")]
    [SerializeField] private Animator animator;
    [SerializeField] private string animatorPoseParam = "PoseIndex";

    [Header("Sprites (opcional)")]
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Sprite[] poseSprites;

    public PoseType CurrentPose { get; private set; } = PoseType.Idle;

    public float LastInputTime { get; private set; } = -999f;
    public PoseType LastInputPose { get; private set; } = PoseType.Idle;

    public void SetPose(PoseType pose)
    {
        if (CurrentPose == pose) return;

        CurrentPose = pose;

        if (pose != PoseType.Idle)
        {
            LastInputTime = Time.time;
            LastInputPose = pose;
        }

        ApplyToAnimator(pose);
        ApplyToSprite(pose);
    }

    private void ApplyToAnimator(PoseType pose)
    {
        if (animator == null) return;
        animator.SetInteger(animatorPoseParam, (int)pose);
    }

    private void ApplyToSprite(PoseType pose)
    {
        if (spriteRenderer == null || poseSprites == null) return;
        int index = (int)pose;
        if (index < poseSprites.Length && poseSprites[index] != null)
            spriteRenderer.sprite = poseSprites[index];
    }

    public bool MatchesPose(PoseType required)
    {
        return CurrentPose == required;
    }
}