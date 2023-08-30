using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridHex : MonoBehaviour, IDataHandler
{
    private List<UnitSO> containedUnits;
    public bool isOccupied;

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

    public int GetContainedMovementRangeData()
    {
        return ((IDataHandler)this).GetMovementRangeData(containedUnits);
    }
}
