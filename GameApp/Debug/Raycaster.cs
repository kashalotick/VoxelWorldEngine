using LearningOpenTK.Core.DTO;
using LearningOpenTK.Core.Primitives;
using LearningOpenTK.Resources.Interfaces;
using OpenTK.Graphics.OpenGL4;
using VoxelWorldEngine.Core.Raycasting;
using VoxelWorldEngine.Utils;

namespace GameApp.Debug;

public class Raycaster : ILoadable, IRenderable
{
    private IShader _shader;

    public Ray LastRay { get; private set; }
    public RayHit LastHit { get; private set; }
    
    private bool _meshInitialized = false;
    private LineMesh _rayMesh;
    private RayHitPointsMesh _rayHitPointsMesh;

    public Raycaster(IShader shader)
    {
        _shader = shader;
    }

    public void Load()
    {
        // InitRayMesh();
    }

    private void InitRayMesh()
    {
        _meshInitialized = true;
        _rayMesh = new LineMesh();
        _rayMesh.Load(BufferUsageHint.DynamicDraw);
        _rayHitPointsMesh = new RayHitPointsMesh();
        _rayHitPointsMesh.Load(BufferUsageHint.DynamicDraw);
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

    public RayHit Shoot(Ray ray, IRaycastable target)
    {
        var rayHit = target.Raycast(ray);
        LastRay = ray;
        LastHit = rayHit;
        return rayHit;
    }

    public void Trace()
    {
        Console.WriteLine(LastRay);
        Console.WriteLine(LastHit);
        if (!_meshInitialized) return;

        _rayMesh.UpdateRay(LastRay, LastHit);
        _rayHitPointsMesh.UpdateHitPoints(LastRay, LastHit);
    }


    public void Dispose()
    {
        if (!_meshInitialized) return;

        _rayMesh.Dispose();
        _rayHitPointsMesh.Dispose();
    }
}