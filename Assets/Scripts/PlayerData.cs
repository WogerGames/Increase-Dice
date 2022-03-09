using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerData
{
    public int coin;
    public int shards;
    public int cards;

    public List<AvailableTower> availableKind;

    public int towerPrice;

    public static PlayerData MineData;

    public PlayerData()
    {
        coin = 80;
        shards = 100;
        towerPrice = 10;

        availableKind = new()
        {
            new() { kind = TowerKind.Simple },
            new() { kind = TowerKind.Power },
            new() { kind = TowerKind.Wind },
            new() { kind = TowerKind.Ice },
            new() { kind = TowerKind.Toxic }
        };
    }
}

public class AvailableTower
{
    public TowerKind kind;
    public int count;
}
