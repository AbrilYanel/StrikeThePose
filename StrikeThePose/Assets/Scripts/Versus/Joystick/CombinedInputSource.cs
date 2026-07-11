using UnityEngine;

public class CombinedInputSource : MonoBehaviour, IPlayerInputSource
{
    [Header("Fuentes")]
    [Tooltip("Fuente principal, normalmente GamepadInputSource.")]
    [SerializeField] private MonoBehaviour primarySourceComponent;
    [Tooltip("Fuente secundaria/fallback, normalmente KeyboardInputSource.")]
    [SerializeField] private MonoBehaviour secondarySourceComponent;
    [SerializeField] private bool useSecondarySource = true;

    private IPlayerInputSource _primarySource;
    private IPlayerInputSource _secondarySource;

    private bool PrimaryAvailable =>
        _primarySource != null &&
        primarySourceComponent != null &&
        primarySourceComponent.isActiveAndEnabled;

    private bool SecondaryAvailable =>
        useSecondarySource &&
        _secondarySource != null &&
        secondarySourceComponent != null &&
        secondarySourceComponent.isActiveAndEnabled;

    private void Awake()
    {
        _primarySource = primarySourceComponent as IPlayerInputSource;
        _secondarySource = secondarySourceComponent as IPlayerInputSource;

        if (_primarySource == null)
        {
            Debug.LogError(
                "[CombinedInputSource] Primary Source no implementa IPlayerInputSource.",
                this
            );
        }

        if (secondarySourceComponent != null && _secondarySource == null)
        {
            Debug.LogError(
                "[CombinedInputSource] Secondary Source no implementa IPlayerInputSource.",
                this
            );
        }
    }

    public bool MoveLeft() =>
        (PrimaryAvailable && _primarySource.MoveLeft()) ||
        (SecondaryAvailable && _secondarySource.MoveLeft());

    public bool MoveRight() =>
        (PrimaryAvailable && _primarySource.MoveRight()) ||
        (SecondaryAvailable && _secondarySource.MoveRight());

    public bool PoseA() =>
        (PrimaryAvailable && _primarySource.PoseA()) ||
        (SecondaryAvailable && _secondarySource.PoseA());

    public bool PoseB() =>
        (PrimaryAvailable && _primarySource.PoseB()) ||
        (SecondaryAvailable && _secondarySource.PoseB());

    public bool PoseC() =>
        (PrimaryAvailable && _primarySource.PoseC()) ||
        (SecondaryAvailable && _secondarySource.PoseC());

    public bool PoseD() =>
        (PrimaryAvailable && _primarySource.PoseD()) ||
        (SecondaryAvailable && _secondarySource.PoseD());
}
