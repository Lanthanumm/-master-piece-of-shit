using Godot;
using System;

public partial class Movement : CharacterBody3D
{
	[ExportCategory("Movement")]
	[Export] public float speed;
	[Export] public float jumpVelocity;
	[Export] public float gravity;
	[Export] public float acceleration;
	[Export] public float decceleration;

	[ExportCategory("Player parts")]
	[Export] public Node3D head;

	[ExportCategory("Settings")]
	[Export] public float mouseSensivity;

    public override void _Ready()
    {
        Input.MouseMode = Input.MouseModeEnum.Captured;
    }

    public override void _Input(InputEvent @event)
    {
        if (@event is InputEventMouseMotion eventMouseMotion)
		{
			RotateY(eventMouseMotion.Relative.X * mouseSensivity * -0.01f);
			head.RotateX(eventMouseMotion.Relative.Y * mouseSensivity * -0.01f);
			
			Vector3 newRotation = head.Rotation;
			newRotation.X = Mathf.Clamp(head.Rotation.X, Mathf.DegToRad(-89), Mathf.DegToRad(89));
			head.Rotation = newRotation;
		}
	}

	public override void _PhysicsProcess(double delta)
	{
		Vector3 velocity = Velocity;

		if (!IsOnFloor())
			velocity.Y -= gravity * (float)delta;

		if (Input.IsActionJustPressed("jump") && IsOnFloor())
			velocity.Y = jumpVelocity;

		Vector2 inputDir = Input.GetVector("left", "right", "forward", "backward");
		Vector3 direction = (Transform.Basis * new Vector3(inputDir.X, 0, inputDir.Y)).Normalized();
		if (direction != Vector3.Zero)
		{
			velocity.X = Mathf.MoveToward(Velocity.X, direction.X * speed, acceleration);
			velocity.Z = Mathf.MoveToward(Velocity.Z, direction.Z * speed, acceleration);
		}
		else
		{
			velocity.X = Mathf.MoveToward(Velocity.X, 0, decceleration);
			velocity.Z = Mathf.MoveToward(Velocity.Z, 0, decceleration);
		}

		Velocity = velocity;
		MoveAndSlide();
	}
}
