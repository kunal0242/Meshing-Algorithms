using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MeshGeneration
{
    static List<Vector3> vert = new List<Vector3>();

    public static MeshData GenerateMeshMap(int deltaX, int deltaZ, List<Vector3> vertices, float minDistThreshold)
    {
        int height = deltaZ;
        int width = deltaX;

        vert = vertices;

        MeshData _createMesh = new MeshData(width, height);
        int vertexIndexCount = 0;

        for (int z = 0; z < height; z++)
        {
            for (int x = 0; x < width; x++)
            {
                _createMesh.vertices[vertexIndexCount] = vert[vertexIndexCount];

                if (x < width - 1 && z < height - 1)
                {
                    if (Vector3.Distance(vert[vertexIndexCount], vert[vertexIndexCount + width]) < minDistThreshold &&
                        Vector3.Distance(vert[vertexIndexCount + width], vert[vertexIndexCount + 1]) < minDistThreshold &&
                        Vector3.Distance(vert[vertexIndexCount], vert[vertexIndexCount + 1]) < minDistThreshold)
                    {
                        _createMesh.AddTriangle(vertexIndexCount, vertexIndexCount + width, vertexIndexCount + 1);
                    }

                    if (Vector3.Distance(vert[vertexIndexCount + width], vert[vertexIndexCount + width + 1]) < minDistThreshold &&
                        Vector3.Distance(vert[vertexIndexCount + width + 1], vert[vertexIndexCount + 1]) < minDistThreshold &&
                        Vector3.Distance(vert[vertexIndexCount + width], vert[vertexIndexCount + 1]) < minDistThreshold)
                    {
                        _createMesh.AddTriangle(vertexIndexCount + width, vertexIndexCount + width + 1, vertexIndexCount + 1);
                    }

                }

                vertexIndexCount++;
            }
        }

        return _createMesh;
    }
}

public class MeshData
{
    public Vector3[] vertices;
    public int[] triangles;
    //public Vector2[] uvs;

    int width = 0;
    int height = 0;
    int trianglesForSubmesh = 24;

    int triangleIndex;

    public MeshData(int meshWidth, int meshHeight)
    {
        width = meshWidth;
        height = meshHeight;

        vertices = new Vector3[meshWidth * meshHeight];
        //uvs = new Vector2[meshWidth * meshHeight];
        triangles = new int[(meshWidth - 1) * (meshHeight - 1) * 6];
    }

    public void AddTriangle(int a, int b, int c)
    {
        triangles[triangleIndex] = a;
        triangles[triangleIndex + 1] = b;
        triangles[triangleIndex + 2] = c;
        triangleIndex += 3;
    }

    public GameObject CreateMesh()
    {
        GameObject meshSO = new GameObject();

        meshSO.AddComponent<MeshRenderer>();
        meshSO.AddComponent<MeshFilter>();

        meshSO.GetComponent<MeshFilter>().mesh.vertices = vertices;
        meshSO.GetComponent<MeshFilter>().mesh.subMeshCount = (triangles.Length / trianglesForSubmesh) + 1;

        List<Material> submeshMaterials = new List<Material>();
        Material material = new Material(Shader.Find("Standard (Specular setup)")) { name = "Default" };
        Debug.Log("Material color is : " + material.color);
        int submeshCount = 0;

        for (int i = 0; i < triangles.Length - trianglesForSubmesh; i += trianglesForSubmesh)
        {
            int[] tri = new int[24];

            for (int j = 0; j < trianglesForSubmesh; j++)
            {
                tri[j] = triangles[i + j];
            }

            //tri[0] = triangles[i + 0];
            //tri[1] = triangles[i + 1];
            //tri[2] = triangles[i + 2];

            //tri[3] = triangles[i + 3];
            //tri[4] = triangles[i + 4];
            //tri[5] = triangles[i + 5];

            meshSO.GetComponent<MeshFilter>().mesh.SetTriangles(tri, submeshCount);
            submeshMaterials.Add(material);

            submeshCount++;
        }
        //mesh.triangles = triangles;
        //mesh.uv = uvs;

        meshSO.GetComponent<MeshRenderer>().sharedMaterials = new Material[submeshMaterials.Count];

        for (int i = 0; i < submeshMaterials.Count; i++)
        {
            meshSO.GetComponent<MeshRenderer>().sharedMaterials[i] = submeshMaterials[i];
        }

        meshSO.GetComponent<MeshFilter>().mesh.RecalculateNormals();
        return meshSO;
    }
}
