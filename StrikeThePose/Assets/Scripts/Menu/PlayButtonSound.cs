using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
public class PlayButtonSound : MonoBehaviour
{
    [Header("Audio")]
    [Tooltip("Sonido que se reproduce al presionar Play")]
    [SerializeField] private AudioClip playSound;
    [Tooltip("AudioSource a usar. Si se deja vacío, se busca uno en este mismo GameObject o se crea uno automáticamente.")]
    [SerializeField] private AudioSource audioSource;

    [Header("Escena destino")]
    [Tooltip("Nombre de la escena a cargar (debe estar agregada en Build Settings)")]
    [SerializeField] private string sceneToLoad = "Level1";

    [Header("Timing")]
    [Tooltip("Si está activo, espera a que el sonido termine de reproducirse antes de cambiar de escena. Si está desactivado, usa el delay fijo de abajo.")]
    [SerializeField] private bool waitForClipToFinish = true;
    [Tooltip("Delay fijo en segundos antes de cambiar de escena (solo se usa si 'waitForClipToFinish' está desactivado)")]
    [SerializeField] private float fixedDelay = 0.3f;

    private bool _isLoading = false;

    private void Awake()
    {
        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();

        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();

        audioSource.playOnAwake = false;
    }

   
    public void OnPlayPressed()
    {
        if (_isLoading) return; // Evita doble click mientras carga
        _isLoading = true;

        StartCoroutine(PlaySoundThenLoadScene());
    }

    private IEnumerator PlaySoundThenLoadScene()
    {
        float waitTime = fixedDelay;

        if (playSound != null)
        {
            audioSource.clip = playSound;
            audioSource.Play();

            if (waitForClipToFinish)
                waitTime = playSound.length;
        }

        yield return new WaitForSeconds(waitTime);

        SceneManager.LoadScene(sceneToLoad);
    }
}