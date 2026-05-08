using UnityEngine;
using UnityEngine.Events;


public class GameManager : MonoBehaviour
{

    public static GameManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

 

    public int Score { get; private set; }
    public int Combo { get; private set; }
    public int MaxCombo { get; private set; }
    public int Misses { get; private set; }

    [Header("Puntuación")]
    [SerializeField] private int pointsPerHit = 100;
    [SerializeField] private int comboBonusEvery = 5;  // cada N combo, bonus x2

  

    [System.Serializable] public class ResultEvent : UnityEvent<bool, PoseType> { }
    public ResultEvent OnObstacleResultEvent;

    

    public void OnObstacleResult(bool success, PoseType poseRequired)
    {
        if (success)
        {
            Combo++;
            if (Combo > MaxCombo) MaxCombo = Combo;

            int bonus = (Combo > 0 && Combo % comboBonusEvery == 0) ? 2 : 1;
            Score += pointsPerHit * bonus;

            Debug.Log($"[GameManager]  HIT | Score: {Score} | Combo: {Combo}x");
        }
        else
        {
            Misses++;
            Combo = 0;
            Debug.Log($"[GameManager] X MISS | Misses: {Misses}");
        }

        OnObstacleResultEvent?.Invoke(success, poseRequired);
    }

  
    public void ResetGame()
    {
        Score = 0;
        Combo = 0;
        MaxCombo = 0;
        Misses = 0;
    }
}