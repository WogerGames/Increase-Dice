using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine;
using System.Linq;
using TMPro;


public class UI : MonoBehaviour
{
    [SerializeField] TMP_Text coin;
    [SerializeField] TMP_Text shards;
    [SerializeField] TMP_Text labelPrice;
    [SerializeField] TMP_Text wave;
    [SerializeField] TMP_Text nextWave;
    [SerializeField] Button btnTower;
    [SerializeField] Tower towerPrefab;
    [SerializeField] IncreaseUI increase;
    [SerializeField] GameOverUI gameOver;
    [SerializeField] GameObject panelNextWave;
    [SerializeField] GameObject pashalka;

    private void Start()
    {
        increase.gameObject.SetActive(false);
        gameOver.gameObject.SetActive(false);
        pashalka.gameObject.SetActive(false);

        btnTower.onClick.AddListener(SpawnTower);

        GameManager.Instance.onWaveCompleted += Wave_Completed;
        GameManager.Instance.onGameOver += GameOver;
    }

    private void GameOver()
    {
        gameOver.Init();
    }

    private void Wave_Completed()
    {
        panelNextWave.GetComponent<Animator>().Play("UI Show Next Wave");
    }

    private void SpawnTower()
    {
        if (GameManager.Instance.Data.coin <= GameManager.Instance.TowerPrice)
            return;

        var cell = GetCell();
        if (cell)
        {
            var t = Instantiate(towerPrefab, cell.transform.position, Quaternion.identity);

            cell.tower = t;

            GameManager.Instance.Data.coin -= GameManager.Instance.TowerPrice;
            GameManager.Instance.TowerPrice += 10;
        }
    }

    void Update()
    {
        coin.text = GameManager.Instance.Data.coin.ToString();
        shards.text = GameManager.Instance.Data.shards.ToString();
        labelPrice.text = GameManager.Instance.TowerPrice.ToString();
        wave.text = GameManager.Instance.Wave.ToString();
        nextWave.text = $"Волна {GameManager.Instance.Wave}";
    }

    public void OpenIncreaseint(Tower tower)
    {
        increase.Init(tower);
    }

    public void OpenPashalka()
    {
        pashalka.SetActive(true);
    }

    Cell GetCell()
    {
        var cells = GameManager.Instance.Player.Cells;
        var free = cells.ToList().FindAll(c => !c.tower);

        if (free.Any())
            return free[Random.Range(0, free.Count)];

        return null;
    }
}
