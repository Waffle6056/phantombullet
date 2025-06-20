using Godot;
using System;

public partial class Target : Area3D
{

	[Signal]
	public delegate void TargetTriggeredEventHandler();

    public bool Hit = false;

	[Export]
	public Node3D HitIndicator;

	[Export]
	public bool IsWatching = false;

	[Export]
	public float AreaScale = 3.0f;

	public Area3D WatchedArea;
	public CollisionShape3D WatchedAreaShape;
	public MeshInstance3D WatchedAreaDecal;


	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		if (HitIndicator == null)
		{
			GD.PrintErr($"Target {Name}: HitIndicator is not set. Please set it in the inspector.");
		}

		WatchedArea = GetNodeOrNull<Area3D>("WatchArea");
		if (WatchedArea == null)
		{
			GD.PrintErr($"Target {Name}: WatchedArea is unavailable. Nonessential.");
		}
		else
		{
			WatchedAreaShape = WatchedArea.GetNodeOrNull<CollisionShape3D>("Detector");
			if (WatchedAreaShape == null)
			{
				GD.PrintErr($"Target {Name}: WatchedAreaShape is unavailable. Area will not watch.");
			}
			else
			{
				WatchedAreaShape.Scale = new Vector3(AreaScale, AreaScale, AreaScale);
			}

			WatchedAreaDecal = WatchedArea.GetNodeOrNull<MeshInstance3D>("MeshInstance");
			if (WatchedAreaDecal == null)
			{
				GD.PrintErr($"Target {Name}: WatchedAreaDecal is unavailable. Area will not be visible.");
			}
			else
			{
				WatchedAreaDecal.Scale = new Vector3(AreaScale, AreaScale, AreaScale);
			}
		}
	}

	public void WatchAreaEntered(Node3D body)
	{
		if (IsWatching && (body is Player || body.IsInGroup("bullet")))
		{
			// TODO: take action after a 3sec delay, during which the target can be shot to cancel
			GD.Print($"Target {Name}: triggered by {body.Name}.");
		}
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (HitIndicator != null)
		{
			HitIndicator.Visible = Hit;
		}

		if (WatchedArea != null)
		{
			WatchedArea.Visible = IsWatching;
		}
	}

	public virtual void HitTarget()
	{
		// signifies when the target is hit
		Hit = true;
		IsWatching = false;
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
