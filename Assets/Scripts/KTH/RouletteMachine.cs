using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RouletteMachine : MonoBehaviour
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

    public bool SkillUp;
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
        StartCoroutine(AutomateSlotRotation());
    }

    private void Update()
    {
        if (SkillUp)
        {
            // 각 슬롯의 상태를 초기화 (슬롯 이미지 초기화 및 버튼 비활성화)
            for (int i = 0; i < Slot.Length; i++)
            {
                // 슬롯에 표시된 이미지 초기화 (회전 중 이미지만 초기화)
                foreach (var slotImage in DisplayItemSlots[i].SlotSprite)
                {
                    // 회전 중일 때는 계속 랜덤하게 이미지를 표시
                    if (slotImage.sprite == null)
                    {
                        slotImage.sprite = SkillSprite[Random.Range(0, SkillSprite.Length)];
                    }
                }

                // 버튼 비활성화
                //Slot[i].interactable = false;
            }

            // 기존의 결과 리스트 초기화 (슬롯의 결과를 랜덤으로 다시 설정)
            ResultIndexList.Clear();
            for (int i = 0; i < Slot.Length; i++)
            {
                ResultIndexList.Add(Random.Range(0, SkillSprite.Length));  // 새로운 랜덤 결과 설정
            }

            // 슬롯 회전 자동화 시작
            StartCoroutine(AutomateSlotRotation());

            SkillUp = false;  // SkillUp을 false로 변경하여 상태 초기화
        }
    }



    IEnumerator AutomateSlotRotation()
    {
        Time.timeScale = 0f;

        // 각 슬롯을 일정 시간 동안 회전시킴
        for (int i = 0; i < Slot.Length; i++)
        {
            StartList.Add(i); // 슬롯 인덱스를 시작 리스트에 저장
            ResultIndexList.Add(Random.Range(0, SkillSprite.Length)); // 각 슬롯의 랜덤 결과 인덱스 저장
            yield return StartCoroutine(StartSlot(i)); // 슬롯 회전 시작
        }
    }


    IEnumerator StartSlot(int SlotIndex)
    {
        // 슬롯 회전 애니메이션
        for (int i = 0; i < (ItemCnt * (6 + SlotIndex * 4) + answer[SlotIndex]) * 2; i++)
        {
            SlotSkillObject[SlotIndex].transform.localPosition -= new Vector3(0, 50f, 0);
            if (SlotSkillObject[SlotIndex].transform.localPosition.y < 50f)
            {
                SlotSkillObject[SlotIndex].transform.localPosition += new Vector3(0, 300f, 0);
            }

            // 회전 중 랜덤 이미지 업데이트
            foreach (var slotImage in DisplayItemSlots[SlotIndex].SlotSprite)
            {
                slotImage.sprite = SkillSprite[Random.Range(0, SkillSprite.Length)];
            }
            yield return new WaitForSecondsRealtime(0.02f);
        }

        // 슬롯이 멈춘 후 선택된 스킬 반영
        UpdateSlotResult(SlotIndex);
    }

    void UpdateSlotResult(int SlotIndex)
    {
        // 선택된 스킬 인덱스를 ResultIndexList에 저장
        int resultIndex = ResultIndexList[SlotIndex];

        // 해당 스킬을 표시할 이미지 업데이트
        UpdateSlotUI(SlotIndex, resultIndex);

        // 버튼을 활성화하여 클릭할 수 있도록 설정 (효과 적용 버튼)
        Slot[SlotIndex].interactable = true;
        Slot[SlotIndex].onClick.RemoveAllListeners(); // 기존 리스너 제거
        Slot[SlotIndex].onClick.AddListener(() => ApplySkillEffect(resultIndex)); // 클릭 시 ApplySkillEffect 호출

    }


    void UpdateSlotUI(int SlotIndex, int resultIndex)
    {
        // 해당 슬롯의 스프라이트 업데이트
        foreach (var slotImage in DisplayItemSlots[SlotIndex].SlotSprite)
        {
            slotImage.sprite = SkillSprite[resultIndex];
        }
    }

    public void ApplySkillEffect(int resultIndex)
    {
        // 스킬 효과 적용 (예: 스피드, 체력 증가 등)
        var skillEffect = skillEffects[resultIndex];
        string skillType = skillEffect.skillType;
        float value = skillEffect.value;

        // PlayerData의 스탯 업데이트
        PlayerData.Instance.UpdatePlayerStats(skillType, value);

        // 적용된 효과에 대한 로그 출력 (디버깅용)
        //Debug.Log($"Skill: {skillType} - Effect Value: {value}");

        SkillUp = true;
        SlotMachineUI.SetActive(false);

        Time.timeScale = 1f;
    }
}
