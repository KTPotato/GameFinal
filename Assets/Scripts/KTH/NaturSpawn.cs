using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NaturSpawn : MonoBehaviour
{
    [System.Serializable]
    public struct PrefabData
    {
        public GameObject prefab;     // ������ ������
        public float yPosition;       // �ش� �������� Y ��ġ
        public int spawnCount;        // �ش� �������� ������ ����
    }

    public PrefabData[] prefabsWithY;            // �����հ� Y �� �� ���� ���� ���Ե� �迭
    public MeshCollider ground;                 // MeshCollider ������Ʈ
    public float minDistanceBetweenPrefabs = 2.0f; // ������ �� �ּ� �Ÿ�
    public float minDistanceBetweenDifferentPrefabs = 3.0f; // ���� �ٸ� ������ �� �ּ� �Ÿ�

    private List<Vector3> spawnPositions = new List<Vector3>(); // �̹� ��ġ�� ��ġ

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
                // Mesh �� ���� ��ġ ����
                Vector3 randomPosition = GetRandomPositionOnMesh();

                // �Ÿ� �˻� (��ġ�� �ʰ� ��ġ)
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
                    randomPosition.y = prefabData.yPosition; // ������ Y �� ���
                    Instantiate(prefabData.prefab, randomPosition, Quaternion.identity);
                    spawnPositions.Add(randomPosition); // ��ġ ����
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

        // ������ �ﰢ�� ����
        int triangleIndex = Random.Range(0, triangles.Length / 3) * 3;

        // �ﰢ���� �̷�� �� ��
        Vector3 p1 = ground.transform.TransformPoint(vertices[triangles[triangleIndex]]);
        Vector3 p2 = ground.transform.TransformPoint(vertices[triangles[triangleIndex + 1]]);
        Vector3 p3 = ground.transform.TransformPoint(vertices[triangles[triangleIndex + 2]]);

        // �ﰢ�� ������ ������ �� ��� (Barycentric ��ǥ)
        float r1 = Mathf.Sqrt(Random.value);
        float r2 = Random.value;

        Vector3 randomPoint = (1 - r1) * p1 + (r1 * (1 - r2)) * p2 + (r1 * r2) * p3;

        return randomPoint;
    }
}