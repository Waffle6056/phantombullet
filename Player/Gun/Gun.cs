using Godot;
using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;

public partial class Gun : Node3D
{
	[Signal]
	public delegate void SuccessfulFireEventHandler();

	[Export]
	public Loader PreloadedLoader;

	[Export]
	public Node Clip;

	List<Bullet> Bullets = new List<Bullet>();
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Load(PreloadedLoader);
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
		// right now, replace the current bullets with the new ones
		int ocount = Bullets.Count;

		// *copy* over the bullets
		Bullets = new List<Bullet>(loader.HeldBullets.Length);

		for (int i = 0; i < loader.HeldBullets.Length; i++)
		{
			var bullet = loader.HeldBullets[i];
			Bullet clonedBullet = (Bullet)bullet.Duplicate();

			// add to clip, to store 'local' references to the bullets
			Bullets.Add(clonedBullet);
			Clip.AddChild(clonedBullet);
		}

		GD.Print("Loaded from loader. ∆" + (Bullets.Count - ocount) + ". (+" + Bullets.Count + ", -" + ocount + ")");
	}
    public void Load(Bullet bullet)
    {
        Bullets.Add(bullet);
    }
}
