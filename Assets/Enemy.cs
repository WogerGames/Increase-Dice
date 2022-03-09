using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class Enemy : MonoBehaviour
{
    [SerializeField] Shard shardPrefab;
    [SerializeField] float speed = 3f;
    [SerializeField] TMP_Text labelHP;

    Vector3 dir;

    HealthComponent health;
    Trajectory path;

    public Action<EnemyDestroyedData> onEnemyAnnig;
    public Action onGameOver;

    int curPointIdx;
    int reward;
    bool anniged;

    private void Awake()
    {
        
    }

    public void Init(long hp, Trajectory trajectory)
    {
        path = trajectory;
        curPointIdx = 0;

        health = GetComponent<HealthComponent>();

        health.value = hp;

        transform.position += Vector3.up * UnityEngine.Random.Range(-0.01f, 0.01f);

        var bonus = hp / 100;
        bonus *= 50;
        reward = 30 + (int)bonus;
    }

    private void Update()
    {
        dir = transform.position - path.points[curPointIdx].position;

        if (dir.sqrMagnitude < 0.3f)
        {
            if (curPointIdx < path.points.Count - 1)
            {
                curPointIdx++;
            }
            else if(!GameManager.Instance.IsOver)
            {
                onGameOver?.Invoke();
            }
        }

        dir.Normalize();

        transform.position -= dir * (speed * Time.deltaTime);
    }

    public void Damage(int damage)
    {
        health.value -= damage;

        if(!anniged && health.value <= 0)
        {
            onEnemyAnnig?.Invoke(new EnemyDestroyedData { reward = reward, enemy = this });
            Destroy(gameObject);

            anniged = true;

            if (UnityEngine.Random.Range(0, 100) < 10)
                SpawnShard();
        }
    }

    private void LateUpdate()
    {
        labelHP.text = health.value.ToString();
    }

    void SpawnShard()
    {
        var spawnDir = Vector3.Cross(dir, Vector3.up);
        if(UnityEngine.Random.Range(0,10) < 5)
        {
            spawnDir *= -1f;
        }
        
        var shard = Instantiate(shardPrefab, transform.position, Quaternion.identity);
        shard.Init(spawnDir);
    }
}

public class EnemyDestroyedData
{
    public Enemy enemy;
    public int reward;
}
