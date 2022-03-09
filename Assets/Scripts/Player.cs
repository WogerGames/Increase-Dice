using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public Cell[] Cells { get; set; }


    private void Awake()
    {
        Cells = GetComponentsInChildren<Cell>();
    }
}
