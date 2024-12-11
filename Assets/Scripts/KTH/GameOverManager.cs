using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverManager : MonoBehaviour
{
    public GameObject gameOverUI;  // Game Over UI 오브젝트

    public List<GameObject> allTargets = new List<GameObject>();

    private bool isGameOver = false;

    void Start()
    {
        // Game Over UI를 시작 시 비활성화
        if (gameOverUI != null)
        {
            gameOverUI.SetActive(false);
        }
    }

    void Update()
    {
        if (isGameOver) return; // 게임 오버 상태면 더 이상 체크하지 않음

        allTargets.Clear();
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        allTargets.AddRange(players);

        bool isTargetExist = false; // 매 프레임마다 초기화

        foreach (GameObject obj in allTargets)
        {
            // 자신을 제외한 오브젝트가 있는지 확인
            if (obj.GetInstanceID() != gameObject.GetInstanceID())
            {
                isTargetExist = true;
                break;
            }
        }

        if (!isTargetExist) // 타겟이 없을 때만 Game Over 처리
        {
            ActivateGameOverUI();
        }
        else
        {
            //Debug.Log("플레이어가 있습니다.");
        }
    }

    public void ActivateGameOverUI()
    {
        isGameOver = true; // Game Over 상태로 설정

        // Game Over UI 활성화
        if (gameOverUI != null)
        {
            gameOverUI.SetActive(true); // 활성화로 변경
        }

        // 게임 멈춤 (선택 사항)
        Time.timeScale = 0f;

        //Debug.Log("Game Over! UI Activated because the Player was destroyed.");
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