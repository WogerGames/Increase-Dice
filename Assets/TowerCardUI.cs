using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using static Settings;

public class TowerCardUI : MonoBehaviour
{
    [SerializeField] GameObject available;
    [SerializeField] GameObject unavailable;

    [SerializeField] TMP_Text kind;

    public void InitAvailable(AvailableTower tower)
    {
        available.SetActive(true);
        unavailable.SetActive(false);

        kind.text = $"{tower.kind} {tower.count}/{CardsToNextLevel}";
    }

    public void InitUnavailable()
    {
        available.SetActive(false);
        unavailable.SetActive(true);

    }
}
