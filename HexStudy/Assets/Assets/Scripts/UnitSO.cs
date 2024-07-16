using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CreateUnit", menuName = "Assets/Create/CreateUnit", order = 1)]
public class UnitSO : ScriptableObject, IMoveable
{
    [SerializeField] private int movementRange;
    [SerializeField] private string unitName;
    [SerializeField] private GameObject unitPrefab;
    [SerializeField] private int unitHealth;
    [SerializeField] private int unitDamage;
    public int MovementRange => movementRange;

    public GameObject UnitPrefab { get => unitPrefab; set => unitPrefab = value; }
}
