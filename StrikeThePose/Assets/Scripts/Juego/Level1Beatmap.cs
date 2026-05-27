using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public static class Level1Beatmap
{
#if UNITY_EDITOR
    [MenuItem("RhythmGame/Nivel 1/Cargar BeatMap Fácil")]
    private static void LoadLevel1Easy()
    {
        Beatmap bm = FindBeatmapAsset("Level1_Easy");
        if (bm == null) return;

        Undo.RecordObject(bm, "Cargar BeatMap Nivel 1 Fácil");

        bm.bpm = 120f;
        bm.startOffsetSeconds = 3.0f;

        // ─── EVENTOS FÁCIL (20 obstáculos - 100% simples, gran espaciado) ───────
        bm.events.Clear();
        bm.events.AddRange(new BeatEvent[]
        {
            // ── INTRO
            new BeatEvent { beat =   7f, requiredPose = PoseType.PoseA,  holePositionX = -999f },
            new BeatEvent { beat =  13f, requiredPose = PoseType.PoseC,  holePositionX = -999f },
 
            // ── VERSO 1 — cada 12 beats, solo simples
            new BeatEvent { beat =  15f, requiredPose = PoseType.PoseA,  holePositionX = -999f },
            new BeatEvent { beat =  27f, requiredPose = PoseType.PoseC,  holePositionX = -999f },
            new BeatEvent { beat =  39f, requiredPose = PoseType.PoseA,  holePositionX = -999f },
            new BeatEvent { beat =  51f, requiredPose = PoseType.PoseB,  holePositionX = -999f },
 
            // ── PRE-CORO — cada 8 beats, solo simples
            new BeatEvent { beat =  55f, requiredPose = PoseType.PoseA,  holePositionX = -999f },
            new BeatEvent { beat =  63f, requiredPose = PoseType.PoseB,  holePositionX = -999f },
            new BeatEvent { beat =  71f, requiredPose = PoseType.PoseA,  holePositionX = -999f },
 
            // ── CORO 1 — cada 12 beats, solo simples
            new BeatEvent { beat =  75f, requiredPose = PoseType.PoseA,  holePositionX = -999f },
            new BeatEvent { beat =  87f, requiredPose = PoseType.PoseB,  holePositionX = -999f },
            new BeatEvent { beat =  99f, requiredPose = PoseType.PoseA,  holePositionX = -999f },
            new BeatEvent { beat = 111f, requiredPose = PoseType.PoseC,  holePositionX = -999f }, // Cambiado combo a simple
 
            // ── PUENTE — cada 10 beats, solo simples
            new BeatEvent { beat = 115f, requiredPose = PoseType.PoseA,  holePositionX = -999f },
            new BeatEvent { beat = 125f, requiredPose = PoseType.PoseC,  holePositionX = -999f },
            new BeatEvent { beat = 135f, requiredPose = PoseType.PoseD,  holePositionX = -999f },
            new BeatEvent { beat = 145f, requiredPose = PoseType.PoseB,  holePositionX = -999f },
 
            // ── OUTRO — cada 8 beats, solo simples
            new BeatEvent { beat = 155f, requiredPose = PoseType.PoseA,  holePositionX = -999f },
            new BeatEvent { beat = 163f, requiredPose = PoseType.PoseC,  holePositionX = -999f },
            new BeatEvent { beat = 171f, requiredPose = PoseType.PoseD,  holePositionX = -999f },
            new BeatEvent { beat = 179f, requiredPose = PoseType.PoseA,  holePositionX = -999f }
        });

        EditorUtility.SetDirty(bm);
        AssetDatabase.SaveAssets();
        Debug.Log($"[Level1Loader] {bm.events.Count} eventos cargados en FÁCIL ('{bm.name}'). ✓");
    }

    [MenuItem("RhythmGame/Nivel 1/Cargar BeatMap Normal")]
    private static void LoadLevel1Normal()
    {
        Beatmap bm = FindBeatmapAsset("Level1_Normal");
        if (bm == null) return;

        Undo.RecordObject(bm, "Cargar BeatMap Nivel 1 Normal");

        bm.bpm = 120f;
        bm.startOffsetSeconds = 3.0f;

        // ─── EVENTOS NORMAL (El original del usuario: 40 obstáculos - mezcla equilibrada) ───
        bm.events.Clear();
        bm.events.AddRange(new BeatEvent[]
        {
            // ── INTRO
            new BeatEvent { beat =   7f, requiredPose = PoseType.PoseA,  holePositionX = -999f },
            new BeatEvent { beat =  13f, requiredPose = PoseType.PoseC,  holePositionX = -999f },
 
            // ── VERSO 1 — cada 6 beats, solo simples
            new BeatEvent { beat =  15f, requiredPose = PoseType.PoseA,  holePositionX = -999f },
            new BeatEvent { beat =  21f, requiredPose = PoseType.PoseB,  holePositionX = -999f },
            new BeatEvent { beat =  27f, requiredPose = PoseType.PoseC,  holePositionX = -999f },
            new BeatEvent { beat =  33f, requiredPose = PoseType.PoseD,  holePositionX = -999f },
            new BeatEvent { beat =  39f, requiredPose = PoseType.PoseA,  holePositionX = -999f },
            new BeatEvent { beat =  45f, requiredPose = PoseType.PoseC,  holePositionX = -999f },
            new BeatEvent { beat =  51f, requiredPose = PoseType.PoseB,  holePositionX = -999f },
 
            // ── PRE-CORO — cada 4 beats, solo simples
            new BeatEvent { beat =  55f, requiredPose = PoseType.PoseA,  holePositionX = -999f },
            new BeatEvent { beat =  59f, requiredPose = PoseType.PoseD,  holePositionX = -999f },
            new BeatEvent { beat =  63f, requiredPose = PoseType.PoseB,  holePositionX = -999f },
            new BeatEvent { beat =  67f, requiredPose = PoseType.PoseC,  holePositionX = -999f },
            new BeatEvent { beat =  71f, requiredPose = PoseType.PoseA,  holePositionX = -999f },
 
            // ── CORO 1 — cada 6 beats, 4 simples de entrada luego simple+combo
            new BeatEvent { beat =  75f, requiredPose = PoseType.PoseA,  holePositionX = -999f },
            new BeatEvent { beat =  81f, requiredPose = PoseType.PoseC,  holePositionX = -999f },
            new BeatEvent { beat =  87f, requiredPose = PoseType.PoseB,  holePositionX = -999f },
            new BeatEvent { beat =  93f, requiredPose = PoseType.PoseD,  holePositionX = -999f },
            new BeatEvent { beat =  99f, requiredPose = PoseType.PoseA,  holePositionX = -999f },
            new BeatEvent { beat = 105f, requiredPose = PoseType.PoseAB, holePositionX = -999f }, // W+A
            new BeatEvent { beat = 111f, requiredPose = PoseType.PoseBC, holePositionX = -999f }, // A+S
 
            // ── PUENTE — cada 5 beats, mezcla suave
            new BeatEvent { beat = 115f, requiredPose = PoseType.PoseA,  holePositionX = -999f },
            new BeatEvent { beat = 120f, requiredPose = PoseType.PoseAB, holePositionX = -999f },
            new BeatEvent { beat = 125f, requiredPose = PoseType.PoseC,  holePositionX = -999f },
            new BeatEvent { beat = 130f, requiredPose = PoseType.PoseBC, holePositionX = -999f },
            new BeatEvent { beat = 135f, requiredPose = PoseType.PoseD,  holePositionX = -999f },
            new BeatEvent { beat = 140f, requiredPose = PoseType.PoseAD, holePositionX = -999f },
            new BeatEvent { beat = 145f, requiredPose = PoseType.PoseB,  holePositionX = -999f },
            new BeatEvent { beat = 150f, requiredPose = PoseType.PoseA,  holePositionX = -999f },
 
            // ── OUTRO — cada 4 beats, alternando simple+combo, cierre simple
            new BeatEvent { beat = 155f, requiredPose = PoseType.PoseA,  holePositionX = -999f },
            new BeatEvent { beat = 159f, requiredPose = PoseType.PoseAD, holePositionX = -999f },
            new BeatEvent { beat = 163f, requiredPose = PoseType.PoseC,  holePositionX = -999f },
            new BeatEvent { beat = 167f, requiredPose = PoseType.PoseBC, holePositionX = -999f },
            new BeatEvent { beat = 171f, requiredPose = PoseType.PoseD,  holePositionX = -999f },
            new BeatEvent { beat = 175f, requiredPose = PoseType.PoseAB, holePositionX = -999f },
            new BeatEvent { beat = 179f, requiredPose = PoseType.PoseA,  holePositionX = -999f },
            new BeatEvent { beat = 183f, requiredPose = PoseType.PoseC,  holePositionX = -999f }
        });

        EditorUtility.SetDirty(bm);
        AssetDatabase.SaveAssets();
        Debug.Log($"[Level1Loader] {bm.events.Count} eventos cargados en NORMAL ('{bm.name}'). ✓");
    }

    [MenuItem("RhythmGame/Nivel 1/Cargar BeatMap Difícil")]
    private static void LoadLevel1Hard()
    {
        Beatmap bm = FindBeatmapAsset("Level1_Hard");
        if (bm == null) return;

        Undo.RecordObject(bm, "Cargar BeatMap Nivel 1 Difícil");

        bm.bpm = 120f;
        bm.startOffsetSeconds = 3.0f;

        // ─── EVENTOS DIFÍCIL (65 obstáculos - alta densidad, muchas combinaciones, ritmos rápidos) ───
        bm.events.Clear();
        bm.events.AddRange(new BeatEvent[]
        {
            // ── INTRO (Espaciado normal, calentamiento)
            new BeatEvent { beat =   4f, requiredPose = PoseType.PoseA,  holePositionX = -999f },
            new BeatEvent { beat =   7f, requiredPose = PoseType.PoseC,  holePositionX = -999f },
            new BeatEvent { beat =  10f, requiredPose = PoseType.PoseB,  holePositionX = -999f },
            new BeatEvent { beat =  13f, requiredPose = PoseType.PoseD,  holePositionX = -999f },
 
            // ── VERSO 1 — cada 4 beats (ritmo acelerado)
            new BeatEvent { beat =  15f, requiredPose = PoseType.PoseA,  holePositionX = -999f },
            new BeatEvent { beat =  19f, requiredPose = PoseType.PoseB,  holePositionX = -999f },
            new BeatEvent { beat =  23f, requiredPose = PoseType.PoseC,  holePositionX = -999f },
            new BeatEvent { beat =  27f, requiredPose = PoseType.PoseD,  holePositionX = -999f },
            new BeatEvent { beat =  31f, requiredPose = PoseType.PoseA,  holePositionX = -999f },
            new BeatEvent { beat =  35f, requiredPose = PoseType.PoseC,  holePositionX = -999f },
            new BeatEvent { beat =  39f, requiredPose = PoseType.PoseB,  holePositionX = -999f },
            new BeatEvent { beat =  43f, requiredPose = PoseType.PoseAB, holePositionX = -999f }, // Combo!
            new BeatEvent { beat =  47f, requiredPose = PoseType.PoseCD, holePositionX = -999f }, // Combo!
            new BeatEvent { beat =  51f, requiredPose = PoseType.PoseBC, holePositionX = -999f }, // Combo!
 
            // ── PRE-CORO — ráfagas veloces cada 2 beats! (Exige reacción rápida)
            new BeatEvent { beat =  55f, requiredPose = PoseType.PoseA,  holePositionX = -999f },
            new BeatEvent { beat =  57f, requiredPose = PoseType.PoseD,  holePositionX = -999f },
            new BeatEvent { beat =  59f, requiredPose = PoseType.PoseB,  holePositionX = -999f },
            new BeatEvent { beat =  61f, requiredPose = PoseType.PoseC,  holePositionX = -999f },
            new BeatEvent { beat =  63f, requiredPose = PoseType.PoseAD, holePositionX = -999f }, // Combo!
            new BeatEvent { beat =  65f, requiredPose = PoseType.PoseBC, holePositionX = -999f }, // Combo!
            new BeatEvent { beat =  67f, requiredPose = PoseType.PoseA,  holePositionX = -999f },
            new BeatEvent { beat =  69f, requiredPose = PoseType.PoseC,  holePositionX = -999f },
            new BeatEvent { beat =  71f, requiredPose = PoseType.PoseCD, holePositionX = -999f }, // Combo!
            new BeatEvent { beat =  73f, requiredPose = PoseType.PoseAB, holePositionX = -999f }, // Combo!
 
            // ── CORO 1 — cada 3 beats, alta intensidad de combo poses
            new BeatEvent { beat =  75f, requiredPose = PoseType.PoseA,  holePositionX = -999f },
            new BeatEvent { beat =  78f, requiredPose = PoseType.PoseAB, holePositionX = -999f },
            new BeatEvent { beat =  81f, requiredPose = PoseType.PoseC,  holePositionX = -999f },
            new BeatEvent { beat =  84f, requiredPose = PoseType.PoseBC, holePositionX = -999f },
            new BeatEvent { beat =  87f, requiredPose = PoseType.PoseB,  holePositionX = -999f },
            new BeatEvent { beat =  90f, requiredPose = PoseType.PoseCD, holePositionX = -999f },
            new BeatEvent { beat =  93f, requiredPose = PoseType.PoseD,  holePositionX = -999f },
            new BeatEvent { beat =  96f, requiredPose = PoseType.PoseAD, holePositionX = -999f },
            new BeatEvent { beat =  99f, requiredPose = PoseType.PoseA,  holePositionX = -999f },
            new BeatEvent { beat = 102f, requiredPose = PoseType.PoseAB, holePositionX = -999f },
            new BeatEvent { beat = 105f, requiredPose = PoseType.PoseBC, holePositionX = -999f },
            new BeatEvent { beat = 108f, requiredPose = PoseType.PoseCD, holePositionX = -999f },
            new BeatEvent { beat = 111f, requiredPose = PoseType.PoseAD, holePositionX = -999f },
            new BeatEvent { beat = 114f, requiredPose = PoseType.PoseCD, holePositionX = -999f },
 
            // ── PUENTE — cada 4 beats, mezcla de combos continuos
            new BeatEvent { beat = 115f, requiredPose = PoseType.PoseA,  holePositionX = -999f },
            new BeatEvent { beat = 119f, requiredPose = PoseType.PoseAB, holePositionX = -999f },
            new BeatEvent { beat = 123f, requiredPose = PoseType.PoseC,  holePositionX = -999f },
            new BeatEvent { beat = 127f, requiredPose = PoseType.PoseBC, holePositionX = -999f },
            new BeatEvent { beat = 131f, requiredPose = PoseType.PoseD,  holePositionX = -999f },
            new BeatEvent { beat = 135f, requiredPose = PoseType.PoseCD, holePositionX = -999f },
            new BeatEvent { beat = 139f, requiredPose = PoseType.PoseAD, holePositionX = -999f },
            new BeatEvent { beat = 143f, requiredPose = PoseType.PoseBC, holePositionX = -999f },
            new BeatEvent { beat = 147f, requiredPose = PoseType.PoseAB, holePositionX = -999f },
            new BeatEvent { beat = 151f, requiredPose = PoseType.PoseAD, holePositionX = -999f },
 
            // ── OUTRO — cada 2 beats (final frenético)
            new BeatEvent { beat = 155f, requiredPose = PoseType.PoseA,  holePositionX = -999f },
            new BeatEvent { beat = 157f, requiredPose = PoseType.PoseAD, holePositionX = -999f },
            new BeatEvent { beat = 159f, requiredPose = PoseType.PoseC,  holePositionX = -999f },
            new BeatEvent { beat = 161f, requiredPose = PoseType.PoseCD, holePositionX = -999f },
            new BeatEvent { beat = 163f, requiredPose = PoseType.PoseB,  holePositionX = -999f },
            new BeatEvent { beat = 165f, requiredPose = PoseType.PoseBC, holePositionX = -999f },
            new BeatEvent { beat = 167f, requiredPose = PoseType.PoseD,  holePositionX = -999f },
            new BeatEvent { beat = 169f, requiredPose = PoseType.PoseAB, holePositionX = -999f },
            new BeatEvent { beat = 171f, requiredPose = PoseType.PoseA,  holePositionX = -999f },
            new BeatEvent { beat = 173f, requiredPose = PoseType.PoseAD, holePositionX = -999f },
            new BeatEvent { beat = 175f, requiredPose = PoseType.PoseCD, holePositionX = -999f },
            new BeatEvent { beat = 177f, requiredPose = PoseType.PoseBC, holePositionX = -999f },
            new BeatEvent { beat = 179f, requiredPose = PoseType.PoseA,  holePositionX = -999f },
            new BeatEvent { beat = 181f, requiredPose = PoseType.PoseCD, holePositionX = -999f },
            new BeatEvent { beat = 183f, requiredPose = PoseType.PoseAB, holePositionX = -999f }
        });

        EditorUtility.SetDirty(bm);
        AssetDatabase.SaveAssets();
        Debug.Log($"[Level1Loader] {bm.events.Count} eventos cargados en DIFÍCIL ('{bm.name}'). ✓");
    }

    private static Beatmap FindBeatmapAsset(string assetName)
    {
        Beatmap bm = Selection.activeObject as Beatmap;
        if (bm == null || bm.name != assetName)
        {
            string[] guids = AssetDatabase.FindAssets($"{assetName} t:Beatmap");
            if (guids.Length > 0)
            {
                bm = AssetDatabase.LoadAssetAtPath<Beatmap>(AssetDatabase.GUIDToAssetPath(guids[0]));
            }
        }

        if (bm == null)
        {
            Debug.LogError($"[Level1Loader] No se encontró un Beatmap llamado '{assetName}'.\n" +
                           $"Por favor, crea un Beatmap (clic derecho > Create > RhythmGame > BeatMap), nómbralo '{assetName}' y vuelve a intentarlo.");
        }
        return bm;
    }
#endif
}
