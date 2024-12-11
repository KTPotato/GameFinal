using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Portal : MonoBehaviour
{
    private bool isPlayerInPortal = false; // �÷��̾ ��Ż �ȿ� �ִ��� Ȯ��
    private Coroutine portalCoroutine; // �ڷ�ƾ ���� ����

    private void OnTriggerEnter(Collider other)
    {
        // �浹�� ��ü�� �÷��̾����� Ȯ��
        if (other.CompareTag("Player"))
        {
            //Debug.Log("�÷��̾ ��Ż�� �����߽��ϴ�.");
            isPlayerInPortal = true;

            // 3�� ��� �� �� ��ȯ �ڷ�ƾ ����
            if (portalCoroutine == null)
            {
                portalCoroutine = StartCoroutine(WaitAndLoadNextScene());
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // �÷��̾ ��Ż���� ����� ó��
        if (other.CompareTag("Player"))
        {
            //Debug.Log("�÷��̾ ��Ż���� �������ϴ�.");
            isPlayerInPortal = false;

            // �ڷ�ƾ �ߴ�
            if (portalCoroutine != null)
            {
                StopCoroutine(portalCoroutine);
                portalCoroutine = null;
            }
        }
    }

    private IEnumerator WaitAndLoadNextScene()
    {
        float waitTime = 3.0f; // ��� �ð�
        float elapsedTime = 0f;

        // 3�� ���� ��Ż �ȿ� �ִ��� Ȯ��
        while (elapsedTime < waitTime)
        {
            if (!isPlayerInPortal)
            {
                yield break; // �÷��̾ ������ �ڷ�ƾ ����
            }

            elapsedTime += Time.deltaTime;
            yield return null; // ���� �����ӱ��� ���
        }

        // 3�� ���� ��Ż�� �ӹ����ٸ� �� ��ȯ
        //Debug.Log("3�� ���� ��Ż�� �ӹ������ϴ�. ���� ������ �̵��մϴ�.");
        LoadNextScene();
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
            //Debug.Log("������ ���Դϴ�. �ٽ� ù ��° ������ �̵��մϴ�.");
            nextSceneIndex = 0; // ù ��° ������ ���ư�
        }

        // �� ��ȯ ���� Player�� Canvas ��ü�� �����ϵ��� ����
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            DontDestroyOnLoad(player); // Player ��ü�� �� ��ȯ �Ŀ��� ����
        }

        // ���� �� �ε�
        SceneManager.LoadScene(nextSceneIndex);
    }
}