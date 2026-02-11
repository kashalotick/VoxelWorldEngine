using System.Numerics;
using DotnetNoise;
using VoxelWorldEngine.Core.Generators;
using Vector2Int = VoxelWorldEngine.DataStructures.Common.Structures.Vectors.Vector2Int;

namespace VoxelWorldEngine.Core.Builders;

public class HeightMapMeshBuilder
{
    private HeightMapGenerator _heightMapGenerator;
    private DensityGenerator _densityGenerator;


    public HeightMapMeshBuilder()
    {
        _heightMapGenerator = new HeightMapGenerator(new FastNoise(123));
        _densityGenerator = new DensityGenerator(_heightMapGenerator);
    }

    public (List<Vector3> vertices, List<int> triangles, List<Vector3> normals) RunFast()
    {
        const int size = 32;
        var heightmap = GenerateHeightMap(size);

        var mesh = GenerateMesh(heightmap);

        return mesh;
    }

    public float[,] GenerateHeightMap(int size)
    {
        var heightmap = new float[size, size];
        for (int x = 0; x < size; x++)
        for (int y = 0; y < size; y++)
        {
            // var z = -16;
            // var position = new Vector3Int(x, y, z);
            // var height = _densityGenerator.GetValue(position) / 10;
            
            var height = _heightMapGenerator.GetValue(new Vector2Int(x, y));
            
            heightmap[x, y] = height;
        }

        return heightmap;
    }

    public (List<Vector3> vertices, List<int> triangles, List<Vector3> normals) GenerateMesh(float[,] heightmap)
    {
        var vertices = new List<Vector3>();
        var triangles = new List<int>();
        var normals = new List<Vector3>();
        var size = heightmap.GetLength(0);
        var halfSize = size / 2;
        halfSize = 0;
        
        for (var x = 0; x < size; x++)
        for (var y = 0; y < size; y++)
        {
            var height = heightmap[x, y];
            var vertex = new Vector3(x - halfSize, y - halfSize, height);
            vertices.Add(vertex);
            normals.Add(Vector3.UnitY);
        }

        for (var x = 0; x < size - 1; x++)
        for (var y = 0; y < size - 1; y++)
        {
            var topLeft = x * size + y;
            var topRight = topLeft + 1;
            var bottomLeft = (x + 1) * size + y;
            var bottomRight = bottomLeft + 1;

            // Перший трикутник (проти годинникової стрілки)
            triangles.Add(bottomLeft);
            triangles.Add(topRight);
            triangles.Add(topLeft);

            // Другий трикутник
            triangles.Add(bottomLeft);
            triangles.Add(bottomRight);
            triangles.Add(topRight);
        }

        return (vertices, triangles, normals);
    }
}