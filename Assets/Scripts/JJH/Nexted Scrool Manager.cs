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
        // �Ÿ��� ���� 0~1�� pos ����
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
        // Ÿ�� ��ġ�� ��ũ�ѹ� �̵�
        scrollbar.value = Mathf.Lerp(scrollbar.value, targetPos, 0.1f);
    }

    // ���� ��ġ�� �̵�
    void MoveToNext()
    {
        if (targetIndex < SIZE - 1)
        {
            targetIndex++;
            targetPos = pos[targetIndex];
        }
    }

    // ���� ��ġ�� �̵�
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
        // ���� Ȱ��ȭ�� ĳ���� �г� �ε��� ����
        PlayerPrefs.SetInt("ActiveCharacterIndex", targetIndex);
        //Debug.Log("���� �ε���" + targetIndex);
        PlayerPrefs.Save();

        // ���� ������ �̵�
        SceneManager.LoadScene("GameScene"); // "NextSceneName"�� �̵��� �� �̸����� ����
    }

}
