using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct Face 
{
    public List<Vector3> Vertices { get; private set; }
    public List<int> Triangles { get; private set; }
    public List<Vector2> UVs { get; private set; }
    //public List<Vector3> Normals { get; private set; }

    public Face(List<Vector3> vertices, List<int> triangles, List<Vector2> uvs/*, List<Vector3> normals*/)
    {
        Vertices = vertices;
        Triangles = triangles;
        UVs = uvs;
        //Normals = normals;
    }

}


[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class HexRenderer : MonoBehaviour
{
    private Mesh HexMesh;
    private MeshFilter HexMeshFilter;
    private MeshRenderer HexMeshRenderer;
    private List<Face> HexFaces;

    public Material HexMaterial;

    private void Awake()
    {
        HexMeshFilter = GetComponent<MeshFilter>();
        HexMeshRenderer = GetComponent<MeshRenderer>();

        HexMesh = new Mesh();
        HexMesh.name = "Hex";

        HexMeshFilter.mesh = HexMesh;
        HexMeshRenderer.material = HexMaterial;
        
    }

    void OnEnable()
    {
        DrawMesh();
    }
    
    //private void OnValidate()
    //{
    //    if (Application.isPlaying)
    //    {
    //        DrawMesh();
    //    }
    //}
    void DrawMesh()
    {
        DrawFaces();
        CombineFaces();
    }
    void DrawFaces()
    {

    }
    void CombineFaces()
    {
        List<Vector3> HexVertices = new List<Vector3>();
        List<int> HexTriangles = new List<int>();
        List<Vector2> HexUVs = new List<Vector2>();

        for ( int i = 0;i < HexFaces.Count; i++)
        {
            HexVertices.AddRange(HexFaces[i].Vertices);
            HexUVs.AddRange(HexFaces[i].UVs);

            int Offset = (4 * i);
            foreach (int triangle  in HexFaces[i].Triangles)
            {
                HexTriangles.Add(triangle+Offset);
            }
        }

        HexMesh.vertices = HexVertices.ToArray();
        HexMesh.triangles = HexTriangles.ToArray();
        HexMesh.uv = HexUVs.ToArray();
        HexMesh.RecalculateNormals();
    }
}
