using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(MeshShapeVerifier))]
public class GridGenerator : MonoBehaviour
{
    public GameObject hexPrefab;
    public HexRenderer hexRenderer;
    public bool isPrefabValid;

    /// <summary>
    /// number of tiles along x axis of grid
    /// </summary>
    [SerializeField] private int tilesPerRow;
    /// <summary>
    /// number of tiles along z axis of grid
    /// </summary>
    [SerializeField] private int tilesPerColumn;
    private GameObject[,] hexArray;
    /// <summary>
    /// Hexes can either have pointed tops or flat sides. True if hex is a flat top variant.
    /// </summary>
    private bool isFlatTopped;
    private float hexDepth;
    private float hexWidth;
    /// <summary>
    /// Z axis spacing is always 75% of the height of a pointed top hex.
    /// </summary>
    private float zOffset;
    /// <summary>
    /// X axis offset on alternating rows is always half the width of a pointed top hex.
    /// </summary>
    private float xOffset;
    /// <summary>
    /// Rotation for hex prefab depending on if it is a pointed top hex or a flat top hex
    /// </summary>
    private float hexRotation;
    private MeshShapeVerifier hexagonalShapeVerifier;
    private List<GameObject> generatedHexes = new List<GameObject>();

    public GameObject[,] HexArray { get => hexArray; set => hexArray = value; }
    public int Width { get => tilesPerRow; set => tilesPerRow = value; }
    public int Depth { get => tilesPerColumn; set => tilesPerColumn = value; }
    public List<GameObject> GeneratedHexes { get => generatedHexes; set => generatedHexes = value; }

    private void Awake()
    {
        hexagonalShapeVerifier = transform.GetComponent<MeshShapeVerifier>();
        if (hexPrefab != null)
        {
            isPrefabValid = hexagonalShapeVerifier.VerifyHexagonalMesh(hexPrefab);
        }  
    }
    private void Start()
    {
        if (isPrefabValid)
        {
            AcquirePrefabDimensions(hexPrefab);
        }
        else
        {
            isFlatTopped = hexRenderer.GenerateHexagonalPrismMeshAndReturnHexOrientation(out hexWidth, out hexDepth);
            Debug.Log($"Generating new mesh with dimensions of [{hexWidth}] width by [{hexDepth}] depth");
        }
        CalculateOrientationDerivatives();
        DefaultGeneration();
    }


    // Exposing below to inspector
    [ContextMenu("Generate Grid")]
    public void DefaultGeneration()
    {
        GridGeneration(Width, Depth);
    }

    /// <summary>
    /// Generates a gWidth x gDepth grid of flat top hexes. Adjusts rotation if pointed top hexes are given as prefab.
    /// </summary>
    /// <param name="gridWidth">number of hexes along the x axis</param>
    /// <param name="gridDepth">number of hexes along the z axis</param>
    private void GridGeneration(int gridWidth, int gridDepth)
    {
        GameObject newHex;
        Width = gridWidth;
        Depth = gridDepth;
        HexArray = new GameObject[Width, Depth];
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

                //If given prefab isn't valid render some defaults
                if (!isPrefabValid)
                {
                    newHex = hexRenderer.RenderHex();
                }

                //if prefab is valid we instantiate it
                else
                {
                    //pointedtop -> needs to be rotated, because grid is generated on the assumption of flat tops
                    if (!isFlatTopped)
                    {
                        newHex = Instantiate(hexPrefab);
                    }
                    //flat top
                    else
                    {
                        newHex = Instantiate(hexPrefab);
                    }
                }
                //make a hex at the location and name it with its 2D dimensions
                newHex.name = "hex " + x + " " + z;
                newHex.transform.SetPositionAndRotation(new Vector3(xPos, 0, z * zOffset), Quaternion.Euler(0, hexRotation, 0));
                newHex.transform.SetParent(this.gameObject.transform);
                HexArray[x, z] = newHex.gameObject;
                GeneratedHexes.Add(newHex);
            }
        }
    }


    /// <summary>
    /// Gets the width and depth of a given hexagonal prefab from the mesh. Also derives Z and X axis offsets. Assumes prefab is a true hexagonal shape.
    /// </summary>
    /// <param name="prefab">Given hexagonal prefab</param>
    private void AcquirePrefabDimensions(GameObject prefab)
    {
        Mesh mesh = prefab.GetComponentInChildren<MeshFilter>().sharedMesh;// find better mthodology
        float Diameter1 = mesh.bounds.size.x * hexPrefab.transform.localScale.x;
        float Diameter2 = mesh.bounds.size.z * hexPrefab.transform.localScale.z;

        //Rotate dimensions appropriately depending on if pointed top or flat top (Longer diameter is always the pointed side)
        //Pointed tops
        if (Diameter1 > Diameter2)
        {
            hexDepth = Diameter1;
            hexWidth = Diameter2;
            isFlatTopped = true;
        }
        //Flat top
        else
        {
            hexDepth = Diameter2;
            hexWidth = Diameter1;
            isFlatTopped = false;
        }
        //isPointedTop = Diameter1 > Diameter2;
        //hexDepth = isPointedTop ? Diameter1 : Diameter2;
        //hexWidth = isPointedTop ? Diameter2 : Diameter1;

    }

    /// <summary>
    /// Calculates constants that are defined after acquiring hex prefab's dimensions
    /// </summary>
    private void CalculateOrientationDerivatives()
    {
        //Calculate offsets
        zOffset = 0.75f * hexDepth;
        xOffset = hexWidth / 2;
        //Determine rotation
        hexRotation = isFlatTopped ? 90 : 0;
    }

    /// <summary>
    /// Resets the grid and cleans up all pieces.
    /// </summary>
    private void ResetGrid()
    {
        foreach (GameObject gridPiece in generatedHexes)
        {
            Destroy(gridPiece.gameObject);
        }
        generatedHexes.Clear();
    }

}
