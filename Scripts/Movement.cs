using Godot;
using System;

public partial class Movement : CharacterBody3D
{
	[ExportCategory("Movement")]
	[Export] public float speed; //standart speed
	[Export] public float sprintingSpeed; //speed in sprinting state
	[Export] public float jumpVelocity; //instant force added when jumping
	[Export] public float gravity; //gravity
	[Export] public float acceleration; 
	[Export] public float decceleration;

	[ExportCategory("Player parts")]
	[Export] public Node3D head; //top part of player, contains camera
	[Export] public Camera3D camera;

	[ExportCategory("Settings")]
	[Export] public float mouseSensivity;
	[Export] public float fov;

	private float currentSpeed;

    public override void _Ready()
    {
        Input.MouseMode = Input.MouseModeEnum.Captured;
    }

    public override void _Input(InputEvent @event)
    {
		//handle FPS camera
        if (@event is InputEventMouseMotion eventMouseMotion)
		{
			RotateY(eventMouseMotion.Relative.X * mouseSensivity * -0.01f);
			head.RotateX(eventMouseMotion.Relative.Y * mouseSensivity * -0.01f);
			
			Vector3 newRotation = head.Rotation;
			newRotation.X = Mathf.Clamp(head.Rotation.X, Mathf.DegToRad(-89), Mathf.DegToRad(89));
			head.Rotation = newRotation;
		}

		//start sprinting
		if (@event.IsActionPressed("sprint"))
		{
			currentSpeed = sprintingSpeed;
		}
    }

	public override void _PhysicsProcess(double delta)
	{
		Vector3 velocity = Velocity;

		//handle gravity
		if (!IsOnFloor())
			velocity.Y -= gravity * (float)delta;

		if (Input.IsActionJustPressed("jump") && IsOnFloor())
			velocity.Y = jumpVelocity;

		//take and modify an input for movement
		Vector2 inputDir = Input.GetVector("left", "right", "forward", "backward");
		Vector3 direction = (Transform.Basis * new Vector3(inputDir.X, 0, inputDir.Y)).Normalized();

		//handle movement
		if (direction != Vector3.Zero)
		{
			velocity.X = Mathf.MoveToward(Velocity.X, direction.X * currentSpeed, acceleration);
			velocity.Z = Mathf.MoveToward(Velocity.Z, direction.Z * currentSpeed, acceleration);
			if (currentSpeed == sprintingSpeed && camera.Fov < fov + 8) camera.Fov = Mathf.MoveToward(camera.Fov, fov + 8, 1);
			velocity.X = Mathf.MoveToward(Velocity.X, direction.X * speed, acceleration);
			velocity.Z = Mathf.MoveToward(Velocity.Z, direction.Z * speed, acceleration);
		}
		else
		{
			velocity.X = Mathf.MoveToward(Velocity.X, 0, decceleration);
			velocity.Z = Mathf.MoveToward(Velocity.Z, 0, decceleration);

			currentSpeed = speed;
			if (camera.Fov > fov) camera.Fov = Mathf.MoveToward(camera.Fov, fov - 8, 1);
		}

		//apply velocity
		Velocity = velocity;
		MoveAndSlide();
	}
}