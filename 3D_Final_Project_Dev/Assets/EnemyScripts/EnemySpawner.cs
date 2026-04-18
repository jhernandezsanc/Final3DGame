using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public GameObject turretPrefab;
    public Transform[] spawnPoints;

    // Example method to spawn a turret at a random spawn point
    public void SpawnTurretAtRandom() {
        if (turretPrefab == null || spawnPoints.Length == 0) return;

        int index = Random.Range(0, spawnPoints.Length);
        Transform spawn = spawnPoints[index];

        Instantiate(turretPrefab, spawn.position, spawn.rotation);
    }
    // Example method to spawn a turret at a specific index
    public void SpawnTurretAtIndex(int index) {
        if (turretPrefab == null || spawnPoints.Length == 0) return;
        if (index < 0 || index >= spawnPoints.Length) return;

        Transform spawn = spawnPoints[index];
        Instantiate(turretPrefab, spawn.position, spawn.rotation);
    }
}