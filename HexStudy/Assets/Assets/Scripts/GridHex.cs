using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridHex : MonoBehaviour, IDataHandler
{
    private List<UnitSO> containedUnits;
    public bool isOccupied;
    public HexRenderer _hexRenderer;
    public int minRange;

    List<UnitSO> IDataHandler.containedUnits { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }

    private void Awake()
    {
        containedUnits = new List<UnitSO>();
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
}
