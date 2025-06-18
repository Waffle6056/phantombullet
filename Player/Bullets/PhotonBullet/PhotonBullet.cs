using Godot;
using System;

public partial class PhotonBullet : Bullet
{
    [Export]
    public RayCast3D Ray;

    [Export]
    public AnimationPlayer photonAnimator;

    public override string GetBulletType()
    {
		return "Photon";
    }

    // Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		// RAYCAST SETUP
		// set rotation to 90, 0, 0 (x, y, z)
		Ray.CollideWithAreas = true;
		Ray.CollideWithBodies = true;
		Ray.Enabled = false;
	}
    public override void Fired(Gun gun)
    {
        base.Fired(gun);
        GD.Print("Firing photon bullet...");
        photonAnimator.Play("Fire");
        FirePhoton();
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

        Ray.Enabled = false; // disable raycast after firing
    }
    public void Destroy()
    {
        QueueFree();
    }
    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
	{
        photonAnimator.SpeedScale = BulletTime.TimeScale;
	}
}
