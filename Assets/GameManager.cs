using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using static Settings;

public class GameManager : MonoBehaviour
{
    [SerializeField] Player playerPrefab;
    [SerializeField] Enemy enemyPrefab;
    [SerializeField] Tower towerPrefab;
    [SerializeField] float spawnRate = 3f;

    [Space]

    [SerializeField] List<Transform> pointsPlayrs;
    [SerializeField] List<Transform> pointsSpawns;
    [SerializeField] List<Trajectory> trajectories;

    [Space]

    [SerializeField] float delayBetweenWaves = 3f;

    public Action onEnemySpawn;
    public Action onWaveCompleted;
    public Action onGameOver;

    public List<Enemy> enemies = new();

    public static GameManager Instance { get; private set; }
    public int Wave { get; set; }
    public bool IsOver { get; set; }
    public Player Player { get; set; }
    public PlayerData Data { get; set; }
    public int TowerPrice { get; set; }
    public Tower TowerPrefab => towerPrefab;

    AIBehaviour ai;


    
    int numberEnemyInWave;
    int lastHpIncrease;
    int curIdxSpawn;
    int countAnnigedEnemies;

    private void Awake()
    {
        if (!Menu.inited)
            UnityEngine.SceneManagement.SceneManager.LoadScene(0);

        Instance = this;

        Data = PlayerData.MineData;

        TowerPrice = 10;
    }

    private void Start()
    {
        LeanTween.init(1600);

        onWaveCompleted += Wave_Completed;

        Wave_Completed();

        int countPlayers = pointsPlayrs.Count;
        for (int i = 0; i < countPlayers; i++)
        {
            var point = pointsPlayrs[i];
            var p = Instantiate(playerPrefab, point.position, Quaternion.identity);
            
            if(i == 0)// Временный критерий 
            {
                Player = p;
            }
            else
            {
                ai = p.gameObject.AddComponent<AIBehaviour>();
            }
        }
    }

    private void Wave_Completed()
    {
        Wave++;
        Data.cards++;

        StartCoroutine(Delay());

        IEnumerator Delay()
        {
            yield return new WaitForSeconds(delayBetweenWaves);

            countAnnigedEnemies = 0;
            numberEnemyInWave = 0;
            //lastHpIncrease = 0;

            StartCoroutine(Spawn());
        }
    }

    IEnumerator Spawn()
    {
        while (numberEnemyInWave < MaxEnemyWave && !IsOver)
        {
            yield return new WaitForSeconds(spawnRate);

            numberEnemyInWave++;
            
            var pos = pointsSpawns[curIdxSpawn].position;
            var e = Instantiate(enemyPrefab, pos, Quaternion.identity);
            var hp = CalculateEnemyHP();
            e.Init(hp, trajectories[curIdxSpawn]);
            e.onEnemyAnnig += Enemy_Anniged;
            e.onGameOver += GameOver;

            curIdxSpawn++;
            if (curIdxSpawn == pointsSpawns.Count)
                curIdxSpawn = 0;

            enemies.Add(e);

            onEnemySpawn?.Invoke();
        }
    }

    private void GameOver()
    {
        IsOver = true;

        PlayerData.MineData = Data;

        onGameOver?.Invoke();
    }

    private void Enemy_Anniged(EnemyDestroyedData edd)
    {
        Data.coin += edd.reward;

        if (ai)
            ai.Data.coin += edd.reward;

        enemies.Remove(edd.enemy);

        countAnnigedEnemies++;

        if (countAnnigedEnemies == MaxEnemyWave)
            onWaveCompleted?.Invoke();
    }

    int CalculateEnemyHP()
    {
        int result = 10 * Wave + lastHpIncrease;

        if (numberEnemyInWave % 10 == 0)
        {
            lastHpIncrease = numberEnemyInWave * (int)Mathf.Pow(Wave, 1.3f);
        }
    
        return result;
    }
}

[Serializable]
public class Trajectory
{
    public List<Transform> points;
}
