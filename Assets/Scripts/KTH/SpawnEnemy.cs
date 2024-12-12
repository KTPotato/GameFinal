using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.VersionControl;
using UnityEngine;

public class SpawnEnemy : MonoBehaviour
{
    public List<GameObject> enemyPrefabs; // 소환할 몬스터 프리팹 리스트
    public GameObject spawnPointPrefab; // 스폰 포인트 프리팹
    public int numberOfSpawnPoints = 5; // 생성할 스폰 포인트 개수
    public int enemiesPerSpawnPoint = 3; // 각 스폰 포인트당 몬스터 개수
    public Vector2 spawnArea = new Vector2(10f, 10f); // 스폰 범위
    public string enemyTag = "Enemy"; // 몬스터 태그
    public float spawnYPosition = 1.0f; // 고정된 y 좌표
    public GameObject portalPrefab; // 포탈 프리팹
    public GameObject ground; // Ground 오브젝트 참조
    public float portalYPosition = 1.0f; // 포탈의 y 좌표 설정

    private List<GameObject> spawnPoints = new List<GameObject>();
    private int currentPhase = 0; // 현재 소환 단계 (0: 첫 번째 프리팹, 1: 두 번째 프리팹, 2: 혼합)
    private bool isSpawning = false;
    private bool portalSpawned = false; // 포탈 생성 여부

    void Start()
    {
        if (spawnPointPrefab == null || enemyPrefabs == null || enemyPrefabs.Count < 2 || portalPrefab == null || ground == null)
        {
            //Debug.LogError("필수 프리팹 또는 오브젝트가 설정되지 않았습니다.");
            return;
        }

        GenerateSpawnPoints();
        SpawnNextPhase();
    }

    void Update()
    {
        // 모든 몬스터가 제거되었는지 확인하고 다음 단계로 진행
        if (!isSpawning && GameObject.FindGameObjectsWithTag(enemyTag).Length == 0)
        {
            //Debug.Log("모든 몬스터가 제거되었습니다. 다음 단계로 진행합니다.");
            SpawnNextPhase();
        }
    }

    // 랜덤한 위치에 스폰 포인트 생성
    void GenerateSpawnPoints()
    {
            for (int i = 0; i < numberOfSpawnPoints; i++)
        {
            Vector3 randomPosition = new Vector3(
                Random.Range(-spawnArea.x, spawnArea.x),
                spawnYPosition, // 고정된 y 값
                Random.Range(-spawnArea.y, spawnArea.y)
            );

                GameObject spawnPoint = Instantiate(spawnPointPrefab, randomPosition, Quaternion.identity);
                spawnPoints.Add(spawnPoint) ;
        }
    }

    // 다음 단계 소환 처리
    void SpawnNextPhase()
    {
        if (currentPhase > 2) // 3단계를 완료하면 더 이상 소환하지 않음
        {
            //Debug.Log("모든 단계를 완료했습니다. 포탈을 생성합니다.");
            SpawnPortal();
            return;
        }

        switch (currentPhase)
        {
            case 0:
                SpawnAllEnemies(enemyPrefabs[0]); // 첫 번째 프리팹 소환
                break;
            case 1:
                SpawnAllEnemies(enemyPrefabs[1]); // 두 번째 프리팹 소환
                break;
            case 2:
                SpawnMixedEnemies(enemyPrefabs[0], enemyPrefabs[1]); // 두 프리팹 섞어서 소환
                break;
        }

        currentPhase++;
    }

    // 포탈 생성
    void SpawnPortal()
    {
        if (portalSpawned)
        {
            //Debug.Log("포탈은 이미 생성되었습니다.");
            return;
        }

        if (ground != null)
        {
            Renderer groundRenderer = ground.GetComponent<Renderer>();
            if (groundRenderer != null)
            {
                // Ground 중앙 위치 계산
                Vector3 groundCenter = groundRenderer.bounds.center;

                // 포탈 생성 위치 설정 (사용자가 지정한 Y 값으로 변경)
                Vector3 portalPosition = new Vector3(groundCenter.x, portalYPosition, groundCenter.z);

                // 포탈 생성
                Instantiate(portalPrefab, portalPosition, Quaternion.identity);
                portalSpawned = true; // 포탈 생성 플래그 설정
                //Debug.Log("포탈이 생성되었습니다!");
            }
            else
            {
                //Debug.LogError("Ground 오브젝트에 Renderer 컴포넌트가 없습니다.");
            }
        }
        else
        {
            //Debug.LogError("Ground 오브젝트가 설정되지 않았습니다.");
        }
    }

    // 모든 스폰 포인트에서 특정 프리팹 몬스터를 소환
    void SpawnAllEnemies(GameObject prefab)
    {
        StartCoroutine(SpawnEnemiesCoroutine(prefab));
    }

    // 모든 스폰 포인트에서 두 개의 프리팹 몬스터를 섞어서 소환
    void SpawnMixedEnemies(GameObject prefab1, GameObject prefab2)
    {
        StartCoroutine(SpawnMixedEnemiesCoroutine(prefab1, prefab2));
    }

    // 특정 프리팹으로 몬스터를 소환
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
                yield return new WaitForSeconds(0.1f); // 소환 간 약간의 지연 시간
            }
        }

        isSpawning = false;
    }

    // 두 개의 프리팹을 섞어서 몬스터를 소환
    IEnumerator SpawnMixedEnemiesCoroutine(GameObject prefab1, GameObject prefab2)
    {
        isSpawning = true;

        foreach (GameObject spawnPoint in spawnPoints)
        {
            for (int i = 0; i < enemiesPerSpawnPoint; i++)
            {
                Vector3 position = spawnPoint.transform.position;
                position.y = spawnYPosition;

                // 프리팹을 번갈아가며 소환
                GameObject prefabToSpawn = (i % 2 == 0) ? prefab1 : prefab2;
                GameObject enemy = Instantiate(prefabToSpawn, position, Quaternion.identity);
                enemy.tag = enemyTag;
                yield return new WaitForSeconds(0.1f);
            }
        }

        isSpawning = false;
    }
}
