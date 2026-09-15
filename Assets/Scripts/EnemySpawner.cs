using UnityEngine;
using System.Collections.Generic;
using UnityEditor.Animations;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private EnemyStats stats;
    [SerializeField] private EnemyStats stats1;
    [SerializeField] private EnemyController enemyPrefab;
    [SerializeField] private List<Transform> spawnpoints;
    [SerializeField] private int maxEnemies;
    [SerializeField] private Transform SpawnContainer;

    private List<int> SlotsUsed = new();

    private void Start()
    {
        SpawnEnemy();
    }

    private void SpawnEnemy()
    {
        SlotsUsed.Clear();
        for (int i = 0; i < maxEnemies; i++)
        {
            int randomIndex = getSlotForEnemy();
            Transform spawnPoint = spawnpoints[randomIndex];
            EnemyController enemy = Instantiate(enemyPrefab, spawnPoint.position, spawnPoint.rotation);
            enemy.Initialize(stats);
        }
    }

    private int getSlotForEnemy()
    {
        int slot = 0;
        for (int i = 0; i < spawnpoints.Count; i++)
        {
            int randomIndex = Random.Range(0, spawnpoints.Count);
            if (!SlotsUsed.Contains(randomIndex))
            {
                slot = randomIndex;
                SlotsUsed.Add(randomIndex);
                return slot;
            }
        }
        return slot;
    }

    public void FindSpawnPoints()
    {
        spawnpoints.Clear();
        foreach (Transform child in SpawnContainer)
        {
            spawnpoints.Add(child);
        }
    }
}
