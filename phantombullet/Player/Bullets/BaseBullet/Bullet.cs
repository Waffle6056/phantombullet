using Godot;
using System;

public partial class Bullet : AnimatableBody3D
{
	[Export]
	public float ProjectileSpeed = 5f;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

    public override void _PhysicsProcess(double delta)
    {
		Vector3 motion = (float)delta * -GlobalBasis[2] * ProjectileSpeed;
		KinematicCollision3D col = MoveAndCollide(motion);
		//GD.Print(motion);
		if (col != null)
		{
			GD.Print(col.GetCollider());
			QueueFree();
		}
    }
}
