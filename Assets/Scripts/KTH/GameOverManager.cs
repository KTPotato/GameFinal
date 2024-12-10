using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverManager : MonoBehaviour
{
    [Header("References")]
    public GameObject player;      // Player 오브젝트
    public GameObject gameOverUI;  // Game Over UI 오브젝트

    private bool isGameOver = false;

    void Update()
    {
        // Player가 삭제되었는지 확인
        if (player == null && !isGameOver)
        {
            ActivateGameOverUI();
        }
    }

    private void ActivateGameOverUI()
    {
        isGameOver = true;

        // Game Over UI 활성화
        if (gameOverUI != null)
        {
            gameOverUI.SetActive(true);
        }

        // 게임 멈춤 (선택 사항)
        Time.timeScale = 0f;

        Debug.Log("Game Over! UI Activated because the Player was destroyed.");
    }

    // 버튼 클릭 시 호출될 메서드
    public void LoadMainMenu()
    {
        // Time.timeScale을 복원
        Time.timeScale = 1f;

        // 첫 번째 씬(메인 메뉴)로 이동
        SceneManager.LoadScene(0);
    }
}