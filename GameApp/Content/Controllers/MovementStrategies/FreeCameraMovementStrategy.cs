using GameApp.Content.Systems;
using GameEngine.Core;
using OpenTK.Mathematics;

namespace GameApp.Content.Controllers.MovementStrategies;

public class FreeCameraMovementStrategy : FlightMovementStrategy
{
    public FreeCameraMovementStrategy(
        Camera camera,
        CharacterPhysics physics,
        float mouseSensitivity,
        float moveSpeed,
        float sprintMultiplier,
        float verticalSpeed
    )
        : base(camera, physics, mouseSensitivity, moveSpeed, sprintMultiplier, verticalSpeed)
    {
    }
    
    protected override void ApplyPhysics(double deltaTime, Vector3 velocity)
    {
        Camera.Position += velocity * (float)deltaTime;
        Physics.Teleport(Camera.Position);
    }
}