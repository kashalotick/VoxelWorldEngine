using LearningOpenTK.Core;
using LearningOpenTK.Core.DTO;
using LearningOpenTK.Core.Primitives;
using LearningOpenTK.Resources.Interfaces;
using OpenTK.Graphics.OpenGL4;
using VoxelWorldEngine.Core.Raycasting;
using VoxelWorldEngine.Utils;

namespace GameApp.Debug;

public class RayShooter : ILoadable, IRenderable
{
    private IShader _shader;

    private (Ray ray, RayHit hit) _shotRay = new();
    private LineMesh _rayMesh;
    private RayHitPointsMesh _rayHitPointsMesh;

    public RayShooter(IShader shader)
    {
        _shader = shader;
    }

    public void Load()
    {
        _rayMesh = new LineMesh();
        _rayMesh.Load(BufferUsageHint.DynamicDraw);
        _rayHitPointsMesh = new RayHitPointsMesh();
        _rayHitPointsMesh.Load(BufferUsageHint.DynamicDraw);
    }

    public void Render(RenderContext renderContext)
    {
        _shader.Use();
        var viewProjection = renderContext.ViewMatrix * renderContext.ProjectionMatrix3D;
        _shader.SetMatrix4("uViewProjection", viewProjection);

        _rayMesh.Render();

   
        _rayHitPointsMesh.Render();
        
    }

    public void Update(Ray ray, RayHit? hit = null)
    {
        _shotRay = (ray, hit ?? new RayHit());
        _rayMesh.UpdateRay(_shotRay.ray, _shotRay.hit);
        var hitline = (bool)hit?.IsHit ? $"{hit?.HitIn.FancyString()} -> {hit?.HitOut.FancyString()}" : "";
        Console.WriteLine($"\nRay: {ray.Origin.FancyString()} -> {ray.Direction.FancyString()}"
                          + $"\nHit: {hit?.IsHit}  {hitline}");
  
        _rayHitPointsMesh.UpdateHitPoints(_shotRay.ray, _shotRay.hit);
        
    }

    public void Dispose()
    {
        _rayMesh.Dispose();
        _rayHitPointsMesh.Dispose();
    }
}