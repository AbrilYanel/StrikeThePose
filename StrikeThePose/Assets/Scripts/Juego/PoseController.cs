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
    [Tooltip("Nombre del parámetro int en el Animator Controller")]
    [SerializeField] private string animatorPoseParam = "PoseIndex";

    [Header("Sprites (opcional, alternativa al Animator)")]
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Sprite[] poseSprites;

    public PoseType CurrentPose { get; private set; } = PoseType.Idle;




    public void SetPose(PoseType pose)
    {
        if (CurrentPose == pose) return;

        CurrentPose = pose;

        ApplyToAnimator(pose);
        ApplyToSprite(pose);

        Debug.Log($"[PoseController] Pose activa {pose}");
    }

    // Aplicación visual


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


    // Consulta para el sistema de detección de colisiones



    public bool MatchesPose(PoseType required)
    {
        return CurrentPose == required;
    }
}