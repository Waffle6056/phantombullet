using Godot;
using System;

public partial class TimedTarget : Target
{
	[Export]
	public float HitDuration = 2f;
    public double HitDurationRemaining = 0;
    // Called when the node enters the scene tree for the first time.


	// Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _PhysicsProcess(double delta)
    {
		base._PhysicsProcess(delta); 
		if (HitDurationRemaining <= 0)
			Hit = false;
		else
			HitDurationRemaining -= delta * BulletTime.TimeScale;
		//GD.Print(HitDurationRemaining+" "+Hit);
    }

    public override void HitTarget()
	{
		base.HitTarget();
		HitDurationRemaining = HitDuration;
	}
}
