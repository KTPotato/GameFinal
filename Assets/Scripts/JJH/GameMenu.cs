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
        // ���� ������ �̵�
        SceneManager.LoadScene("jjh"); // "NextSceneName"�� �̵��� �� �̸����� ����
    }
    void ExitGame()
    {
        #if UNITY_EDITOR
        // Unity �����Ϳ����� �÷��� ��带 ����
        EditorApplication.isPlaying = false;    
        #else
            // ����� ȯ�濡���� ������ ����
            Application.Quit();
        #endif
    }
}