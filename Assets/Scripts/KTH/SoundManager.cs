using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SoundManager : MonoBehaviour
{
    public AudioSource bgm; // 기본 BGM
    public AudioClip bossBgm; // 보스 스테이지 BGM
    private string currentScene; // 현재 씬 이름 저장

    private static SoundManager instance; // 싱글톤 패턴

    void Awake()
    {
        // SoundManager가 중복되지 않도록 설정
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
        // 현재 씬 이름 저장
        currentScene = SceneManager.GetActiveScene().name;
        bgm.Play();
    }

    // Update is called once per frame
    void Update()
    {
        // 씬이 변경되었는지 확인
        string newScene = SceneManager.GetActiveScene().name;
        if (newScene != currentScene)
        {
            currentScene = newScene;
            OnSceneChanged();
        }
    }

    private void OnSceneChanged()
    {
        // 보스 스테이지에서는 BGM 변경
        if (currentScene == "BossStage")
        {
            bgm.Stop();
            bgm.clip = bossBgm;
            bgm.Play();
        }
        else
        {
            // 다른 씬으로 돌아오면 기본 BGM으로 설정
            if (bgm.clip != bossBgm)
            {
                bgm.Stop();
                bgm.clip = bgm.clip; // 원래의 BGM
                bgm.Play();
            }
        }
    }
}