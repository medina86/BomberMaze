using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using UnityEngine;

public class PlayerMoves : MonoBehaviour
{
    public float moveSpeed = 5f; // Brzina kretanja igrača
    public GeneratingRandomBlocks mapGenerator;
    public GameObject bombPrefab; // Prefab bombe
    public float bombCooldown = 2f; // Vrijeme hlađenja između postavljanja bombi
    private float lastBombTime; 
    
    private void Start()
    {
        // Pronalaženje referenca na skriptu koja generira mapu
        mapGenerator = FindObjectOfType<GeneratingRandomBlocks>();
    }

    void Update()
    {
        // Računanje kretanja po osima X i Z
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveZ = Input.GetAxisRaw("Vertical");

        // Normaliziranje vektora kretanja
        Vector3 moveDirection = new Vector3(moveX, 0f, moveZ).normalized;

        // Računanje nove pozicije igrača
        Vector3 newPosition = transform.position + moveDirection * moveSpeed * Time.deltaTime;

        // Provjera tipa ćelije na novoj poziciji
        CellType cellType = mapGenerator.GetCellType(newPosition);

        // Ako je nova pozicija zrak, premjesti igrača
        if (cellType == CellType.Air)
        {
            transform.position = newPosition;
        }
        if (Input.GetKeyDown(KeyCode.Space) && Time.time > lastBombTime + bombCooldown)
        {
            // Instanciranje bombe na poziciji 
            if (mapGenerator.GetCellType(newPosition) == CellType.Air)
            {
                // Provjeri da li nema kolizije s drugim objektima
                if (mapGenerator.GetCellType(newPosition) == CellType.Air)
                {
                    // Postavi bombu na poziciju igrača
                    Instantiate(bombPrefab, newPosition, Quaternion.identity);
                    // Ažuriraj vrijeme kada je postavljena zadnja bomba
                    lastBombTime = Time.time;
                }
            }
            
            lastBombTime = Time.time;
        }
    }
}
