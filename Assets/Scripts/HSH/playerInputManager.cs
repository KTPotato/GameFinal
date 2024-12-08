using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class playerInputManager : MonoBehaviour
{
    [SerializeField] private Image hpImage;
    [SerializeField] private TMP_Text hpText;
    [SerializeField] private Image expImage;
    [SerializeField] private TMP_Text expText;

    private GameObject player;
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        player.GetComponent<Player1Ctrl>().hpImage = hpImage;
        player.GetComponent<Player1Ctrl>().hpText = hpText;
        player.GetComponent<Player1Ctrl>().expImage = expImage;
        player.GetComponent<Player1Ctrl>().expText = expText;
    }
}
