using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(TilemapRefresher))]
public class TilemapFixerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        var t = target as TilemapRefresher;

        if (GUILayout.Button("Refresh Tilemap"))
        {
            t.RefreshAllTilesOnTilemap();
        }
    }
}