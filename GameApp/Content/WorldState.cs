namespace GameApp.Content;

public class WorldState
{
    public Player Player { get; set; } = new Player();
    
    public WorldState() { }
    public WorldState(Player player)
    {
        Player = player;
    }
    
    public static explicit operator WorldStateDto(WorldState obj)
    {
        return new WorldStateDto()
        {
            Player = (PlayerDto)obj.Player
        };
    }
}

public record WorldStateDto
{
    public PlayerDto Player { get; set; }
    public static explicit operator WorldState(WorldStateDto dto)
    {
        return new WorldState()
        {
            Player = (Player)dto.Player
        };
    }
}