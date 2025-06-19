using Godot;
using System;

public partial class OnTriggerDeadshotBullet : Bullet
{
    [Export]
    public Vector3 DirectionVector;
    [Export]
    public TrajectoryIndicator TrajectoryIndicator;
    [Export]
    public bool Oneshot = true;
    bool hasTurned = false;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
    }

    public override string GetBulletType()
    {
        return "Deadshot";
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {

    }
    public override void Fired(Gun gun)
    {
        base.Fired(gun);
        gun.Connect(Gun.SignalName.SuccessfulFire, Callable.From(Turn));
        TrajectoryIndicator.GlobalBasis = Basis.LookingAt(DirectionVector);
    }
    void Turn()
    {
        if (Oneshot && hasTurned) 
            return;
        //GD.Print(GlobalRotation);
        GlobalBasis = Basis.LookingAt(DirectionVector);
        TrajectoryIndicator.ProcessMode = ProcessModeEnum.Disabled;
        //TrajectoryIndicator.GlobalBasis = GlobalBasis.Rotated(Axis.Normalized(), (float)(AngleDegrees / 180 * Math.PI));
        //GD.Print(GlobalRotation);
        if (Oneshot)
        {
            hasTurned = true;
            TrajectoryIndicator.ProcessMode = ProcessModeEnum.Disabled;
            TrajectoryIndicator.Visible = false;
        }
    }
}
