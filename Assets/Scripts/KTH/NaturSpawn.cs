using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NaturSpawn : MonoBehaviour
{
    [SerializeField] private GameObject cylinderObject; // Cylinder 오브젝트를 지정
    [SerializeField] private List<GameObject> prefabs; // 배치할 프리팹 리스트
    [SerializeField] private int spawnCount = 50; // 생성할 오브젝트 수
    [SerializeField] private float minimumDistance = 2f; // 오브젝트 사이 최소 거리

    private float cylinderRadius;
    private float cylinderHeight;
    private List<Vector3> spawnedPositions = new List<Vector3>(); // 이미 생성된 위치들

    private void Start()
    {
        // Cylinder의 반지름과 높이 가져오기
        MeshRenderer renderer = cylinderObject.GetComponent<MeshRenderer>();
        cylinderRadius = renderer.bounds.extents.x; // X축 반지름
        cylinderHeight = renderer.bounds.size.y; // Y축 높이

        SpawnPrefabsOnCylinder();
    }

    private void SpawnPrefabsOnCylinder()
    {
        int attempts = 0;

        for (int i = 0; i < spawnCount; i++)
        {
            Vector3 randomPosition = Vector3.zero; // 초기화
            bool positionFound = false;

            // 최대 시도 횟수 (무한 루프 방지)
            while (attempts < 100)
            {
                randomPosition = GetRandomPositionOnCylinder();

                // 기존 위치들과의 거리 검사
                if (IsPositionValid(randomPosition))
                {
                    spawnedPositions.Add(randomPosition); // 유효한 위치를 목록에 추가
                    positionFound = true;
                    break;
                }
                attempts++;
            }

            // 유효한 위치가 발견되었을 때만 프리팹 생성
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
                return false; // 최소 거리 조건을 만족하지 않으면 false 반환
            }
        }
        return true; // 모든 위치와의 거리를 만족하면 true 반환
    }
}