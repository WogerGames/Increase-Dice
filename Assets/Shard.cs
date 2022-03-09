using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shard : MonoBehaviour
{
    [SerializeField] GameObject tapEffectPrefab;
    [SerializeField] LeanTweenType easeType;
    [SerializeField] AnimationCurve yPosCurve;
    [SerializeField] float spawnAnimDuration = 1f;

    [SerializeField] GameObject ring;

    public void Init(Vector3 dir)
    {
        StartSpawnAnimation(dir);
    }

   
    void Update()
    {
        transform.Rotate(new Vector3(0, 100 * Time.deltaTime, 0));
    }

    private void OnMouseDown()
    {
        Destroy(gameObject);
        Instantiate(tapEffectPrefab, transform.position, Quaternion.identity);
        PlayerData.MineData.shards++;
    }

    void StartSpawnAnimation(Vector3 dir)
    {
        var target = transform.position + dir;

        LeanTween.value
        (
            gameObject,
            pos => { transform.position = pos; },
            transform.position,
            target,
            spawnAnimDuration
        ).setEase(easeType);

        LeanTween.value
        (
            gameObject,
            t =>
            {
                var pos = transform.position;
                pos.y = yPosCurve.Evaluate(t);
                transform.position = pos;
            },
            0,
            1,
            spawnAnimDuration
        ).setEase(easeType);

        LeanTween.value
        (
            gameObject,
            s =>
            {
                transform.localScale = Vector3.one * s;
            },
            0.1f,
            1,
            spawnAnimDuration
        ).setEase(easeType);
    }
}
