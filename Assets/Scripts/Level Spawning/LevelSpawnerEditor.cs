using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(LevelSpawner))]
public class LevelSpawnerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        // Draw the normal LevelSpawner inspector
        DrawDefaultInspector();

        GUILayout.Space(10);

        LevelSpawner levelSpawner = (LevelSpawner)target;

        if (GUILayout.Button("Initialize Room Lists"))
        {
            levelSpawner.InitializeRoomLists();

            // Mark the object as changed so Unity saves the lists
            EditorUtility.SetDirty(levelSpawner);

            Debug.Log("Room lists initialized!");
        }
    }
}