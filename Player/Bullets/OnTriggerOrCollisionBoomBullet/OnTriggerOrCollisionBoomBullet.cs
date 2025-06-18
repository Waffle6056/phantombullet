using Godot;
using System;
using System.Threading.Tasks;

public partial class OnTriggerOrCollisionBoomBullet : Bullet
{
    public bool HasExploded = false;
    [Export]
    public AnimationPlayer AnimationPlayer;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
        if (HasExploded)
            AnimationPlayer.SpeedScale = BulletTime.TimeScale;
	}

    public override void Fired(Gun gun)
    {
        base.Fired(gun);
        gun.Connect(Gun.SignalName.SuccessfulFire, Callable.From(OnCollision));
    }
    public override void OnCollision()
    {
        if (HasExploded)
            return;
        HasExploded = true;
        AnimationPlayer.Play("Explode");
    }
    public void Destroy()
    {
        QueueFree();
    }
}
