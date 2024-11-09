using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterSpawner : MonoBehaviour
{
    public GameObject[] characterPrefabs;

    void Start()
    {
        // ���� ������ ����� Ȱ��ȭ�� ĳ���� �ε��� �ҷ�����
        int activeCharacterIndex = PlayerPrefs.GetInt("ActiveCharacterIndex", 0);

        // �ش� �ε����� ĳ���� ����
        if (activeCharacterIndex >= 0 && activeCharacterIndex < characterPrefabs.Length)
        {
            Instantiate(characterPrefabs[activeCharacterIndex], transform.position, Quaternion.identity);
        }
    }
}
