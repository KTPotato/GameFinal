using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Portal : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        // �浹�� ��ü�� �÷��̾����� Ȯ��
        if (other.CompareTag("Player"))
        {
            Debug.Log("�÷��̾ ��Ż�� �����߽��ϴ�. ���� ������ �̵��մϴ�.");
            LoadNextScene();
        }
    }

    private void LoadNextScene()
    {
        // ���� ���� ���� �ε����� ������
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;

        // ���� ���� ���� �ε��� ���
        int nextSceneIndex = currentSceneIndex + 1;

        // ���� ������ ���� ������ ������
        int totalScenes = SceneManager.sceneCountInBuildSettings;

        // ���� ���� ������ ���� �Ѿ�� ù ��° ������ ���ư����� ����
        if (nextSceneIndex >= totalScenes)
        {
            Debug.Log("������ ���Դϴ�. �ٽ� ù ��° ������ �̵��մϴ�.");
            nextSceneIndex = 0; // ù ��° ������ ���ư�
        }

        // ���� �� �ε�
        SceneManager.LoadScene(nextSceneIndex);
    }
}
