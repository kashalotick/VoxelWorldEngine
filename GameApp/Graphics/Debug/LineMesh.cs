using LearningOpenTK.Engine.Meshes;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using VoxelWorldEngine.Core.Raycasting;

namespace GameApp.Graphics.Debug;

public class LineMesh : Mesh<LineVertex, uint>
{
    public LineMesh() : base(
        new LineVertex[2], // placeholder — оновимо через UpdateRay
        new uint[] { 0, 1 } // індекси двох точок
    )
    {
    }

    /// <summary>
    ///     Оновлює позиції вершин за даними Ray.
    ///     Якщо є RayHit — підсвічує точку потрапляння.
    /// </summary>
    public void UpdateRay(Ray ray, RayHit? hit = null)
    {
        var endPoint = ray.Origin + ray.Direction * ray.Length;

        if (hit.HasValue && hit.Value.IsHit)
        {
            var h = hit.Value;

            // 4 точки: Origin, HitIn, HitOut, End
            var newVerts = new LineVertex[]
            {
                new((Vector3)ray.Origin, new Vector3(1f, 1f, 0f)), // жовтий
                new((Vector3)h.HitIn, new Vector3(1f, 1f, 0f)), // жовтий end
                new((Vector3)h.HitIn, new Vector3(0f, 1f, 0f)), // зелений
                new((Vector3)h.HitOut, new Vector3(0f, 1f, 0f)), // зелений end
                new((Vector3)h.HitOut, new Vector3(1f, 0f, 0f)), // червоний
                new((Vector3)endPoint, new Vector3(1f, 0f, 0f)) // червоний end
            };

            // 3 лінії: [0→1], [2→3], [4→5]
            var newIndices = new uint[] { 0, 1, 2, 3, 4, 5 };

            UpdateVertices(newVerts);
            UpdateIndices(newIndices);
        }
        else
        {
            // Немає попадання — просто суцільна жовта лінія
            var newVerts = new LineVertex[]
            {
                new((Vector3)ray.Origin, new Vector3(1f, 1f, 0f)),
                new((Vector3)endPoint, new Vector3(1f, 1f, 0f))
            };

            UpdateVertices(newVerts);
            UpdateIndices(new uint[] { 0, 1 });
        }
    }

    protected override void DefineAttributePointers()
    {
        var stride = SizeOfTVertex; // sizeof(LineVertex)

        // location = 0 → Position
        GL.EnableVertexAttribArray(0);
        GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, stride, 0);

        // location = 1 → Color
        GL.EnableVertexAttribArray(1);
        GL.VertexAttribPointer(1, 3, VertexAttribPointerType.Float, false, stride, 3 * sizeof(float));
    }

    // Перевизначаємо Render — промінь це ЛІНІЯ, не трикутники
    public override void Render()
    {
        if (Vao == 0) return;

        GL.BindVertexArray(Vao);
        GL.DrawElements(PrimitiveType.Lines, Indices.Length, DrawElementsType.UnsignedInt, 0);
    }
}