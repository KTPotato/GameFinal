using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Unity.VisualScripting;

public class Nested_Scrool_Manager : MonoBehaviour, IBeginDragHandler, IDragHandler ,IEndDragHandler
{
    public Scrollbar scrollbar;
    public Button nextButton;
    public Button nextButton1;
    public Button previousButton;
    public Button previousButton1;

    const int SIZE = 3;
    float[] pos = new float[SIZE];
    float distance,curPos ,targetPos;
    bool isDrag;
    int targetIndex;

    void Start()
    {
        //거리에 따라 0~1인 pos대입
        distance = 1f / (SIZE - 1);
        for (int i = 0; i < SIZE; i++) { pos[i] = distance * i; }
        nextButton.onClick.AddListener(MoveToNext);
        previousButton.onClick.AddListener(MoveToPrevious);
        nextButton1.onClick.AddListener(MoveToNext);
        previousButton1.onClick.AddListener(MoveToPrevious);
    }

    //드래그 시작했을 때
    public void OnBeginDrag(PointerEventData eventData)
    {
        curPos = SetPos();
    }
    //드래그 중일때
    public void OnDrag(PointerEventData eventData)
    {
        isDrag = true;
    }
    //드래그가 끝날때
    public void OnEndDrag(PointerEventData eventData)
    {
        isDrag = false;
        
        targetPos = SetPos();
    }

    void Update()
    {
        if (!isDrag)scrollbar.value = Mathf.Lerp(scrollbar.value, targetPos, 0.1f);
    }
    float SetPos() 
    {
        for (int i = 0; i < SIZE; i++)
            if (scrollbar.value < pos[i] + distance * 0.5f && scrollbar.value > pos[i] - distance * 0.5f)
            {
                targetIndex = i;
                return pos[i];
            }
        return 0;
    }

    // 다음 위치로 이동
    void MoveToNext()
    {
        if (targetIndex < SIZE - 1)
        {
            targetIndex++;
            targetPos = pos[targetIndex];
        }
    }

    // 이전 위치로 이동
    void MoveToPrevious()
    {
        if (targetIndex > 0)
        {
            targetIndex--;
            targetPos = pos[targetIndex];
        }
    }
}