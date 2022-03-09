using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using static Settings;

public class ChestReward : MonoBehaviour
{
    [SerializeField] RewardTowerUI cardPrefab;
    [SerializeField] Transform parent;

    [Header("Контейнер типов")]
    [SerializeField] Tower towerPrefab;

    public System.Action onCardsRewarded;

    public void Init()
    {
        Clear();

        var legendKinds = towerPrefab.Sets.ToList().FindAll(t => t.towerKind.IsLegendary()).Select(k => k.towerKind).ToList();
        var simpleKinds = towerPrefab.Sets.ToList().FindAll(t => !t.towerKind.IsLegendary()).Select(k => k.towerKind).ToList();

        int countSimpleKind = Random.Range(2, 4);

        for (int i = 0; i < countSimpleKind; i++)
        {
            RewardCards(simpleKinds);
        }

        var legendary = Random.Range(0, 100) < 10;

        if (legendary)
        {
            RewardCards(legendKinds);
        }

        void RewardCards(List<TowerKind> kinds)
        {
            int count = Random.Range(1, MaxRewardCards);

            var kind = kinds[Random.Range(0, kinds.Count)];

            var availableTower = new AvailableTower { kind = kind, count = count };

            Instantiate(cardPrefab, parent).Init(availableTower);

            AddCardsToTower(availableTower);
        }

        onCardsRewarded?.Invoke();
    }

    void AddCardsToTower(AvailableTower availableTower)
    {
        var availableTowers = PlayerData.MineData.availableKind;

        var tower = availableTowers.Find(t => t.kind == availableTower.kind);
        
        if (tower != null)
        {
            tower.count += availableTower.count;
        }
        else
        {
            availableTowers.Add(new() 
            {
                kind = availableTower.kind, 
                count = availableTower.count 
            });
        }
    }

    void Clear()
    {
        foreach (Transform item in parent)
        {
            Destroy(item.gameObject);
        }
    }
}
