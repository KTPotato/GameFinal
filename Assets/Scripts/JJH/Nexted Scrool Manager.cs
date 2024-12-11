using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Nexted_Scrool_Manager : MonoBehaviour
{
    public Scrollbar scrollbar;
    public Button nextButton;
    public Button nextButton1;
    public Button previousButton;
    public Button previousButton1;
    public Button GameStart;

    public GameObject[] characterPanels;

    const int SIZE = 3;
    float[] pos = new float[SIZE];
    float distance, targetPos;
    int targetIndex;

    void Start()
    {
        // 거리에 따라 0~1인 pos 대입
        distance = 1f / (SIZE - 1);
        for (int i = 0; i < SIZE; i++) { pos[i] = distance * i; }
        
        scrollbar.interactable = false;

        nextButton.onClick.AddListener(MoveToNext);
        previousButton.onClick.AddListener(MoveToPrevious);
        nextButton1.onClick.AddListener(MoveToNext);
        previousButton1.onClick.AddListener(MoveToPrevious);

        GameStart.onClick.AddListener(OnGameStart);
    }

    void Update()
    {
        // 타겟 위치로 스크롤바 이동
        scrollbar.value = Mathf.Lerp(scrollbar.value, targetPos, 0.1f);
    }

    // 다음 위치로 이동
    void MoveToNext()
    {
        if (targetIndex < SIZE - 1)
        {
            targetIndex++;
            targetPos = pos[targetIndex];
        }
    }

    // 이전 위치로 이동
    void MoveToPrevious()
    {
        if (targetIndex > 0)
        {
            targetIndex--;
            targetPos = pos[targetIndex];
        }
    }
    void OnGameStart()
    {
        // 현재 활성화된 캐릭터 패널 인덱스 저장
        PlayerPrefs.SetInt("ActiveCharacterIndex", targetIndex);
        //Debug.Log("보스 인덱스" + targetIndex);
        PlayerPrefs.Save();

        // 다음 씬으로 이동
        SceneManager.LoadScene("GameScene"); // "NextSceneName"을 이동할 씬 이름으로 변경
    }

}
