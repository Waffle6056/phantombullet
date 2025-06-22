using Godot;
using System;

public partial class TeleporterBullet : Bullet
{
    public override void _Ready()
	{
	}

	public override string GetBulletType()
	{
		return "Teleporter";
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{

	}
    public override void Fired(Gun gun)
    {
        base.Fired(gun);
		gun.Connect(Gun.SignalName.SuccessfulFire, Callable.From(OnFire_Teleport));
    }

	public override void OnCollision()
	{
	// QueueFree();
	// Teleport();
	// don't need bc we have special stuff
	}

	public void OnFire_Teleport()
	{
		QueueFree();
		Teleport(GlobalPosition);
	}

	public override void OnCollision_Teleport(KinematicCollision3D col)
	{
		// find surface normal, offset by a bit, and teleport there
		QueueFree();

		if (col == null)
			return;

		Vector3 targetPosition = col.GetPosition() + col.GetNormal() * 1.1f;

		Teleport(targetPosition);
	}

	void Teleport(Vector3 targetPosition)
	{
		//GD.Print("Teleporting from " + MyPlayer.GlobalPosition + " to " + GlobalPosition);
		MyPlayer.GlobalPosition = targetPosition;

		// velocity not conserved
		MyPlayer.Velocity = Vector3.Zero;
	}
}
