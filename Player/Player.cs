using Godot;
using System;

public partial class Player : CharacterBody3D
{
	[Export]
	public Gun Gun;

	[Export]
	public float Speed = 5.0f;
    [Export]
    public float JumpVelocity = 4.5f;
	public static Player Instance;
	public override void _Ready()
	{
		base._Ready();
		Instance = this;
	}

	public override void _PhysicsProcess(double delta)//this is the builtin template btw
	{
		Vector3 velocity = Velocity;

		// Add the gravity.
		if (!IsOnFloor())
		{
			velocity += GetGravity() * (float)delta * BulletTime.TimeScale;
		}

		// Handle Jump.
		if (Input.IsActionJustPressed("Jump") && IsOnFloor())
		{
			velocity.Y = JumpVelocity;
		}

		// Get the input direction and handle the movement/deceleration.
		// As good practice, you should replace UI actions with custom gameplay actions.
		Vector2 inputDir = Input.GetVector("Left", "Right", "Up", "Down");
		Vector3 direction = (Transform.Basis * new Vector3(inputDir.X, 0, inputDir.Y)).Normalized();
		if (direction != Vector3.Zero)
		{
			velocity.X = direction.X * Speed;
			velocity.Z = direction.Z * Speed;
		}
		else
		{
			velocity.X = Mathf.MoveToward(Velocity.X, 0, Speed);
			velocity.Z = Mathf.MoveToward(Velocity.Z, 0, Speed);
		}
		//GD.Print(velocity);
		Velocity = velocity;
		Velocity *= BulletTime.TimeScale;
		MoveAndSlide();
        Velocity /= BulletTime.TimeScale;
    }
}
