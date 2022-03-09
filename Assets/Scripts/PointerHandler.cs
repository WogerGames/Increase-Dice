using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Settings;

public class PointerHandler : MonoBehaviour
{
    UI ui;
    Tower towerSelected;
    Cell cellSelectedTower;
    Vector3 posTowerOrigin;

    private void Awake()
    {
        ui = FindObjectOfType<UI>();
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            cellSelectedTower = GetClickCell();

            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out var hit))
            {
                var tower = hit.collider.GetComponent<Tower>();

                if(tower)
                {
                    if (tower.Stage < MaxStage)
                    {
                        towerSelected = tower;
                        posTowerOrigin = tower.transform.position;
                    }
                    else if (tower.Level < MaxLevel + 1)
                    {
                        ui.OpenIncreaseint(tower);
                    }
                    else if(tower.Level > MaxLevel)
                    {
                        ui.OpenPashalka();
                    }
                }
            }
        }

        if (Input.GetMouseButton(0))
        {
            if (towerSelected)
                MoveTower();
        }

        if (Input.GetMouseButtonUp(0))
        {
            if (towerSelected)
            {
                Cell cell = GetClickCell();

                if (cell 
                    && cell.tower
                    && cell.tower != towerSelected 
                    && cell.tower.Stage == towerSelected.Stage
                    && cell.tower.Kind == towerSelected.Kind
                    && cell.tower.Stage < MaxStage)
                {
                    cellSelectedTower.tower = null;
                    Destroy(towerSelected.gameObject);
                    cell.tower.Upgrade();
                }
                else
                {
                    Tower tower = towerSelected;
                    tower.GetComponent<Collider>().enabled = false;
                    LeanTween.value
                    (
                        gameObject,
                        pos => tower.transform.position = pos,
                        towerSelected.transform.position,
                        posTowerOrigin,
                        0.3f
                    ).setOnComplete(() => tower.GetComponent<Collider>().enabled = true);
                    towerSelected = null;
                    cellSelectedTower = null;
                }
                   
            }
        }
    }

    void MoveTower()
    {
        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        var hits = Physics.RaycastAll(ray);

        float distanceToGround = 8;

        foreach (var hit in hits)
        {
            if (hit.collider.gameObject.CompareTag("Ground"))
            {
                distanceToGround = hit.distance;
                break;
            }
        }

        Vector3 mousePos = new()
        {
            x = Input.mousePosition.x,
            y = Input.mousePosition.y,
            z = distanceToGround
        };

        var pos = Camera.main.ScreenToWorldPoint(mousePos);
        pos.y = 1f;

        towerSelected.transform.position = pos;
    }

    Cell GetClickCell()
    {
        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        var hits = Physics.RaycastAll(ray);
        Cell cell = null;

        foreach (var hit in hits)
        {
            var c = hit.collider.GetComponent<Cell>();
            if (c)
            {
                cell = c;
                break;
            }
        }

        return cell;
    }
}
