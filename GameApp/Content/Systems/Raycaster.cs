using GameApp.Graphics.Debug;
using LearningOpenTK.Core.Lifecycle;
using LearningOpenTK.Core.Rendering;
using LearningOpenTK.Engine.Resources.Shaders;
using OpenTK.Graphics.OpenGL4;
using VoxelModule.Core.Raycasting;

namespace GameApp.Content.Systems;

public class Raycaster : ILoadable, IRenderable
{
    private readonly IShader _shader;

    private bool _meshInitialized;
    private RayHitPointsMesh _rayHitPointsMesh;
    private LineMesh _rayMesh;

    public Raycaster(IShader shader)
    {
        _shader = shader;
    }

    public Ray LastRay { get; private set; }
    public RayHit LastHit { get; private set; }

    public void Load()
    {
        // InitRayMesh();
    }


    public void Dispose()
    {
        if (!_meshInitialized) return;

        _rayMesh.Dispose();
        _rayHitPointsMesh.Dispose();
    }

    public void Render(RenderContext renderContext)
    {
        if (!_meshInitialized) return;

        _shader.Use();
        var viewProjection = renderContext.ViewMatrix * renderContext.ProjectionMatrix3D;
        _shader.SetMatrix4("uViewProjection", viewProjection);
        _rayMesh.Render();
        _rayHitPointsMesh.Render();
    }

    private void InitRayMesh()
    {
        _meshInitialized = true;
        _rayMesh = new LineMesh();
        _rayMesh.Load(BufferUsageHint.DynamicDraw);
        _rayHitPointsMesh = new RayHitPointsMesh();
        _rayHitPointsMesh.Load(BufferUsageHint.DynamicDraw);
    }

    public RayHit Shoot(Ray ray, IRaycastable target)
    {
        var rayHit = target.Raycast(ray);
        LastRay = ray;
        LastHit = rayHit;
        return rayHit;
    }

    public void Trace()
    {
        if (!_meshInitialized) return;
        Console.WriteLine(LastRay);
        Console.WriteLine(LastHit);

        _rayMesh.UpdateRay(LastRay, LastHit);
        _rayHitPointsMesh.UpdateHitPoints(LastRay, LastHit);
    }
}