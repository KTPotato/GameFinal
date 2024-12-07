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
        // �̱��� ����
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
        // Player1Ctrl �ν��Ͻ��� ã�Ƽ� ����
        playerCtrl = FindObjectOfType<Player1Ctrl>();

        // �ʱ⿡�� SlotMachine UI�� ��Ȱ��ȭ
        if (SlotMachine != null)
        {
            SlotMachine.SetActive(false);
        }
    }

    void Update()
    {
        // Player1Ctrl�� �����ϰ�, �������� �Ǿ����� SlotMachine UI�� Ȱ��ȭ
        if (playerCtrl != null && playerCtrl.isLevelUp)
        {
            ShowSlotMachine();
            playerCtrl.isLevelUp = false; // ������ ���� ����
        }
    }

    public void ShowSlotMachine()
    {
        if (SlotMachine != null)
        {
            SlotMachine.SetActive(true); // ������ �� SlotMachine UI�� Ȱ��ȭ
        }
    }

    public void HideSlotMachine()
    {
        if (SlotMachine != null)
        {
            SlotMachine.SetActive(false); // ��ų ���� �� SlotMachine UI�� ��Ȱ��ȭ
        }
    }
}


//player isLeveUp�� false��