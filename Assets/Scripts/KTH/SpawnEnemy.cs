using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnEnemy : MonoBehaviour
{
    public GameObject enemyPrefab; // 몬스터 프리팹
    public GameObject spawnPointPrefab; // 스폰 포인트 프리팹
    public int numberOfSpawnPoints = 5; // 생성할 스폰 포인트 개수
    public int enemiesPerSpawnPoint = 3; // 각 스폰 포인트당 몬스터 개수
    public Vector2 spawnArea = new Vector2(10f, 10f); // 스폰 범위
    public string enemyTag = "Enemy"; // 몬스터 태그

    private List<GameObject> spawnPoints = new List<GameObject>();
    private bool isSpawning = false;

    void Start()
    {
        if (spawnPointPrefab == null || enemyPrefab == null)
        {
            Debug.LogError("SpawnPointPrefab 또는 EnemyPrefab이 설정되지 않았습니다.");
            return;
        }

        GenerateSpawnPoints();
        SpawnAllEnemies();
    }

    void Update()
    {
        // 몬스터가 모두 제거되었는지 확인하고 다시 소환
        if (!isSpawning && GameObject.FindGameObjectsWithTag(enemyTag).Length == 0)
        {
            Debug.Log("모든 몬스터가 제거되었습니다. 다시 소환합니다.");
            SpawnAllEnemies();
        }
    }

    // 랜덤한 위치에 스폰 포인트 생성
    void GenerateSpawnPoints()
    {
        for (int i = 0; i < numberOfSpawnPoints; i++)
        {
            Vector3 randomPosition = new Vector3(
                Random.Range(-spawnArea.x, spawnArea.x),
                0,
                Random.Range(-spawnArea.y, spawnArea.y)
            );

            GameObject spawnPoint = Instantiate(spawnPointPrefab, randomPosition, Quaternion.identity);
            spawnPoints.Add(spawnPoint);
        }
    }

    // 모든 스폰 포인트에서 몬스터를 소환
    void SpawnAllEnemies()
    {
        StartCoroutine(SpawnEnemiesCoroutine());
    }

    // 코루틴을 사용하여 몬스터를 한 번에 소환
    IEnumerator SpawnEnemiesCoroutine()
    {
        isSpawning = true;

        foreach (GameObject spawnPoint in spawnPoints)
        {
            for (int i = 0; i < enemiesPerSpawnPoint; i++)
            {
                Vector3 position = spawnPoint.transform.position;
                GameObject enemy = Instantiate(enemyPrefab, position, Quaternion.identity);
                enemy.tag = enemyTag; // 몬스터에 태그 지정
                yield return new WaitForSeconds(0.1f); // 소환 간 약간의 지연 시간
            }
        }

        isSpawning = false;
    }
}
