using Godot;
using System;

public partial class Bullet : AnimatableBody3D
{
	[Export]
	public float ProjectileSpeed = 5f;
	[Export]
	public Area3D TrackingArea;
	[Export]
	public float HomingScaling = 1f;

	[Export]
	public bool IsHoming = true;

	public Player MyPlayer;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{

	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public virtual void Fired(Gun gun)// triggered when gun fires this bullet
	{

	}
	Node3D closestTarget()
	{
		Node3D target = null;
		float closestDistance = -1;
		foreach (Node3D t in TrackingArea.GetOverlappingAreas())
		{
			if (target == null || GlobalPosition.DistanceTo(t.GlobalPosition) < closestDistance)
			{
				target = t;
				closestDistance = GlobalPosition.DistanceTo(t.GlobalPosition);
			}
		}
		//GD.Print(target);
		return target;
	}
	void homeTowards(Node3D Target)
	{
		if (IsHoming)
			GlobalBasis = Basis.LookingAt(Target.GlobalPosition - GlobalPosition);
	}
	public override void _PhysicsProcess(double delta)
	{
		TrackingArea.Scale += Vector3.One * HomingScaling * (float)delta * BulletTime.TimeScale;
		Node3D t = closestTarget();
		if (t != null)
			homeTowards(t);

		Vector3 motion = (float)delta * BulletTime.TimeScale * -GlobalBasis[2] * ProjectileSpeed;
		KinematicCollision3D col = MoveAndCollide(motion);
		//GD.Print(motion);
		if (col != null)
		{
			//GD.Print(col.GetCollider());
			OnCollision();
		}
	}

	public virtual void OnCollision()
	{
		QueueFree();
	}
}
