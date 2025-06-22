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
		gun.Connect(Gun.SignalName.SuccessfulFire, Callable.From(OnCollision));
    }

    public override void OnCollision()
    {
		QueueFree();
		Teleport();
    }

	void Teleport()
	{
		//GD.Print("Teleporting from " + MyPlayer.GlobalPosition + " to " + GlobalPosition);
		MyPlayer.GlobalPosition = GlobalPosition;
		MyPlayer.Velocity = Vector3.Zero;
	}
}
