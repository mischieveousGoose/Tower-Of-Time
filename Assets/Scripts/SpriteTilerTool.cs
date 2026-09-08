using UnityEditor;
using UnityEngine;

public static class SpriteTilerTool
{
    [MenuItem("Tools/Sprite Tools/Set Tiled Size From Box Collider")]
    public static void SetTiledSizeFromCollider()
    {
        foreach (GameObject obj in Selection.gameObjects)
        {
            SpriteRenderer sr = obj.GetComponent<SpriteRenderer>();
            BoxCollider2D collider = obj.GetComponent<BoxCollider2D>();
            Transform transform = obj.transform;

            if (sr == null || collider == null)
                continue;

            Undo.RecordObject(collider, "Set Box Collider Size");
            Undo.RecordObject(transform, "Set Transform Scale");
            Undo.RecordObject(sr, "Set Sprite Renderer Draw Mode");

            sr.drawMode = SpriteDrawMode.Tiled;

            sr.size = sr.size * transform.localScale;

            Vector2 size = sr.size;

            collider.size = size;
            transform.localScale = new Vector3(1, 1, 1);
        }
    }

    [MenuItem("Tools/Sprite Tools/Set Tiled Size From Box Collider", true)]
    private static bool ValidateSetTiledSizeFromCollider()
    {
        return Selection.gameObjects.Length > 0;
    }
}