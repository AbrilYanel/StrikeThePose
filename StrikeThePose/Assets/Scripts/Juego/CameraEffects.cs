using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class CameraEffects : MonoBehaviour
{
    [Header("Shake")]
    private Vector3 _originalPos;
    private float _shakeTimer;
    [SerializeField] private float _shakeIntensity = 0.2f;

    [Header("Flash Error (Rojo)")]
    [SerializeField] private Image _damageOverlay;
    [SerializeField] private Color _errorFlashColor = new Color(1f, 0f, 0f, 0.4f);
    [SerializeField] private float _errorFlashDuration = 0.3f;

    [Header("Flash Acierto (Verde/Dorado)")]
    [SerializeField] private Image _successOverlay;
    [SerializeField] private Color _successFlashColor = new Color(0.2f, 1f, 0.4f, 0.3f);
    [SerializeField] private float _successFlashDuration = 0.2f;

    void Start()
    {
        _originalPos = transform.localPosition;

        if (_damageOverlay != null)
            _damageOverlay.color = Color.clear;

        if (_successOverlay != null)
            _successOverlay.color = Color.clear;
    }

    void Update()
    {
        if (_shakeTimer > 0)
        {
            transform.localPosition = _originalPos + Random.insideUnitSphere * _shakeIntensity;
            _shakeTimer -= Time.deltaTime;
        }
        else
        {
            transform.localPosition = _originalPos;
        }
    }

    // ─── ERROR ───
    public void PlayErrorFeedback()
    {
        _shakeTimer = _errorFlashDuration;

        if (_damageOverlay != null)
        {
            StopCoroutine(nameof(ErrorFlashRoutine));
            StartCoroutine(ErrorFlashRoutine());
        }
    }

    private IEnumerator ErrorFlashRoutine()
    {
        float elapsed = 0f;
        while (elapsed < _errorFlashDuration)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(_errorFlashColor.a, 0f, elapsed / _errorFlashDuration);
            _damageOverlay.color = new Color(_errorFlashColor.r, _errorFlashColor.g, _errorFlashColor.b, alpha);
            yield return null;
        }
        _damageOverlay.color = Color.clear;
    }

    // ─── ACIERTO ───
    public void PlaySuccessFeedback()
    {
        if (_successOverlay != null)
        {
            StopCoroutine(nameof(SuccessFlashRoutine));
            StartCoroutine(SuccessFlashRoutine());
        }
    }

    private IEnumerator SuccessFlashRoutine()
    {
        float elapsed = 0f;
        while (elapsed < _successFlashDuration)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(_successFlashColor.a, 0f, elapsed / _successFlashDuration);
            _successOverlay.color = new Color(_successFlashColor.r, _successFlashColor.g, _successFlashColor.b, alpha);
            yield return null;
        }
        _successOverlay.color = Color.clear;
    }
}