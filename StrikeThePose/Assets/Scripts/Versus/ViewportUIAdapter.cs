using UnityEngine;
using UnityEngine.UI;


[ExecuteAlways]
[RequireComponent(typeof(RectTransform))]
[RequireComponent(typeof(RectMask2D))]
public class ViewportUIAdapter : MonoBehaviour
{
    public enum ScaleMode
    {
        // Muestra toda la UI. Puede dejar espacio libre arriba/abajo o a los lados.
        Fit,

        // Ocupa todo el viewport. Puede recortar parte de la UI.
        Fill,

        // Ajusta usando únicamente el ancho disponible.
        MatchWidth,

        // Ajusta usando únicamente la altura disponible.
        MatchHeight
    }

    public enum ContentAlignment
    {
        Center,
        Top,
        Bottom,
        Left,
        Right,
        TopLeft,
        TopRight,
        BottomLeft,
        BottomRight
    }

    [Header("Contenido")]
    [Tooltip("RectTransform hijo que contiene todos los elementos de la UI original.")]
    [SerializeField] private RectTransform contentRoot;

    [Header("Diseño original")]
    [Tooltip("Resolución para la cual fue diseñada originalmente la interfaz.")]
    [SerializeField] private Vector2 designResolution = new Vector2(1920f, 1080f);

    [Header("Adaptación")]
    [SerializeField] private ScaleMode scaleMode = ScaleMode.Fit;
    [SerializeField] private ContentAlignment alignment = ContentAlignment.Center;
    [Tooltip("Permite previsualizar el resultado fuera de Play Mode.")]
    [SerializeField] private bool updateOutsidePlayMode = true;

    private RectTransform _viewportRoot;
    private Vector2 _lastViewportSize = new Vector2(-1f, -1f);
    private Vector2 _lastDesignResolution = new Vector2(-1f, -1f);
    private ScaleMode _lastScaleMode;
    private ContentAlignment _lastAlignment;

    private void OnEnable()
    {
        _viewportRoot = transform as RectTransform;
        ApplyLayout();
    }

    private void LateUpdate()
    {
        if (!Application.isPlaying && !updateOutsidePlayMode)
            return;

        if (_viewportRoot == null)
            _viewportRoot = transform as RectTransform;

        if (_viewportRoot == null || contentRoot == null)
            return;

        Vector2 currentSize = _viewportRoot.rect.size;

        if (currentSize != _lastViewportSize ||
            designResolution != _lastDesignResolution ||
            scaleMode != _lastScaleMode ||
            alignment != _lastAlignment)
        {
            ApplyLayout();
        }
    }

    private void OnRectTransformDimensionsChange()
    {
        if (!isActiveAndEnabled)
            return;

        if (!Application.isPlaying && !updateOutsidePlayMode)
            return;

        ApplyLayout();
    }

    [ContextMenu("Aplicar adaptación de UI")]
    public void ApplyLayout()
    {
        if (_viewportRoot == null)
            _viewportRoot = transform as RectTransform;

        if (_viewportRoot == null || contentRoot == null)
            return;

        if (designResolution.x <= 0f || designResolution.y <= 0f)
        {
            Debug.LogError(
                "[ViewportUIAdapter] Design Resolution debe ser mayor que cero.",
                this
            );
            return;
        }

        Vector2 viewportSize = _viewportRoot.rect.size;

        if (viewportSize.x <= 0f || viewportSize.y <= 0f)
            return;

        float scaleX = viewportSize.x / designResolution.x;
        float scaleY = viewportSize.y / designResolution.y;
        float uniformScale;

        switch (scaleMode)
        {
            case ScaleMode.Fill:
                uniformScale = Mathf.Max(scaleX, scaleY);
                break;

            case ScaleMode.MatchWidth:
                uniformScale = scaleX;
                break;

            case ScaleMode.MatchHeight:
                uniformScale = scaleY;
                break;

            default:
                uniformScale = Mathf.Min(scaleX, scaleY);
                break;
        }

        contentRoot.anchorMin = new Vector2(0.5f, 0.5f);
        contentRoot.anchorMax = new Vector2(0.5f, 0.5f);
        contentRoot.pivot = new Vector2(0.5f, 0.5f);
        contentRoot.sizeDelta = designResolution;
        contentRoot.localScale = new Vector3(uniformScale, uniformScale, 1f);
        contentRoot.anchoredPosition = CalculateAlignedPosition(
            viewportSize,
            designResolution * uniformScale
        );

        _lastViewportSize = viewportSize;
        _lastDesignResolution = designResolution;
        _lastScaleMode = scaleMode;
        _lastAlignment = alignment;
    }

    private Vector2 CalculateAlignedPosition(
        Vector2 viewportSize,
        Vector2 renderedContentSize
    )
    {
        float horizontalSpace = (viewportSize.x - renderedContentSize.x) * 0.5f;
        float verticalSpace = (viewportSize.y - renderedContentSize.y) * 0.5f;

        switch (alignment)
        {
            case ContentAlignment.Top:
                return new Vector2(0f, verticalSpace);

            case ContentAlignment.Bottom:
                return new Vector2(0f, -verticalSpace);

            case ContentAlignment.Left:
                return new Vector2(-horizontalSpace, 0f);

            case ContentAlignment.Right:
                return new Vector2(horizontalSpace, 0f);

            case ContentAlignment.TopLeft:
                return new Vector2(-horizontalSpace, verticalSpace);

            case ContentAlignment.TopRight:
                return new Vector2(horizontalSpace, verticalSpace);

            case ContentAlignment.BottomLeft:
                return new Vector2(-horizontalSpace, -verticalSpace);

            case ContentAlignment.BottomRight:
                return new Vector2(horizontalSpace, -verticalSpace);

            default:
                return Vector2.zero;
        }
    }
}
