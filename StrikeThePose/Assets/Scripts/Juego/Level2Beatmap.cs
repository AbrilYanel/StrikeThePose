using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public static class Level2Beatmap
{
#if UNITY_EDITOR
    private const float Bpm = 142f;
    private const float StartOffsetSeconds = 2.5f;
    private const float SongDurationSeconds = 77f;

    [MenuItem("RhythmGame/Nivel 2 - Girl Front/Cargar BeatMap Fácil")]
    private static void LoadLevel2Easy()
    {
        Beatmap beatmap = FindBeatmapAsset("Level2_Easy");
        if (beatmap == null) return;

        List<BeatEvent> events = new List<BeatEvent>
        {
            // ── INTRO: beats 1-20 ────────────────────────────────────────────
            N(5f,  PoseType.PoseA),
            N(9f,  PoseType.PoseC),
            N(13f, PoseType.PoseB),
            N(17f, PoseType.PoseD),

            // ── VERSO 1: beats 21-48 ─────────────────────────────────────────
            N(21f, PoseType.PoseA),
            N(25f, PoseType.PoseC),
            N(29f, PoseType.PoseB),
            N(33f, PoseType.PoseD),
            H(37f, PoseType.PoseA, 1.5f),
            N(43f, PoseType.PoseC),
            N(49f, PoseType.PoseB),

            // ── PRE-CORO: beats 53-64 ────────────────────────────────────────
            N(53f, PoseType.PoseA),
            N(57f, PoseType.PoseD),
            N(61f, PoseType.PoseC),

            // ── CORO 1: beats 65-96 ─────────────────────────────────────────
            N(65f, PoseType.PoseA),
            N(69f, PoseType.PoseB),
            N(73f, PoseType.PoseC),
            N(77f, PoseType.PoseD),
            H(81f, PoseType.PoseA, 1.5f),
            N(87f, PoseType.PoseC),
            N(93f, PoseType.PoseB),

            // ── VERSO 2: beats 97-120 ────────────────────────────────────────
            N(97f, PoseType.PoseD),
            N(101f, PoseType.PoseA),
            N(105f, PoseType.PoseC),
            N(109f, PoseType.PoseB),
            H(113f, PoseType.PoseD, 1.5f),
            N(119f, PoseType.PoseA),

            // ── ÁREA BONUS: beats 121-137 ────────────────────────────────────
            BonusStart(121f),
            BonusEnd(137f),

            // ── CORO FINAL / OUTRO: beats 139-176 ─────────────────────────────
            N(139f, PoseType.PoseA),
            N(143f, PoseType.PoseC),
            N(147f, PoseType.PoseB),
            N(151f, PoseType.PoseD),
            N(155f, PoseType.PoseA),
            N(159f, PoseType.PoseC),
            N(163f, PoseType.PoseB),
            N(167f, PoseType.PoseD),
            H(171f, PoseType.PoseA, 1.8f),
        };

        ApplyBeatmap(beatmap, events, "FÁCIL");
    }

    [MenuItem("RhythmGame/Nivel 2 - Girl Front/Cargar BeatMap Normal")]
    private static void LoadLevel2Normal()
    {
        Beatmap beatmap = FindBeatmapAsset("Level2_Normal");
        if (beatmap == null) return;

        List<BeatEvent> events = new List<BeatEvent>
        {
            // ── INTRO: beats 1-20 ────────────────────────────────────────────
            N(5f,  PoseType.PoseA),
            N(9f,  PoseType.PoseC),
            N(13f, PoseType.PoseB),
            N(17f, PoseType.PoseD),

            // ── VERSO 1: beats 21-48 ─────────────────────────────────────────
            N(21f, PoseType.PoseA),
            N(24f, PoseType.PoseB),
            N(27f, PoseType.PoseC),
            N(30f, PoseType.PoseD),
            N(33f, PoseType.PoseAB),
            N(36f, PoseType.PoseC),
            H(39f, PoseType.PoseA, 1.2f),
            N(43f, PoseType.PoseB),
            N(46f, PoseType.PoseD),

            // ── PRE-CORO: beats 49-64 ────────────────────────────────────────
            N(49f, PoseType.PoseBC),
            N(52f, PoseType.PoseA),
            N(55f, PoseType.PoseCD),
            N(58f, PoseType.PoseB),
            N(61f, PoseType.PoseAD),
            N(64f, PoseType.PoseC),

            // ── CORO 1: beats 65-96 ─────────────────────────────────────────
            N(67f, PoseType.PoseA),
            N(70f, PoseType.PoseAB),
            N(73f, PoseType.PoseC),
            H(76f, PoseType.PoseB, 1.2f),
            N(80f, PoseType.PoseD),
            N(83f, PoseType.PoseBC),
            N(86f, PoseType.PoseA),
            N(89f, PoseType.PoseAD),
            N(92f, PoseType.PoseC),
            N(95f, PoseType.PoseB),

            // ── VERSO 2: beats 97-120 ────────────────────────────────────────
            N(98f, PoseType.PoseD),
            N(101f, PoseType.PoseAB),
            N(104f, PoseType.PoseC),
            N(107f, PoseType.PoseBC),
            N(110f, PoseType.PoseA),
            H(113f, PoseType.PoseD, 1.2f),
            N(117f, PoseType.PoseB),
            N(120f, PoseType.PoseCD),

            // ── ÁREA BONUS: beats 121-137 ────────────────────────────────────
            BonusStart(121f),
            BonusEnd(137f),

            // ── CORO FINAL / OUTRO: beats 139-176 ─────────────────────────────
            N(139f, PoseType.PoseA),
            N(142f, PoseType.PoseC),
            N(145f, PoseType.PoseAB),
            N(148f, PoseType.PoseD),
            N(151f, PoseType.PoseBC),
            N(154f, PoseType.PoseB),
            N(157f, PoseType.PoseAD),
            N(160f, PoseType.PoseC),
            N(163f, PoseType.PoseCD),
            N(166f, PoseType.PoseA),
            H(171f, PoseType.PoseD, 1.8f),
        };

        ApplyBeatmap(beatmap, events, "NORMAL");
    }

    [MenuItem("RhythmGame/Nivel 2 - Girl Front/Cargar BeatMap Difícil")]
    private static void LoadLevel2Hard()
    {
        Beatmap beatmap = FindBeatmapAsset("Level2_Hard");
        if (beatmap == null) return;

        List<BeatEvent> events = new List<BeatEvent>
        {
            // ── INTRO: beats 1-16 ────────────────────────────────────────────
            N(5f,  PoseType.PoseA),
            N(7f,  PoseType.PoseC),
            N(9f,  PoseType.PoseB),
            N(11f, PoseType.PoseD),
            N(13f, PoseType.PoseAB),
            N(15f, PoseType.PoseCD),

            // ── VERSO 1: beats 17-48 ─────────────────────────────────────────
            N(17f, PoseType.PoseA),
            N(19f, PoseType.PoseB),
            N(21f, PoseType.PoseC),
            N(23f, PoseType.PoseD),
            N(25f, PoseType.PoseAD),
            N(27f, PoseType.PoseBC),
            N(29f, PoseType.PoseA),
            H(31f, PoseType.PoseCD, 0.6f),
            N(33f, PoseType.PoseB),
            N(35f, PoseType.PoseAB),
            N(37f, PoseType.PoseC),
            N(39f, PoseType.PoseAD),
            N(41f, PoseType.PoseD),
            N(43f, PoseType.PoseBC),
            N(45f, PoseType.PoseA),
            N(47f, PoseType.PoseCD),

            // ── PRE-CORO: beats 49-64 ────────────────────────────────────────
            N(49f, PoseType.PoseB),
            N(51f, PoseType.PoseD),
            N(53f, PoseType.PoseAB),
            N(55f, PoseType.PoseC),
            N(57f, PoseType.PoseAD),
            N(59f, PoseType.PoseB),
            H(61f, PoseType.PoseBC, 0.6f),
            N(63f, PoseType.PoseCD),

            // ── CORO 1: beats 65-96 ─────────────────────────────────────────
            N(65f, PoseType.PoseA),
            N(67f, PoseType.PoseAB),
            N(69f, PoseType.PoseC),
            N(71f, PoseType.PoseBC),
            N(73f, PoseType.PoseB),
            N(75f, PoseType.PoseCD),
            H(77f, PoseType.PoseD, 0.6f),
            N(79f, PoseType.PoseAD),
            N(81f, PoseType.PoseA),
            N(83f, PoseType.PoseAB),
            N(85f, PoseType.PoseC),
            N(87f, PoseType.PoseBC),
            N(89f, PoseType.PoseB),
            N(91f, PoseType.PoseCD),
            H(93f, PoseType.PoseD, 0.6f),
            N(95f, PoseType.PoseAD),

            // ── VERSO 2: beats 97-120 ────────────────────────────────────────
            N(97f, PoseType.PoseC),
            N(99f, PoseType.PoseA),
            N(101f, PoseType.PoseAB),
            N(103f, PoseType.PoseD),
            N(105f, PoseType.PoseBC),
            N(107f, PoseType.PoseB),
            N(109f, PoseType.PoseCD),
            N(111f, PoseType.PoseA),
            H(113f, PoseType.PoseAD, 0.6f),
            N(115f, PoseType.PoseC),
            N(117f, PoseType.PoseAB),
            N(119f, PoseType.PoseD),

            // ── ÁREA BONUS: beats 121-137 ────────────────────────────────────
            BonusStart(121f),
            BonusEnd(137f),

            // ── CORO FINAL / OUTRO: beats 139-176 ─────────────────────────────
            N(139f, PoseType.PoseA),
            N(141f, PoseType.PoseAB),
            N(143f, PoseType.PoseC),
            N(145f, PoseType.PoseBC),
            N(147f, PoseType.PoseB),
            N(149f, PoseType.PoseCD),
            N(151f, PoseType.PoseD),
            N(153f, PoseType.PoseAD),
            N(155f, PoseType.PoseA),
            H(157f, PoseType.PoseC, 0.6f),
            N(159f, PoseType.PoseAB),
            N(161f, PoseType.PoseD),
            N(163f, PoseType.PoseBC),
            N(165f, PoseType.PoseB),
            N(167f, PoseType.PoseCD),
            N(169f, PoseType.PoseAD),
            N(171f, PoseType.PoseD),
            H(173f, PoseType.PoseA, 1f),
        };

        ApplyBeatmap(beatmap, events, "DIFÍCIL");
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
            $"Cargar BeatMap Nivel 2 Girl Front {difficultyLabel}"
        );

        beatmap.bpm = Bpm;
        beatmap.startOffsetSeconds = StartOffsetSeconds;
        beatmap.events.Clear();
        beatmap.events.AddRange(events);

        EditorUtility.SetDirty(beatmap);
        AssetDatabase.SaveAssets();

        ValidateBeatmapDuration(beatmap, difficultyLabel);

        Debug.Log(
            $"[Level2Beatmap] Girl Front {difficultyLabel}: " +
            $"{events.Count} eventos cargados | BPM {Bpm} | " +
            $"Duración objetivo {SongDurationSeconds:F0}s."
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
                $"[Level2Beatmap] {difficultyLabel}: el último evento termina " +
                $"en {lastEventEnd:F2}s, fuera de la duración objetivo " +
                $"de {SongDurationSeconds:F2}s."
            );
        }

        if (beatmap.song != null &&
            Mathf.Abs(beatmap.song.length - SongDurationSeconds) > 1f)
        {
            Debug.LogWarning(
                $"[Level2Beatmap] El AudioClip asignado a '{beatmap.name}' " +
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
                $"[Level2Beatmap] No se encontró un Beatmap llamado " +
                $"'{assetName}'. Crealo desde Create > RhythmGame > BeatMap."
            );
        }

        return beatmap;
    }
#endif
}
