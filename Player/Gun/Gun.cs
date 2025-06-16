using Godot;
using System;
using System.Collections.Generic;

public partial class Gun : Node3D
{
	[Signal]
	public delegate void SuccessfulFireEventHandler();
	[Export]
	public Loader preloadedLoader;
	List<Bullet> Bullets = new List<Bullet>();
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Load(preloadedLoader);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (Input.IsActionJustPressed("Fire") && Bullets.Count > 0)
		{
			Fire(Bullets[0]);
			Bullets.RemoveAt(0);
		}
	}
	public void Fire(Bullet bullet)
    {
        EmitSignal(SignalName.SuccessfulFire);
        bullet.ProcessMode = ProcessModeEnum.Inherit;	
        bullet.GlobalPosition = GlobalPosition;
        bullet.GlobalRotation = GlobalRotation;
        //GD.Print(bullet.GlobalTransform + " " + GlobalTransform);
		bullet.Visible = true;
		//GD.Print("FIRED "+bullet);
		bullet.Fired(this);
	}
	public void Load(Loader loader)
	{
		Bullets = new List<Bullet>(loader.HeldBullets);
	}
    public void Load(Bullet bullet)
    {
        Bullets.Add(bullet);
    }
}
