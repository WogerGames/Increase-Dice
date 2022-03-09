using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using TMPro;
using static Settings;

public class IncreaseUI : MonoBehaviour
{
    [SerializeField] AnimationCurve curveChance;
    [SerializeField] TMP_Text labelShards;
    [SerializeField] TMP_Text labelChance;
    [SerializeField] TMP_Text labelUseShards;
    [SerializeField] TMP_Text labelTimeElapsed;
    [SerializeField] Slider slider;
    [SerializeField] Button btnIncrease;
    [SerializeField] GameObject labelIncrease;
    [SerializeField] GameObject labelSucces;
    [SerializeField] GameObject labelFailed;

    [Header("Системы частиц")]
    [SerializeField] GameObject[] effectStep_1;
    [SerializeField] GameObject[] effectStep_2;
    [SerializeField] GameObject[] effectSucces;

    Tower tower;

    int level;
    int usingShards;
    int chance;
    bool increaseProgress;
    bool increaseTryed;
    float timeElapsed;

    public void Init(Tower tower)
    {
        this.tower = tower;

        gameObject.SetActive(true);
        labelSucces.SetActive(false);
        labelFailed.SetActive(false);

        DisableAllEffects();

        level = tower.Level;

        slider.value = 0;
        slider.onValueChanged.AddListener(SliderValue_Changed);
        btnIncrease.onClick.AddListener(Increase_Clicked);

        labelUseShards.text = MaxUsingShards().ToString();
    }

    private void Increase_Clicked()
    {
        increaseProgress = true;

        btnIncrease.interactable = false;
        SetActiveEffects(effectStep_1, true);
    }

    private void SliderValue_Changed(float value)
    {
        var count = (float)MaxUsingShards() * value;
        usingShards = Mathf.FloorToInt(count);
    }

    private void Update()
    {
        int startChance = (int)curveChance.Evaluate(level);
        chance = startChance + (usingShards / (level + 1));
        labelChance.text = $"{chance}%";

        labelShards.text = $"x{usingShards}";

        if (increaseProgress)
            Progress();

        if (GameManager.Instance.Data.shards < usingShards)
        {
            labelTimeElapsed.text = "У вас недостаточно кристаллов";
            btnIncrease.interactable = false;
            labelIncrease.SetActive(false);
            labelTimeElapsed.gameObject.SetActive(true);
        }
        else
        {
            btnIncrease.interactable = true;
            labelIncrease.SetActive(true);
            labelTimeElapsed.gameObject.SetActive(false);
        }

        if (tower.TimerIncrease < IncreaseResetTime)
        {
            btnIncrease.interactable = false;

            labelIncrease.SetActive(false);
            labelTimeElapsed.gameObject.SetActive(true);

            int time = Mathf.FloorToInt(IncreaseResetTime - tower.TimerIncrease);
            int min = time / 60;
            int sec = time % 60;
            labelTimeElapsed.text = $"До повторной попытки усиления {min}:{sec}";
        }
        else
        {
            if(!increaseProgress && GameManager.Instance.Data.shards >= usingShards)
                btnIncrease.interactable = true;

            if (GameManager.Instance.Data.shards >= usingShards)
            {
                labelIncrease.SetActive(true);
                labelTimeElapsed.gameObject.SetActive(false);
            }
        }

        
    }

    void Progress()
    {
        timeElapsed += Time.deltaTime;

        if(timeElapsed > 5.5f)
        {
            SetActiveEffects(effectStep_2, true);
        }

        if(timeElapsed > 11 && !increaseTryed)
        {
            increaseTryed = true;

            GameManager.Instance.Data.shards -= usingShards;

            if (chance > Random.Range(0, 100))
            {
                SetActiveEffects(effectSucces, true);
                tower.Increase();

                labelSucces.SetActive(true);

                level = tower.Level;
                slider.value = 0;
                labelUseShards.text = MaxUsingShards().ToString();

            }
            else
            {
                labelFailed.SetActive(true);
                tower.TimerIncrease = 0;
            }

        }

        if(timeElapsed > 13.9f)
        {
            increaseTryed = false;
            increaseProgress = false;
            btnIncrease.interactable = true;
            DisableAllEffects();
            timeElapsed = 0;

            labelSucces.SetActive(false);
            labelFailed.SetActive(false);

            if(tower.Level > MaxLevel)
            {
                gameObject.SetActive(false);
            }
        }
    }

    int MaxUsingShards()
    {
        int startChance = (int)curveChance.Evaluate(level);
        int requiredChance = 100 - startChance;
        return requiredChance * (level + 1);
    }

    void SetActiveEffects(GameObject[] effects, bool value)
    {
        foreach (var item in effects)
        {
            item.SetActive(value);
        }
    }

    void DisableAllEffects()
    {
        SetActiveEffects(effectStep_1, false);
        SetActiveEffects(effectStep_2, false);
        SetActiveEffects(effectSucces, false);
    }
}
