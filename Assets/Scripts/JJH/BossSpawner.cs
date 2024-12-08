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
            Instantiate(bossPrefabs[characterIndex], Vector3.zero, Quaternion.identity);
        }
        else
        {
            Debug.LogError("Invalid character index, cannot spawn boss.");
        }
    }
}
