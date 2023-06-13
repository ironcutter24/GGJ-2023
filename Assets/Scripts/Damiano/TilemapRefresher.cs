using UnityEngine;
using UnityEngine.Tilemaps;

[RequireComponent(typeof(Tilemap))]
public class TilemapRefresher : MonoBehaviour
{
    public void RefreshAllTilesOnTilemap()
    {
        Tilemap map = GetComponent<Tilemap>();
        map.RefreshAllTiles();
    }
}