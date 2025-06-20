using Godot;
using System;

public partial class OnTriggerTurningBullet : Bullet
{
	[Export]
	public Vector3 Axis;
    [Export]
    public float AngleDegrees;
	[Export]
	public TrajectoryIndicator TrajectoryIndicator;
    [Export]
    public bool Oneshot = true;
    bool hasTurned = false;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{
        if (TrajectoryIndicator == null)
        {
            GD.PrintErr($"OnTriggerTurningBullet {Name}: TrajectoryIndicator is not set. Please set it in the inspector.");
        }
	}

    public override string GetBulletType()
    {
        return "Turny";
    }

	// Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {

    }
    public override void Fired(Gun gun)
    {
        base.Fired(gun);
		gun.Connect(Gun.SignalName.SuccessfulFire, Callable.From(Turn));
        TrajectoryIndicator.GlobalBasis = GlobalBasis.Rotated(Axis.Normalized(), (float)(AngleDegrees / 180 * Math.PI));
    }
	void Turn()
	{
        if (Oneshot && hasTurned)
            return;
        //GD.Print(GlobalRotation);
        GlobalBasis = GlobalBasis.Rotated(Axis.Normalized(), (float)(AngleDegrees / 180 * Math.PI));
        TrajectoryIndicator.GlobalBasis = GlobalBasis.Rotated(Axis.Normalized(), (float)(AngleDegrees / 180 * Math.PI));
        //GD.Print(GlobalRotation);
        if (Oneshot)
        {
            hasTurned = true;
            TrajectoryIndicator.ProcessMode = ProcessModeEnum.Disabled;
            TrajectoryIndicator.Visible = false;
        }
    }
}
