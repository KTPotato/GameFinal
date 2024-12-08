using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossSpawner : MonoBehaviour
{
    public GameObject[] bossPrefabs; // 보스 프리팹 배열

    void Start()
    {
        // 저장된 캐릭터 인덱스 불러오기
        int selectedCharacterIndex = PlayerPrefs.GetInt("ActiveCharacterIndex", 0); // 기본값은 0

        // 해당 인덱스에 맞는 보스를 생성
        SpawnBoss(selectedCharacterIndex);
    }

    void SpawnBoss(int characterIndex)
    {
        // 보스 생성 (선택된 캐릭터에 맞는 보스를 생성)
        if (characterIndex >= 0 && characterIndex < bossPrefabs.Length)
        {
            Instantiate(bossPrefabs[characterIndex], Vector3.zero, Quaternion.identity);
        }
        else
        {
            Debug.LogError("Invalid character index, cannot spawn boss.");
        }
    }
}
