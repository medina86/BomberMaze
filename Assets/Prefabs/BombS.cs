using UnityEngine;

public class Bomb : MonoBehaviour
{
    public float explosionTime = 3f; // Vrijeme nakon kojeg bomba eksplodira
    public float explosionRadius = 5f; // Radijus eksplozije
    private GeneratingRandomBlocks grid;

    void Start()
    {
        // Pokreni timer za eksplodiranje bombe
        Invoke("Explode", explosionTime);
    }

    void Explode()
    {
        // Provjeri objekte u eksploziji u svim smjerovima
        CheckExplosion(Vector3.right); // Provjeri horizontalno prema desno
        CheckExplosion(Vector3.left); // Provjeri horizontalno prema lijevo
        CheckExplosion(Vector3.forward); // Provjeri vertikalno prema naprijed
        CheckExplosion(Vector3.back); // Provjeri vertikalno prema nazad

        // Uništi bombu nakon eksplozije
        Destroy(gameObject);
    }

    void CheckExplosion(Vector3 direction)
    {
        // Provjeri objekte u eksploziji u određenom smjeru
        RaycastHit hit;
        if (Physics.Raycast(transform.position, direction, out hit, explosionRadius))
        {
            // Provjeri je li pogodeni objekt uništiv
            GameObject objects = hit.collider.gameObject;
            if (objects.CompareTag("Destroyable"))
            {
                // Uništi objekt
                Destroy(objects);

                // Promijeni uništeni objekt u zrak
                Vector3 cellPosition = objects.transform.position;
                SetCellType(cellPosition, CellType.Air);
            }
          
        }
    }

    void SetCellType(Vector3 position, CellType cellType)
    {
        // Pronađi koordinate u gridu na osnovu pozicije objekta
        grid = FindObjectOfType<GeneratingRandomBlocks>();
        if (grid != null)
        {
            int gridX = Mathf.RoundToInt((position.x - grid.minBounds.x) / grid.cellSize);
            int gridZ = Mathf.RoundToInt((position.z - grid.minBounds.z) / grid.cellSize);

            // Postavi tip ćelije na zrak na određenoj poziciji
            grid.SetCellType(gridX, gridZ, CellType.Air);
        }
    }
}
