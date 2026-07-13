using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif


public static class Level1Beatmap
{
#if UNITY_EDITOR
    private const float Bpm = 120f;
    private const float StartOffsetSeconds = 3f;
    private const float SongDurationSeconds = 100f;

    [MenuItem("RhythmGame/Nivel 1 - Q&A/Cargar BeatMap Fácil")]
    private static void LoadLevel1Easy()
    {
        Beatmap beatmap = FindBeatmapAsset("Level1_Easy");
        if (beatmap == null) return;

        List<BeatEvent> events = new List<BeatEvent>
        {
            // ── INTRO: beats 1-20 ────────────────────────────────────────────
            N(5f,  PoseType.PoseA),
            N(9f,  PoseType.PoseC),
            N(13f, PoseType.PoseB),
            N(17f, PoseType.PoseD),

            // ── VERSO 1: beats 21-64 ─────────────────────────────────────────
            N(21f, PoseType.PoseA),
            N(25f, PoseType.PoseC),
            N(29f, PoseType.PoseB),
            N(33f, PoseType.PoseA),
            N(37f, PoseType.PoseD),
            N(41f, PoseType.PoseC),
            H(45f, PoseType.PoseA, 2f),
            N(51f, PoseType.PoseB),
            N(57f, PoseType.PoseC),
            N(63f, PoseType.PoseD),

            // ── CORO 1: beats 65-100 ─────────────────────────────────────────
            N(67f, PoseType.PoseA),
            N(73f, PoseType.PoseB),
            N(79f, PoseType.PoseC),
            N(85f, PoseType.PoseD),
            H(91f, PoseType.PoseA, 2f),
            N(97f, PoseType.PoseC),

            // ── VERSO 2: beats 101-128 ────────────────────────────────────────
            N(101f, PoseType.PoseB),
            N(107f, PoseType.PoseA),
            N(113f, PoseType.PoseD),
            N(119f, PoseType.PoseC),
            N(125f, PoseType.PoseB),

            // ── ÁREA BONUS: beats 129-145 ────────────────────────────────────
            BonusStart(129f),
            BonusEnd(145f),

            // ── CORO FINAL / OUTRO: beats 147-194 ─────────────────────────────
            N(147f, PoseType.PoseA),
            N(153f, PoseType.PoseC),
            N(159f, PoseType.PoseB),
            N(165f, PoseType.PoseD),
            N(171f, PoseType.PoseA),
            N(177f, PoseType.PoseC),
            N(183f, PoseType.PoseB),
            H(189f, PoseType.PoseD, 2f),
        };

        ApplyBeatmap(beatmap, events, "FÁCIL");
    }

    [MenuItem("RhythmGame/Nivel 1 - Q&A/Cargar BeatMap Normal")]
    private static void LoadLevel1Normal()
    {
        Beatmap beatmap = FindBeatmapAsset("Level1_Normal");
        if (beatmap == null) return;

        List<BeatEvent> events = new List<BeatEvent>
        {
            // ── INTRO: beats 1-20 ────────────────────────────────────────────
            N(5f,  PoseType.PoseA),
            N(9f,  PoseType.PoseC),
            N(13f, PoseType.PoseB),
            N(17f, PoseType.PoseD),

            // ── VERSO 1: beats 21-52 ─────────────────────────────────────────
            N(21f, PoseType.PoseA),
            N(25f, PoseType.PoseB),
            N(29f, PoseType.PoseC),
            N(33f, PoseType.PoseD),
            N(37f, PoseType.PoseAB),
            N(41f, PoseType.PoseC),
            H(45f, PoseType.PoseA, 1.5f),
            N(49f, PoseType.PoseB),

            // ── PRE-CORO: beats 53-64 ────────────────────────────────────────
            N(53f, PoseType.PoseD),
            N(57f, PoseType.PoseBC),
            N(61f, PoseType.PoseA),
            N(63f, PoseType.PoseCD),

            // ── CORO 1: beats 65-100 ─────────────────────────────────────────
            N(65f, PoseType.PoseA),
            N(69f, PoseType.PoseC),
            H(73f, PoseType.PoseB, 1.5f),
            N(77f, PoseType.PoseD),
            N(81f, PoseType.PoseAB),
            N(85f, PoseType.PoseC),
            N(89f, PoseType.PoseAD),
            N(93f, PoseType.PoseB),
            N(97f, PoseType.PoseBC),

            // ── VERSO 2: beats 101-128 ────────────────────────────────────────
            N(101f, PoseType.PoseC),
            N(105f, PoseType.PoseD),
            N(109f, PoseType.PoseA),
            N(113f, PoseType.PoseAB),
            N(117f, PoseType.PoseB),
            N(121f, PoseType.PoseCD),
            H(125f, PoseType.PoseA, 1.5f),

            // ── ÁREA BONUS: beats 129-145 ────────────────────────────────────
            BonusStart(129f),
            BonusEnd(145f),

            // ── CORO FINAL / OUTRO: beats 147-194 ─────────────────────────────
            N(147f, PoseType.PoseD),
            N(151f, PoseType.PoseAD),
            N(155f, PoseType.PoseA),
            N(159f, PoseType.PoseC),
            N(163f, PoseType.PoseBC),
            N(167f, PoseType.PoseB),
            N(171f, PoseType.PoseD),
            N(175f, PoseType.PoseAB),
            N(179f, PoseType.PoseC),
            N(183f, PoseType.PoseCD),
            H(189f, PoseType.PoseA, 2f),
        };

        ApplyBeatmap(beatmap, events, "NORMAL");
    }

