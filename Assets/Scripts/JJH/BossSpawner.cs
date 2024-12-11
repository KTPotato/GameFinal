using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossSpawner : MonoBehaviour
{
    public GameObject[] bossPrefabs; // ���� ������ �迭

    void Start()
    {
        // ����� ĳ���� �ε��� �ҷ�����
        int selectedCharacterIndex = PlayerPrefs.GetInt("ActiveCharacterIndex", 0); // �⺻���� 0

        // �ش� �ε����� �´� ������ ����
        SpawnBoss(selectedCharacterIndex);
    }

    void SpawnBoss(int characterIndex)
    {
        // ���� ���� (���õ� ĳ���Ϳ� �´� ������ ����)
        if (characterIndex >= 0 && characterIndex < bossPrefabs.Length)
        {
            // ������ y = 2 ��ġ�� ����
            Vector3 spawnPosition = new Vector3(0, 2, -20); // x, y, z ��ǥ ����
            Instantiate(bossPrefabs[characterIndex], spawnPosition, Quaternion.identity);
        }
        else
        {
            //Debug.LogError("Invalid character index, cannot spawn boss.");
        }
    }
}
