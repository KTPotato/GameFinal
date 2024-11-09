using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NaturSpawn : MonoBehaviour
{
    public GameObject[] prefabs;                // 랜덤으로 생성할 프리팹 배열
    public int spawnCount = 10;                 // 생성할 개수
    public Transform ground;                    // Cylinder 바닥 오브젝트
    public float minDistanceBetweenPrefabs = 2.0f; // 프리팹 간의 최소 거리

    private List<Vector3> spawnPositions = new List<Vector3>(); // 이미 사용된 위치 저장
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

            // 겹침 방지를 위한 거리 검사
            bool canSpawn = true;
            foreach (Vector3 pos in spawnPositions)
            {
                if (Vector3.Distance(pos, spawnPosition) < minDistanceBetweenPrefabs)
                {
                    canSpawn = false;
                    break;
                }
            }

            // 최소 거리 조건을 만족하면 프리팹을 생성
            if (canSpawn)
            {
                GameObject randomPrefab = prefabs[Random.Range(0, prefabs.Length)];
                Instantiate(randomPrefab, spawnPosition, Quaternion.identity);
                spawnPositions.Add(spawnPosition); // 위치 저장
                spawnedCount++;
            }
        }
    }

    Vector3 GetRandomPositionWithinCollider()
    {
        // Collider의 바운드 계산
        Bounds bounds = groundCollider.bounds;

        // Collider의 범위 내에서 랜덤 위치 선택
        float randomX = Random.Range(bounds.min.x, bounds.max.x);
        float randomZ = Random.Range(bounds.min.z, bounds.max.z);
        float yPosition = bounds.center.y; // 높이는 Collider의 중심을 기준으로 설정

        return new Vector3(randomX, yPosition, randomZ);
    }
}