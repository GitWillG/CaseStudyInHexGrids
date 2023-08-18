using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridGenerator : MonoBehaviour
{
    //number of tiles along x axis of grid
    public int Width;
    //number of tiles along z axis of grid
    public int Depth;

    public GameObject hexPrefab;
    public GameObject[,] hexArray;


    //actual dimensions of our prefab for refference
    public float hexWidth;
    public float hexDepth;

    //zOffset is 0.75 * depth of the hex
    public float zOffset = 1.5f;
    //xOffset is half of the Width
    public float xOffset = 0.866f;

    private void Start()
    {
        Mesh mesh = hexPrefab.GetComponentInChildren<MeshFilter>().sharedMesh;
        Debug.Log(mesh.bounds.size.x * hexPrefab.transform.localScale.x + " " + mesh.bounds.size.z * hexPrefab.transform.localScale.z);
        hexWidth = mesh.bounds.size.x * hexPrefab.transform.localScale.x;
        hexDepth = mesh.bounds.size.z * hexPrefab.transform.localScale.z;
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
    /// Generates a gWidth x gDepth grid of hexes
    /// </summary>
    /// <param name="gWidth">number of hexes along the x axis</param>
    /// <param name="gDepth">number of hexes along the z axis</param>
    public void GridGeneration(int gWidth, int gDepth)
    {
        Width = gWidth;
        Depth = gDepth;
        hexArray = new GameObject[Width, Depth];
        //2 Dimension grid
        for (int x = 0; x < Width; x++)
        {
            for (int z = 0; z < Depth; z++)
            {
                //Adjust x positioning based on tile width
                float xPos = x * hexWidth;

                //every other row we offset the x position for the zig-zag positioning
                if (z % 2 == 1)
                {
                    xPos += xOffset;
                }
                //make a hex at the location and name it with its 2D dimensions
                GameObject HexOb = (GameObject)Instantiate(hexPrefab, new Vector3(xPos, 0, z * zOffset), Quaternion.Euler(0, 90, 0));
                HexOb.name = "Hex " + x + " " + z;
                HexOb.transform.SetParent(this.gameObject.transform);
                hexArray[x, z] = HexOb.gameObject;
            }
        }
    }
}
