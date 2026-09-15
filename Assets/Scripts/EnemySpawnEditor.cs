using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
[CustomEditor(typeof(EnemySpawner))]
public class EnemySpawnEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        EnemySpawner enemySpawner = (EnemySpawner)target;

        EditorGUILayout.Space(3);
        if (GUILayout.Button("Find Points", GUILayout.Height(30)))
        {
            enemySpawner.FindSpawnPoints();
            EditorUtility.SetDirty(enemySpawner);
        }
    }
}

#endif