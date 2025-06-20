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
        if (AnimationPlayer == null)
        {
            GD.PrintErr($"OnTriggerOrCollisionBoomBullet {Name}: AnimationPlayer is not set. Please set it in the inspector.");
        }
	}

    public override string GetBulletType()
    {
        return "Boom";
    }

    // Called when the node enters the scene tree for the first time.
    public override void _EnterTree()
    {
        base._EnterTree();
        AnimationPlayer.Connect(AnimationPlayer.SignalName.AnimationFinished, Callable.From(Destroy));
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
