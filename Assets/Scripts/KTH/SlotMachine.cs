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
        for (int i = 0; i < ItemCnt * Slot.Length; i++)
        {
            StartList.Add(i);
        }

        for (int i = 0; i < Slot.Length; i++)
        {
            for (int j = 0; j < ItemCnt; j++)
            {
                Slot[i].interactable = false;

                int randomIndex = Random.Range(0, StartList.Count);
                if (i == 0 && j == 1 || i == 1 && j == 0 || i == 2 && j == 2)
                {
                    ResultIndexList.Add(StartList[randomIndex]);
                }
                DisplayItemSlots[i].SlotSprite[j].sprite = SkillSprite[StartList[randomIndex]];

                if (j == 0)
                {
                    DisplayItemSlots[i].SlotSprite[ItemCnt].sprite = SkillSprite[StartList[randomIndex]];
                }
                StartList.RemoveAt(randomIndex);
            }
        }

        for (int i = 0; i < Slot.Length; i++)
        {
            StartCoroutine(StartSlot(i));
        }

        // UI가 활성화된 상태에서 게임을 일시정지
        Time.timeScale = 0f;
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
        Debug.Log($"ClickBtn 호출됨: index = {index}, ResultIndexList.Count = {ResultIndexList.Count}");

        if (index < 0 || index >= ResultIndexList.Count)
        {
            Debug.LogError($"잘못된 index 값: {index}. ResultIndexList 범위를 초과했습니다.");
            return;
        }

        // 선택된 스킬 효과 적용
        if (skillEffects.TryGetValue(ResultIndexList[index], out var skillEffect))
        {
            PlayerData.Instance.UpdatePlayerStats(skillEffect.skillType, skillEffect.value);
            Debug.Log($"스킬 {skillEffect.skillType}이(가) {skillEffect.value}만큼 업데이트되었습니다.");
        }
        else
        {
            Debug.LogWarning($"스킬 효과를 찾을 수 없습니다: index = {ResultIndexList[index]}");
        }

        // UI 비활성화 및 게임 재개
        SlotMachineUI.SetActive(false);
        Time.timeScale = 1f;
    }
}