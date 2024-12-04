using System.Collections;
using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI; // NavMeshSurface를 사용하기 위해 필요

public class NaturSpawn : MonoBehaviour
{
    [System.Serializable]
    public struct PrefabData
    {
        public GameObject prefab;     // 생성할 프리팹
        public float yPosition;       // 해당 프리팹의 Y 위치
        public int spawnCount;        // 해당 프리팹의 생성할 개수
    }

    public PrefabData[] prefabsWithY;             // 프리팹과 Y 값 및 생성 개수 포함된 배열
    public MeshCollider ground;                  // MeshCollider 오브젝트
    public NavMeshSurface navMeshSurface;        // NavMeshSurface 컴포넌트
    public float minDistanceBetweenPrefabs = 2.0f; // 프리팹 간 최소 거리
    public float minDistanceBetweenDifferentPrefabs = 3.0f; // 서로 다른 프리팹 간 최소 거리
    public float borderOffset = 2.0f;            // 맵 경계로부터의 최소 거리

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

        if (navMeshSurface == null)
        {
            Debug.LogError("NavMeshSurface component is not assigned.");
            return;
        }

        // 맵에 이미 배치된 프리팹의 위치를 가져옴
        InitializeSpawnPositions();

        SpawnPrefabsRandomly();

        // NavMesh 업데이트
        navMeshSurface.BuildNavMesh();
    }

    void InitializeSpawnPositions()
    {
        GameObject[] existingPrefabs = GameObject.FindGameObjectsWithTag("SpawnPrefab"); // 미리 정의된 태그 사용
        foreach (GameObject prefab in existingPrefabs)
        {
            spawnPositions.Add(prefab.transform.position);
        }
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
                Vector3 randomPosition = GetRandomPositionOnMesh();

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
                    randomPosition.y = prefabData.yPosition; // 지정된 Y 값 사용
                    GameObject spawnedPrefab = Instantiate(prefabData.prefab, randomPosition, Quaternion.identity);
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

        Bounds bounds = mesh.bounds;
        Vector3 boundsCenter = ground.transform.TransformPoint(bounds.center);
        Vector3 boundsSize = bounds.size * ground.transform.localScale.x;

        Vector3 p1, p2, p3;
        Vector3 randomPoint;
        int attempts = 0;

        do
        {
            int triangleIndex = Random.Range(0, triangles.Length / 3) * 3;

            p1 = ground.transform.TransformPoint(vertices[triangles[triangleIndex]]);
            p2 = ground.transform.TransformPoint(vertices[triangles[triangleIndex + 1]]);
            p3 = ground.transform.TransformPoint(vertices[triangles[triangleIndex + 2]]);

            float r1 = Mathf.Sqrt(Random.Range(0.2f, 1.0f));
            float r2 = Random.Range(0.2f, 1.0f);
            randomPoint = (1 - r1) * p1 + (r1 * (1 - r2)) * p2 + (r1 * r2) * p3;

            attempts++;
        } while (!IsPointInsideBounds(randomPoint, boundsCenter, boundsSize, borderOffset) && attempts < 10);

        return randomPoint;
    }

    bool IsPointInsideBounds(Vector3 point, Vector3 center, Vector3 size, float borderOffset)
    {
        float halfX = size.x / 2 - borderOffset;
        float halfZ = size.z / 2 - borderOffset;

        return Mathf.Abs(point.x - center.x) <= halfX && Mathf.Abs(point.z - center.z) <= halfZ;
    }
}