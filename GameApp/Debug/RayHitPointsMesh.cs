using LearningOpenTK.Engine.Meshes;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using VoxelWorldEngine.Core.Raycasting;

namespace GameApp.Debug;

public class RayHitPointsMesh : Mesh<LineVertex, uint>
{
    public RayHitPointsMesh() : base(new LineVertex[4], new uint[] { 0, 1, 2, 3 })
    {
    }

    public void UpdateHitPoints(Ray ray, RayHit hit)
    {
        var endPoint = ray.Origin + ray.Direction * ray.Length;


        var newVerts = new LineVertex[]
        {
            new((Vector3)(ray.Origin), new Vector3(1f, 1f, 0f)), // жовта ● HitIn
            new((Vector3)(hit.HitIn), new Vector3(0f, 1f, 0f)), // зелена ● HitIn
            new((Vector3)(hit.HitOut), new Vector3(1f, 0f, 0f)), // червона  ● HitOut
            new((Vector3)(endPoint),     new Vector3(1f, 0f, 1f)), // рожева end
        };

        UpdateVertices(newVerts);
    }

    protected override void DefineAttributePointers()
    {
        int stride = SizeOfTVertex; // sizeof(LineVertex)

        // location = 0 → Position
        GL.EnableVertexAttribArray(0);
        GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, stride, 0);

        // location = 1 → Color
        GL.EnableVertexAttribArray(1);
        GL.VertexAttribPointer(1, 3, VertexAttribPointerType.Float, false, stride, 3 * sizeof(float));
    }


    public new void Render()
    {
        if (Vao == 0) return;

        GL.PointSize(8f); // розмір точок
        GL.BindVertexArray(Vao);
        GL.DrawElements(PrimitiveType.Points, Indices.Length, DrawElementsType.UnsignedInt, 0);
    }
}