using OpenTK.Mathematics;

namespace GameApp.Content;

public class Player
{
    public Vector3 Position { get; set; } = new(0, 0, 0);
    public Vector3 ViewDirection { get; set; } = new(0, 0, 0);
    public Matrix4 ViewMatrix { get; set; }
    public int ChunkViewRadius { get; set; } = 5;
    public int ChunkViewHeightRadius { get; set; } = 5;

    public static explicit operator PlayerDto(Player obj)
    {
        return new PlayerDto
        {
            Position = (System.Numerics.Vector3)obj.Position,
            ViewDirection = (System.Numerics.Vector3)obj.ViewDirection,
            ChunkViewRadius = obj.ChunkViewRadius,
            ChunkViewHeightRadius = obj.ChunkViewHeightRadius
        };
    }
}

public record PlayerDto
{
    public System.Numerics.Vector3 Position { get; set; }
    public System.Numerics.Vector3 ViewDirection { get; set; }
    public int ChunkViewRadius { get; set; }
    public int ChunkViewHeightRadius { get; set; }


    public static explicit operator Player(PlayerDto dto)
    {
        return new Player
        {
            Position = (Vector3)dto.Position,
            ViewDirection = (Vector3)dto.ViewDirection,
            ChunkViewRadius = dto.ChunkViewRadius,
            ChunkViewHeightRadius = dto.ChunkViewHeightRadius
        };
    }
}