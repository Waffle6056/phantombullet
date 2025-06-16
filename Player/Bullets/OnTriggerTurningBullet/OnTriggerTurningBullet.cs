using Godot;
using System;

public partial class OnTriggerTurningBullet : Bullet
{
	[Export]
	public Vector3 Axis;
    [Export]
    public float AngleDegrees;

    // Called when the node enters the scene tree for the first time.
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
		gun.Connect(Gun.SignalName.SuccessfulFire, Callable.From(Turn));
    }
	void Turn()
	{
		GD.Print(GlobalRotation);
		GlobalBasis = GlobalBasis.Rotated(Axis.Normalized(), (float)(AngleDegrees / 180 * Math.PI));
        GD.Print(GlobalRotation);
    }
}
