using System.Numerics;

namespace VoxelWorldEngine.DataStructures.Mesh;

public class Mesh
{
    public List<Vector3> Vertices;
    public List<int> Triangles;
    public List<Vector3> Normals;

    public Mesh()
    {
        Vertices = [];
        Triangles = [];
        Normals = [];
    }

    public void AddFace(Vector3Int.Vector3Int position, Vector3Int.Vector3Int normal, int size)
    {
        int verticesCount = Vertices.Count;
        
        var faceVertices = GetFaceVertices(position, normal, size);
        Vertices.AddRange(faceVertices);
        
        Triangles.Add(verticesCount); 
        Triangles.Add(verticesCount + 1); 
        Triangles.Add(verticesCount + 2);
        Triangles.Add(verticesCount); 
        Triangles.Add(verticesCount + 2); 
        Triangles.Add(verticesCount + 3);
        
        var normalVector = new Vector3(normal.X, normal.Y, normal.Z);;
        Normals.Add(normalVector);
        Normals.Add(normalVector);
        Normals.Add(normalVector);
        Normals.Add(normalVector);

    }

    private Vector3[] GetFaceVertices(Vector3Int.Vector3Int position, Vector3Int.Vector3Int normal, int size)
    {
        Vector3[] vertices = new Vector3[4];
        Vector3 pos = new Vector3(position.X, position.Y, position.Z);
        float s = size;

        // Передня грань (Z+)
        if (normal.Z == 1)
        {
            vertices[0] = pos + new Vector3(0, 0, s);
            vertices[1] = pos + new Vector3(s, 0, s);
            vertices[2] = pos + new Vector3(s, s, s);
            vertices[3] = pos + new Vector3(0, s, s);
        }
        // Задня грань (Z-)
        else if (normal.Z == -1)
        {
            vertices[0] = pos + new Vector3(s, 0, 0);
            vertices[1] = pos + new Vector3(0, 0, 0);
            vertices[2] = pos + new Vector3(0, s, 0);
            vertices[3] = pos + new Vector3(s, s, 0);
        }
        // Права грань (X+)
        else if (normal.X == 1)
        {
            vertices[0] = pos + new Vector3(s, 0, s);
            vertices[1] = pos + new Vector3(s, 0, 0);
            vertices[2] = pos + new Vector3(s, s, 0);
            vertices[3] = pos + new Vector3(s, s, s);
        }
        // Ліва грань (X-)
        else if (normal.X == -1)
        {
            vertices[0] = pos + new Vector3(0, 0, 0);
            vertices[1] = pos + new Vector3(0, 0, s);
            vertices[2] = pos + new Vector3(0, s, s);
            vertices[3] = pos + new Vector3(0, s, 0);
        }
        // Верхня грань (Y+)
        else if (normal.Y == 1)
        {
            vertices[0] = pos + new Vector3(0, s, s);
            vertices[1] = pos + new Vector3(s, s, s);
            vertices[2] = pos + new Vector3(s, s, 0);
            vertices[3] = pos + new Vector3(0, s, 0);
        }
        // Нижня грань (Y-)
        else if (normal.Y == -1)
        {
            vertices[0] = pos + new Vector3(0, 0, 0);
            vertices[1] = pos + new Vector3(s, 0, 0);
            vertices[2] = pos + new Vector3(s, 0, s);
            vertices[3] = pos + new Vector3(0, 0, s);
        }

        return vertices;
    }
}