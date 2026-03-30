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

    public Ray PrevRay { get; private set; }
    public RayHit PrevHit {get; private set;}
    private (Ray ray, RayHit hit) _traced = new();
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

    public RayHit Shoot(Ray ray, IRaycastable target)
    {
        var rayHit = target.Raycast(ray);
        PrevRay = ray;
        PrevHit =  rayHit;
        return rayHit;
    }

    public void Trace()
    {
        _traced = (PrevRay, PrevHit);
        _rayMesh.UpdateRay(_traced.ray, _traced.hit);
        var hitline = (bool)PrevHit.IsHit ? $"{PrevHit.HitIn.FancyString()} -> {PrevHit.HitOut.FancyString()}" : "";
        // Console.WriteLine($"\nRay: {PrevRay.Origin.FancyString()} -> {PrevRay.Direction.FancyString()}"
        //                   + $"\nHit: {PrevHit.IsHit}  {hitline}");

        _rayHitPointsMesh.UpdateHitPoints(_traced.ray, _traced.hit);
    }


    public void Dispose()
    {
        _rayMesh.Dispose();
        _rayHitPointsMesh.Dispose();
    }
}