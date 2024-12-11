using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SoundManager : MonoBehaviour
{
    public AudioSource bgm; // �⺻ BGM
    public AudioClip bossBgm; // ���� �������� BGM
    private string currentScene; // ���� �� �̸� ����

    private static SoundManager instance; // �̱��� ����

    void Awake()
    {
        // SoundManager�� �ߺ����� �ʵ��� ����
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        // ���� �� �̸� ����
        currentScene = SceneManager.GetActiveScene().name;
        bgm.Play();
    }

    // Update is called once per frame
    void Update()
    {
        // ���� ����Ǿ����� Ȯ��
        string newScene = SceneManager.GetActiveScene().name;
        if (newScene != currentScene)
        {
            currentScene = newScene;
            OnSceneChanged();
        }
    }

    private void OnSceneChanged()
    {
        // ���� �������������� BGM ����
        if (currentScene == "BossStage")
        {
            bgm.Stop();
            bgm.clip = bossBgm;
            bgm.Play();
        }
        else
        {
            // �ٸ� ������ ���ƿ��� �⺻ BGM���� ����
            if (bgm.clip != bossBgm)
            {
                bgm.Stop();
                bgm.clip = bgm.clip; // ������ BGM
                bgm.Play();
            }
        }
    }
}