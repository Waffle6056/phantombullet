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
	public bool IsWatching = false;

	[Export]
	public float AreaScale = 3.0f;

	public Area3D WatchedArea;
	public CollisionShape3D WatchedAreaShape;
	public MeshInstance3D WatchedAreaDecal;
	[Export]
	public OmniLight3D Light;

	[Export]
	public Node3D[] TheseArePartOfMe = [];

	private Rid[] excludedRids = [];

	[Export]
	public Lever lever;
	[Export]
	public float MaxTimeTilAngry = 1.0f;
    public double CurrentTimeTilAngry = 1.0f;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{
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

		lever.Visible = !IsWatching;
	}

	// public void WatchAreaEntered(Node3D body)
	// {
	// 	if (!Hit && IsWatching && (body is Player || body.IsInGroup("bullet")))
	// 	{
	// 		// make a raycast to the body to check if line-of sight (uninterrupted)
	// 		var spaceState = GetWorld3D().DirectSpaceState;
	// 		var query = PhysicsRayQueryParameters3D.Create(GlobalPosition, body.GlobalPosition, CollisionMask);
	// 		query.Exclude = [.. excludedRids, (body as CollisionObject3D).GetRid()];
	// 		var result = spaceState.IntersectRay(query);
	// 		// if result is empty, line of sight is clear

	// 		if (result.Count == 0)
	// 		{
	// 			// TODO: take action after a 3sec delay, during which the target can be shot to cancel
	// 			// GD.Print($"Target {Name}: I SEE {body.Name}.");
	// 		}
	// 	}
	// }

    // public void CheckWatchArea()
    // {
	// 	if (!WatchedArea.HasOverlappingBodies())
	// 		return;
	// 	foreach (Node3D body in WatchedArea.GetOverlappingBodies())
	// 		WatchAreaEntered(body);
    // }

    public override void _PhysicsProcess(double delta)
	{
		Monitoring = !Hit;
		//GD.Print(WatchedArea.OverlapsBody(Player.Instance) + " " + CurrentTimeTilAngry);
		if (IsWatching && !Hit && WatchedArea != null && WatchedArea.OverlapsBody(Player.Instance) && IsLineOfSight(Player.Instance))
		{
			Light.LightColor = new Color(1, 0, 0);
			CurrentTimeTilAngry -= delta * BulletTime.TimeScale;
			if (CurrentTimeTilAngry <= 0)
				Player.Instance.Dead = true;
		}
		else
		{
			Light.LightColor = new Color(1, 1, 0);
			CurrentTimeTilAngry = MaxTimeTilAngry;
		}
	}

	public bool IsLineOfSight(Node3D target)
	{
		var spaceState = GetWorld3D().DirectSpaceState;
		var query = PhysicsRayQueryParameters3D.Create(GlobalPosition, target.GlobalPosition);
		query.Exclude = [GetRid(), (target as CollisionObject3D).GetRid()];
		var result = spaceState.IntersectRay(query);
		return result.Count == 0;
	}

    // Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (WatchedArea != null)
		{
			WatchedArea.Visible = !Hit && IsWatching;
			WatchedAreaShape.Scale = new Vector3(AreaScale, AreaScale, AreaScale);
			WatchedAreaDecal.Scale = new Vector3(AreaScale, AreaScale, AreaScale);
			Light.OmniRange = AreaScale;
			// CheckWatchArea();
		}
		if (Monitoring)
			foreach (Node3D body in GetOverlappingBodies())
				IsShot(body);

	}

	public virtual void HitTarget()
	{
		// signifies when the target is hit
		Hit = true;
		//IsWatching = false;
		lever.Activate();
	}

	public void IsShot(Node3D body)
    {
		if (body is Bullet || body is Player)
			HitTarget();
		//GD.Print("SHOT");
		if (body is OnTriggerOrCollisionBoomBullet)
		{
			handleBoom(body as OnTriggerOrCollisionBoomBullet);
		}
		else if (body is Bullet)
		{
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
			bullet.OnCollision();
		}
	}
}
