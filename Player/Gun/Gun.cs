using Godot;
using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;

public partial class Gun : Node3D
{
	[Signal]
	public delegate void SuccessfulFireEventHandler();

	[Export]
	public Loader PreloadedLoader;

	[Export]
	public Node Clip;

	[Export]
	public Player Bearer;

	List<Bullet> Bullets = new List<Bullet>();

	[Export]
	public float SwayAmount = 0.003f;

	[Export]
	public float SwaySmoothness = 16.0f;

	[Export]
	public RayCast3D Ray;

	private Vector2 LookDelta = Vector2.Zero;
	private Vector3 TargetOffset = Vector3.Zero;
	private Vector3 CurrentOffset = Vector3.Zero;

	public override void _Input(InputEvent @event)
	{
		if (@event is InputEventMouseMotion motion)
		{
			LookDelta = motion.Relative;
		}
	}


	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		// check if bearer is set
		if (Bearer == null)
		{
			GD.PrintErr("WARNING: Gun's 'Bearer' property is set to null.");
		}

		Load(PreloadedLoader);

		// RAYCAST SETUP
		// set rotation to 90, 0, 0 (x, y, z)
		Ray.CollideWithAreas = true;
		Ray.CollideWithBodies = true;
		// Ray.Enabled = false;
		Ray.Enabled = true;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		// Alter gun rotation to simulate sway
		TargetOffset = new Vector3(LookDelta.Y * SwayAmount, LookDelta.X * SwayAmount, 0.0f);
		CurrentOffset = CurrentOffset.Lerp(TargetOffset, (float)delta * SwaySmoothness);
		Rotation = CurrentOffset;

		if (Input.IsActionJustPressed("Fire") && Bullets.Count > 0)
		{
			Fire(Bullets[0]);
		}
	}
	public void Fire(Bullet bullet)
    {
		if (!GetNode<AnimationPlayer>("AnimationPlayer").IsPlaying())
		{
			EmitSignal(SignalName.SuccessfulFire);
			bullet.ProcessMode = ProcessModeEnum.Inherit;
			bullet.GlobalPosition = GetNode<Node3D>("BulletEmitter").GlobalPosition;
			bullet.GlobalRotation = GetNode<Node3D>("BulletEmitter").GlobalRotation;
			GetNode<AnimationPlayer>("AnimationPlayer").Play("Fire");
			//GD.Print(bullet.GlobalTransform + " " + GlobalTransform);
			bullet.Visible = true;
			GD.Print("FIRED "+ (bullet is PhotonBullet ? "PhotonBullet" : "Bullet"));
			if (bullet is PhotonBullet)
			{
				GD.Print("Firing photon bullet...");
				// bullet.QueueFree();
				FirePhoton();
			}
			else
			{
				bullet.Fired(this);
			}
			// Bullets.RemoveAt(0);
		}
	}

	public void FirePhoton()
	{
		if (Ray == null)
		{
			GD.PrintErr("WARNING: Gun's 'Ray' property is set to null. Cannot fire photon bullet.");
			return;
		}

		Ray.Enabled = true; // enable raycast to detect collisions
		Ray.ClearExceptions(); // clear any previous exceptions

		// iterative cast until hit something that is not a bullet or item, or until it goes out of bounds (doesn't collide with anything)
		while (true)
		{
			Ray.ForceRaycastUpdate();

			if (!Ray.IsColliding())
			{
				GD.Print("Quitting bc raycast did not hit anything.");
				break; // no collider found, exit loop
			}

			var collider = Ray.GetCollider() as Node3D;
			if (collider == null) break;

			GD.Print("Raycast hit: " + collider.Name);

			if (collider.IsInGroup("target"))
			{
				GD.Print("Hit target.");
				(collider as Target).HitTarget();
				Ray.AddExceptionRid(Ray.GetColliderRid()); // ignore this collider for the next raycast
				continue;
			}
			else if (collider.IsInGroup("bullet") || collider.IsInGroup("item"))
			{
				GD.Print("Hit bullet or item.");
				Ray.AddExceptionRid(Ray.GetColliderRid()); // ignore this collider for the next raycast
				continue;
			}
			else
			{
				GD.Print("Quitting bc hit " + collider.Name);
				break;
			}

		}

		// Ray.Enabled = false; // disable raycast after firing
	}

	public void Load(Loader loader)
	{
		// right now, replace the current bullets with the new ones
		int ocount = Bullets.Count;

		// *copy* over the bullets
		Bullets = new List<Bullet>(loader.HeldBullets.Length);

		for (int i = 0; i < loader.HeldBullets.Length; i++)
		{
			var bullet = loader.HeldBullets[i];
			Bullet clonedBullet = (Bullet)bullet.Duplicate();

			Load(clonedBullet);
		}

		GD.Print("Loaded from loader. ∆" + (Bullets.Count - ocount) + ". (+" + Bullets.Count + ", -" + ocount + ")");
	}

    private void Load(Bullet bullet)
    {
		// set the owner
		bullet.MyPlayer = Bearer;
		
		// add to clip, to store 'local' references to the bullets
		Bullets.Add(bullet);
		Clip.AddChild(bullet);
    }
}
