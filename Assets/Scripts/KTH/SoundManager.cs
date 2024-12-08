using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public AudioSource bgm;
    // Start is called before the first frame update
    void Start()
    {
        // ���� ����� ���� BGM�� ��� �����ϱ� ���� Don'tDestroyOnLoad() ȣ��
        DontDestroyOnLoad(gameObject);

        bgm.Play();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
