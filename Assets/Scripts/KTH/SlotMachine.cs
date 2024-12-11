using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SlotMachine : MonoBehaviour
{
    public GameObject[] SlotSkillObject;
    public Button[] Slot;

    public Sprite[] SkillSprite;

    [System.Serializable]
    public class DisplayItemSlot
    {
        public List<Image> SlotSprite = new List<Image>();
    }
    public DisplayItemSlot[] DisplayItemSlots;

    public GameObject SlotMachineUI; // UI 전체를 비활성화할 오브젝트
    public List<int> StartList = new List<int>();
    public List<int> ResultIndexList = new List<int>();
    int ItemCnt = 3;
    int[] answer = { 2, 3, 1 };

    // 스킬 타입과 증가 값을 저장할 딕셔너리
    private Dictionary<int, (string skillType, float value)> skillEffects = new Dictionary<int, (string, float)>
    {
        { 0, ("speed", 1.0f) },
        { 1, ("firespeed", 0.1f) },
        { 2, ("maxHp", 10.0f) },
        { 3, ("Hp", 20.0f) },
        { 4, ("dmg", 2.0f) },
        { 5, ("crossfireLevel", 1) },
        { 6, ("fan_fireLevel", 1) },
        { 7, ("spinballLevel", 1) }
    };

    void Start()
    {
       

    }


    IEnumerator StartSlot(int SlotIndex)
    {
        for (int i = 0; i < (ItemCnt * (6 + SlotIndex * 4) + answer[SlotIndex]) * 2; i++)
        {
            SlotSkillObject[SlotIndex].transform.localPosition -= new Vector3(0, 50f, 0);
            if (SlotSkillObject[SlotIndex].transform.localPosition.y < 50f)
            {
                SlotSkillObject[SlotIndex].transform.localPosition += new Vector3(0, 300f, 0);
            }
            yield return new WaitForSecondsRealtime(0.02f); // 게임이 일시정지 상태에서도 동작하도록 WaitForSecondsRealtime 사용
        }
        for (int i = 0; i < Slot.Length; i++)
        {
            Slot[i].interactable = true;
        }
    }

    public void ClickBtn(int index)
    {

    }
}