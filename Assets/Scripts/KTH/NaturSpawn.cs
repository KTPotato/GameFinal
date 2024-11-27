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
    public Transform ground;                     // Cylinder ������Ʈ
    public float minDistanceBetweenPrefabs = 2.0f; // ������ �� �ּ� �Ÿ�
    public float minDistanceBetweenDifferentPrefabs = 3.0f; // ���� �ٸ� ������ �� �ּ� �Ÿ�

    private float cylinderRadius;                // Cylinder ������
    private Vector3 cylinderCenter;              // Cylinder �߽�
    private List<Vector3> spawnPositions = new List<Vector3>(); // �̹� ��ġ�� ��ġ

    void Start()
    {
        var cylinderCollider = ground.GetComponent<Collider>();
        if (cylinderCollider == null)
        {
            Debug.LogError("Ground object needs a Collider component.");
            return;
        }

        // Cylinder �߽ɰ� ������ ���
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
                // ���� ���� �� ������ ��ġ ����
                Vector3 randomPosition = GetRandomPositionWithinCircle();

                // �Ÿ� �˻� (��ġ�� �ʰ� ��ġ)
                bool canSpawn = true;

                // ������ ������ �� �Ÿ� �˻�
                foreach (Vector3 pos in spawnPositions)
                {
                    if (Vector3.Distance(pos, randomPosition) < minDistanceBetweenPrefabs)
                    {
                        canSpawn = false;
                        break;
                    }
                }

                // �ٸ� ������ �� �Ÿ� �˻�
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
                    randomPosition.y = prefabData.yPosition; // ������ Y �� ���
                    Instantiate(prefabData.prefab, randomPosition, Quaternion.identity);
                    spawnPositions.Add(randomPosition); // ��ġ ����
                    spawnedCount++;
                }
            }
        }
    }

    Vector3 GetRandomPositionWithinCircle()
    {
        // ���� ���� �� ������ �� ����
        float angle = Random.Range(0, Mathf.PI * 2); // ����
        float distance = Random.Range(0, cylinderRadius); // ������ �� �Ÿ�

        float x = cylinderCenter.x + Mathf.Cos(angle) * distance;
        float z = cylinderCenter.z + Mathf.Sin(angle) * distance;
        float y = cylinderCenter.y; // ���̴� Cylinder �߽� ���� (������ �����տ� �´� Y�� ������)

        return new Vector3(x, y, z);
    }
}