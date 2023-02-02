using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MapManager : MonoBehaviour
{
    [Header("Map Settings")]
    [SerializeField]
    private int height;
    [SerializeField]
    private int width;
    [SerializeField]
    private float grassThreshold;
    [SerializeField]
    private int flowerSpawnRate;
    [SerializeField]
    private int buildResourceSpawnRate;
    [SerializeField]
    private int mapDetail;

    private string seed;
    private int mapXOffset;
    private int mapYOffset;

    [Header("Tilemaps")]
    [SerializeField]
    private Tilemap groundTileMap;
    [SerializeField]
    private Tilemap interactableTileMap;
    [SerializeField]
    private Tilemap buildTemplateTileMap;
    [SerializeField]
    private Tilemap workTileMap;
    [SerializeField]
    private Tilemap temporaryTileMap;

    [Header("Tiles")]
    [SerializeField]
    private List<TileBase> grassTiles;
    [SerializeField]
    private List<TileBase> dirtTiles;
    [SerializeField]
    private List<TileBase> honeycombTiles;
    [SerializeField]
    private List<TileBase> flowerTiles;
    [SerializeField]
    private List<TileBase> buildingResourceTiles;

    [Header("Individual Tiles")]
    [SerializeField]
    private TileBase honeycombSafe;
    [SerializeField]
    private TileBase honeycombFail;
    [SerializeField]
    private TileBase honeycombTemplate;
    [SerializeField]
    private TileBase workTile;
    [SerializeField]
    private TileBase honeycombNectar;
    [SerializeField]
    private TileBase honeycombPollen;
    [SerializeField]
    private TileBase honeycombHoney;
    [SerializeField]
    private TileBase honeycombWax;

    [Header("Resource Banks")]
    [SerializeField]
    private Vector3Int pollenBank;
    [SerializeField]
    private Vector3Int nectarBank;
    [SerializeField]
    private Vector3Int honeyBank;
    [SerializeField]
    private Vector3Int waxBank;

    private void Update()
    {
    }

    public void FlowerSpawn()
    {
        bool hasSpawned = false;
        while(!hasSpawned)
        {
            Vector3Int randomLocation = new Vector3Int(UnityEngine.Random.Range(0, width) - mapXOffset, UnityEngine.Random.Range(0, height) - mapYOffset, 0);
            if (!interactableTileMap.HasTile(randomLocation))
            {
                TileBase currentTile = groundTileMap.GetTile(randomLocation);
                if (grassTiles.Contains(currentTile))
                {
                    interactableTileMap.SetTile(randomLocation, RandomTile(flowerTiles));
                    Debug.Log($"Spawning new flower at {randomLocation}.");
                    hasSpawned = true;
                }
            }
        }
    }

    public void GenerateMap()
    {
        GenerateSeed();
        GenerateOffsets();
        GenerateGround();
        GenerateFlowers();
        GenerateBuildingResources();
    }

    private void GenerateSeed()
    {
        seed = System.Guid.NewGuid().ToString();
        Debug.Log($"Seed: {seed}");
    }

    private void GenerateOffsets() {
        mapXOffset = width / 2;
        mapYOffset = height / 2;
    }

    private void GenerateGround()
    {
        var rand = new System.Random(seed.GetHashCode());
        int offsetX = rand.Next(-10000, 10000);
        int offsetY = rand.Next(-10000, 10000);

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Debug.Log($"{x}, {y}: {Mathf.PerlinNoise((float)(x + offsetX)/(float)(width / mapDetail), (float)(y + offsetY) / (float)(height / mapDetail))}");
                float perlinNoise = Mathf.PerlinNoise((float)(x + offsetX) / (float)(width / mapDetail), (float)(y + offsetY) / (float)(height / mapDetail ));

                groundTileMap.SetTile(new Vector3Int(x - mapXOffset, y - mapYOffset, 0), perlinNoise < grassThreshold ? RandomTile(grassTiles) : RandomTile(dirtTiles));
            }
        }
    }

    private void GenerateFlowers()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Vector3Int currentPosition = new Vector3Int(x - mapXOffset, y - mapYOffset, 0);
                if (!interactableTileMap.HasTile(currentPosition))
                {
                    TileBase currentTile = groundTileMap.GetTile(currentPosition);
                    if (grassTiles.Contains(currentTile))
                    {
                        Debug.Log($"{x - mapXOffset}, {y - mapYOffset} is a grass tile.");
                        if (UnityEngine.Random.Range(0, 100) < flowerSpawnRate)
                        {
                            interactableTileMap.SetTile(new Vector3Int(x - mapXOffset, y - mapYOffset, 0), RandomTile(flowerTiles));
                            Debug.Log($"Spawned a flower at {x - mapXOffset}, {y - mapYOffset}.");
                        }
                    }
                }
            }
        }
    }

    private void GenerateBuildingResources()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Vector3Int currentPosition = new Vector3Int(x - mapXOffset, y - mapYOffset, 0);
                if (!interactableTileMap.HasTile(currentPosition))
                {
                    TileBase currentTile = groundTileMap.GetTile(currentPosition);
                    if (dirtTiles.Contains(currentTile))
                    {
                        Debug.Log($"{x - mapXOffset}, {y - mapYOffset} is a dirt tile.");
                        if (UnityEngine.Random.Range(0, 100) < buildResourceSpawnRate)
                        {
                            interactableTileMap.SetTile(new Vector3Int(x - mapXOffset, y - mapYOffset, 0), RandomTile(buildingResourceTiles));
                            Debug.Log($"Spawned a build resource at {x - mapXOffset}, {y - mapYOffset}.");
                        }
                    }
                }
            }
        }
    }

    private TileBase RandomTile(List<TileBase> tiles)
    {
        int count = tiles.Count;
        return tiles[UnityEngine.Random.Range(0, count)];
    }

    public bool IsPositionInBounds(Vector3Int position)
    {
        int x = position.x;
        int y = position.y;
        return ((x > -mapXOffset && x < width - mapXOffset) && (y > -mapYOffset && y < height - mapYOffset)) ? true : false;
    }

    public bool IsSurroundingHoneycombTile(Vector3Int position, Tilemap tilemap)
    {
        Vector3Int[] surroundingPositions;
        if(position.y % 2 == 0)
        {
            Debug.Log($"Position.y is Even: {position}.");
            surroundingPositions = new Vector3Int[]{
                new Vector3Int(position.x + 1, position.y, 0),
                new Vector3Int(position.x, position.y + 1, 0),
                new Vector3Int(position.x - 1, position.y + 1, 0),
                new Vector3Int(position.x - 1, position.y, 0),
                new Vector3Int(position.x - 1, position.y - 1, 0),
                new Vector3Int(position.x, position.y - 1, 0)
            };
        }
        else
        {
            Debug.Log($"Position.y is Odd: {position}.");
            surroundingPositions = new Vector3Int[]{
                new Vector3Int(position.x + 1, position.y, 0),
                new Vector3Int(position.x + 1, position.y + 1, 0),
                new Vector3Int(position.x, position.y + 1, 0),
                new Vector3Int(position.x - 1, position.y, 0),
                new Vector3Int(position.x, position.y - 1, 0),
                new Vector3Int(position.x + 1, position.y - 1, 0)
            };
        }

        foreach (Vector3Int v in surroundingPositions)
        {
            TileBase surroundingTile = tilemap.GetTile(v);
            if(IsHoneyCombTile(surroundingTile))
            {
                Debug.Log($"Matching Surrounding Tile at {v} for position {position}.");
                return true;
            }
        }

        return false;
    }

    public void FinishFlowerJob(Vector3Int position)
    {
        interactableTileMap.SetTile(position, null);
        workTileMap.SetTile(position, null);
    }

    public void FinishPollenJob(Vector3Int position)
    {
        workTileMap.SetTile(position, null);
    }

    public void FinishHoneyJob(Vector3Int position)
    {
        workTileMap.SetTile(position, null);
    }

    public void FinishWaxJob(Vector3Int position)
    {
        workTileMap.SetTile(position, null);
    }

    public void FinishHiveJob(Vector3Int position)
    {
        interactableTileMap.SetTile(position, honeycombTiles[UnityEngine.Random.Range(0,honeycombTiles.Count)]);
        workTileMap.SetTile(position, null);
        buildTemplateTileMap.SetTile(position, null);
    }

    public bool IsHoneyCombTile(TileBase tile)
    {
        if(!(honeycombTiles.Contains(tile) || honeycombNectar.Equals(tile) || honeycombPollen.Equals(tile) || honeycombHoney.Equals(tile) || honeycombWax.Equals(tile)))
        {
            return false;
        }

        return true;
    }

    public Tilemap GetGroundTileMap()
    {
        return groundTileMap;
    }

    public Tilemap GetInteractableTileMap()
    {
        return interactableTileMap;
    }

    public Tilemap GetBuildTemplateTileMap()
    {
        return buildTemplateTileMap;
    }

    public Tilemap GetWorkTileMap()
    {
        return workTileMap;
    }

    public Tilemap GetTemporaryTileMap()
    {
        return temporaryTileMap;
    }

    public TileBase GetHoneycombSafe()
    {
        return honeycombSafe;
    }

    public TileBase GetHoneycombFail()
    {
        return honeycombFail;
    }

    public TileBase GetHoneycombTemplate()
    {
        return honeycombTemplate;
    }

    public TileBase GetHoneycombNectar()
    {
        return honeycombNectar;
    }

    public TileBase GetHoneycombHoney()
    {
        return honeycombHoney;
    }

    public TileBase GetHoneycombWax()
    {
        return honeycombWax;
    }

    public TileBase GetHoneycombPollen()
    {
        return honeycombPollen;
    }

    public TileBase GetWorkTile()
    {
        return workTile;
    }

    public List<TileBase> GetHoneycombTiles()
    {
        return honeycombTiles;
    }

    public List<TileBase> GetFlowerTiles()
    {
        return flowerTiles;
    }

    public int GetHeight() { return height; }

    public int GetWidth() { return width; }

    public Vector3Int GetPollenBankCellLocation()
    {
        return pollenBank;
    }

    public Vector3Int GetNectarBankCellLocation()
    {
        return nectarBank;
    }

    public Vector3 GetPollenBankWorldLocation()
    {
        return interactableTileMap.CellToWorld(pollenBank);
    }

    public Vector3 GetNectarBankWorldLocation()
    {
        return interactableTileMap.CellToWorld(nectarBank);
    }
}
