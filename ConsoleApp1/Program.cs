using System.Diagnostics;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Text;
using DotnetNoise;
using Raylib_cs;
using VoxelWorldEngine.Core.Builders;
using VoxelWorldEngine.Core.Generators;
using VoxelWorldEngine.DataStructures.Chunk;
using VoxelWorldEngine.DataStructures.LinearOctree;
using VoxelWorldEngine.DataStructures.Vector3Int;
using Mesh = VoxelWorldEngine.DataStructures.Mesh.Mesh;

namespace ConsoleApp1;

public class Program
{
    private const int GLSL_VERSION = 330;

    public static void Main(string[] args)
    {
        Run();
    }

    public static unsafe void Run()
    {
        Raylib.SetTraceLogLevel(TraceLogLevel.Warning);
        Raylib.InitWindow(1920, 1080, "Voxel Octree Project");

        // TODO: move to chunk region;
        // var sw = Stopwatch.StartNew(); //**
        //
        // Console.WriteLine($"Етап 1. Початок: {sw.ElapsedMilliseconds} мс"); sw.Restart(); // **
        // Console.WriteLine($"Етап 2. Генерація чанка та меша: {sw.ElapsedMilliseconds} мс"); sw.Restart(); // **
        //
        // // return;
        //
        // Console.WriteLine($"Етап 3. Створення вікна: {sw.ElapsedMilliseconds} мс"); sw.Restart(); // **
        //
        //
        // Console.WriteLine($"Етап 4. Конвертація в рейліб: {sw.ElapsedMilliseconds} мс"); sw.Restart(); // **
        // sw.Stop();

        var chunkRegion = GenerateChunkRegion(new Vector3Int(0, 0, 0), 2);


        Raylib.SetWindowState(ConfigFlags.ResizableWindow);

        Raylib.DisableCursor();
        var camera = new Camera3D
        {
            Position = new Vector3(25, 15, 25),
            Target = new Vector3(0, 0, 0),
            Up = new Vector3(0, 0, 1),
            FovY = 45,
            Projection = CameraProjection.Perspective
        };

        // Raylib.SetTargetFPS(165);

        try
        {
            while (!Raylib.WindowShouldClose())
            {
                Raylib.UpdateCamera(ref camera, CameraMode.Free);
                Raylib.BeginDrawing();
                Raylib.ClearBackground(Color.SkyBlue);

                Raylib.BeginMode3D(camera);

                chunkRegion.Draw();

                // Raylib.DrawModel(model2, Vector3.Zero, 1.0f, Color.Black);
                Raylib.DrawGrid(100, 1.0f);
                Raylib.EndMode3D();

                Raylib.DrawFPS(10, 10);
                Raylib.EndDrawing();
            }
        }
        finally
        {
            chunkRegion.Unload();

            Raylib.CloseWindow();
        }
    }

    public static Mesh GenerateMeshHeightmap()
    {
        var heightmapMeshBuilder = new HeightMapMeshBuilder();
        var hmMesh = heightmapMeshBuilder.RunFast();
        var mesh = new Mesh(Vector3Int.Zero)
        {
            Vertices = hmMesh.vertices,
            Triangles = hmMesh.triangles,
            Normals = hmMesh.normals
        };
        return mesh;
    }

    public class ChunkRegion
    {
        public const float Scale = 0.25f;

        public List<Chunk> Chunks = [];
        public List<List<Model>> RayLibModels = [];
        public List<List<Mesh>> Meshes = [];

        public void Draw()
        {
            for (int i = 0; i < RayLibModels.Count; i++)
            {
                var chunk = Chunks[i];
                var chunkRenderPosition = chunk.Position.ToVector3() * LinearOctree.Size;

                for (int j = 0; j < RayLibModels[i].Count; j++)
                {
                    var model = RayLibModels[i][j];
                    var mesh = Meshes[i][j];
                    var position = chunkRenderPosition + mesh.PositionOffset;
                    Raylib.DrawModel(model, position * Scale, Scale, Color.White);
                }
            }
        }

        public void Unload()
        {
            for (int i = 0; i < RayLibModels.Count; i++)
            {
                var modelList = RayLibModels[i];
                foreach (var model2 in modelList)
                {
                    Raylib.UnloadModel(model2);
                }
            }
        }
    }

    public static ChunkRegion GenerateChunkRegion(Vector3Int observer, int radius)
    {
        var chunkRegion = new ChunkRegion();
        radius -= 1;

        var chunkCounter = 0;
        for (int x = -radius; x < radius + 1; x++)
        for (int y = -radius; y < radius + 1; y++)
        {
            var position = observer + new Vector3Int(x, y, 0);
            var chunk = new Chunk(position);
            chunkRegion.Chunks.Add(chunk);
            chunkRegion.Meshes.Add([]);
            chunkRegion.RayLibModels.Add([]);

            var meshes = GenerateMesh(position);
            foreach (var mesh in meshes)
            {
                var rayLibMesh = CreateRaylibMesh(mesh.Vertices, mesh.Triangles, mesh.Normals);
                var model = Raylib.LoadModelFromMesh(rayLibMesh);
         
                chunkRegion.Meshes[chunkCounter].Add(mesh);
                chunkRegion.RayLibModels[chunkCounter].Add(model);
            }

            chunkCounter++;
        }

        Console.WriteLine(chunkRegion.Meshes[0].Count);
        Console.WriteLine(chunkCounter);
        return chunkRegion;
    }

    public static List<Mesh> GenerateMesh(Vector3Int chunkPosition)
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

            // Console.WriteLine(sb.ToString());
        }
    }

    public static unsafe Raylib_cs.Mesh CreateRaylibMesh(
        List<Vector3> vertices,
        List<int> triangles,
        List<Vector3> normals
    )
    {
        var mesh = new Raylib_cs.Mesh();

        mesh.VertexCount = vertices.Count;
        mesh.TriangleCount = triangles.Count / 3;

        // Вертекси
        mesh.Vertices = (float*)Marshal.AllocHGlobal((vertices.Count * 3 * sizeof(float)));
        for (var i = 0; i < vertices.Count; i++)
        {
            mesh.Vertices[i * 3 + 0] = vertices[i].X;
            mesh.Vertices[i * 3 + 1] = vertices[i].Y;
            mesh.Vertices[i * 3 + 2] = vertices[i].Z;
        }

        // Індекси
        mesh.Indices = (ushort*)Marshal.AllocHGlobal((triangles.Count * sizeof(ushort)));
        for (var i = 0; i < triangles.Count; i++)
        {
            mesh.Indices[i] = (ushort)triangles[i];
        }

        // Нормалі
        mesh.Normals = (float*)Marshal.AllocHGlobal((normals.Count * 3 * sizeof(float)));
        for (var i = 0; i < normals.Count; i++)
        {
            mesh.Normals[i * 3 + 0] = normals[i].X;
            mesh.Normals[i * 3 + 1] = normals[i].Y;
            mesh.Normals[i * 3 + 2] = normals[i].Z;
        }

        // Додай texcoords (UV) - просто нулі
        mesh.TexCoords = (float*)Marshal.AllocHGlobal((vertices.Count * 2 * sizeof(float)));
        for (var i = 0; i < vertices.Count * 2; i++)
        {
            mesh.TexCoords[i] = 0f;
        }

        Raylib.UploadMesh(ref mesh, false);
        return mesh;
    }
}