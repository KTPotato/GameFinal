using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverManager : MonoBehaviour
{
    public GameObject gameOverUI;  // Game Over UI ������Ʈ

    public List<GameObject> allTargets = new List<GameObject>();

    private bool isGameOver = false;

    void Start()
    {
        // Game Over UI�� ���� �� ��Ȱ��ȭ
        if (gameOverUI != null)
        {
            gameOverUI.SetActive(false);
        }
    }

    void Update()
    {
        if (isGameOver) return; // ���� ���� ���¸� �� �̻� üũ���� ����

        allTargets.Clear();
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        allTargets.AddRange(players);

        bool isTargetExist = false; // �� �����Ӹ��� �ʱ�ȭ

        foreach (GameObject obj in allTargets)
        {
            // �ڽ��� ������ ������Ʈ�� �ִ��� Ȯ��
            if (obj.GetInstanceID() != gameObject.GetInstanceID())
            {
                isTargetExist = true;
                break;
            }
        }

        if (!isTargetExist) // Ÿ���� ���� ���� Game Over ó��
        {
            ActivateGameOverUI();
        }
        else
        {
            //Debug.Log("�÷��̾ �ֽ��ϴ�.");
        }
    }

    public void ActivateGameOverUI()
    {
        isGameOver = true; // Game Over ���·� ����

        // Game Over UI Ȱ��ȭ
        if (gameOverUI != null)
        {
            gameOverUI.SetActive(true); // Ȱ��ȭ�� ����
        }

        // ���� ���� (���� ����)
        Time.timeScale = 0f;

        //Debug.Log("Game Over! UI Activated because the Player was destroyed.");
    }

    // ��ư Ŭ�� �� ȣ��� �޼���
    public void LoadMainMenu()
    {
        // Time.timeScale�� ����
        Time.timeScale = 1f;

        // ù ��° ��(���� �޴�)�� �̵�
        SceneManager.LoadScene(0);
    }
}