using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridGenerator : MonoBehaviour
{
    //number of tiles along x axis of grid
    private int width;
    //number of tiles along z axis of grid
    private int depth;

    public GameObject hexPrefab;
    private GameObject[,] hexArray;


    private bool pointedTop;
    //actual dimensions of our prefab for refference
    private float hexDepth;
    private float hexWidth;
    //zOffset is 0.75 * depth of the hex
    private float zOffset;
    //xOffset is half of the Width
    private float xOffset;

    public int Width { get => width; set => width = value; }
    public int Depth { get => depth; set => depth = value; }
    public GameObject[,] HexArray { get => hexArray; set => hexArray = value; }

    private void Awake()
    {
        if (hexPrefab != null) { AcquirePrefabDimensions(hexPrefab); }
    }

    /// <summary>
    /// Exposing below to inspector
    /// </summary>
    [ContextMenu("Generate Grid")]
    public void DefaultGeneration()
    {
        GridGeneration(Width, Depth);
    }

    /// <summary>
    /// Generates a gWidth x gDepth grid of flat top hexes. Adjusts rotation if pointed top hexes are given as prefab.
    /// </summary>
    /// <param name="gWidth">number of hexes along the x axis</param>
    /// <param name="gDepth">number of hexes along the z axis</param>
    public void GridGeneration(int gWidth, int gDepth)
    {
        Width = gWidth;
        Depth = gDepth;
        HexArray = new GameObject[Width, Depth];
        //2 Dimension grid
        for (int x = 0; x < Width; x++)
        {
            for (int z = 0; z < Depth; z++)
            {
                //Adjust x positioning based on tile width
                float xPos = x  * hexWidth;

                //every other row we offset the x position for the zig-zag positioning
                if (z % 2 == 1)
                {
                    xPos += xOffset;
                }
                //make a hex at the location and name it with its 2D dimensions
                GameObject HexOb;
                //PointedTop -> needs to be rotated, because grid is generated on the assumption of flat tops
                if (pointedTop)
                {
                    HexOb = (GameObject)Instantiate(hexPrefab, new Vector3(xPos, 0, z * zOffset), Quaternion.Euler(0, 90, 0));
                }
                //Flat top
                else
                {
                    HexOb = (GameObject)Instantiate(hexPrefab, new Vector3(xPos, 0, z * zOffset), Quaternion.Euler(0, 0, 0));
                }
                HexOb.name = "Hex " + x + " " + z;
                HexOb.transform.SetParent(this.gameObject.transform);
                HexArray[x, z] = HexOb.gameObject;
            }
        }
    }

    /// <summary>
    /// Calculates width and depth of a given hexagonal prefab. Also derives Z and X axis offsets. Assumes prefab is a true hexagonal shape.
    /// </summary>
    /// <param name="prefab">Given hexagonal prefab</param>
    private void AcquirePrefabDimensions(GameObject prefab)
    {
        Mesh mesh = prefab.GetComponentInChildren<MeshFilter>().sharedMesh;
        float Diameter1 = mesh.bounds.size.x * hexPrefab.transform.localScale.x;
        float Diameter2 = mesh.bounds.size.z * hexPrefab.transform.localScale.z;

        //Rotate dimensions appropriately depending on if pointed top or flat top (Longer diameter is always the pointed side)
        //Pointed tops
        if (Diameter1 > Diameter2)
        {
            hexDepth = Diameter1;
            hexWidth = Diameter2;
            pointedTop = true;
        }
        //Flat top
        else
        {
            hexDepth = Diameter2;
            hexWidth = Diameter1;
            pointedTop = false;
        }

        //calculate offsets
        zOffset = 0.75f * hexDepth; //Z axis spacing is always 75% of the height of a pointed top hex 
        xOffset = hexWidth / 2; //X axis offset on alternating rows is always half the width of a pointed top hex
    }

}
