using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Portal : MonoBehaviour
{
    private bool isPlayerInPortal = false; // 플레이어가 포탈 안에 있는지 확인
    private Coroutine portalCoroutine; // 코루틴 참조 저장

    private void OnTriggerEnter(Collider other)
    {
        // 충돌한 객체가 플레이어인지 확인
        if (other.CompareTag("Player"))
        {
            //Debug.Log("플레이어가 포탈에 진입했습니다.");
            isPlayerInPortal = true;

            // 3초 대기 후 씬 전환 코루틴 시작
            if (portalCoroutine == null)
            {
                portalCoroutine = StartCoroutine(WaitAndLoadNextScene());
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // 플레이어가 포탈에서 벗어나면 처리
        if (other.CompareTag("Player"))
        {
            //Debug.Log("플레이어가 포탈에서 나갔습니다.");
            isPlayerInPortal = false;

            // 코루틴 중단
            if (portalCoroutine != null)
            {
                StopCoroutine(portalCoroutine);
                portalCoroutine = null;
            }
        }
    }

    private IEnumerator WaitAndLoadNextScene()
    {
        float waitTime = 3.0f; // 대기 시간
        float elapsedTime = 0f;

        // 3초 동안 포탈 안에 있는지 확인
        while (elapsedTime < waitTime)
        {
            if (!isPlayerInPortal)
            {
                yield break; // 플레이어가 나가면 코루틴 종료
            }

            elapsedTime += Time.deltaTime;
            yield return null; // 다음 프레임까지 대기
        }

        // 3초 동안 포탈에 머물렀다면 씬 전환
        //Debug.Log("3초 동안 포탈에 머물렀습니다. 다음 씬으로 이동합니다.");
        LoadNextScene();
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
            //Debug.Log("마지막 씬입니다. 다시 첫 번째 씬으로 이동합니다.");
            nextSceneIndex = 0; // 첫 번째 씬으로 돌아감
        }

        // 씬 전환 전에 Player와 Canvas 객체를 유지하도록 설정
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            DontDestroyOnLoad(player); // Player 객체를 씬 전환 후에도 유지
        }

        // 다음 씬 로드
        SceneManager.LoadScene(nextSceneIndex);
    }
}