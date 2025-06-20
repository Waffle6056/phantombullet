using Godot;
using System;

public partial class Target : Area3D
{
    public bool Hit = false;
	[Export]
	public Node3D HitIndicator;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		if (HitIndicator == null)
		{
			GD.PrintErr($"Target {Name}: HitIndicator is not set. Please set it in the inspector.");
			return;
		}
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (HitIndicator != null)
		{
			HitIndicator.Visible = Hit;
		}

	}

	public virtual void HitTarget()
	{
		// signifies when the target is hit
		Hit = true;
	}

	public void IsShot(Node3D body)
	{
		if (body.IsInGroup("bullet"))
		{
			HitTarget();
			(body as Bullet).OnCollision();
		}
	}
}
