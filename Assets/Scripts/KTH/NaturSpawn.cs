using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NaturSpawn : MonoBehaviour
{
    public GameObject[] prefabs;                // �������� ������ ������ �迭
    public int spawnCount = 10;                 // ������ ����
    public Transform ground;                    // Cylinder �ٴ� ������Ʈ
    public float minDistanceBetweenPrefabs = 2.0f; // ������ ���� �ּ� �Ÿ�

    private List<Vector3> spawnPositions = new List<Vector3>(); // �̹� ���� ��ġ ����
    private Collider groundCollider;

    void Start()
    {
        groundCollider = ground.GetComponent<Collider>();
        if (groundCollider == null)
        {
            Debug.LogError("Ground object needs a Collider component.");
            return;
        }

        SpawnPrefabsOnGround();
    }

    void SpawnPrefabsOnGround()
    {
        if (prefabs.Length == 0)
        {
            Debug.LogWarning("Prefabs array is not set!");
            return;
        }

        int spawnedCount = 0;

        while (spawnedCount < spawnCount)
        {
            Vector3 spawnPosition = GetRandomPositionWithinCollider();

            // ��ħ ������ ���� �Ÿ� �˻�
            bool canSpawn = true;
            foreach (Vector3 pos in spawnPositions)
            {
                if (Vector3.Distance(pos, spawnPosition) < minDistanceBetweenPrefabs)
                {
                    canSpawn = false;
                    break;
                }
            }

            // �ּ� �Ÿ� ������ �����ϸ� �������� ����
            if (canSpawn)
            {
                GameObject randomPrefab = prefabs[Random.Range(0, prefabs.Length)];
                Instantiate(randomPrefab, spawnPosition, Quaternion.identity);
                spawnPositions.Add(spawnPosition); // ��ġ ����
                spawnedCount++;
            }
        }
    }

    Vector3 GetRandomPositionWithinCollider()
    {
        // Collider�� �ٿ�� ���
        Bounds bounds = groundCollider.bounds;

        // Collider�� ���� ������ ���� ��ġ ����
        float randomX = Random.Range(bounds.min.x, bounds.max.x);
        float randomZ = Random.Range(bounds.min.z, bounds.max.z);
        float yPosition = bounds.center.y; // ���̴� Collider�� �߽��� �������� ����

        return new Vector3(randomX, yPosition, randomZ);
    }
}