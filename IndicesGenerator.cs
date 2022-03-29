using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class CollisonMeshGenerator
{
    static List<Vector3> vertex = new List<Vector3>();

    public static CollisonMeshData GenerateCollisonMeshData(int deltaX, int deltaZ, List<Vector3> vertices)
    {
        int height = deltaZ;
        int width = deltaX;

        CollisonMeshData _collisonMeshdata = new CollisonMeshData(width, height);

        vertex = vertices;

        int vertexIndexCount = 0;

        for (int z = 0; z < height; z++)
        {
            for (int x = 0; x < width; x++)
            {
                _collisonMeshdata.vertices[vertexIndexCount] = vertex[vertexIndexCount];

                if (x < width - 1 && z < height - 1)
                {
                    _collisonMeshdata.AddCollsionTriangle(vertexIndexCount, vertexIndexCount + width, vertexIndexCount + 1);
                    _collisonMeshdata.AddCollsionTriangle(vertexIndexCount + width, vertexIndexCount + width + 1, vertexIndexCount + 1);

                }

                vertexIndexCount++;
            }
        }

        return _collisonMeshdata;
    }
}

public class IndicesGenerator
{
    static List<Vector3> vertex = new List<Vector3>();

    public static IndicesData GenerateIndicesData(int deltaX, int deltaZ, List<Vector3> vertices)
    {
        int height = deltaZ;
        int width = deltaX;

        IndicesData _indicesData = new IndicesData(width, height);

        vertex = vertices;

        for (int verticesIndex = 0, vert = 0, z = 0; z < height; z++)
        {
            for (int x = 0; x < width; x++)
            {
                _indicesData.vertices[verticesIndex] = vertex[verticesIndex];

                if (x < width - 1 && z < height - 1)
                {
                    _indicesData.AddIndices(vert, vert + width);
                    _indicesData.AddIndices(vert + width, vert + 1 + width);
                    _indicesData.AddIndices(vert + 1 + width, vert + 1);
                    _indicesData.AddIndices(vert + 1, vert);
                    _indicesData.AddIndices(vert + width, vert + 1);
                }

                vert++;
                verticesIndex++;
            }
            //vert++;
        }

        return _indicesData;
    }
}

public class IndicesData
{
    public Vector3[] vertices;
    public int[] indices;

    public IndicesData(int meshWidth, int meshHeight)
    {
        vertices = new Vector3[meshWidth * meshHeight];
        indices = new int[meshWidth * meshHeight * 10];
    }

    int indexCount = 0;

    public void AddIndices(int a, int b)
    {
        indices[indexCount + 0] = a;

        indices[indexCount + 1] = b;

        indexCount += 2;
    }

    public Mesh CreateIndicesMesh()
    {
        Mesh mesh = new Mesh();

        mesh.Clear();
        mesh.vertices = vertices;
        mesh.SetIndices(indices, MeshTopology.Lines, 0);

        return mesh;
    }
}

public class CollisonMeshData
{
    public Vector3[] vertices;
    public int[] triangles;

    int triangleIndex = 0;

    public CollisonMeshData(int meshWidth, int meshHeight)
    {
        vertices = new Vector3[meshWidth * meshHeight];
        triangles = new int[(meshWidth - 1) * (meshHeight - 1) * 6];
    }

    public void AddCollsionTriangle(int a, int b, int c)
    {
        triangles[triangleIndex] = a;
        triangles[triangleIndex + 1] = b;
        triangles[triangleIndex + 2] = c;
        triangleIndex += 3;
    }

    public Mesh CreateCollsionMesh()
    {
        Mesh mesh = new Mesh();
        mesh.Clear();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        //mesh.uv = uvs;
        mesh.RecalculateNormals();
        return mesh;
    }
}