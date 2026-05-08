using UnityEngine;
using TMPro; 

public class UIManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI comboText;

    void Update()
    {
        if (GameManager.Instance != null)
        {
            scoreText.text = $"Puntos: {GameManager.Instance.Score}";
            comboText.text = GameManager.Instance.Combo > 0
                ? $"Combo: {GameManager.Instance.Combo}x"
                : "";
        }
    }
}