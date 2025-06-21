using Godot;
using System;
using System.Linq;
using System.Xml.XPath;

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

	[Export]
	public Node3D[] TheseArePartOfMe = [];

	private Rid[] excludedRids = [];

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

		excludedRids.Append(GetRid());
		foreach (var part in TheseArePartOfMe)
		{
			if (part is CollisionObject3D collisionObject)
			{
				excludedRids.Append(collisionObject.GetRid());
			}
		}
	}

	public void WatchAreaEntered(Node3D body)
	{
		if (IsWatching && (body is Player || body.IsInGroup("bullet")))
		{
			// make a raycast to the body to check if line-of sight (uninterrupted)
			var spaceState = GetWorld3D().DirectSpaceState;
			var query = PhysicsRayQueryParameters3D.Create(GlobalPosition, body.GlobalPosition);
			query.Exclude = [.. excludedRids, (body as CollisionObject3D).GetRid()];
			var result = spaceState.IntersectRay(query);
			// if result is empty, line of sight is clear

			if (result.Count == 0)
			{
				// TODO: take action after a 3sec delay, during which the target can be shot to cancel
				GD.Print($"Target {Name}: I SEE {body.Name}.");
			}
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
			if (body is OnTriggerOrCollisionBoomBullet)
				handleBoom((OnTriggerOrCollisionBoomBullet)body);
			else
				HitTarget();
			(body as Bullet).OnCollision();
		}
	}

	private void handleBoom(OnTriggerOrCollisionBoomBullet bullet)
	{
		// check if line of sight, and only then mark as hit
		var spaceState = GetWorld3D().DirectSpaceState;
		var query = PhysicsRayQueryParameters3D.Create(GlobalPosition, bullet.GlobalPosition);
		query.Exclude = [.. excludedRids, bullet.GetRid()];
		var result = spaceState.IntersectRay(query);
		if (result.Count == 0)
		{
			HitTarget();
		} else
		{
			GD.Print($"Target {Name}: Boom bullet {bullet.Name} hit, but line of sight is blocked.");
			GD.Print(result);
		}
	}
}
