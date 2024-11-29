using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnEnemy : MonoBehaviour
{
    public GameObject enemyPrefab; // ���� ������
    public GameObject spawnPointPrefab; // ���� ����Ʈ ������
    public int numberOfSpawnPoints = 5; // ������ ���� ����Ʈ ����
    public int enemiesPerSpawnPoint = 3; // �� ���� ����Ʈ�� ���� ����
    public Vector2 spawnArea = new Vector2(10f, 10f); // ���� ����
    public string enemyTag = "Enemy"; // ���� �±�

    private List<GameObject> spawnPoints = new List<GameObject>();
    private bool isSpawning = false;

    void Start()
    {
        if (spawnPointPrefab == null || enemyPrefab == null)
        {
            Debug.LogError("SpawnPointPrefab �Ǵ� EnemyPrefab�� �������� �ʾҽ��ϴ�.");
            return;
        }

        GenerateSpawnPoints();
        SpawnAllEnemies();
    }

    void Update()
    {
        // ���Ͱ� ��� ���ŵǾ����� Ȯ���ϰ� �ٽ� ��ȯ
        if (!isSpawning && GameObject.FindGameObjectsWithTag(enemyTag).Length == 0)
        {
            Debug.Log("��� ���Ͱ� ���ŵǾ����ϴ�. �ٽ� ��ȯ�մϴ�.");
            SpawnAllEnemies();
        }
    }

    // ������ ��ġ�� ���� ����Ʈ ����
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

    // ��� ���� ����Ʈ���� ���͸� ��ȯ
    void SpawnAllEnemies()
    {
        StartCoroutine(SpawnEnemiesCoroutine());
    }

    // �ڷ�ƾ�� ����Ͽ� ���͸� �� ���� ��ȯ
    IEnumerator SpawnEnemiesCoroutine()
    {
        isSpawning = true;

        foreach (GameObject spawnPoint in spawnPoints)
        {
            for (int i = 0; i < enemiesPerSpawnPoint; i++)
            {
                Vector3 position = spawnPoint.transform.position;
                GameObject enemy = Instantiate(enemyPrefab, position, Quaternion.identity);
                enemy.tag = enemyTag; // ���Ϳ� �±� ����
                yield return new WaitForSeconds(0.1f); // ��ȯ �� �ణ�� ���� �ð�
            }
        }

        isSpawning = false;
    }
}
