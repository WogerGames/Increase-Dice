using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Projectile : MonoBehaviour
{
    [SerializeField] float speed = 10;

    int damage;

    Enemy enemy;

    public void Init(int damage, Enemy e, Material mat)
    {
        enemy = e;
        this.damage = damage;
        GetComponent<MeshRenderer>().material = mat;

        var p = GetComponentInChildren<ParticleSystem>();
        if (p)
        {
            var m = p.main;
            m.startColor = mat.color;
        }
    }

    private void Update()
    {
        
        if (enemy)
        {
            transform.position = Vector3.MoveTowards(transform.position, enemy.transform.position, Time.deltaTime * speed);

            if (Vector3.Distance(transform.position, enemy.transform.position) < 0.1f)
            {
                enemy.Damage(damage);
                Destroy(gameObject);

                //onEnemyAnnig?.Invoke();
            }
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
