using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public AudioSource bgm;
    // Start is called before the first frame update
    void Start()
    {
        // 씬이 변경될 때도 BGM을 계속 유지하기 위해 Don'tDestroyOnLoad() 호출
        DontDestroyOnLoad(gameObject);

        bgm.Play();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
