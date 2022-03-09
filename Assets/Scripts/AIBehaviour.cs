using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using static Settings;

public class AIBehaviour : MonoBehaviour
{
    Tower towerPrefab;

    Player player;

    public PlayerData Data { get; set; }

    private void Awake()
    {
        player = GetComponent<Player>();
        Data = new();
        towerPrefab = GameManager.Instance.TowerPrefab;
    }

    private void Update()
    {
        var cell = GetRandomCell();
        if (cell && Data.coin >= Data.towerPrice)
        {
            var t = Instantiate(towerPrefab, cell.transform.position, Quaternion.identity);

            cell.tower = t;

            Data.coin -= Data.towerPrice;
            Data.towerPrice += 10;
        }
        else if (!cell)
        {
            var towers = player.Cells.Select(c => c.tower).ToList();

            Dictionary<TowerKind, List<Tower>> towersBykind = new();

            foreach (var tower in towers)
            {
                if (towersBykind.ContainsKey(tower.Kind))
                {
                    towersBykind[tower.Kind].Add(tower);
                }
                else
                {
                    towersBykind.Add(tower.Kind, new() { tower });
                }
            }

            foreach (var group in towersBykind)
            {
                for (int i = 0; i < MaxStage; i++)
                {
                    var byStages = group.Value.FindAll(t => t.Stage == i);
                    if(byStages.Count > 1)
                    {
                        var t = byStages.First();
                        if (t.Stage < MaxStage)
                        {
                            player.Cells.ToList().Find(c => c.tower == t).tower = null;
                            Destroy(t.gameObject);

                            byStages.Last().Upgrade();

                            break;
                        }
                    }
                }
            }
        }
    }

    Cell GetRandomCell()
    {
        var cells = player.Cells;
        var free = cells.ToList().FindAll(c => !c.tower);

        if (free.Any())
            return free[Random.Range(0, free.Count)];

        return null;
    }
}
