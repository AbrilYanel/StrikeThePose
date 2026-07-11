using UnityEngine;

/// <summary>
/// Permite alternar entre split screen y dos Game Views independientes.
/// También puede mantener los Canvas en Screen Space - Overlay y ajustar
/// automáticamente el área de UI correspondiente a cada jugador.
/// </summary>
public class SplitScreenCameraSetup : MonoBehaviour
{
    public enum ViewMode
    {
        SplitScreen,
        SeparateGameViews
    }

    [Header("Modo de visualización")]
    [SerializeField] private ViewMode viewMode = ViewMode.SplitScreen;

    [Header("Cámaras")]
    [SerializeField] private Camera player1Camera;
    [SerializeField] private Camera player2Camera;

    [Header("UI")]
    [Tooltip("Activalo para conservar Screen Space - Overlay.")]
    [SerializeField] private bool useOverlayCanvases = true;
    [SerializeField] private Canvas player1Canvas;
    [SerializeField] private Canvas player2Canvas;
    [Tooltip("RectTransform padre de todos los elementos visuales de la UI de P1.")]
    [SerializeField] private RectTransform player1UIRoot;
    [Tooltip("RectTransform padre de todos los elementos visuales de la UI de P2.")]
    [SerializeField] private RectTransform player2UIRoot;

    [Header("Layers")]
    [SerializeField] private string player1LayerName = "Track_P1";
    [SerializeField] private string player2LayerName = "Track_P2";

    private void Awake()
    {
        ApplyConfiguration();
    }

    [ContextMenu("Aplicar configuración de cámaras y UI")]
    public void ApplyConfiguration()
    {
        int player1Layer = LayerMask.NameToLayer(player1LayerName);
        int player2Layer = LayerMask.NameToLayer(player2LayerName);

        if (player1Camera == null || player2Camera == null)
        {
            Debug.LogError(
                "[SplitScreenCameraSetup] Faltan una o ambas cámaras.",
                this
            );
            return;
        }

        if (player1Layer < 0 || player2Layer < 0)
        {
            Debug.LogError(
                "[SplitScreenCameraSetup] Las layers Track_P1 y/o Track_P2 no existen.",
                this
            );
            return;
        }

        player1Camera.cullingMask = 1 << player1Layer;
        player2Camera.cullingMask = 1 << player2Layer;

        if (viewMode == ViewMode.SplitScreen)
            ConfigureSplitScreen();
        else
            ConfigureSeparateGameViews();

        ConfigureCanvases();
        ConfigureAudioListeners();
    }

    private void ConfigureSplitScreen()
    {
        // Las dos cámaras se dibujan en Display 1.
        player1Camera.targetDisplay = 0;
        player2Camera.targetDisplay = 0;

        player1Camera.rect = new Rect(0f, 0f, 0.5f, 1f);
        player2Camera.rect = new Rect(0.5f, 0f, 0.5f, 1f);
    }

    private void ConfigureSeparateGameViews()
    {
        // targetDisplay usa índices desde cero:
        // 0 = Display 1 y 1 = Display 2.
        player1Camera.targetDisplay = 0;
        player2Camera.targetDisplay = 1;

        // Cada cámara ocupa por completo su propio Game View.
        player1Camera.rect = new Rect(0f, 0f, 1f, 1f);
        player2Camera.rect = new Rect(0f, 0f, 1f, 1f);
    }

    private void ConfigureCanvases()
    {
        if (player1Canvas == null || player2Canvas == null)
        {
            Debug.LogWarning(
                "[SplitScreenCameraSetup] No se asignaron ambos Canvas.",
                this
            );
            return;
        }

        if (useOverlayCanvases)
        {
            player1Canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            player2Canvas.renderMode = RenderMode.ScreenSpaceOverlay;

            player1Canvas.worldCamera = null;
            player2Canvas.worldCamera = null;

            if (viewMode == ViewMode.SplitScreen)
            {
                // Ambos Canvas viven en Display 1, pero cada UIRoot ocupa una mitad.
                player1Canvas.targetDisplay = 0;
                player2Canvas.targetDisplay = 0;

                SetUIRootArea(
                    player1UIRoot,
                    new Vector2(0f, 0f),
                    new Vector2(0.5f, 1f)
                );

                SetUIRootArea(
                    player2UIRoot,
                    new Vector2(0.5f, 0f),
                    new Vector2(1f, 1f)
                );
            }
            else
            {
                // Cada Canvas usa un display y ocupa la pantalla completa.
                player1Canvas.targetDisplay = 0;
                player2Canvas.targetDisplay = 1;

                SetUIRootArea(
                    player1UIRoot,
                    Vector2.zero,
                    Vector2.one
                );

                SetUIRootArea(
                    player2UIRoot,
                    Vector2.zero,
                    Vector2.one
                );
            }
        }
        else
        {
            // Alternativa tradicional: un Canvas asociado a cada cámara.
            player1Canvas.renderMode = RenderMode.ScreenSpaceCamera;
            player2Canvas.renderMode = RenderMode.ScreenSpaceCamera;

            player1Canvas.worldCamera = player1Camera;
            player2Canvas.worldCamera = player2Camera;

            player1Canvas.targetDisplay = player1Camera.targetDisplay;
            player2Canvas.targetDisplay = player2Camera.targetDisplay;

            SetUIRootArea(player1UIRoot, Vector2.zero, Vector2.one);
            SetUIRootArea(player2UIRoot, Vector2.zero, Vector2.one);
        }
    }

    private static void SetUIRootArea(
        RectTransform uiRoot,
        Vector2 anchorMin,
        Vector2 anchorMax
    )
    {
        if (uiRoot == null)
            return;

        uiRoot.anchorMin = anchorMin;
        uiRoot.anchorMax = anchorMax;
        uiRoot.offsetMin = Vector2.zero;
        uiRoot.offsetMax = Vector2.zero;
        uiRoot.localScale = Vector3.one;
    }

    private void ConfigureAudioListeners()
    {
        AudioListener player1Listener =
            player1Camera.GetComponent<AudioListener>();

        AudioListener player2Listener =
            player2Camera.GetComponent<AudioListener>();

        if (player1Listener != null)
            player1Listener.enabled = true;

        if (player2Listener != null)
            player2Listener.enabled = false;
    }
}
