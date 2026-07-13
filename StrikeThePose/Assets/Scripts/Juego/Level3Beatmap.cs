using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public static class Level3Beatmap
{
#if UNITY_EDITOR
    private const float Bpm = 158f;
    private const float StartOffsetSeconds = 2.5f;
    private const float SongDurationSeconds = 176f;

    private static readonly PoseType[] EasyPatternA =
    {
        PoseType.PoseA,
        PoseType.PoseC,
        PoseType.PoseB,
        PoseType.PoseD,
    };

    private static readonly PoseType[] EasyPatternB =
    {
        PoseType.PoseA,
        PoseType.PoseB,
        PoseType.PoseC,
        PoseType.PoseD,
        PoseType.PoseC,
        PoseType.PoseA,
    };

    private static readonly PoseType[] NormalPattern =
    {
        PoseType.PoseA,
        PoseType.PoseC,
        PoseType.PoseB,
        PoseType.PoseD,
        PoseType.PoseAB,
        PoseType.PoseC,
        PoseType.PoseAD,
        PoseType.PoseB,
        PoseType.PoseBC,
        PoseType.PoseD,
        PoseType.PoseCD,
        PoseType.PoseA,
    };

    private static readonly PoseType[] NormalChorusPattern =
    {
        PoseType.PoseA,
        PoseType.PoseAB,
        PoseType.PoseC,
        PoseType.PoseBC,
        PoseType.PoseB,
        PoseType.PoseCD,
        PoseType.PoseD,
        PoseType.PoseAD,
    };

    private static readonly PoseType[] HardPattern =
    {
        PoseType.PoseA,
        PoseType.PoseAB,
        PoseType.PoseC,
        PoseType.PoseBC,
        PoseType.PoseB,
        PoseType.PoseCD,
        PoseType.PoseD,
        PoseType.PoseAD,
        PoseType.PoseC,
        PoseType.PoseAB,
        PoseType.PoseB,
        PoseType.PoseBC,
        PoseType.PoseD,
        PoseType.PoseCD,
        PoseType.PoseA,
        PoseType.PoseAD,
    };

    [MenuItem("RhythmGame/Nivel 3 - Strawberry Rush/Cargar BeatMap Fácil")]
    private static void LoadLevel3Easy()
    {
        Beatmap beatmap = FindBeatmapAsset("Level3_Easy");
        if (beatmap == null) return;

        List<BeatEvent> events = new List<BeatEvent>();

        // ── INTRO: beats 1-32 ────────────────────────────────────────────────
        AddPattern(events, 5f, 29f, 8f, EasyPatternA);

        // ── VERSO 1: beats 33-96 ─────────────────────────────────────────────
        AddPattern(
            events,
            33f,
            93f,
            6f,
            EasyPatternB,
            holdEvery: 7,
            holdDuration: 1.5f
        );

        // ── CORO 1: beats 97-160 ─────────────────────────────────────────────
        AddPattern(
            events,
            97f,
            157f,
            4f,
            EasyPatternA,
            holdEvery: 10,
            holdDuration: 1f
        );

        // ── VERSO 2: beats 161-224 ────────────────────────────────────────────
        AddPattern(
            events,
            161f,
            221f,
            6f,
            EasyPatternB,
            holdEvery: 8,
            holdDuration: 1.5f
        );

        // ── ÁREA BONUS: beats 225-249 ────────────────────────────────────────
        events.Add(BonusStart(225f));
        events.Add(BonusEnd(249f));

        // ── PUENTE: beats 251-304 ─────────────────────────────────────────────
        AddPattern(
            events,
            251f,
            301f,
            6f,
            EasyPatternA,
            holdEvery: 7,
            holdDuration: 1.5f
        );

        // ── CORO 2: beats 305-368 ─────────────────────────────────────────────
        AddPattern(
            events,
            305f,
            365f,
            4f,
            EasyPatternB,
            holdEvery: 10,
            holdDuration: 1f
        );

        // ── CORO FINAL / OUTRO: beats 369-458 ─────────────────────────────────
        AddPattern(
            events,
            369f,
            441f,
            6f,
            EasyPatternA,
            holdEvery: 8,
            holdDuration: 1.5f
        );
        events.Add(H(455f, PoseType.PoseD, 0.8f));

        ApplyBeatmap(beatmap, events, "FÁCIL");
    }

    [MenuItem("RhythmGame/Nivel 3 - Strawberry Rush/Cargar BeatMap Normal")]
    private static void LoadLevel3Normal()
    {
        Beatmap beatmap = FindBeatmapAsset("Level3_Normal");
        if (beatmap == null) return;

        List<BeatEvent> events = new List<BeatEvent>();

        // ── INTRO: beats 1-32 ────────────────────────────────────────────────
        AddPattern(events, 5f, 29f, 6f, EasyPatternB);

        // ── VERSO 1: beats 33-96 ─────────────────────────────────────────────
        AddPattern(
            events,
            33f,
            93f,
            4f,
            NormalPattern,
            holdEvery: 9,
            holdDuration: 1f
        );

        // ── CORO 1: beats 97-160 ─────────────────────────────────────────────
        AddPattern(
            events,
            97f,
            157f,
            3f,
            NormalChorusPattern,
            holdEvery: 12,
            holdDuration: 0.8f
        );

        // ── VERSO 2: beats 161-224 ────────────────────────────────────────────
        AddPattern(
            events,
            161f,
            221f,
            4f,
            NormalPattern,
            holdEvery: 10,
            holdDuration: 1f
        );

        // ── ÁREA BONUS: beats 225-249 ────────────────────────────────────────
        events.Add(BonusStart(225f));
        events.Add(BonusEnd(249f));

        // ── PUENTE: beats 251-304 ─────────────────────────────────────────────
        AddPattern(
            events,
            251f,
            301f,
            4f,
            NormalPattern,
            holdEvery: 9,
            holdDuration: 1f
        );

        // ── CORO 2: beats 305-368 ─────────────────────────────────────────────
        AddPattern(
            events,
            305f,
            365f,
            3f,
            NormalChorusPattern,
            holdEvery: 12,
            holdDuration: 0.8f
        );

        // ── CORO FINAL / OUTRO: beats 369-458 ─────────────────────────────────
        AddPattern(
            events,
            369f,
            445f,
            4f,
            NormalPattern,
            holdEvery: 10,
            holdDuration: 1f
        );
        events.Add(H(455f, PoseType.PoseAD, 0.8f));

        ApplyBeatmap(beatmap, events, "NORMAL");
    }

    [MenuItem("RhythmGame/Nivel 3 - Strawberry Rush/Cargar BeatMap Difícil")]
    private static void LoadLevel3Hard()
    {
        Beatmap beatmap = FindBeatmapAsset("Level3_Hard");
        if (beatmap == null) return;

        List<BeatEvent> events = new List<BeatEvent>();

        // ── INTRO: beats 1-32 ────────────────────────────────────────────────
        AddPattern(events, 5f, 29f, 4f, NormalPattern);

        // ── VERSO 1: beats 33-96 ─────────────────────────────────────────────
        AddPattern(
            events,
            33f,
            93f,
            3f,
            HardPattern,
            holdEvery: 14,
            holdDuration: 0.6f
        );

        // ── CORO 1: beats 97-160 ─────────────────────────────────────────────
        AddPattern(
            events,
            97f,
            157f,
            2f,
            HardPattern,
            holdEvery: 16,
            holdDuration: 0.45f
        );

        // ── VERSO 2: beats 161-224 ────────────────────────────────────────────
        AddPattern(
            events,
            161f,
            221f,
            3f,
            HardPattern,
            holdEvery: 14,
            holdDuration: 0.6f
        );

        // ── ÁREA BONUS: beats 225-249 ────────────────────────────────────────
        events.Add(BonusStart(225f));
        events.Add(BonusEnd(249f));

        // ── PUENTE: beats 251-304 ─────────────────────────────────────────────
        AddPattern(
            events,
            251f,
            301f,
            3f,
            HardPattern,
            holdEvery: 14,
            holdDuration: 0.6f
        );

        // ── CORO 2: beats 305-368 ─────────────────────────────────────────────
        AddPattern(
            events,
            305f,
            365f,
            2f,
            HardPattern,
            holdEvery: 16,
            holdDuration: 0.45f
        );

        // ── CORO FINAL / OUTRO: beats 369-458 ─────────────────────────────────
        AddPattern(
            events,
            369f,
            447f,
            2f,
            HardPattern,
            holdEvery: 16,
            holdDuration: 0.45f
        );
        events.Add(H(455f, PoseType.PoseA, 0.8f));

        ApplyBeatmap(beatmap, events, "DIFÍCIL");
    }

    private static void AddPattern(
        List<BeatEvent> events,
        float startBeat,
        float endBeat,
        float beatStep,
        PoseType[] posePattern,
        int holdEvery = 0,
        float holdDuration = 0f)
    {
        int noteIndex = 0;

        for (float beat = startBeat; beat <= endBeat; beat += beatStep)
        {
            PoseType pose = posePattern[noteIndex % posePattern.Length];

            bool makeHold =
                holdEvery > 0 &&
                holdDuration > 0f &&
                (noteIndex + 1) % holdEvery == 0;

            events.Add(
                makeHold
                    ? H(beat, pose, holdDuration)
                    : N(beat, pose)
            );

            noteIndex++;
        }
    }

    private static BeatEvent N(float beat, PoseType pose)
    {
        return new BeatEvent
        {
            beat = beat,
            requiredPose = pose,
            holePositionX = -999f,
            isHoldNote = false,
            holdDuration = 0f
        };
    }

    private static BeatEvent H(float beat, PoseType pose, float duration)
    {
        return new BeatEvent
        {
            beat = beat,
            requiredPose = pose,
            holePositionX = -999f,
            isHoldNote = true,
            holdDuration = duration
        };
    }

    private static BeatEvent BonusStart(float beat)
    {
        return new BeatEvent
        {
            beat = beat,
            isBonusAreaStart = true
        };
    }

    private static BeatEvent BonusEnd(float beat)
    {
        return new BeatEvent
        {
            beat = beat,
            isBonusAreaEnd = true
        };
    }

    private static void ApplyBeatmap(
        Beatmap beatmap,
        List<BeatEvent> events,
        string difficultyLabel)
    {
        Undo.RecordObject(
            beatmap,
            $"Cargar BeatMap Nivel 3 Strawberry Rush {difficultyLabel}"
        );

        events.Sort((a, b) => a.beat.CompareTo(b.beat));

        beatmap.bpm = Bpm;
        beatmap.startOffsetSeconds = StartOffsetSeconds;
        beatmap.events.Clear();
        beatmap.events.AddRange(events);

        EditorUtility.SetDirty(beatmap);
        AssetDatabase.SaveAssets();

        ValidateBeatmapDuration(beatmap, difficultyLabel);

        int playableNoteCount = 0;
        foreach (BeatEvent beatEvent in events)
        {
            if (!beatEvent.isBonusAreaStart && !beatEvent.isBonusAreaEnd)
                playableNoteCount++;
        }

        Debug.Log(
            $"[Level3Beatmap] Strawberry Rush {difficultyLabel}: " +
            $"{playableNoteCount} notas y {events.Count} eventos cargados | " +
            $"BPM {Bpm} | Duración objetivo {SongDurationSeconds:F0}s."
        );
    }

    private static void ValidateBeatmapDuration(
        Beatmap beatmap,
        string difficultyLabel)
    {
        float lastEventEnd = 0f;

        foreach (BeatEvent beatEvent in beatmap.events)
        {
            if (beatEvent.isBonusAreaStart || beatEvent.isBonusAreaEnd)
                continue;

            float eventEnd = beatmap.BeatToSeconds(beatEvent.beat);

            if (beatEvent.isHoldNote)
                eventEnd += beatEvent.holdDuration;

            lastEventEnd = Mathf.Max(lastEventEnd, eventEnd);
        }

        if (lastEventEnd > SongDurationSeconds)
        {
            Debug.LogWarning(
                $"[Level3Beatmap] {difficultyLabel}: el último evento termina " +
                $"en {lastEventEnd:F2}s, fuera de la duración objetivo " +
                $"de {SongDurationSeconds:F2}s."
            );
        }

        if (beatmap.song != null &&
            Mathf.Abs(beatmap.song.length - SongDurationSeconds) > 1f)
        {
            Debug.LogWarning(
                $"[Level3Beatmap] El AudioClip asignado a '{beatmap.name}' " +
                $"dura {beatmap.song.length:F2}s, pero este chart fue diseñado " +
                $"para aproximadamente {SongDurationSeconds:F2}s."
            );
        }
    }

    private static Beatmap FindBeatmapAsset(string assetName)
    {
        Beatmap beatmap = Selection.activeObject as Beatmap;

        if (beatmap == null || beatmap.name != assetName)
        {
            string[] guids = AssetDatabase.FindAssets(
                $"{assetName} t:Beatmap"
            );

            if (guids.Length > 0)
            {
                string path = AssetDatabase.GUIDToAssetPath(guids[0]);
                beatmap = AssetDatabase.LoadAssetAtPath<Beatmap>(path);
            }
        }

        if (beatmap == null)
        {
            Debug.LogError(
                $"[Level3Beatmap] No se encontró un Beatmap llamado " +
                $"'{assetName}'. Crealo desde Create > RhythmGame > BeatMap."
            );
        }

        return beatmap;
    }
#endif
}
