using OpenCover.Framework.Model;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ParseTextFile : MonoBehaviour
{
    public GameObject outputObject;
    public TextAsset givenJson;
    public Mesh mesh;
    public int points;
    public float min;
    public float max;
    public List<Vector3> parsedPoints = new List<Vector3>();
    public ConvexHullCalculator conv = new ConvexHullCalculator();
    public List<Vector3> convertedVerts = new List<Vector3>();
    public List<int> convertedTris = new List<int>();
    public List<Vector3> convertedNormals = new List<Vector3>();
    // Start is called before the first frame update
    void Start()
    {
        mesh = new Mesh();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    [ContextMenu("create points")]
    void GeneratePoints()
    {
        GenerateRandomPoints(points, min, max);
    }

    [ContextMenu("null lists")]
    void nullLists()
    {
        parsedPoints.Clear();
        convertedVerts.Clear();
        convertedTris.Clear();
        convertedNormals.Clear();
    }

    void GenerateRandomPoints(int noPoints, float rangeMin, float rangeMax)
    {
        for (int i = 0; i < noPoints; i++)
        {
            float x = Random.Range(rangeMin, rangeMax);
            float y = Random.Range(rangeMin, rangeMax);
            float z = Random.Range(rangeMin, rangeMax);
            parsedPoints.Add(new Vector3(x, y, z));
        }
    }

    [ContextMenu("create mesh")]
    void MakeHullMesh()
    {
        conv.GenerateHull(parsedPoints, true, ref convertedVerts, ref convertedTris, ref convertedNormals);
        mesh.vertices = convertedVerts.ToArray();
        mesh.triangles = convertedTris.ToArray();
        mesh.normals = convertedNormals.ToArray();
        mesh.RecalculateBounds();

        outputObject.GetComponent<MeshFilter>().mesh = mesh;
    }

}
