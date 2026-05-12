using UnityEngine;

public class SingleEnemySpawner : MonoBehaviour
{
    public GameObject chaserPrefab;
    public bool spawnOnStart = true;

    private bool hasSpawned = false;

    void Start()
    {
        if (spawnOnStart)
        {
            SpawnChaser();
        }
    }

    public void SpawnChaser()
    {
        if (hasSpawned) return;

        Instantiate(chaserPrefab, transform.position, transform.rotation);
        hasSpawned = true;
    }
}