using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowersUI : MonoBehaviour
{
    [SerializeField] Tower towerPrefab;
    [SerializeField] Transform parent;
    [SerializeField] TowerCardUI cardPrefab;


    public void Init()
    {
        UpdateCards();
    }

    public void UpdateCards()
    {
        Clear();

        var availableKinds = PlayerData.MineData.availableKind;

        var countUnavailable = towerPrefab.Sets.Length - availableKinds.Count;

        foreach (var item in availableKinds)
        {
            Instantiate(cardPrefab, parent).InitAvailable(item);
        }

        for (int i = 0; i < countUnavailable; i++)
        {
            Instantiate(cardPrefab, parent).InitUnavailable();
        }
    }

    private void Clear()
    {
        foreach (Transform item in parent)
        {
            Destroy(item.gameObject);
        }
    }
}
