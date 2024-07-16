using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridHex : MonoBehaviour, IDataHandler
{
    public List<UnitSO> containedUnits { get; set; }
    public bool isOccupied;
    public HexRenderer _hexRenderer;
    private MeshRenderer _meshRenderer;
    public int minRange;
    public Vector3 topOfHexPosition;


    private void Awake()
    {
        containedUnits = new List<UnitSO>();
        _meshRenderer = GetComponent<MeshRenderer>();
    }
    private void Start()
    {
        FindTopOfHex();
    }
    public List<UnitSO> GetData()
    {
        if (containedUnits == null) { return null; }
        return containedUnits;
    }

    public void RegisterData(List<UnitSO> units)
    {
        foreach (UnitSO unit in units)
        {
            containedUnits.Add(unit);
        }
        isOccupied = true;
    }

    public void UnregisterData(List<UnitSO> units)
    {
        containedUnits.Clear();
        isOccupied = false;
    }

    public Vector3 GetHexRoofPosition()
    {
        float yPosition = _hexRenderer.Height / 2;
        return new Vector3(transform.position.x, yPosition, transform.position.z);
    }

    public int GetMovementRangeData()
    {
        if (containedUnits.Count == 0) { return 0; }
        int minRange;
        minRange = containedUnits[0].MovementRange;
        foreach (UnitSO unit in containedUnits)
        {
            if (unit.MovementRange < minRange)
            {
                minRange = unit.MovementRange;
            }
        }
        return minRange;
    }

    private void FindTopOfHex()
    {
        topOfHexPosition = new Vector3(transform.position.x, transform.position.y + _meshRenderer.bounds.max.y, transform.position.z);
    }
}
