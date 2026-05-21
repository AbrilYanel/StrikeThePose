using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class CameraEffects : MonoBehaviour
{
    public static CameraEffects Instance;

    [Header("Configuración Shake")]
    private Vector3 _originalPos;
    private float _shakeTimer;
    [SerializeField] private float _shakeIntensity = 0.2f;

    [Header("Configuración Flash Rojo")]
    [SerializeField] private Image _damageOverlay; // Arrastra aquí la imagen del Canvas
    [SerializeField] private Color _flashColor = new Color(1, 0, 0, 0.4f);
    [SerializeField] private float _flashDuration = 0.3f;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        _originalPos = transform.localPosition;

        // Asegurarse de que el flash empiece invisible
        if (_damageOverlay != null)
        {
            _damageOverlay.color = new Color(_flashColor.r, _flashColor.g, _flashColor.b, 0f);
        }
    }

    void Update()
    {
        // Lógica de Shake
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


    public void PlayErrorFeedback()
    {
        // Iniciar Shake
        _shakeTimer = _flashDuration;

        // Iniciar Flash
        if (_damageOverlay != null)
        {
            StopAllCoroutines();
            StartCoroutine(FlashRoutine());
        }
    }

    private IEnumerator FlashRoutine()
    {
        float elapsed = 0f;

        while (elapsed < _flashDuration)
        {
            elapsed += Time.deltaTime;
            // Interpolar el alpha de 0.4 a 0
            float alpha = Mathf.Lerp(_flashColor.a, 0f, elapsed / _flashDuration);
            _damageOverlay.color = new Color(_flashColor.r, _flashColor.g, _flashColor.b, alpha);
            yield return null;
        }
    }
}
