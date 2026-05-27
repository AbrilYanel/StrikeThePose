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

        bm.events.Clear();
        bm.events.AddRange(new BeatEvent[]
        {
            // ── INTRO
            new BeatEvent { beat =   7f, requiredPose = PoseType.PoseA,  holePositionX = -999f, isHoldNote = true, holdDuration = 2.0f }, // Nota larga (Suficiente espacio hasta beat 13)
            new BeatEvent { beat =  13f, requiredPose = PoseType.PoseC,  holePositionX = -999f },
 
            // ── VERSO 1 (Notas muy espaciadas de 12 beats, ideales para notas sostenidas sin colisión)
            new BeatEvent { beat =  15f, requiredPose = PoseType.PoseA,  holePositionX = -999f, isHoldNote = true, holdDuration = 2.5f }, // Nota larga
            new BeatEvent { beat =  27f, requiredPose = PoseType.PoseC,  holePositionX = -999f },
            new BeatEvent { beat =  39f, requiredPose = PoseType.PoseA,  holePositionX = -999f, isHoldNote = true, holdDuration = 2.5f }, // Nota larga
            new BeatEvent { beat =  51f, requiredPose = PoseType.PoseB,  holePositionX = -999f },
 
            // ── PRE-CORO
            new BeatEvent { beat =  55f, requiredPose = PoseType.PoseA,  holePositionX = -999f },
            new BeatEvent { beat =  63f, requiredPose = PoseType.PoseB,  holePositionX = -999f },
            new BeatEvent { beat =  71f, requiredPose = PoseType.PoseA,  holePositionX = -999f },
 
            // ── CORO 1
            new BeatEvent { beat =  75f, requiredPose = PoseType.PoseA,  holePositionX = -999f, isHoldNote = true, holdDuration = 2.0f }, // Nota larga
            new BeatEvent { beat =  87f, requiredPose = PoseType.PoseB,  holePositionX = -999f },
            new BeatEvent { beat =  99f, requiredPose = PoseType.PoseA,  holePositionX = -999f },
            new BeatEvent { beat = 111f, requiredPose = PoseType.PoseC,  holePositionX = -999f },
 
            // ── PUENTE: ÁREA BONUS (Del beat 115 al 130)
            new BeatEvent { beat = 115f, isBonusAreaStart = true },
            new BeatEvent { beat = 130f, isBonusAreaEnd = true },
            new BeatEvent { beat = 135f, requiredPose = PoseType.PoseD,  holePositionX = -999f, isHoldNote = true, holdDuration = 2.0f }, // Nota larga
            new BeatEvent { beat = 145f, requiredPose = PoseType.PoseB,  holePositionX = -999f },
 
            // ── OUTRO
            new BeatEvent { beat = 155f, requiredPose = PoseType.PoseA,  holePositionX = -999f },
            new BeatEvent { beat = 163f, requiredPose = PoseType.PoseC,  holePositionX = -999f },
            new BeatEvent { beat = 171f, requiredPose = PoseType.PoseD,  holePositionX = -999f },
            new BeatEvent { beat = 179f, requiredPose = PoseType.PoseA,  holePositionX = -999f, isHoldNote = true, holdDuration = 3.0f }  // Cierre dramático con nota larga
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

        bm.events.Clear();
        bm.events.AddRange(new BeatEvent[]
        {
            // ── INTRO
            new BeatEvent { beat =   7f, requiredPose = PoseType.PoseA,  holePositionX = -999f, isHoldNote = true, holdDuration = 2.0f }, // Nota larga (Espacio amplio hasta beat 13)
            new BeatEvent { beat =  13f, requiredPose = PoseType.PoseC,  holePositionX = -999f },
 
            // ── VERSO 1 (Espaciado de 6 beats = 3.0 segundos, excelente para notas sostenidas de 1.5 segundos)
            new BeatEvent { beat =  15f, requiredPose = PoseType.PoseA,  holePositionX = -999f, isHoldNote = true, holdDuration = 1.5f }, // Nota larga (Termina antes del beat 21)
            new BeatEvent { beat =  21f, requiredPose = PoseType.PoseB,  holePositionX = -999f },
            new BeatEvent { beat =  27f, requiredPose = PoseType.PoseC,  holePositionX = -999f, isHoldNote = true, holdDuration = 1.5f }, // Nota larga (Termina antes del beat 33)
            new BeatEvent { beat =  33f, requiredPose = PoseType.PoseD,  holePositionX = -999f },
            new BeatEvent { beat =  39f, requiredPose = PoseType.PoseA,  holePositionX = -999f },
            new BeatEvent { beat =  45f, requiredPose = PoseType.PoseC,  holePositionX = -999f },
            new BeatEvent { beat =  51f, requiredPose = PoseType.PoseB,  holePositionX = -999f },
 
            // ── PRE-CORO (Construcción de tensión, ráfagas cortas)
            new BeatEvent { beat =  55f, requiredPose = PoseType.PoseA,  holePositionX = -999f },
            new BeatEvent { beat =  59f, requiredPose = PoseType.PoseD,  holePositionX = -999f },
            new BeatEvent { beat =  63f, requiredPose = PoseType.PoseB,  holePositionX = -999f },
            new BeatEvent { beat =  67f, requiredPose = PoseType.PoseC,  holePositionX = -999f },
            new BeatEvent { beat =  71f, requiredPose = PoseType.PoseA,  holePositionX = -999f },
 
            // ── CORO 1 (Espaciado de 6 beats)
            new BeatEvent { beat =  75f, requiredPose = PoseType.PoseA,  holePositionX = -999f, isHoldNote = true, holdDuration = 2.0f }, // Nota larga (Termina en beat 79)
            new BeatEvent { beat =  81f, requiredPose = PoseType.PoseC,  holePositionX = -999f },
            new BeatEvent { beat =  87f, requiredPose = PoseType.PoseB,  holePositionX = -999f, isHoldNote = true, holdDuration = 2.0f }, // Nota larga (Termina en beat 91)
            new BeatEvent { beat =  93f, requiredPose = PoseType.PoseD,  holePositionX = -999f },
            new BeatEvent { beat =  99f, requiredPose = PoseType.PoseA,  holePositionX = -999f },
            new BeatEvent { beat = 105f, requiredPose = PoseType.PoseAB, holePositionX = -999f },
            new BeatEvent { beat = 111f, requiredPose = PoseType.PoseBC, holePositionX = -999f },
 
            // ── PUENTE: ÁREA BONUS (Del beat 115 al 130)
            new BeatEvent { beat = 115f, isBonusAreaStart = true },
            new BeatEvent { beat = 130f, isBonusAreaEnd = true },
            new BeatEvent { beat = 135f, requiredPose = PoseType.PoseD,  holePositionX = -999f, isHoldNote = true, holdDuration = 1.5f }, // Nota larga
            new BeatEvent { beat = 140f, requiredPose = PoseType.PoseAD, holePositionX = -999f },
            new BeatEvent { beat = 145f, requiredPose = PoseType.PoseB,  holePositionX = -999f },
            new BeatEvent { beat = 150f, requiredPose = PoseType.PoseA,  holePositionX = -999f },
 
            // ── OUTRO
            new BeatEvent { beat = 155f, requiredPose = PoseType.PoseA,  holePositionX = -999f },
            new BeatEvent { beat = 159f, requiredPose = PoseType.PoseAD, holePositionX = -999f },
            new BeatEvent { beat = 163f, requiredPose = PoseType.PoseC,  holePositionX = -999f },
            new BeatEvent { beat = 167f, requiredPose = PoseType.PoseBC, holePositionX = -999f },
            new BeatEvent { beat = 171f, requiredPose = PoseType.PoseD,  holePositionX = -999f },
            new BeatEvent { beat = 175f, requiredPose = PoseType.PoseAB, holePositionX = -999f },
            new BeatEvent { beat = 179f, requiredPose = PoseType.PoseA,  holePositionX = -999f },
            new BeatEvent { beat = 183f, requiredPose = PoseType.PoseC,  holePositionX = -999f, isHoldNote = true, holdDuration = 3.0f }  // Final sostenido espectacular
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

        bm.events.Clear();
        bm.events.AddRange(new BeatEvent[]
        {
            // ── INTRO
            new BeatEvent { beat =   4f, requiredPose = PoseType.PoseA,  holePositionX = -999f, isHoldNote = true, holdDuration = 1.0f }, // Nota larga corta
            new BeatEvent { beat =   7f, requiredPose = PoseType.PoseC,  holePositionX = -999f },
            new BeatEvent { beat =  10f, requiredPose = PoseType.PoseB,  holePositionX = -999f },
            new BeatEvent { beat =  13f, requiredPose = PoseType.PoseD,  holePositionX = -999f },
 
            // ── VERSO 1
            new BeatEvent { beat =  15f, requiredPose = PoseType.PoseA,  holePositionX = -999f, isHoldNote = true, holdDuration = 1.5f }, // Nota larga rápida
            new BeatEvent { beat =  19f, requiredPose = PoseType.PoseB,  holePositionX = -999f },
            new BeatEvent { beat =  23f, requiredPose = PoseType.PoseC,  holePositionX = -999f },
            new BeatEvent { beat =  27f, requiredPose = PoseType.PoseD,  holePositionX = -999f },
            new BeatEvent { beat =  31f, requiredPose = PoseType.PoseA,  holePositionX = -999f, isHoldNote = true, holdDuration = 1.5f }, // Nota larga rápida
            new BeatEvent { beat =  35f, requiredPose = PoseType.PoseC,  holePositionX = -999f },
            new BeatEvent { beat =  39f, requiredPose = PoseType.PoseB,  holePositionX = -999f },
            new BeatEvent { beat =  43f, requiredPose = PoseType.PoseAB, holePositionX = -999f },
            new BeatEvent { beat =  47f, requiredPose = PoseType.PoseCD, holePositionX = -999f },
            new BeatEvent { beat =  51f, requiredPose = PoseType.PoseBC, holePositionX = -999f },
 
            // ── PRE-CORO (Sección frenética de 2 beats, no lleva notas largas para evitar colisiones)
            new BeatEvent { beat =  55f, requiredPose = PoseType.PoseA,  holePositionX = -999f },
            new BeatEvent { beat =  57f, requiredPose = PoseType.PoseD,  holePositionX = -999f },
            new BeatEvent { beat =  59f, requiredPose = PoseType.PoseB,  holePositionX = -999f },
            new BeatEvent { beat =  61f, requiredPose = PoseType.PoseC,  holePositionX = -999f },
            new BeatEvent { beat =  63f, requiredPose = PoseType.PoseAD, holePositionX = -999f },
            new BeatEvent { beat =  65f, requiredPose = PoseType.PoseBC, holePositionX = -999f },
            new BeatEvent { beat =  67f, requiredPose = PoseType.PoseA,  holePositionX = -999f },
            new BeatEvent { beat =  69f, requiredPose = PoseType.PoseC,  holePositionX = -999f },
            new BeatEvent { beat =  71f, requiredPose = PoseType.PoseCD, holePositionX = -999f },
            new BeatEvent { beat =  73f, requiredPose = PoseType.PoseAB, holePositionX = -999f },
 
            // ── CORO 1 (Notas largas entre ráfagas rápidas)
            new BeatEvent { beat =  75f, requiredPose = PoseType.PoseA,  holePositionX = -999f, isHoldNote = true, holdDuration = 1.0f }, // Nota larga de 1s (Termina en beat 77, libre en 78)
            new BeatEvent { beat =  78f, requiredPose = PoseType.PoseAB, holePositionX = -999f },
            new BeatEvent { beat =  81f, requiredPose = PoseType.PoseC,  holePositionX = -999f },
            new BeatEvent { beat =  84f, requiredPose = PoseType.PoseBC, holePositionX = -999f },
            new BeatEvent { beat =  87f, requiredPose = PoseType.PoseB,  holePositionX = -999f, isHoldNote = true, holdDuration = 1.0f }, // Nota larga de 1s (Termina en beat 89, libre en 90)
            new BeatEvent { beat =  90f, requiredPose = PoseType.PoseCD, holePositionX = -999f },
            new BeatEvent { beat =  93f, requiredPose = PoseType.PoseD,  holePositionX = -999f },
            new BeatEvent { beat =  96f, requiredPose = PoseType.PoseAD, holePositionX = -999f },
            new BeatEvent { beat =  99f, requiredPose = PoseType.PoseA,  holePositionX = -999f },
            new BeatEvent { beat = 102f, requiredPose = PoseType.PoseAB, holePositionX = -999f },
            new BeatEvent { beat = 105f, requiredPose = PoseType.PoseBC, holePositionX = -999f },
            new BeatEvent { beat = 108f, requiredPose = PoseType.PoseCD, holePositionX = -999f },
            new BeatEvent { beat = 111f, requiredPose = PoseType.PoseAD, holePositionX = -999f, isHoldNote = true, holdDuration = 1.0f }, // Nota larga corta antes del bonus
            new BeatEvent { beat = 114f, requiredPose = PoseType.PoseCD, holePositionX = -999f },
 
            // ── PUENTE: ÁREA BONUS (Del beat 115 al 130)
            new BeatEvent { beat = 115f, isBonusAreaStart = true },
            new BeatEvent { beat = 130f, isBonusAreaEnd = true },
            new BeatEvent { beat = 135f, requiredPose = PoseType.PoseD,  holePositionX = -999f, isHoldNote = true, holdDuration = 1.5f }, // Nota larga
            new BeatEvent { beat = 139f, requiredPose = PoseType.PoseAD, holePositionX = -999f },
            new BeatEvent { beat = 143f, requiredPose = PoseType.PoseBC, holePositionX = -999f },
            new BeatEvent { beat = 147f, requiredPose = PoseType.PoseAB, holePositionX = -999f },
            new BeatEvent { beat = 151f, requiredPose = PoseType.PoseAD, holePositionX = -999f },
 
            // ── OUTRO
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
            new BeatEvent { beat = 183f, requiredPose = PoseType.PoseAB, holePositionX = -999f, isHoldNote = true, holdDuration = 3.0f }  // Cierre dramático difícil
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
