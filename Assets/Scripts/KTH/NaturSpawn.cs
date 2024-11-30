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
    public float borderOffset = 2.0f;            // �� ���κ����� �ּ� �Ÿ�

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

        // Mesh�� ��� (Bounds) ��������
        Bounds bounds = mesh.bounds;
        Vector3 boundsCenter = ground.transform.TransformPoint(bounds.center);
        Vector3 boundsSize = bounds.size * ground.transform.localScale.x; // ������ �ݿ�

        Vector3 p1, p2, p3;
        Vector3 randomPoint;
        int attempts = 0;

        do
        {
            int triangleIndex = Random.Range(0, triangles.Length / 3) * 3;

            // �ﰢ���� �̷�� �� ��
            p1 = ground.transform.TransformPoint(vertices[triangles[triangleIndex]]);
            p2 = ground.transform.TransformPoint(vertices[triangles[triangleIndex + 1]]);
            p3 = ground.transform.TransformPoint(vertices[triangles[triangleIndex + 2]]);

            // �ﰢ�� ���� ���� �� ���� (�߽ɺ� �켱)
            float r1 = Mathf.Sqrt(Random.Range(0.2f, 1.0f)); // �߽ɺο� �������� ���� ����
            float r2 = Random.Range(0.2f, 1.0f);
            randomPoint = (1 - r1) * p1 + (r1 * (1 - r2)) * p2 + (r1 * r2) * p3;

            // ��� �Ÿ� ���� Ȯ��
            attempts++;
        } while (!IsPointInsideBounds(randomPoint, boundsCenter, boundsSize, borderOffset) && attempts < 10);

        return randomPoint;
    }

    // ��� ���ο� �ִ��� Ȯ��
    bool IsPointInsideBounds(Vector3 point, Vector3 center, Vector3 size, float borderOffset)
    {
        float halfX = size.x / 2 - borderOffset;
        float halfZ = size.z / 2 - borderOffset;

        return Mathf.Abs(point.x - center.x) <= halfX && Mathf.Abs(point.z - center.z) <= halfZ;
    }
}