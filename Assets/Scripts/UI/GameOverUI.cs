using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using TMPro;
using System;

public class GameOverUI : MonoBehaviour
{
    [SerializeField] TMP_Text countWaves;
    [SerializeField] TMP_Text countCards;

    [SerializeField] Button btnMenu;

    public void Init()
    {
        gameObject.SetActive(true);

        btnMenu.onClick.AddListener(Menu_Clicked);

        countWaves.text = "�������� ���� " + GameManager.Instance.Wave.ToString();
        countCards.text = "���������� �������� " + GameManager.Instance.Data.cards.ToString();
    }

    private void Menu_Clicked()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(0);
    }
}
