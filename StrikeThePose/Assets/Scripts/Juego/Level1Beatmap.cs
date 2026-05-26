using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public static class Level1Beatmap
{
#if UNITY_EDITOR
    [MenuItem("RhythmGame/Cargar BeatMap Nivel 1")]
    private static void LoadLevel1()
    {
        Beatmap bm = Selection.activeObject as Beatmap;
        if (bm == null)
        {
            string[] guids = AssetDatabase.FindAssets("Level1 t:Beatmap");
            if (guids.Length > 0)
                bm = AssetDatabase.LoadAssetAtPath<Beatmap>(AssetDatabase.GUIDToAssetPath(guids[0]));
        }

        if (bm == null)
        {
            Debug.LogError("[Level1Loader] No se encontró un Beatmap. " +
                           "Creá uno con Create > RhythmGame > BeatMap, llamalo 'Level1' y seleccionalo.");
            return;
        }

        Undo.RecordObject(bm, "Cargar BeatMap Nivel 1");

        // ─── META ────────────────────────────────────────────────────────────
        bm.bpm = 120f;
        bm.startOffsetSeconds = 3.0f;  // el MP3 tiene ~3s de intro silenciosa

        // ─── EVENTOS ─────────────────────────────────────────────────────────
        //
        //  Estructura (Cherry Bullet - Q&A, ~100s a 120 BPM):
        //
        //  INTRO      beats  1-14   → 2 obstáculos muy espaciados / solo simples
        //  VERSO 1    beats 15-54   → cada 6 beats / solo simples
        //  PRE-CORO   beats 55-74   → cada 4 beats / solo simples
        //  CORO 1     beats 75-114  → cada 4 beats / 4 simples de entrada, luego simple+combo
        //  PUENTE     beats 115-154 → cada 5 beats / mezcla suave
        //  OUTRO      beats 155-191 → cada 4 beats / alternando simple+combo, cierre simple
        //
        //  Total: 40 obstáculos  (78% simples / 22% combinados)
        //
        bm.events.Clear();
        bm.events.AddRange(new BeatEvent[]
        {
            // ── INTRO ───────────────────────────────────────────────────────
            new BeatEvent { beat =   7f, requiredPose = PoseType.PoseA,  holePositionX = -999f },
            new BeatEvent { beat =  13f, requiredPose = PoseType.PoseC,  holePositionX = -999f },
 
            // ── VERSO 1 — cada 6 beats, solo simples ────────────────────────
            new BeatEvent { beat =  15f, requiredPose = PoseType.PoseA,  holePositionX = -999f },
            new BeatEvent { beat =  21f, requiredPose = PoseType.PoseB,  holePositionX = -999f },
            new BeatEvent { beat =  27f, requiredPose = PoseType.PoseC,  holePositionX = -999f },
            new BeatEvent { beat =  33f, requiredPose = PoseType.PoseD,  holePositionX = -999f },
            new BeatEvent { beat =  39f, requiredPose = PoseType.PoseA,  holePositionX = -999f },
            new BeatEvent { beat =  45f, requiredPose = PoseType.PoseC,  holePositionX = -999f },
            new BeatEvent { beat =  51f, requiredPose = PoseType.PoseB,  holePositionX = -999f },
 
            // ── PRE-CORO — cada 4 beats, solo simples ───────────────────────
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
 
            // ── PUENTE — cada 5 beats, mezcla suave ─────────────────────────
            new BeatEvent { beat = 115f, requiredPose = PoseType.PoseA,  holePositionX = -999f },
            new BeatEvent { beat = 120f, requiredPose = PoseType.PoseAB, holePositionX = -999f },
            new BeatEvent { beat = 125f, requiredPose = PoseType.PoseC,  holePositionX = -999f },
            new BeatEvent { beat = 130f, requiredPose = PoseType.PoseBC, holePositionX = -999f },
            new BeatEvent { beat = 135f, requiredPose = PoseType.PoseD,  holePositionX = -999f },
            new BeatEvent { beat = 140f, requiredPose = PoseType.PoseAD, holePositionX = -999f },
            new BeatEvent { beat = 145f, requiredPose = PoseType.PoseB,  holePositionX = -999f },
            new BeatEvent { beat = 150f, requiredPose = PoseType.PoseA,  holePositionX = -999f },
 
            // ── OUTRO — cada 4 beats, alternando simple+combo, cierre simple ─
            new BeatEvent { beat = 155f, requiredPose = PoseType.PoseA,  holePositionX = -999f },
            new BeatEvent { beat = 159f, requiredPose = PoseType.PoseAD, holePositionX = -999f },
            new BeatEvent { beat = 163f, requiredPose = PoseType.PoseC,  holePositionX = -999f },
            new BeatEvent { beat = 167f, requiredPose = PoseType.PoseBC, holePositionX = -999f },
            new BeatEvent { beat = 171f, requiredPose = PoseType.PoseD,  holePositionX = -999f },
            new BeatEvent { beat = 175f, requiredPose = PoseType.PoseAB, holePositionX = -999f },
            new BeatEvent { beat = 179f, requiredPose = PoseType.PoseA,  holePositionX = -999f },
            new BeatEvent { beat = 183f, requiredPose = PoseType.PoseC,  holePositionX = -999f },
        });

        EditorUtility.SetDirty(bm);
        AssetDatabase.SaveAssets();
        Debug.Log($"[Level1Loader] {bm.events.Count} eventos cargados en '{bm.name}'. ✓");
    }
#endif
}
