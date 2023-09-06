using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexSelector : MonoBehaviour
{
    public int currentMask;
    public List<UnitSO> test = new List<UnitSO>();
    private GridHex selectedHex;


    private void Awake()
    {
        currentMask = LayerMask.NameToLayer("Hex");
    }

    // Update is called once per frame
    void Update()
    {
        HoverOverHex();
        
    }

    private void HoverOverHex()
    {
        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, Mathf.Infinity) && Input.GetMouseButtonDown(0))
        {
            SelectHex(hit.transform.GetComponent<GridHex>()); 
        }
    }
    private void SelectHex(GridHex hoveredHex)
    {
        selectedHex = hoveredHex;
        if (selectedHex.isOccupied)
        {
            hoveredHex.RegisterData(test);
        }
        else
        {
            Debug.Log(hoveredHex.GetComponent<IDataHandler>().GetMovementRangeData());
        }
    }
    
}
