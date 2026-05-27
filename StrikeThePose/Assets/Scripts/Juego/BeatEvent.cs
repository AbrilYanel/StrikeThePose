using System;
using UnityEngine;

[Serializable]
public class BeatEvent
{
    [Tooltip("En qué beat ocurre (1 = primer beat, puede ser decimal ej: 1.5)")]
    public float beat = 1f;
    [Tooltip("Pose que requiere este obstáculo")]
    public PoseType requiredPose = PoseType.PoseA;
    [Tooltip("Posición X del hueco. Dejá en -999 para que sea random en runtime.")]
    public float holePositionX = -999f;
    public bool IsHoleRandom => holePositionX <= -999f;
}
