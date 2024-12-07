using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UiController : MonoBehaviour
{
    public static UiController Instance;

    public GameObject SlotMachine;

    private Player1Ctrl playerCtrl;

    private void Awake()
    {
        // 싱글턴 설정
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        // Player1Ctrl 인스턴스를 찾아서 저장
        playerCtrl = FindObjectOfType<Player1Ctrl>();

        // 초기에는 SlotMachine UI를 비활성화
        if (SlotMachine != null)
        {
            SlotMachine.SetActive(false);
        }
    }

    void Update()
    {
        // Player1Ctrl이 존재하고, 레벨업이 되었으면 SlotMachine UI를 활성화
        if (playerCtrl != null && playerCtrl.isLevelUp)
        {
            ShowSlotMachine();
            playerCtrl.isLevelUp = false; // 레벨업 상태 리셋
        }
    }

    public void ShowSlotMachine()
    {
        if (SlotMachine != null)
        {
            SlotMachine.SetActive(true); // 레벨업 시 SlotMachine UI를 활성화
        }
    }

    public void HideSlotMachine()
    {
        if (SlotMachine != null)
        {
            SlotMachine.SetActive(false); // 스킬 선택 후 SlotMachine UI를 비활성화
        }
    }
}


//player isLeveUp을 false로