    [MenuItem("RhythmGame/Nivel 1 - Q&A/Cargar BeatMap Difícil")]
    private static void LoadLevel1Hard()
    {
        Beatmap beatmap = FindBeatmapAsset("Level1_Hard");
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
            N(31f, PoseType.PoseCD),
            N(33f, PoseType.PoseB),
            N(35f, PoseType.PoseAB),
            N(37f, PoseType.PoseC),
            N(39f, PoseType.PoseAD),
            N(41f, PoseType.PoseD),
            N(43f, PoseType.PoseBC),
            H(45f, PoseType.PoseA, 0.75f),
            N(47f, PoseType.PoseCD),

            // ── PRE-CORO: beats 49-64 ────────────────────────────────────────
            N(49f, PoseType.PoseB),
            N(51f, PoseType.PoseD),
            N(53f, PoseType.PoseAB),
            N(55f, PoseType.PoseC),
            N(57f, PoseType.PoseAD),
            N(59f, PoseType.PoseB),
            N(61f, PoseType.PoseBC),
            N(63f, PoseType.PoseCD),

            // ── CORO 1: beats 65-96 ──────────────────────────────────────────
            N(65f, PoseType.PoseA),
            N(67f, PoseType.PoseAB),
            N(69f, PoseType.PoseC),
            N(71f, PoseType.PoseBC),
            H(73f, PoseType.PoseB, 0.75f),
            N(75f, PoseType.PoseCD),
            N(77f, PoseType.PoseD),
            N(79f, PoseType.PoseAD),
            N(81f, PoseType.PoseA),
            N(83f, PoseType.PoseAB),
            N(85f, PoseType.PoseC),
            N(87f, PoseType.PoseBC),
            H(89f, PoseType.PoseD, 0.75f),
            N(91f, PoseType.PoseCD),
            N(93f, PoseType.PoseB),
            N(95f, PoseType.PoseAD),

            // ── VERSO 2: beats 97-128 ────────────────────────────────────────
            N(97f, PoseType.PoseC),
            N(99f, PoseType.PoseA),
            N(101f, PoseType.PoseAB),
            N(103f, PoseType.PoseD),
            N(105f, PoseType.PoseBC),
            N(107f, PoseType.PoseB),
            N(109f, PoseType.PoseCD),
            N(111f, PoseType.PoseA),
            N(113f, PoseType.PoseAD),
            N(115f, PoseType.PoseC),
            N(117f, PoseType.PoseAB),
            N(119f, PoseType.PoseD),
            N(121f, PoseType.PoseBC),
            N(123f, PoseType.PoseB),
            H(125f, PoseType.PoseA, 0.75f),
            N(127f, PoseType.PoseCD),

            // ── ÁREA BONUS: beats 129-145 ────────────────────────────────────
            BonusStart(129f),
            BonusEnd(145f),

            // ── CORO FINAL / OUTRO: beats 147-194 ─────────────────────────────
            N(147f, PoseType.PoseA),
            N(149f, PoseType.PoseAB),
            N(151f, PoseType.PoseC),
            N(153f, PoseType.PoseBC),
            N(155f, PoseType.PoseB),
            N(157f, PoseType.PoseCD),
            N(159f, PoseType.PoseD),
            N(161f, PoseType.PoseAD),
            N(163f, PoseType.PoseA),
            N(165f, PoseType.PoseC),
            N(167f, PoseType.PoseAB),
            N(169f, PoseType.PoseD),
            N(171f, PoseType.PoseBC),
            N(173f, PoseType.PoseB),
            N(175f, PoseType.PoseCD),
            N(177f, PoseType.PoseA),
            N(179f, PoseType.PoseAD),
            N(181f, PoseType.PoseC),
            N(183f, PoseType.PoseAB),
            N(185f, PoseType.PoseD),
            N(187f, PoseType.PoseBC),
            H(190f, PoseType.PoseA, 1.5f),
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
            $"Cargar BeatMap Nivel 1 Q&A {difficultyLabel}"
        );

        beatmap.bpm = Bpm;
        beatmap.startOffsetSeconds = StartOffsetSeconds;
        beatmap.events.Clear();
        beatmap.events.AddRange(events);

        EditorUtility.SetDirty(beatmap);
        AssetDatabase.SaveAssets();

        ValidateBeatmapDuration(beatmap, difficultyLabel);

        Debug.Log(
            $"[Level1Beatmap] Q&A {difficultyLabel}: " +
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
                $"[Level1Beatmap] {difficultyLabel}: el último evento termina " +
                $"en {lastEventEnd:F2}s, fuera de la duración objetivo " +
                $"de {SongDurationSeconds:F2}s."
            );
        }

        if (beatmap.song != null &&
            Mathf.Abs(beatmap.song.length - SongDurationSeconds) > 1f)
        {
            Debug.LogWarning(
                $"[Level1Beatmap] El AudioClip asignado a '{beatmap.name}' " +
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
                $"[Level1Beatmap] No se encontró un Beatmap llamado " +
                $"'{assetName}'. Crealo desde Create > RhythmGame > BeatMap."
            );
        }

        return beatmap;
    }
#endif
}
