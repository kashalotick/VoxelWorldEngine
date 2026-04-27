using GameApp.Content.Systems;
using LearningOpenTK.Core;
using OpenTK.Mathematics;

namespace GameApp.Content.Controllers.MovementStrategies;

public class FreeCameraBaseMovementStrategy : FlightBaseMovementStrategy
{
    public FreeCameraBaseMovementStrategy(
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

    public override MovementMode Mode => MovementMode.FreeCamera;

    protected override void ApplyPhysics(double deltaTime, Vector3 velocity)
    {
        Camera.Position += velocity * (float)deltaTime;
        Physics.Teleport(Camera.Position);
    }
}