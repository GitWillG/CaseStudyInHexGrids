using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class GridGenerator : MonoBehaviour
{
    public GameObject hexPrefab;
    public HexRenderer hexRenderer;
    public bool isPrefabValid;


    /// <summary>
    /// number of tiles along x axis of grid
    /// </summary>
    [SerializeField] private int width;
    /// <summary>
    /// number of tiles along z axis of grid
    /// </summary>
    [SerializeField] private int depth;
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

    public GameObject[,] HexArray { get => hexArray; set => hexArray = value; }
    public int Width { get => width; set => width = value; }
    public int Depth { get => depth; set => depth = value; }

    private void Awake()
    {
        if (hexPrefab != null)
        {
            VerifyHexagonalMesh(hexPrefab);
        }  
    }
    private void Start()
    {
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
    public void GridGeneration(int gridWidth, int gridDepth)
    {
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
                //make a hex at the location and name it with its 2D dimensions
                GameObject newHex;

                //If given prefab isn't valud render some defaults
                if (!isPrefabValid)
                {
                    newHex = hexRenderer.RenderHex();
                    Debug.Log(IsHexagonalPrism(newHex.GetComponent<MeshFilter>().mesh));
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
                newHex.name = "hex " + x + " " + z;
                newHex.transform.SetPositionAndRotation(new Vector3(xPos, 0, z * zOffset), Quaternion.Euler(0, hexRotation, 0));
                newHex.transform.SetParent(this.gameObject.transform);
                HexArray[x, z] = newHex.gameObject;
            }
        }
    }


    //TODO: Verify hex is actually a hexagon by comparing both diameters to a constant (that should exist for hexagons), throw exception if it fails.

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
    /// Checks that geometry of mesh alines with mathematical definition of a hexagonal prism.
    /// </summary>
    /// <param name="mesh">Given mesh to check</param>
    /// <returns>True if given mesh is a hexagon</returns>
    bool IsHexagonalPrism(Mesh mesh)
    {
        Vector3[] vertices = mesh.vertices;
        int[][] faces = new int[mesh.triangles.Length / 3][];

        for (int i = 0; i < faces.Length; i++)
        {
            faces[i] = new int[] { mesh.triangles[i * 3], mesh.triangles[i * 3 + 1], mesh.triangles[i * 3 + 2] };
        }

        // Set tolerance value
        float parallelTolerance = 0.01f;

        // Perform checks for lateral faces ensuring that there are 6 lateral faces or a multiple therof 
        bool[] isFaceLateral = new bool[faces.Length];
        int lateralFaceCount = 0;

        for (int i = 0; i < faces.Length; i++)
        {
            int[] face = faces[i];

            Vector3 faceNormal = Vector3.Cross(vertices[face[1]] - vertices[face[0]], vertices[face[2]] - vertices[face[0]]).normalized;
            float dot = Vector3.Dot(faceNormal, Vector3.up); // Assuming the hexagonal prism is aligned with the world up direction

            if (Mathf.Abs(dot) < 1 - parallelTolerance)
            {
                isFaceLateral[i] = true;
                lateralFaceCount++;
            }
        }

        if (lateralFaceCount % 6 != 0) // Hexagonal prisms have 6 lateral faces, but meshes may have a multiple of this
        {
            Debug.Log("lateral face count failue");
            return false;
        }
        return true;
    }

    /// <summary>
    /// Verifies that a mesh is a hexagonal prism
    /// </summary>
    /// <param name="prefab">The prefab containing a mesh to be analyzed</param>
    void VerifyHexagonalMesh(GameObject prefab)
    {
        MeshFilter[] meshFilters = prefab.GetComponentsInChildren<MeshFilter>();

        foreach (MeshFilter meshFilter in meshFilters)
        {
            if (IsHexagonalPrism(meshFilter.sharedMesh))
            {
                isPrefabValid = true;
                AcquirePrefabDimensions(hexPrefab);
            }
            else
            {
                isPrefabValid = false;
                Debug.Log($"The mesh of object '{meshFilter.gameObject.name}' is not a hexagonal prism.");
                hexDepth = hexRenderer.HexRadius * 2;
                hexWidth = (float)(Math.Sqrt(3) * hexRenderer.HexRadius);
                isFlatTopped = hexRenderer.isFlatTopped;
            }
            CalculateOrientationDerivatives();
        }
    }
}
