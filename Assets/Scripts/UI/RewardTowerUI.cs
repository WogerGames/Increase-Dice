using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class RewardTowerUI : MonoBehaviour
{
    [SerializeField] TMP_Text countCards;
    [SerializeField] TowerCardUI towerCard;

    public void Init(AvailableTower tower)
    {
        countCards.text = $"x{tower.count}";

        towerCard.InitAvailable(tower);
    }
}
