using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NaturSpawn : MonoBehaviour
{
    [System.Serializable]
    public struct PrefabData
    {
        public GameObject prefab;     // 생성할 프리팹
        public float yPosition;       // 해당 프리팹의 Y 위치
        public int spawnCount;        // 해당 프리팹의 생성할 개수
    }

    public PrefabData[] prefabsWithY;            // 프리팹과 Y 값 및 생성 개수 포함된 배열
    public Transform ground;                     // Cylinder 오브젝트
    public float minDistanceBetweenPrefabs = 2.0f; // 프리팹 간 최소 거리
    public float minDistanceBetweenDifferentPrefabs = 3.0f; // 서로 다른 프리팹 간 최소 거리

    private float cylinderRadius;                // Cylinder 반지름
    private Vector3 cylinderCenter;              // Cylinder 중심
    private List<Vector3> spawnPositions = new List<Vector3>(); // 이미 배치된 위치

    void Start()
    {
        var cylinderCollider = ground.GetComponent<Collider>();
        if (cylinderCollider == null)
        {
            Debug.LogError("Ground object needs a Collider component.");
            return;
        }

        // Cylinder 중심과 반지름 계산
        cylinderRadius = ground.localScale.x * 0.5f;
        cylinderCenter = ground.position;

        SpawnPrefabsRandomly();
    }

    void SpawnPrefabsRandomly()
    {
        if (prefabsWithY.Length == 0)
        {
            Debug.LogWarning("No prefabs set!");
            return;
        }

        foreach (PrefabData prefabData in prefabsWithY)
        {
            int spawnedCount = 0;

            while (spawnedCount < prefabData.spawnCount)
            {
                // 원형 범위 내 무작위 위치 생성
                Vector3 randomPosition = GetRandomPositionWithinCircle();

                // 거리 검사 (겹치지 않게 배치)
                bool canSpawn = true;

                // 동일한 프리팹 간 거리 검사
                foreach (Vector3 pos in spawnPositions)
                {
                    if (Vector3.Distance(pos, randomPosition) < minDistanceBetweenPrefabs)
                    {
                        canSpawn = false;
                        break;
                    }
                }

                // 다른 프리팹 간 거리 검사
                if (canSpawn)
                {
                    foreach (PrefabData otherPrefabData in prefabsWithY)
                    {
                        if (otherPrefabData.prefab != prefabData.prefab)
                        {
                            foreach (Vector3 pos in spawnPositions)
                            {
                                if (Vector3.Distance(pos, randomPosition) < minDistanceBetweenDifferentPrefabs)
                                {
                                    canSpawn = false;
                                    break;
                                }
                            }
                        }
                    }
                }

                if (canSpawn)
                {
                    randomPosition.y = prefabData.yPosition; // 지정된 Y 값 사용
                    Instantiate(prefabData.prefab, randomPosition, Quaternion.identity);
                    spawnPositions.Add(randomPosition); // 위치 저장
                    spawnedCount++;
                }
            }
        }
    }

    Vector3 GetRandomPositionWithinCircle()
    {
        // 원형 영역 내 무작위 점 생성
        float angle = Random.Range(0, Mathf.PI * 2); // 각도
        float distance = Random.Range(0, cylinderRadius); // 반지름 내 거리

        float x = cylinderCenter.x + Mathf.Cos(angle) * distance;
        float z = cylinderCenter.z + Mathf.Sin(angle) * distance;
        float y = cylinderCenter.y; // 높이는 Cylinder 중심 기준 (각각의 프리팹에 맞는 Y로 설정됨)

        return new Vector3(x, y, z);
    }
}