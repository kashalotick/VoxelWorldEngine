using DotnetNoise;
using LearningOpenTK.Core;
using LearningOpenTK.Entities.UI;
using LearningOpenTK.Entities.World;
using LearningOpenTK.Entities.World.LightSources;
using LearningOpenTK.Meshes;
using LearningOpenTK.Resources;
using LearningOpenTK.UI;
using LearningOpenTK.Utils;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using VoxelWorldEngine.Core.Builders;
using VoxelWorldEngine.Core.Generators;
using VoxelWorldEngine.DataStructures.Special.Structures.Chunks;
using Vector3Int = VoxelWorldEngine.DataStructures.Common.Structures.Vectors.Vector3Int;
using VoxelMesh = VoxelWorldEngine.DataStructures.Special.Collections.Meshes.Mesh;

namespace ConsoleApp1;

public class Program
{
    public static void Main(string[] args)
    {
        var game = new Game(1200, 900, "Voxel engine test");
        game.SetDefaultScene((w, h, input) => new DemoScene(w, h, input, GenerateWorldObjectList));
        game.Run();
    }

    public static List<WorldObject> GenerateWorldObjectList(Shader shader, Texture texture)
    {
        var chunkMeshes = GenerateVoxelMesh(new Vector3Int(0, 0, 0));
        var worldObjectList = new List<WorldObject>();
        foreach (var mesh in chunkMeshes)
        {
            var worldObject = CreateWorldObject(mesh, shader, texture);
            worldObjectList.Add(worldObject);
        }

        return worldObjectList;
    }


    public static WorldObject CreateWorldObject(VoxelMesh voxelMesh, Shader shader, Texture texture)
    {
        var mesh = ConvertToGameMesh(voxelMesh);
        var obj = new WorldObject(mesh, shader, texture);
        obj.Transform.Position = (Vector3)voxelMesh.PositionOffset;
        obj.Transform.Position -= Vector3.UnitY * 10;
        obj.Transform.Rotation = new Vector3(-MathHelper.PiOver2, 0, 0);
        return obj;
    }

    private static WorldObjectMesh ConvertToGameMesh(VoxelMesh voxelMesh)
    {
        const int stride = 11;
        var vertices = new float[voxelMesh.Vertices.Count * stride];
        for (int i = 0; i < vertices.Length; i += stride)
        {
            var normalizedIndex = i / stride;
            var position = voxelMesh.Vertices[normalizedIndex];
            var normal = voxelMesh.Normals[normalizedIndex];
            var color = new Vector3(0.41f, 0.69f, 0.89f);
            vertices[i] = position.X;
            vertices[i+1] = position.Y;
            vertices[i+2] = position.Z;
            
            vertices[i+3] = normal.X;
            vertices[i+4] = normal.Y;
            vertices[i+5] = normal.Z;
            
            vertices[i+6] = color.X;
            vertices[i+7] = color.Y;
            vertices[i+8] = color.Z;
            
            var uv = GetUv(i % 4);
            
            vertices[i+9] = uv.X;
            vertices[i+10] = uv.X;
        }
        

        var indices = new ushort[voxelMesh.Triangles.Count];
        for (int i = 0; i < indices.Length; i++)
        {
            indices[i] = (ushort)voxelMesh.Triangles[i];
        }

        var mesh = new WorldObjectMesh(vertices, indices);
        return mesh;

        Vector2 GetUv(int localIndex)
        {
            return localIndex switch
            {
                0 => new Vector2(1, 1),
                1 => new Vector2(0, 1),
                2 => new Vector2(0, 0),
                3 => new Vector2(1, 0),
                _ => new Vector2(0, 0)
            };
        }
    }


    public static List<VoxelMesh> GenerateVoxelMesh(Vector3Int chunkPosition)
    {
        var chunk = new Chunk(chunkPosition);

        var fastNoise = new FastNoise(123);
        var heightMapGenerator = new HeightMapGenerator(fastNoise);


        var densityGenerator = new DensityGenerator(heightMapGenerator);


        var octreeBuilder = new OctreeBuilder(densityGenerator);
        var meshBuilder = new MeshBuilder();

        var octree = octreeBuilder.Build(chunk);

        // PrintOctree();

        chunk.Octree = octree;
        var meshes = meshBuilder.BuildMeshList(octree);
        // var meshes = new List<Mesh> {meshBuilder.Build(octree)};

        return meshes;

        void PrintOctree()
        {
            // var sb = new StringBuilder(4000000); 

            var min = new Vector3Int(0, 0, 0);
            var max = new Vector3Int(256, 256, 16);

            using StreamWriter writer = new StreamWriter("here.txt");

            for (int z = min.Z; z < max.Z; z++)
            {
                for (int x = min.X; x < max.X; x++)
                {
                    for (int y = min.Y; y < max.Y; y++)
                    {
                        var position = new Vector3Int(x, y, z);
                        var node = octree.GetNode(position);
                        var isSolid = node.IsSolid;
                        writer.Write(isSolid ? " 1 " : " · ");
                    }

                    writer.Write('\n');
                }

                writer.Write("\n\n");
            }
        }
    }
}