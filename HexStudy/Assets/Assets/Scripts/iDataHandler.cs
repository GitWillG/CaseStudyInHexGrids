using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// Handles registering, unregistering, and providing access to contained UnitSOs
/// </summary>
public interface IDataHandler
{
    public List<UnitSO> containedUnits { get; set; }
    /// <summary>
    /// Stores a list of contained units.
    /// </summary>
    /// <param name="units">List of unitSOs</param>
    public void RegisterData(List<UnitSO> units);

    /// <summary>
    /// Removes stored list of contianed units.
    /// </summary>
    /// <param name="units">List of stored unitSOs</param>
    public void UnregisterData(List<UnitSO> units);

    /// <summary>
    /// Provides external acess to list of contianed units.
    /// </summary>
    /// <returns>A list of all contained unitSOs</returns>
    public List<UnitSO> GetData();

    /// <summary>
    /// Gets movement range data of contained units if it exists. Default implementation seeks the smallest movement range among contained unit(s).
    /// </summary>
    /// <param name="units">Contained unitSOs</param>
    /// <returns>The smallest movement range of contained units</returns>
    public int GetMovementRangeData();
    public Vector3 GetHexRoofPosition();

}

public interface IMoveable
{
    public int MovementRange { get; }
}
