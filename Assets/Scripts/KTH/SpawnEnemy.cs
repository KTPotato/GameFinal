using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.VersionControl;
using UnityEngine;

public class SpawnEnemy : MonoBehaviour
{
    public List<GameObject> enemyPrefabs; // ��ȯ�� ���� ������ ����Ʈ
    public GameObject spawnPointPrefab; // ���� ����Ʈ ������
    public int numberOfSpawnPoints = 5; // ������ ���� ����Ʈ ����
    public int enemiesPerSpawnPoint = 3; // �� ���� ����Ʈ�� ���� ����
    public Vector2 spawnArea = new Vector2(10f, 10f); // ���� ����
    public string enemyTag = "Enemy"; // ���� �±�
    public float spawnYPosition = 1.0f; // ������ y ��ǥ
    public GameObject portalPrefab; // ��Ż ������
    public GameObject ground; // Ground ������Ʈ ����
    public float portalYPosition = 1.0f; // ��Ż�� y ��ǥ ����

    private List<GameObject> spawnPoints = new List<GameObject>();
    private int currentPhase = 0; // ���� ��ȯ �ܰ� (0: ù ��° ������, 1: �� ��° ������, 2: ȥ��)
    private bool isSpawning = false;
    private bool portalSpawned = false; // ��Ż ���� ����

    void Start()
    {
        if (spawnPointPrefab == null || enemyPrefabs == null || enemyPrefabs.Count < 2 || portalPrefab == null || ground == null)
        {
            //Debug.LogError("�ʼ� ������ �Ǵ� ������Ʈ�� �������� �ʾҽ��ϴ�.");
            return;
        }

        GenerateSpawnPoints();
        SpawnNextPhase();
    }

    void Update()
    {
        // ��� ���Ͱ� ���ŵǾ����� Ȯ���ϰ� ���� �ܰ�� ����
        if (!isSpawning && GameObject.FindGameObjectsWithTag(enemyTag).Length == 0)
        {
            //Debug.Log("��� ���Ͱ� ���ŵǾ����ϴ�. ���� �ܰ�� �����մϴ�.");
            SpawnNextPhase();
        }
    }

    // ������ ��ġ�� ���� ����Ʈ ����
    void GenerateSpawnPoints()
    {
            for (int i = 0; i < numberOfSpawnPoints; i++)
        {
            Vector3 randomPosition = new Vector3(
                Random.Range(-spawnArea.x, spawnArea.x),
                spawnYPosition, // ������ y ��
                Random.Range(-spawnArea.y, spawnArea.y)
            );

                GameObject spawnPoint = Instantiate(spawnPointPrefab, randomPosition, Quaternion.identity);
                spawnPoints.Add(spawnPoint) ;
        }
    }

    // ���� �ܰ� ��ȯ ó��
    void SpawnNextPhase()
    {
        if (currentPhase > 2) // 3�ܰ踦 �Ϸ��ϸ� �� �̻� ��ȯ���� ����
        {
            //Debug.Log("��� �ܰ踦 �Ϸ��߽��ϴ�. ��Ż�� �����մϴ�.");
            SpawnPortal();
            return;
        }

        switch (currentPhase)
        {
            case 0:
                SpawnAllEnemies(enemyPrefabs[0]); // ù ��° ������ ��ȯ
                break;
            case 1:
                SpawnAllEnemies(enemyPrefabs[1]); // �� ��° ������ ��ȯ
                break;
            case 2:
                SpawnMixedEnemies(enemyPrefabs[0], enemyPrefabs[1]); // �� ������ ��� ��ȯ
                break;
        }

        currentPhase++;
    }

    // ��Ż ����
    void SpawnPortal()
    {
        if (portalSpawned)
        {
            //Debug.Log("��Ż�� �̹� �����Ǿ����ϴ�.");
            return;
        }

        if (ground != null)
        {
            Renderer groundRenderer = ground.GetComponent<Renderer>();
            if (groundRenderer != null)
            {
                // Ground �߾� ��ġ ���
                Vector3 groundCenter = groundRenderer.bounds.center;

                // ��Ż ���� ��ġ ���� (����ڰ� ������ Y ������ ����)
                Vector3 portalPosition = new Vector3(groundCenter.x, portalYPosition, groundCenter.z);

                // ��Ż ����
                Instantiate(portalPrefab, portalPosition, Quaternion.identity);
                portalSpawned = true; // ��Ż ���� �÷��� ����
                //Debug.Log("��Ż�� �����Ǿ����ϴ�!");
            }
            else
            {
                //Debug.LogError("Ground ������Ʈ�� Renderer ������Ʈ�� �����ϴ�.");
            }
        }
        else
        {
            //Debug.LogError("Ground ������Ʈ�� �������� �ʾҽ��ϴ�.");
        }
    }

    // ��� ���� ����Ʈ���� Ư�� ������ ���͸� ��ȯ
    void SpawnAllEnemies(GameObject prefab)
    {
        StartCoroutine(SpawnEnemiesCoroutine(prefab));
    }

    // ��� ���� ����Ʈ���� �� ���� ������ ���͸� ��� ��ȯ
    void SpawnMixedEnemies(GameObject prefab1, GameObject prefab2)
    {
        StartCoroutine(SpawnMixedEnemiesCoroutine(prefab1, prefab2));
    }

    // Ư�� ���������� ���͸� ��ȯ
    IEnumerator SpawnEnemiesCoroutine(GameObject prefab)
    {
        isSpawning = true;

        foreach (GameObject spawnPoint in spawnPoints)
        {
            for (int i = 0; i < enemiesPerSpawnPoint; i++)
            {
                Vector3 position = spawnPoint.transform.position;
                position.y = spawnYPosition;
                GameObject enemy = Instantiate(prefab, position, Quaternion.identity);
                enemy.tag = enemyTag;
                yield return new WaitForSeconds(0.1f); // ��ȯ �� �ణ�� ���� �ð�
            }
        }

        isSpawning = false;
    }

    // �� ���� �������� ��� ���͸� ��ȯ
    IEnumerator SpawnMixedEnemiesCoroutine(GameObject prefab1, GameObject prefab2)
    {
        isSpawning = true;

        foreach (GameObject spawnPoint in spawnPoints)
        {
            for (int i = 0; i < enemiesPerSpawnPoint; i++)
            {
                Vector3 position = spawnPoint.transform.position;
                position.y = spawnYPosition;

                // �������� �����ư��� ��ȯ
                GameObject prefabToSpawn = (i % 2 == 0) ? prefab1 : prefab2;
                GameObject enemy = Instantiate(prefabToSpawn, position, Quaternion.identity);
                enemy.tag = enemyTag;
                yield return new WaitForSeconds(0.1f);
            }
        }

        isSpawning = false;
    }
}
