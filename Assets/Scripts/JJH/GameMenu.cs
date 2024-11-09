using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEditor;

public class GameMenu : MonoBehaviour
{
    public Button GameStart;
    public Button GameEnd;

    void Start()
    {
        GameStart.onClick.AddListener(OnGameStart);
        GameEnd.onClick.AddListener(ExitGame);
    }

    void Update()
    {
        
    }
    void OnGameStart()
    {
        // 다음 씬으로 이동
        SceneManager.LoadScene("jjh"); // "NextSceneName"을 이동할 씬 이름으로 변경
    }
    void ExitGame()
    {
        #if UNITY_EDITOR
        // Unity 에디터에서는 플레이 모드를 중지
        EditorApplication.isPlaying = false;    
        #else
            // 빌드된 환경에서는 게임을 종료
            Application.Quit();
        #endif
    }
}