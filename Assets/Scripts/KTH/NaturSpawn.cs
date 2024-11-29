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
    public MeshCollider ground;                 // MeshCollider 오브젝트
    public float minDistanceBetweenPrefabs = 2.0f; // 프리팹 간 최소 거리
    public float minDistanceBetweenDifferentPrefabs = 3.0f; // 서로 다른 프리팹 간 최소 거리

    private List<Vector3> spawnPositions = new List<Vector3>(); // 이미 배치된 위치

    void Start()
    {
        if (ground == null)
        {
            Debug.LogError("Ground object needs a MeshCollider component.");
            return;
        }

        if (!ground.sharedMesh)
        {
            Debug.LogError("Ground object does not have a valid shared mesh.");
            return;
        }

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
                // Mesh 내 랜덤 위치 생성
                Vector3 randomPosition = GetRandomPositionOnMesh();

                // 거리 검사 (겹치지 않게 배치)
                bool canSpawn = true;

                foreach (Vector3 pos in spawnPositions)
                {
                    if (Vector3.Distance(pos, randomPosition) < minDistanceBetweenPrefabs)
                    {
                        canSpawn = false;
                        break;
                    }
                }

                if (canSpawn)
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

    Vector3 GetRandomPositionOnMesh()
    {
        Mesh mesh = ground.sharedMesh;
        Vector3[] vertices = mesh.vertices;
        int[] triangles = mesh.triangles;

        // 임의의 삼각형 선택
        int triangleIndex = Random.Range(0, triangles.Length / 3) * 3;

        // 삼각형을 이루는 세 점
        Vector3 p1 = ground.transform.TransformPoint(vertices[triangles[triangleIndex]]);
        Vector3 p2 = ground.transform.TransformPoint(vertices[triangles[triangleIndex + 1]]);
        Vector3 p3 = ground.transform.TransformPoint(vertices[triangles[triangleIndex + 2]]);

        // 삼각형 내부의 임의의 점 계산 (Barycentric 좌표)
        float r1 = Mathf.Sqrt(Random.value);
        float r2 = Random.value;

        Vector3 randomPoint = (1 - r1) * p1 + (r1 * (1 - r2)) * p2 + (r1 * r2) * p3;

        return randomPoint;
    }
}