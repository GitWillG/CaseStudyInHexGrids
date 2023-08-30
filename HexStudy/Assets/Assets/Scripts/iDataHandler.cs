using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public interface iDataHandler
{
    public bool isOccupied { get; }
    public void RegisterData(List<UnitSO> units);

    public void UnregisterData(List<UnitSO> units);

    public List<UnitSO> GetData();

    public int GetMovementRangeData(List<UnitSO> units)
    {
        if (units.Count == 0) { return 0; }
        int minRange;
        minRange = units[0].MovementRange;
        foreach (UnitSO unit in units)
        {
            if (unit.MovementRange < minRange)
            {
                minRange = unit.MovementRange;
            }
        }
        return minRange;
    }

}
public interface iMoveable
{
    public int MovementRange { get; }
}
