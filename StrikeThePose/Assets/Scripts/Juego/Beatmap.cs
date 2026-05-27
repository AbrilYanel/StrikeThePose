using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewBeatMap", menuName = "RhythmGame/BeatMap")]
public class Beatmap : ScriptableObject
{
    [Header("Audio")]
    public AudioClip song;
    [Tooltip("Beats por minuto de la canción")]
    public float bpm = 120f;
    [Tooltip("Offset en segundos al inicio antes de que arranquen los beats")]
    public float startOffsetSeconds = 0f;
    [Header("Obstáculos")]
    public List<BeatEvent> events = new List<BeatEvent>();
    public float SecondsPerBeat => 60f / bpm;
    public float BeatToSeconds(float beat) =>
        startOffsetSeconds + (beat - 1f) * SecondsPerBeat;
}
