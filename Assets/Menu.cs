using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using TMPro;
using static Settings;
using System;

public class Menu : MonoBehaviour
{
    [SerializeField] TMP_Text countCards;
    [SerializeField] Button btnOpenChest;

    [SerializeField] Button btnStart;

    [SerializeField] TowersUI towers;
    [SerializeField] ChestReward chestReward;

    public static bool inited;

    private void Awake()
    {
        inited = true;

        PlayerData.MineData ??= new();

        chestReward.gameObject.SetActive(false);
    }

    private void Start()
    {
        chestReward.onCardsRewarded += Cards_Rewarded;

        btnStart.onClick.AddListener(Start_Clicked);
        btnOpenChest.onClick.AddListener(OpenChest_Clicked);

        countCards.text = $"{PlayerData.MineData.cards}/{CardsToChest}";

        towers.Init();

        UpdateBtnChest();
    }

    private void Cards_Rewarded()
    {
        towers.UpdateCards();
    }

    private void OpenChest_Clicked()
    {
        chestReward.gameObject.SetActive(true);
        chestReward.Init();
    }

    void UpdateBtnChest()
    {
        btnOpenChest.interactable = PlayerData.MineData.cards >= CardsToChest;
    }

    private void Start_Clicked()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(1);
    }
}
