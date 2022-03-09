using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class TowerKindSet
{
    public TowerKind towerKind;
    public float fireRate = 0.5f;
    public int damage = 1;

    [Header("Графические особенности")]
    public Material material;
    public MeshRenderer[] views;
}
