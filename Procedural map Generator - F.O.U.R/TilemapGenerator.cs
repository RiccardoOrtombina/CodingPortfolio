using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[RequireComponent(typeof(Tilemap))]
public class TilemapGenerator : MonoBehaviour
{
    Tilemap tilemap;
    public Tile[] tiles;
    [SerializeField]
    int tilemapSize = 100;
    int specialTileChance;
    int specialTileSelector;

    Vector3Int tilemapPosition;
    Vector3Int tilemapPositionIncreaseX = new Vector3Int(1, 0, 0);

    // Start is called before the first frame update
    void Start()
    {
        tilemap = GetComponent<Tilemap>();
        tilemap.size = new Vector3Int(tilemapSize, tilemapSize, 0);
        tilemap.FloodFill(tilemap.origin, tiles[0]);
        tilemapPosition = tilemap.origin;

        for (int i = 0; i < tilemapSize; i++)
        {
            for (int x = 0; x < tilemapSize; x++)
            {
                specialTileChance = Random.Range(0, 11);

                if (specialTileChance > 9)
                {
                    specialTileSelector = Random.Range(0, tiles.Length);
                    tilemap.SetTile(tilemapPosition, tiles[specialTileSelector]);
                }

                tilemapPosition += tilemapPositionIncreaseX;
            }

            tilemapPosition = new Vector3Int(tilemap.origin.x, tilemapPosition.y + 1, 0);
        }
    }
}
