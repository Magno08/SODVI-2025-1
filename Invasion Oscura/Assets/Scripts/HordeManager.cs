using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HordeManager : MonoBehaviour
{
    public GameObject enemyPrefab; // Prefab del enemigo
    public Transform spawnPoint; // Punto de aparición de los enemigos
    public int hordeSize = 5; // Tamaño de cada horda
    private List<GameObject> currentHorde = new List<GameObject>(); // Lista para almacenar la horda actual
    public float spawnInterval = 1f; // Intervalo entre enemigos generados

    void Start()
    {
        StartCoroutine(SpawnHorde());
    }

    private IEnumerator SpawnHorde()
    {
        while (true)
        {
            if (currentHorde.Count == 0)
            {
                for (int i = 0; i < hordeSize; i++)
                {
                    GameObject enemy = Instantiate(enemyPrefab, spawnPoint.position, spawnPoint.rotation);
                    currentHorde.Add(enemy);
                    yield return new WaitForSeconds(spawnInterval);
                }
            }

            yield return null;
        }
    }

    void Update()
    {
        currentHorde.RemoveAll(enemy => enemy == null); // Remover enemigos muertos de la lista
    }
}
