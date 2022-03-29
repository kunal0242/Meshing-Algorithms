using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class MeshGenerator : MonoBehaviour
{
    List<Mesh> mesh = new List<Mesh>();
    // List<MeshFilter> meshFilters = new List<MeshFilter>();
    List<Vector3> vertices = new List<Vector3>();

    List<GameObject> meshGO = new List<GameObject>();

    //Vector3[] vertices;
    int[] triangles;

    int xSize = 26;
    int zSize = 19;

    float minDistanceThreshold = 5f;

    int meshCount = 0;

    public GameObject meshPrefab;

    void Start()
    {

        //meshGO.Add(Instantiate(meshPrefab, new Vector3(0f, 0f, 0f), Quaternion.identity));
        //Debug.Log("The Length of the mesh list is :" + meshGO.Count);

        //meshPrefab.GetComponent<MeshFilter>().mesh = meshGO[0].GetComponent<MeshFilter>().mesh;

        GenerateVertices();

        //CreateShape();

        //GenerateMesh();
    }

    // Function to generate the random vertices for the mesh
    void GenerateVertices()
    {
        Mesh pointCloudMesh = new Mesh();

        float spacing = 1f;

        for (var z = 0; z < zSize; z++)
        {
            for (var x = 0; x < xSize; x++)
            {
                var point = new Vector3Int
                (
                    Mathf.FloorToInt(x * spacing),
                    Mathf.FloorToInt(Random.Range(-1.0f, 1.0f)),
                    Mathf.FloorToInt(z * spacing)
                );
                vertices.Add(point);
            }
        }

        CreateShape(pointCloudMesh);
    }

    // Function to generate the triangles and quads from the vertex data (i.e. creating shape)
    void CreateShape(Mesh pcMesh)
    {
        int verticesCount = 0;
        int triangleCount = 0;


        triangles = new int[(xSize) * (zSize) * 6];

        for (int z = 0; z < zSize - 1; z++)
        {
            for (int x = 0; x < xSize - 1; x++)
            {
                if (Vector3.Distance(vertices[verticesCount + z], vertices[verticesCount + xSize + z]) < minDistanceThreshold &&
                    Vector3.Distance(vertices[verticesCount + xSize + z], vertices[verticesCount + 1 + z]) < minDistanceThreshold &&
                    Vector3.Distance(vertices[verticesCount + z], vertices[verticesCount + 1 + z]) < minDistanceThreshold)
                {
                    triangles[triangleCount + 0] = verticesCount + z;
                    triangles[triangleCount + 1] = verticesCount + xSize + z;
                    triangles[triangleCount + 2] = verticesCount + 1 + z;
                    triangleCount += 3;
                }

                //if (Vector3.Distance(vertices[verticesCount + 1 + z], vertices[verticesCount + xSize + z]) < minDistanceThreshold &&
                //    Vector3.Distance(vertices[verticesCount + xSize + z], vertices[verticesCount + xSize + 1 + z]) < minDistanceThreshold &&
                //    Vector3.Distance(vertices[verticesCount + 1 + z], vertices[verticesCount + xSize + 1 + z]) < minDistanceThreshold)
                //{
                //    triangles[triangleCount + 3] = verticesCount + 1 + z;
                //    triangles[triangleCount + 4] = verticesCount + xSize + z;
                //    triangles[triangleCount + 5] = verticesCount + xSize + 1 + z;
                //    triangleCount += 3;
                //}


                //triangleCount += 6;
                verticesCount++;
            }
        }

        List<int> triangleList = triangles.ToList();
        Debug.Log("The length of the triangleList is : " + triangleList.Count);
        for (int i = triangleList.Count - 1; i > -1; i--)
        {
            //Debug.LogError("I am inside the for loop");
            if (triangleList[i] != 0)
            {
                triangleList = triangleList.GetRange(0, i + 1);
                Debug.Log("The length of the sub list is : " + triangleList.GetRange(0, i + 1).Count);
                break;
            }
        }

        triangles = triangleList.ToArray();

        // Function to generate the mesh components
        //GenerateMeshComponents(meshCount, vertices, triangles);

        meshCount++;
        mesh.Add(pcMesh);
        GetComponent<MeshFilter>().mesh = mesh[0];

        //pcMesh.Clear();
        //pcMesh.vertices = vertices.ToArray();
        //pcMesh.triangles = triangles;
        //pcMesh.RecalculateNormals();



        mesh[0].Clear();
        mesh[0].vertices = vertices.ToArray();
        mesh[0].triangles = triangles;
        mesh[0].RecalculateNormals();

    }

    private void GenerateMeshComponents() //int meshCnt, List<Vector3> vert, int[] tri
    {
        //mesh.Add(new Mesh()
        //{
        //    vertices = vert.ToArray(),
        //    triangles = tri
        //});

        //Debug.Log("The size of the mesh list is : " + mesh.Count);
        //Debug.Log("The mesh index being passed from the function is : " + meshCnt);

        //GetComponent<MeshFilter>().mesh = mesh[meshCnt];

        //// Function to assign the mesh components to the meshfilter
        //GenerateMesh(meshCnt);


    }


    void GenerateMesh() //int meshIndex
    {
        meshGO[0].GetComponent<MeshFilter>().mesh.Clear();

        //mesh[meshIndex].vertices = vertices.ToArray();
        //mesh[meshIndex].triangles = triangles;

        //Debug.LogError("Vertices count in the mesh : " + mesh[meshIndex].vertices.Length);

        meshGO[0].GetComponent<MeshFilter>().mesh.RecalculateNormals();
    }

    public void OnDrawGizmos()
    {
        if (vertices == null)
            return;

        for (int i = 0; i < vertices.Count; i++)
        {
            Gizmos.DrawSphere(vertices[i], .1f);
        }
    }

    /* 

    /// <Logic for rotating the normals of the mesh>
     
        Vector3[] normals = mesh.normals;

        Quaternion rotateNormals = Quaternion.AngleAxis(45, Vector3.up);

        for (int i = 0; i < normals.Length; i++)
            normals[i] = rotateNormals * normals[i];

        mesh.normals = normals;


    /// <Logic for generating random points>

        for (int z = 0; z < zSize; z++)
        {
            for (int x = 0; x < xSize; x++)
            {
                vertices.Add(new Vector3(x, Random.Range((float)-1.0, (float)1.0), z));
            }
        }

    /// <Logic for combining meshes>

        public void CombineMeshes()
        {
            MeshFilter[] mfil = FindObjectsOfType<MeshFilter>();
            CombineInstance[] combInst = new CombineInstance[mfil.Length];

            int i = 0;
            while (i < mfil.Length)
            {
                combInst[i].mesh = mfil[i].sharedMesh;
                combInst[i].transform = mfil[i].transform.localToWorldMatrix;
                mfil[i].gameObject.SetActive(false);

                i++;
            }

            transform.GetComponent<MeshFilter>().mesh = new Mesh();
            transform.GetComponent<MeshFilter>().mesh.CombineMeshes(combInst);
        }

    // To instantiate in the direction of the normal
    Instantiate(objectToSpawn, _generateNewMeshA.vertices[i] + (0.25f * _generateNewMeshA.normals[i]), Quaternion.identity);
    // To instantiate in the opposite direction of the normal
    Instantiate(objectToSpawn, (_generateNewMeshA.vertices[i] + (-0.25f * _generateNewMeshA.normals[i])), Quaternion.identity);
     */
}


