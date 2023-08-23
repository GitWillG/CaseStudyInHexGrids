using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct Face 
{
    public List<Vector3> Vertices { get; private set; }
    public List<int> Triangles { get; private set; }
    public List<Vector2> UVs { get; private set; }
    //public List<Vector3> Normals { get; private set; }

    public Face(List<Vector3> vertices, List<int> triangles, List<Vector2> uvs)
    {
        Vertices = vertices;
        Triangles = triangles;
        UVs = uvs;
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
    public float InnerHexRadius;
    public float HexRadius;
    public float Height;
    public bool isFlatTopped;

    private void Awake()
    {
        HexMeshFilter = GetComponent<MeshFilter>();
        HexMeshRenderer = GetComponent<MeshRenderer>();

        HexMesh = new Mesh();
        HexMesh.name = "Hex";

        HexMeshFilter.mesh = HexMesh;
        
    }

    //Removed and put inside the UseDefault method.
    //void OnEnable()
    //{
    //        DrawMesh();
    //}

    //comment out on play
    private void OnValidate()
    {
        if (Application.isPlaying)
        {
            DrawMesh();
        }
    }

    void DrawMesh()
    {
        HexMeshRenderer.material = HexMaterial;
        DrawFaces();
        CombineFaces();
    }
    void DrawFaces()
    {
        HexFaces = new List<Face>();
        for (int point = 0; point < 6; point++)
        {
            HexFaces.Add(CreateFace(InnerHexRadius, HexRadius, Height / 2, Height / 2f, point));
        }

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
            foreach (int triangle in HexFaces[i].Triangles)
            {
                HexTriangles.Add(triangle+Offset);
            } 
        }

        HexMesh.vertices = HexVertices.ToArray();
        HexMesh.triangles = HexTriangles.ToArray();
        HexMesh.uv = HexUVs.ToArray();
        HexMesh.RecalculateNormals();
    }

    private Face CreateFace(float innerRadius, float outerRadius, float heightA, float heightB, int point, bool reverse = false)
    {
        Vector3 pointA = GetPoint(innerRadius, heightB, point);
        Vector3 pointB = GetPoint(innerRadius, heightB, (point < 5) ? point + 1 : 0);
        Vector3 pointC = GetPoint(outerRadius, heightA, (point < 5) ? point + 1 : 0);
        Vector3 pointD = GetPoint(outerRadius, heightA, point);

        List<Vector3> vertices = new List<Vector3>() {pointA, pointB, pointC, pointD };
        List<int> triangles = new List<int>() { 0, 1, 2, 2, 3, 0 };
        List<Vector2> uvs = new List<Vector2>() { new Vector2(0, 0), new Vector2(1, 0), new Vector2(1, 1), new Vector2(0, 1) };
        if (reverse)
        {
            triangles.Reverse();
        }

        return new Face(vertices, triangles, uvs);

    }

    protected Vector3 GetPoint(float size, float height, int index)
    {
        float angle_deg = isFlatTopped ? 60 * index: 60 * index - 30;
        float angle_rad = Mathf.PI / 180f * angle_deg;
        return new Vector3 ((size * Mathf.Cos(angle_rad)), height, size * Mathf.Sin(angle_rad));

    }

    /// <summary>
    /// Renders a hex using assigned defaults
    /// </summary>
    /// <returns>A GameObject hex with associated renderers</returns>
    public GameObject RenderHex()
    {
        GameObject hex = new GameObject("Hex", typeof(HexRenderer));

        HexRenderer hexRenderer = hex.GetComponent<HexRenderer>();
        hexRenderer.Height = Height;
        hexRenderer.InnerHexRadius = InnerHexRadius;
        hexRenderer.HexRadius = HexRadius;
        hexRenderer.isFlatTopped = isFlatTopped;
        hexRenderer.HexMaterial = HexMaterial;
        hexRenderer.DrawMesh();

        return hex;
    }
}
