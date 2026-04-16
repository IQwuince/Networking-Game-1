using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    public static SpawnManager Instance { get; private set; }

    [SerializeField] private Transform[] spawnPoints;

    private void Awake()
    {
        Instance = this;
    }

    public Vector3 GetRandomSpawn()
    {
        if (spawnPoints == null || spawnPoints.Length == 0)
            return Vector3.zero;

        int idx = Random.Range(0, spawnPoints.Length);
        return spawnPoints[idx].position;
    }
}