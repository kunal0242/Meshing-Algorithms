using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class MeshingALG : MonoBehaviour
{
    //List<Mesh> meshes = new List<Mesh>();
    List<Vector3> vert = new List<Vector3>();
    int[] tri;
    List<GameObject> meshPrefabList = new List<GameObject>();
    public static List<GameObject> subMeshList = new List<GameObject>();
    public GameObject objectToSpawn;
    [SerializeField]
    GameObject meshPrefab;
    //Test Variables
    int xSize = 25;
    int zSize = 25;
    int meshCount = 0;
    float minDistThreshold = 5f;
    float spacing = 1f;
    int _totalMeshCount;
    float _changeMeshPosition = 0.0f;

    [SerializeField]
    GameObject meshRenderGO;

    MeshSplit.MeshSplitController _meshSplitController;

    [SerializeField]
    Material _blueColorGO;

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit _meshRaycastHit = new RaycastHit();
            bool hit = Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out _meshRaycastHit);

            if (hit)
            {
                Debug.DrawLine(Input.mousePosition, _meshRaycastHit.point);
                Debug.Log("Mesh raycast hit position is : " + _meshRaycastHit.point);
                Debug.Log("The gameObject selected is : " + _meshRaycastHit.collider.transform.name);
                if (subMeshList.Find(_meshRaycastHit.collider.transform.gameObject.Equals))
                {
                    subMeshList.Remove(_meshRaycastHit.collider.transform.gameObject);
                    Debug.Log("Total submeshes are : " + subMeshList.Count);
                    _meshRaycastHit.collider.transform.gameObject.SetActive(false);
                }
                //CheckIfVerticeExists(_meshRaycastHit.point);
            }
            else
            {
                Debug.Log("Raycast did not collide on the gameobject...");
            }
        }
    }

    private void CheckIfVerticeExists(Vector3 pos)
    {
        //if (vert.Count == 0)
        //    return;

        //for (int i = 0; i < vert.Count; i++)
        //{
        //    if (vert[i] == new Vector3(UnityEngine.Random.Range(-pos.x, pos.x), UnityEngine.Random.Range(-pos.y, pos.y), UnityEngine.Random.Range(-pos.z, pos.z)))
        //    {
        //        Debug.Log("Vertices exists in the list and its index is : " + i);
        //    }
        //    else
        //    {
        //        Debug.Log("No vertices found in such position");
        //    }
        //}
    }

    // Start is called before the first frame update
    public void OnCreateMeshButtonClick()
    {
        StartHelper();
    }
    void StartHelper()
    {
        //GetVertices(meshCount);
        GetVertices();
    }

    private void GetVertices()
    {
        //Debug.Log("The count of the meshPrefabList before instanstiate is : " + meshPrefabList.Count);

        //if (meshCount == meshPrefabList.Count)
        //{
        //    meshPrefabList.Add(Instantiate(meshPrefab, new Vector3(0f, 0f, 0f), Quaternion.identity));
        //}

        for (int z = 0; z < zSize; z++)
        {
            for (int x = 0; x < xSize; x++)
            {
                var point = new Vector3Int
                (
                    Mathf.FloorToInt(x * spacing),
                    Mathf.FloorToInt(Random.Range(-1.0f, 1.0f)),
                    Mathf.FloorToInt(z * spacing)
                );
                vert.Add(point);
            }
        }

        Debug.Log("Total vertex are : " + vert.Count);

        //CreateShape(meshCount);
        CreateShape();
    }

    private void CreateShape()
    {
        int verticesCount = 0;
        int triangleCount = 0;

        Mesh _generateNewMesh = new Mesh();

        _generateNewMesh.Clear();
        _generateNewMesh.vertices = vert.ToArray();
        _generateNewMesh.subMeshCount = zSize;

        tri = new int[(xSize) * (zSize) * 6];

        for (int z = 0; z < zSize - 1; z++)
        {
            for (int x = 0; x < xSize - 1; x++)
            {
                if (Vector3.Distance(vert[verticesCount + z], vert[verticesCount + xSize + z]) < minDistThreshold &&
                    Vector3.Distance(vert[verticesCount + xSize + z], vert[verticesCount + 1 + z]) < minDistThreshold &&
                    Vector3.Distance(vert[verticesCount + z], vert[verticesCount + 1 + z]) < minDistThreshold)
                {
                    tri[triangleCount + 0] = verticesCount + z;
                    tri[triangleCount + 1] = verticesCount + xSize + z;
                    tri[triangleCount + 2] = verticesCount + 1 + z;
                    //triangleCount += 3;
                }

                if (Vector3.Distance(vert[verticesCount + 1 + z], vert[verticesCount + xSize + 1 + z]) < minDistThreshold &&
                    Vector3.Distance(vert[verticesCount + xSize + 1 + z], vert[verticesCount + xSize + z]) < minDistThreshold &&
                    Vector3.Distance(vert[verticesCount + 1 + z], vert[verticesCount + xSize + z]) < minDistThreshold)
                {
                    tri[triangleCount + 3] = verticesCount + xSize + z;
                    tri[triangleCount + 4] = verticesCount + xSize + 1 + z;
                    tri[triangleCount + 5] = verticesCount + 1 + z;
                    //triangleCount += 3;
                }

                triangleCount += 6;
                verticesCount++;
            }
            //_generateNewMesh.SetTriangles()
        }

        List<int> triangleList = tri.ToList();
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

        tri = triangleList.ToArray();
        //CreateMesh(frameMeshCount);
        CreateMesh();
    }

    //private void CreateMesh()
    //{
    //    _totalMeshCount = meshPrefabList.Count();
    //    Debug.Log("The count of the meshPrefabList before instanstiate is : " + meshPrefabList.Count);

    //    Mesh generateNewMesh = new Mesh();

    //    generateNewMesh.Clear();
    //    generateNewMesh.vertices = vert.ToArray();
    //    generateNewMesh.triangles = tri;
    //    generateNewMesh.RecalculateNormals();

    //    Debug.Log("The total count of the normals of the mesh is : " + generateNewMesh.normals.Length);
    //    Debug.Log("The total count of the triangles of the mesh is : " + generateNewMesh.triangles.Length);

    //    meshPrefabList.Add(Instantiate(meshPrefab, new Vector3(_changeMeshPosition, 0f, 0f), Quaternion.identity));

    //    meshPrefabList[_totalMeshCount].GetComponent<MeshFilter>().mesh = generateNewMesh;
    //    meshPrefabList[_totalMeshCount].GetComponent<MeshCollider>().sharedMesh = generateNewMesh;
    //    _changeMeshPosition += 25.0f;
    //}

    public void OnDrawGizmos()
    {
        if (vert == null)
            return;

        for (int i = 0; i < vert.Count; i++)
        {
            Gizmos.DrawSphere(vert[i], 0.1f);
        }
    }

    // Function to combine all the meshes in a list of MeshPrefab - It requires a meshrender GO to save all the data
    public void OnCombineMeshes()
    {
        CombineInstance[] combInst = new CombineInstance[subMeshList.Count];

        int i = 0;
        while (i < subMeshList.Count)
        {
            combInst[i].mesh = subMeshList[i].GetComponent<MeshFilter>().sharedMesh;
            combInst[i].transform = subMeshList[i].transform.localToWorldMatrix;
            subMeshList[i].gameObject.SetActive(false);
            i++;
        }

        meshRenderGO.GetComponent<MeshFilter>().mesh = new Mesh();
        meshRenderGO.GetComponent<MeshFilter>().mesh.CombineMeshes(combInst);
    }

    public void OnRaycastMesh()
    {
        for (int i = 0; i < vert.Count; i++)
        {
            RaycastHit _checkMeshRaycast = new RaycastHit();
            bool _meshHit = Physics.Raycast(Camera.main.ScreenPointToRay(vert[i]), out _checkMeshRaycast);

            if (_meshHit)
            {
                Debug.Log("Mesh has collided");
            }
            else
            {
                Debug.Log("Mesh did not collide");
            }
        }

    }
    private void CreateMesh()
    {
        _totalMeshCount = meshPrefabList.Count();

        Mesh _generateNewMeshA = new Mesh();

        _generateNewMeshA.Clear();
        _generateNewMeshA.subMeshCount = 1;
        _generateNewMeshA.vertices = vert.ToArray();
        _generateNewMeshA.SetTriangles(tri, 0);
        _generateNewMeshA.RecalculateNormals();

        Debug.Log("Size of the grid along X-axis is : " + xSize);
        Debug.Log("Size of the grid along Y-axis is : " + zSize);


        // We can proceed to generate the mesh after removing the vertices
        meshPrefabList.Add(Instantiate(meshPrefab, new Vector3(0f, _changeMeshPosition, 0f), Quaternion.identity));

        meshPrefabList[_totalMeshCount].GetComponent<MeshFilter>().mesh = _generateNewMeshA;
        meshPrefabList[_totalMeshCount].GetComponent<MeshCollider>().sharedMesh = _generateNewMeshA;

        //Debug.Log("Mesh is splitted into the submeshes");
        Debug.Log("Submesh count is : " + _generateNewMeshA.subMeshCount);
        //meshPrefabList[_totalMeshCount].SetActive(false);

        //SplitTheMesh();

        Transform currentMeshTransform = meshPrefabList[_totalMeshCount].transform;
        //for (int i = 0; i < meshPrefabList[_totalMeshCount].GetComponent<MeshFilter>().mesh.vertexCount; i++)
        //{
        //    Vector3 currentVertex = currentMeshTransform.TransformPoint(_generateNewMeshA.vertices[i]);
        //    Instantiate(objectToSpawn, currentVertex + -1 * (0.25f * _generateNewMeshA.normals[i]), Quaternion.identity);
        //    Ray _meshHitRay = new Ray(currentVertex, -1 * (currentVertex + (0.25f * _generateNewMeshA.normals[i])));
        //    RaycastHit _meshRaycastHit = new RaycastHit();

        //    bool _hit = Physics.Raycast(_meshHitRay, out _meshRaycastHit);

        //    if (_hit)
        //    {
        //        Debug.Log("yes");
        //        Debug.Log("The gameObject selected is : " + _meshRaycastHit.collider.transform.name);
        //        _meshRaycastHit.collider.transform.GetComponent<MeshRenderer>().material = _blueColorGO;
        //    }
        //    else
        //    {
        //        Debug.Log("improve yourself");
        //    }
        //}


        vert.Clear();

        _changeMeshPosition += 10.0f;
    }

    public void SplitTheMesh()
    {
        _meshSplitController = FindObjectOfType<MeshSplit.MeshSplitController>();
        _meshSplitController.Split();

        Debug.Log("Total number of submesh are : " + subMeshList.Count);

    }
}
