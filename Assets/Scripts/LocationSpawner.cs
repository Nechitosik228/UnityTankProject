using System.Drawing;
using UnityEngine;

public class LocationSpawner : MonoBehaviour
{
    [SerializeField] private GameObject Block;
    private Vector3 spawnPosition;
    private int size = 10;

    private void Start()
    {
        spawnPosition = new Vector3(0,0,0);
        SpawnBlock();
    }

    private void SpawnBlock()
    {
        for (int y = 0; y< size; y++)
        {
            for (int x = 0; x< size; x++)
            {
                Vector3 position = spawnPosition + new Vector3(x, 0, y);
                Instantiate(Block, position, Quaternion.identity);  
            }
        }
    }

    
}
