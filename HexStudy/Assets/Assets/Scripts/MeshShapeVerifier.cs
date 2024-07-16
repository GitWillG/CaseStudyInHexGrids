using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshShapeVerifier : MonoBehaviour
{

    // Set tolerance value; smaller is less tolerant of hexagonal-like structures. Larger value is more lenient in what is a "hexagonal prism"
    private float parallelTolerance = 0.01f;

    public float ParallelTolerance { get => parallelTolerance; set => parallelTolerance = value; }

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

        // Perform checks for lateral faces ensuring that there are 6 lateral faces or a multiple therof 
        bool[] isFaceLateral = new bool[faces.Length];
        int lateralFaceCount = 0;

        for (int i = 0; i < faces.Length; i++)
        {
            int[] face = faces[i];

            Vector3 faceNormal = Vector3.Cross(vertices[face[1]] - vertices[face[0]], vertices[face[2]] - vertices[face[0]]).normalized;
            float dot = Vector3.Dot(faceNormal, Vector3.up); // Assuming the hexagonal prism is aligned with the world up direction

            if (Mathf.Abs(dot) < 1 - ParallelTolerance)
            {
                isFaceLateral[i] = true;
                lateralFaceCount++;
            }
        }

        if (lateralFaceCount % 6 != 0) // Hexagonal prisms have 6 lateral faces, but meshes may have a multiple of this
        {
            Debug.Log("This mesh does not have 6 lateral faces (A geometric property of hexagonal prisms)");
            return false;
        }
        return true;
    }

    /// <summary>
    /// Verifies that a mesh is a hexagonal prism
    /// </summary>
    /// <param name="prefab">The prefab containing a mesh to be analyzed</param>
    public bool VerifyHexagonalMesh(GameObject prefab)
    {
        MeshFilter[] meshFilters = prefab.GetComponentsInChildren<MeshFilter>();

        foreach (MeshFilter meshFilter in meshFilters)
        {
            if (IsHexagonalPrism(meshFilter.sharedMesh)) { return true;}
            else
            {
                Debug.Log($"The mesh of object '{meshFilter.gameObject.name}' is not a hexagonal prism.");
            }
        }
        return false;
    }
}
