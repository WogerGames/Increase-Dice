using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using static Settings;
public class Tower : MonoBehaviour
{
    [SerializeField] Projectile projectilePrefab;
    [SerializeField] Projectile projectileIncreasePrefab;
    [SerializeField] Transform[] allMuzzles;
    [SerializeField] Transform[] allIncreaseas;
    [SerializeField] MuzzleSet[] muzzleSets;
    [SerializeField] TowerKindSet[] kindSets;
    [SerializeField] IncreaseSet[] increasesSets;

    float rate;
    
    public int Stage { get; set; }
    public int Level { get; set; }
    public float TimerIncrease { get; set; }
    public TowerKindSet[] Sets => kindSets;
    public TowerKind Kind { get; set; }
    public TowerKindSet CurSet { get; set; }

    Enemy enemy;
    GameManager gameManager;

    private void Start()
    {
        TimerIncrease = IncreaseResetTime;

        gameManager = FindObjectOfType<GameManager>();

        enemy = FindEnemy();

        gameManager.onEnemySpawn += Enemy_Spawned;

        HideAllIncrease();

        UpdateKind();
        UpdateViewStage();
    }

    public void Init(TowerKind kind)
    {
        Kind = kind;
    }

    public void Upgrade()
    {
        Stage++;

        if (Stage >= 6)
        {
            HideAllGuns();
            UpdateViewLevel();
        }
        else
        {
            UpdateKind();
            UpdateViewStage();
        }
    }

    public void Increase()
    {
        if (Level > MaxLevel)
            return;

        Level++;
        UpdateViewLevel();
    }

    private void Enemy_Spawned()
    {
        if (!enemy)
            UpdateTarget();
    }

    private void Update()
    {
        if (GameManager.Instance.IsOver)
            return;

        rate += Time.deltaTime;
        TimerIncrease += Time.deltaTime;

        if (Stage < MaxStage)
        {
            if (enemy && rate > CurSet.fireRate)
            {
                AtackSimple(enemy);
            }
        }
        else
        {
            if (enemy && rate > CurSet.fireRate)
            {
                AtackIncrease(enemy);
            }
        }

        if (Input.GetKeyDown(KeyCode.U))
        {
            Upgrade();
        }

        if (Input.GetKeyDown(KeyCode.I))
        {
            Increase();
        }
    }

    private void AtackIncrease(Enemy enemy)
    {
        var muzzles = increasesSets[Level].increases;

        foreach (var item in muzzles)
        {
            var pos = item.position + new Vector3(0, 0.35f, 0);
            var p = Instantiate(projectileIncreasePrefab, pos, Quaternion.identity);
            p.Init(CurSet.damage * (Level + 1), enemy, CurSet.material);

            rate = 0;
        }
    }

    public void AtackSimple(Enemy e)
    {
        var muzzles = muzzleSets[Stage].muzzles;

        foreach (var item in muzzles)
        {
            var p = Instantiate(projectilePrefab, item.GetChild(0).position, Quaternion.identity);
            p.Init(CurSet.damage, e, CurSet.material);

            rate = 0;
        }
    }

    void UpdateKind()
    {
        var countKinds = Enum.GetNames(typeof(TowerKind)).Length;
        var kindId = UnityEngine.Random.Range(0, countKinds);
        Kind = (TowerKind)kindId;

        CurSet = kindSets.ToList().Find(k => k.towerKind == Kind);
    }

    void UpdateViewLevel()
    {
        HideAllIncrease();

        var increases = increasesSets[Level].increases;

        foreach (var item in increases)
        {
            item.gameObject.SetActive(true);
            item.GetChild(0).GetComponent<MeshRenderer>().material = CurSet.material;
        }
    }

    void UpdateViewStage()
    {
        HideAllGuns();

        var muzzles = muzzleSets[Stage].muzzles;

        foreach (var item in muzzles)
        {
            item.gameObject.SetActive(true);
            item.GetComponent<MeshRenderer>().material = CurSet.material;
        }

        foreach (var item in CurSet.views)
        {
            item.material = CurSet.material;
        }
    }

    private void Enemy_Anniged(EnemyDestroyedData _)
    {
        enemy.onEnemyAnnig -= Enemy_Anniged;

        if (gameObject)
            UpdateTarget();
    }

    void UpdateTarget()
    {
        StartCoroutine(Delay());

        IEnumerator Delay()
        {
            yield return null;

            if (enemy)
                enemy.onEnemyAnnig -= Enemy_Anniged;

            enemy = FindEnemy();
        }
    }

    protected virtual Enemy FindEnemy()
    {
        if (this == null)
            return null;

        var enemies = FindObjectsOfType<Enemy>();

        float minDist = float.MaxValue;
        Enemy nearest = null;

        foreach (var item in enemies)
        {
            var d = Vector3.Distance(item.transform.position, transform.position);
            if(d < minDist)
            {
                nearest = item;
                minDist = d;
            }
        }

        if (nearest)
            nearest.onEnemyAnnig += Enemy_Anniged;

        return nearest;
    }

    void HideAllGuns()
    {
        foreach (var item in allMuzzles)
        {
            item.gameObject.SetActive(false);
        }
    }

    void HideAllIncrease()
    {
        foreach (var item in allIncreaseas)
        {
            item.gameObject.SetActive(false);
        }
    }

    private void OnDestroy()
    {
        if (enemy)
            enemy.onEnemyAnnig -= Enemy_Anniged;
        gameManager.onEnemySpawn -= Enemy_Spawned;
    }
}

[Serializable]
public class MuzzleSet
{
    public Transform[] muzzles;
}

[Serializable]
public class IncreaseSet
{
    public Transform[] increases;
}

public enum TowerKind : byte
{
    Simple = 0,
    Power  = 1,
    Wind = 2,
    Ice = 3,
    Toxic = 4,
    Energy = 5,
    Shards = 6,
    Coins = 7,
}

public static class TowerKindExt
{
    public static bool IsLegendary(this TowerKind kind)
    {
        return kind switch
        {
            TowerKind.Energy => true,
            TowerKind.Shards => true,
            _ => false,
        };
    }
}
