using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridHex : MonoBehaviour, IDataHandler
{
    private List<UnitSO> containedUnits;
    public bool isOccupied;
    public HexRenderer _hexRenderer;

    List<UnitSO> IDataHandler.containedUnits { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }

    public List<UnitSO> GetData()
    {
        if (containedUnits == null) { return null; }
        return containedUnits;
    }

    public void RegisterData(List<UnitSO> units)
    {
        containedUnits = units;
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
