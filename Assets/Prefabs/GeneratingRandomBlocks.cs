using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;
using Random = UnityEngine.Random;

public enum CellType
{
    Air,
    Wall,
    Object
}

public class GeneratingRandomBlocks : MonoBehaviour
{
    public GameObject RocksSmallPrefab;
    public GameObject SiegeRamPrefab;
    public GameObject SiegeTrebuchetPrefab;
    public GameObject TowerSquarePrefab;
    public GameObject SquareTopPrefab;
    
    public GameObject wallPrefab;
    public GameObject[] objectPrefabs;

    public GameObject Player;
    public CellType[,] grid;
    public Vector3 minBounds;
    public Vector3 maxBounds;
    public float cellSize = 1.0f;

    void Start()
    {
        objectPrefabs = new GameObject[] { RocksSmallPrefab, SiegeRamPrefab, SiegeTrebuchetPrefab, TowerSquarePrefab, SquareTopPrefab };
        DetermineMapBounds();
        InitializeGrid();
        PopulateGrid();
        GenerateObjects();
    }

    void DetermineMapBounds()
    {
        GameObject[]  MapElements = GameObject.FindGameObjectsWithTag("MapElement");

        // Inicijalizacija minBounds i maxBounds prvim elementom
        if (MapElements.Length > 0)
        {
            minBounds = MapElements[0].transform.position;
            maxBounds = MapElements[0].transform.position;
        }

        foreach (var element in MapElements)
        {
            Vector3 position = element.transform.position;
            minBounds = Vector3.Min(minBounds, position);
            maxBounds = Vector3.Max(maxBounds, position);
        }
    }

    void InitializeGrid()
    {
        // Izračun broja ćelija u gridu na temelju minBounds i maxBounds
        int gridSizeX = Mathf.RoundToInt((maxBounds.x - minBounds.x) / cellSize);
        int gridSizeZ = Mathf.RoundToInt((maxBounds.z - minBounds.z) / cellSize);

        // Stvaranje grida s odgovarajućom veličinom
        grid = new CellType[gridSizeZ, gridSizeX];
    }

    void PopulateGrid()
    {
        //Postavljanje vanjskih zidova
        for (int x = 0; x < grid.GetLength(1); x++)
        {
            grid[0, x] = CellType.Wall; // Gornji zid
            grid[grid.GetLength(0) - 1, x] = CellType.Wall; // Donji zid
        }
        for (int z = 0; z < grid.GetLength(0); z++)
        {
            grid[z, 0] = CellType.Wall; // Lijevi zid
            grid[z, grid.GetLength(1) - 1] = CellType.Wall; // Desni zid
        }

        float wallSpacing = 2.0f; // Razmak između unutarnjih zidova
        float minX = minBounds.x + wallSpacing;
        float maxX = maxBounds.x - wallSpacing;
        float minZ = minBounds.z + wallSpacing;
        float maxZ = maxBounds.z - wallSpacing;

        // Za svaku poziciju unutar granica, postavljamo zidove, zrak i objekte naizmjenično
        for (float z = minZ; z < maxZ; z += wallSpacing)
        {
            for (float x = minX; x < maxX; x += wallSpacing)
            {
                // Pretvorimo trenutnu poziciju u koordinate u gridu
                int gridX = Mathf.RoundToInt((x - minBounds.x) / cellSize);
                int gridZ = Mathf.RoundToInt((z - minBounds.z) / cellSize);
                
                // Postavljamo zid, zrak ili objekt ovisno o poziciji
                 if (gridX > 0 && gridX < grid.GetLength(1) - 1 && gridZ > 0 && gridZ < grid.GetLength(0) - 1)
                {
                    if ((gridX + gridZ) % 2 == 0)
                    {
                        grid[gridZ, gridX] = CellType.Wall;
                    }
                    else
                    {
                        grid[gridZ, gridX] = CellType.Air;
                    }
                }

            }
        }

        for (int z = 1; z < grid.GetLength(0) - 1; z++)
        {
            for (int x = 1; x < grid.GetLength(1) - 1; x++)
            {
                grid[1, 1]= CellType.Air;
                grid[1, 2] = CellType.Air;
                grid[2, 1] = CellType.Air;
                // Ako je trenutno polje zrak, a susjedna polja su zidovi, postavljamo ga kao rand objekat
                if (grid[z - 1, x] == CellType.Wall || grid[z + 1, x] == CellType.Wall || grid[z, x - 1] == CellType.Wall || grid[z, x + 1] == CellType.Wall)
                {
                    grid[z, x] = CellType.Object;
                }
            }
        }
    }
    void GenerateObjects()
    {
        for (int z = 0; z < grid.GetLength(0); z++)
        {
            for (int x = 0; x < grid.GetLength(1); x++)
            {
                if (grid[z, x] == CellType.Wall)
                {
                    // Stvori zid na poziciji (x, z)
                    Instantiate(wallPrefab, new Vector3(minBounds.x + x * cellSize, 1.4f, minBounds.z + z * cellSize), Quaternion.identity);
                }
                else if (grid[z, x] == CellType.Object)
                {
                    // Slučajno odaberi prefab objekta
                    GameObject randomObjectPrefab = objectPrefabs[Random.Range(0, objectPrefabs.Length)];
                    // Stvori objekt na poziciji (x, z)
                    Instantiate(randomObjectPrefab, new Vector3(minBounds.x + x * cellSize, 1.4f, minBounds.z + z * cellSize), Quaternion.identity);
                }

                   
            }
        }
       Instantiate(Player, new Vector3(minBounds.x + 1 * cellSize, 1.4f, minBounds.z + 1 * cellSize), Quaternion.identity);
    }
    public CellType GetCellType(Vector3 position)
    {
        // Pretvorimo svjetske koordinate u cijele brojeve kako bismo dobili indekse grid-a
        int xIndex = Mathf.RoundToInt((position.x - minBounds.x) / cellSize);
        int zIndex = Mathf.RoundToInt((position.z - minBounds.z) / cellSize);

        // Provjeravamo je li indeksi unutar granica grid-a
        if (xIndex >= 0 && xIndex < grid.GetLength(1) && zIndex >= 0 && zIndex < grid.GetLength(0))
        {
            // Vraćamo tip ćelije na zadanoj poziciji
            return grid[zIndex, xIndex];
        }
        else
        {
            // Ako je pozicija izvan granica grid-a, vraćamo tip zida
            return CellType.Wall;
        }
        
    }
    private GameObject GetObjectAtPosition(int x, int z)
    {
        // Provjeri da li postoji objekat na poziciji (x, z) i vrati ga ako postoji
        RaycastHit hit;
        Ray ray = new Ray(new Vector3(minBounds.x + x * cellSize, 1.4f, minBounds.z + z * cellSize), Vector3.down);
        if (Physics.Raycast(ray, out hit))
        {
            return hit.collider.gameObject;
        }
        return null;
    }
   
    public void SetCellType(int gridX, int gridZ, CellType type)
    {
        grid[gridZ, gridX] = type;
    }
    
   
}

