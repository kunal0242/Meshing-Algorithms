using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class MapGrid : MonoBehaviour
{
    public MeshFilter _meshFilter;
    public MeshCollider _meshCollider;
    public MeshRenderer _meshRenderer;
    public Material material;
    public GameObject convertToWireFrame;

    public GameObject meshPrefab;

    [HideInInspector]
    public List<GameObject> instantiatedMesh = new List<GameObject>();

    public int width = 5;
    public int height = 5;
    // Temp var
    public float addDistance = 0;

    [HideInInspector]
    public int currentInstantiatedMeshIndex = 0;

    List<Vector3> vert = new List<Vector3>();
    int i = 0;

    public float minDistanceThreshold = 5.0f;

    public void Start()
    {

    }

    public void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit _detectGORaycast = new RaycastHit();

            bool hit = Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out _detectGORaycast);

            if (hit)
            {
                //var collidedMesh = _detectGORaycast.transform.gameObject.GetComponent<MeshFilter>().mesh;
                //var vertices = collidedMesh.vertices.ToList();
                //var tri = collidedMesh.triangles.ToList();

                //tri.RemoveAt(_detectGORaycast.triangleIndex * 3 + 0);
                //tri.RemoveAt(_detectGORaycast.triangleIndex * 3 + 1);
                //tri.RemoveAt(_detectGORaycast.triangleIndex * 3 + 2);

                //collidedMesh.Clear();

                //collidedMesh.vertices = vertices.ToArray();
                //collidedMesh.triangles = tri.ToArray();

                //collidedMesh.RecalculateNormals();

                //_detectGORaycast.transform.gameObject.GetComponent<MeshCollider>().sharedMesh = collidedMesh;

                Debug.Log(_detectGORaycast.triangleIndex);
            }
        }
    }

    public Mesh RemoveTheUnusedVertices(Mesh mesh)
    {
        List<Vector3> verticesList = mesh.vertices.ToList();
        List<int> trianglesList = mesh.triangles.ToList();
        List<Vector3> normalList = mesh.normals.ToList();

        int testVertex = 0;

        while (testVertex < verticesList.Count)
        {
            if (trianglesList.Contains(testVertex))
            {
                testVertex++;
            }
            else
            {
                verticesList.RemoveAt(testVertex);
                normalList.RemoveAt(testVertex);

                for (int i = 0; i < trianglesList.Count; i++)
                {
                    if (trianglesList[i] > testVertex)
                        trianglesList[i]--;
                }
            }
        }

        mesh.vertices = verticesList.ToArray();
        mesh.triangles = trianglesList.ToArray();
        mesh.normals = normalList.ToArray();

        return mesh;
    }

    public void OnGenerateMesh()
    {
        vert = GenerateVericesCoord(width, height);

        convertToWireFrame.SetActive(true);

        currentInstantiatedMeshIndex = instantiatedMesh.Count;

        instantiatedMesh.Add(Instantiate(meshPrefab, new Vector3(addDistance, 0f, 0f), Quaternion.identity));

        DrawMesh(MeshGeneration.GenerateMeshMap(width, height, vert, minDistanceThreshold));

        addDistance += width;
    }

    public void OnGenerateWireFrame()
    {
        for (; i < instantiatedMesh.Count; i++)
        {
            var currentMesh = instantiatedMesh[i].GetComponent<MeshFilter>().mesh;

            List<Vector3> ver = new List<Vector3>();

            ver = currentMesh.vertices.ToList();

            DrawWireFrame(IndicesGenerator.GenerateIndicesData(width, height, ver), CollisonMeshGenerator.GenerateCollisonMeshData(width, height, ver));
        }
        i = 0;
    }

    public void DrawMesh(MeshData meshdata)
    {
        Mesh _generatedMeshdata = new Mesh();
        _generatedMeshdata = meshdata.CreateMesh().GetComponent<MeshFilter>().mesh;

        Debug.Log("The value stored in the triangle mesh array is : " + _generatedMeshdata.triangles.Length);

        instantiatedMesh[currentInstantiatedMeshIndex].gameObject.transform.GetComponent<MeshFilter>().sharedMesh = _generatedMeshdata;
        instantiatedMesh[currentInstantiatedMeshIndex].gameObject.transform.GetComponent<MeshCollider>().sharedMesh = _generatedMeshdata;
    }

    public void DrawWireFrame(IndicesData indicesData, CollisonMeshData collisonMeshdata)
    {
        instantiatedMesh[i].gameObject.transform.GetComponent<MeshFilter>().sharedMesh = indicesData.CreateIndicesMesh();
        instantiatedMesh[i].gameObject.transform.GetComponent<MeshCollider>().sharedMesh = collisonMeshdata.CreateCollsionMesh();
        instantiatedMesh[i].gameObject.transform.GetComponent<MeshRenderer>().material = material == null ? new Material(Shader.Find("Sprites/Default")) { color = Color.black } : material;
    }

    public int addOffset = 0;

    public List<Vector3> GenerateVericesCoord(int deltaX, int deltaZ)
    {
        List<Vector3> vertex = new List<Vector3>();

        for (int z = 0; z < deltaZ; z++)
        {
            for (int x = 0; x < deltaX; x++)
            {
                var point = new Vector3
                (
                    Mathf.FloorToInt(x + addOffset),
                    Random.Range(-1.0f, 1.0f),
                    Mathf.FloorToInt(z)
                );
                vertex.Add(point);
            }
        }
        addOffset += 10;

        return vertex;
    }
}

