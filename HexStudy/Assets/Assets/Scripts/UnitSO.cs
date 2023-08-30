using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitSO : ScriptableObject, iMoveable
{
    [SerializeField] private int movementRange;
    [SerializeField] private string unitName;
    [SerializeField] private GameObject unitPrefab;
    [SerializeField] private int unitHealth;
    [SerializeField] private int unitDamage;
    public int MovementRange => movementRange;


    
}
