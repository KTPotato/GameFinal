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

    public GameObject SlotMachineUI; // UI ��ü�� ��Ȱ��ȭ�� ������Ʈ
    public List<int> StartList = new List<int>();
    public List<int> ResultIndexList = new List<int>();
    int ItemCnt = 3;
    int[] answer = { 2, 3, 1 };

    public bool SkillUp;
    // ��ų Ÿ�԰� ���� ���� ������ ��ųʸ�
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
            // �� ������ ���¸� �ʱ�ȭ (���� �̹��� �ʱ�ȭ �� ��ư ��Ȱ��ȭ)
            for (int i = 0; i < Slot.Length; i++)
            {
                // ���Կ� ǥ�õ� �̹��� �ʱ�ȭ (ȸ�� �� �̹����� �ʱ�ȭ)
                foreach (var slotImage in DisplayItemSlots[i].SlotSprite)
                {
                    // ȸ�� ���� ���� ��� �����ϰ� �̹����� ǥ��
                    if (slotImage.sprite == null)
                    {
                        slotImage.sprite = SkillSprite[Random.Range(0, SkillSprite.Length)];
                    }
                }

                // ��ư ��Ȱ��ȭ
                //Slot[i].interactable = false;
            }

            // ������ ��� ����Ʈ �ʱ�ȭ (������ ����� �������� �ٽ� ����)
            ResultIndexList.Clear();
            for (int i = 0; i < Slot.Length; i++)
            {
                ResultIndexList.Add(Random.Range(0, SkillSprite.Length));  // ���ο� ���� ��� ����
            }

            // ���� ȸ�� �ڵ�ȭ ����
            StartCoroutine(AutomateSlotRotation());

            SkillUp = false;  // SkillUp�� false�� �����Ͽ� ���� �ʱ�ȭ
        }
    }



    IEnumerator AutomateSlotRotation()
    {
        Time.timeScale = 0f;

        // �� ������ ���� �ð� ���� ȸ����Ŵ
        for (int i = 0; i < Slot.Length; i++)
        {
            StartList.Add(i); // ���� �ε����� ���� ����Ʈ�� ����
            ResultIndexList.Add(Random.Range(0, SkillSprite.Length)); // �� ������ ���� ��� �ε��� ����
            yield return StartCoroutine(StartSlot(i)); // ���� ȸ�� ����
        }
    }


    IEnumerator StartSlot(int SlotIndex)
    {
        // ���� ȸ�� �ִϸ��̼�
        for (int i = 0; i < (ItemCnt * (6 + SlotIndex * 4) + answer[SlotIndex]) * 2; i++)
        {
            SlotSkillObject[SlotIndex].transform.localPosition -= new Vector3(0, 50f, 0);
            if (SlotSkillObject[SlotIndex].transform.localPosition.y < 50f)
            {
                SlotSkillObject[SlotIndex].transform.localPosition += new Vector3(0, 300f, 0);
            }

            // ȸ�� �� ���� �̹��� ������Ʈ
            foreach (var slotImage in DisplayItemSlots[SlotIndex].SlotSprite)
            {
                slotImage.sprite = SkillSprite[Random.Range(0, SkillSprite.Length)];
            }
            yield return new WaitForSecondsRealtime(0.02f);
        }

        // ������ ���� �� ���õ� ��ų �ݿ�
        UpdateSlotResult(SlotIndex);
    }

    void UpdateSlotResult(int SlotIndex)
    {
        // ���õ� ��ų �ε����� ResultIndexList�� ����
        int resultIndex = ResultIndexList[SlotIndex];

        // �ش� ��ų�� ǥ���� �̹��� ������Ʈ
        UpdateSlotUI(SlotIndex, resultIndex);

        // ��ư�� Ȱ��ȭ�Ͽ� Ŭ���� �� �ֵ��� ���� (ȿ�� ���� ��ư)
        Slot[SlotIndex].interactable = true;
        Slot[SlotIndex].onClick.RemoveAllListeners(); // ���� ������ ����
        Slot[SlotIndex].onClick.AddListener(() => ApplySkillEffect(resultIndex)); // Ŭ�� �� ApplySkillEffect ȣ��

    }


    void UpdateSlotUI(int SlotIndex, int resultIndex)
    {
        // �ش� ������ ��������Ʈ ������Ʈ
        foreach (var slotImage in DisplayItemSlots[SlotIndex].SlotSprite)
        {
            slotImage.sprite = SkillSprite[resultIndex];
        }
    }

    public void ApplySkillEffect(int resultIndex)
    {
        // ��ų ȿ�� ���� (��: ���ǵ�, ü�� ���� ��)
        var skillEffect = skillEffects[resultIndex];
        string skillType = skillEffect.skillType;
        float value = skillEffect.value;

        // PlayerData�� ���� ������Ʈ
        PlayerData.Instance.UpdatePlayerStats(skillType, value);

        // ����� ȿ���� ���� �α� ��� (������)
        //Debug.Log($"Skill: {skillType} - Effect Value: {value}");

        SkillUp = true;
        SlotMachineUI.SetActive(false);

        Time.timeScale = 1f;
    }
}
