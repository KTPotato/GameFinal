using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterSpawner : MonoBehaviour
{
    public GameObject[] characterPrefabs;

    void Start()
    {
        // 이전 씬에서 저장된 활성화된 캐릭터 인덱스 불러오기
        int activeCharacterIndex = PlayerPrefs.GetInt("ActiveCharacterIndex", 0);

        // 해당 인덱스의 캐릭터 생성
        if (activeCharacterIndex >= 0 && activeCharacterIndex < characterPrefabs.Length)
        {
            Instantiate(characterPrefabs[activeCharacterIndex], transform.position, Quaternion.identity);
        }
    }
}
