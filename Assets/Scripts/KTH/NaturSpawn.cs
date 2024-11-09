using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NaturSpawn : MonoBehaviour
{
    [SerializeField] private GameObject cylinderObject; // Cylinder ������Ʈ�� ����
    [SerializeField] private List<GameObject> prefabs; // ��ġ�� ������ ����Ʈ
    [SerializeField] private int spawnCount = 50; // ������ ������Ʈ ��
    [SerializeField] private float minimumDistance = 2f; // ������Ʈ ���� �ּ� �Ÿ�

    private float cylinderRadius;
    private float cylinderHeight;
    private List<Vector3> spawnedPositions = new List<Vector3>(); // �̹� ������ ��ġ��

    private void Start()
    {
        // Cylinder�� �������� ���� ��������
        MeshRenderer renderer = cylinderObject.GetComponent<MeshRenderer>();
        cylinderRadius = renderer.bounds.extents.x; // X�� ������
        cylinderHeight = renderer.bounds.size.y; // Y�� ����

        SpawnPrefabsOnCylinder();
    }

    private void SpawnPrefabsOnCylinder()
    {
        int attempts = 0;

        for (int i = 0; i < spawnCount; i++)
        {
            Vector3 randomPosition = Vector3.zero; // �ʱ�ȭ
            bool positionFound = false;

            // �ִ� �õ� Ƚ�� (���� ���� ����)
            while (attempts < 100)
            {
                randomPosition = GetRandomPositionOnCylinder();

                // ���� ��ġ����� �Ÿ� �˻�
                if (IsPositionValid(randomPosition))
                {
                    spawnedPositions.Add(randomPosition); // ��ȿ�� ��ġ�� ��Ͽ� �߰�
                    positionFound = true;
                    break;
                }
                attempts++;
            }

            // ��ȿ�� ��ġ�� �߰ߵǾ��� ���� ������ ����
            if (positionFound)
            {
                GameObject prefab = prefabs[Random.Range(0, prefabs.Count)];
                Instantiate(prefab, randomPosition, Quaternion.identity);
            }
        }
    }

    private Vector3 GetRandomPositionOnCylinder()
    {
        float angle = Random.Range(0, 2 * Mathf.PI);
        float distance = Random.Range(0, cylinderRadius);

        float x = cylinderObject.transform.position.x + distance * Mathf.Cos(angle);
        float z = cylinderObject.transform.position.z + distance * Mathf.Sin(angle);
        float y = cylinderObject.transform.position.y + cylinderHeight / 2;

        return new Vector3(x, y, z);
    }

    private bool IsPositionValid(Vector3 position)
    {
        foreach (Vector3 existingPosition in spawnedPositions)
        {
            if (Vector3.Distance(existingPosition, position) < minimumDistance)
            {
                return false; // �ּ� �Ÿ� ������ �������� ������ false ��ȯ
            }
        }
        return true; // ��� ��ġ���� �Ÿ��� �����ϸ� true ��ȯ
    }
}