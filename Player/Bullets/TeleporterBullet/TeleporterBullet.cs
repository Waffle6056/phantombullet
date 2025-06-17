using Godot;
using System;

public partial class TeleporterBullet : Bullet
{
    public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		
	}
    public override void Fired(Gun gun)
    {
        base.Fired(gun);
		gun.Connect(Gun.SignalName.SuccessfulFire, Callable.From(Teleport));
    }

    public override void OnCollision()
    {
		Teleport();
    }

	void Teleport()
	{
		GD.Print("Teleporting from " + MyPlayer.GlobalPosition + " to " + GlobalPosition);
		MyPlayer.GlobalPosition = GlobalPosition;
		MyPlayer.Velocity = Vector3.Zero;

		QueueFree();
	}
}
