using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Portal : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        // 충돌한 객체가 플레이어인지 확인
        if (other.CompareTag("Player"))
        {
            Debug.Log("플레이어가 포탈에 진입했습니다. 다음 씬으로 이동합니다.");
            LoadNextScene();
        }
    }

    private void LoadNextScene()
    {
        // 현재 씬의 빌드 인덱스를 가져옴
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;

        // 다음 씬의 빌드 인덱스 계산
        int nextSceneIndex = currentSceneIndex + 1;

        // 빌드 설정된 씬의 개수를 가져옴
        int totalScenes = SceneManager.sceneCountInBuildSettings;

        // 다음 씬이 마지막 씬을 넘어가면 첫 번째 씬으로 돌아가도록 설정
        if (nextSceneIndex >= totalScenes)
        {
            Debug.Log("마지막 씬입니다. 다시 첫 번째 씬으로 이동합니다.");
            nextSceneIndex = 0; // 첫 번째 씬으로 돌아감
        }

        // 다음 씬 로드
        SceneManager.LoadScene(nextSceneIndex);
    }
}
