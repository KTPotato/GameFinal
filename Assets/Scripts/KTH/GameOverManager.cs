using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverManager : MonoBehaviour
{
    [Header("References")]
    public GameObject player;      // Player ������Ʈ
    public GameObject gameOverUI;  // Game Over UI ������Ʈ

    private bool isGameOver = false;

    void Update()
    {
        // Player�� �����Ǿ����� Ȯ��
        if (player == null && !isGameOver)
        {
            ActivateGameOverUI();
        }
    }

    private void ActivateGameOverUI()
    {
        isGameOver = true;

        // Game Over UI Ȱ��ȭ
        if (gameOverUI != null)
        {
            gameOverUI.SetActive(true);
        }

        // ���� ���� (���� ����)
        Time.timeScale = 0f;

        Debug.Log("Game Over! UI Activated because the Player was destroyed.");
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