using Godot;
using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;

public partial class Gun : Node3D
{
	[Signal]
	public delegate void SuccessfulFireEventHandler();

	[Export]
	public Player Bearer;


	public Bullet[] Bullets = new Bullet[6];
	public bool ChambersFilled {
		get {
			for (int i = 0; i < Bullets.Length; i++)
				if (Bullets[i] == null)
					return false;
			return true;
		} }

	[Export]
	public float SwayAmount = 0.003f;

	[Export]
	public float SwaySmoothness = 16.0f;

	private Vector2 LookDelta = Vector2.Zero;
	private Vector3 TargetOffset = Vector3.Zero;
	private Vector3 CurrentOffset = Vector3.Zero;
    [Export]
    public Vector3 BaseRotation = Vector3.Zero;

    private HashSet<BarrelCylinder> barrelCylinders = new HashSet<BarrelCylinder>();

	public override void _Input(InputEvent @event)
	{
		if (@event is InputEventMouseMotion motion)
		{
			LookDelta = motion.Relative;
		}

		if (Bearer == null)
		{
			GD.PrintErr("WARNING: Gun's 'Bearer' property is set to null.");
		}
	}

	public void RegisterBarrell(BarrelCylinder barrel)
	{
		barrelCylinders.Add(barrel);
	}

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		// check if bearer is set
		if (Bearer == null)
		{
			GD.PrintErr("WARNING: Gun's 'Bearer' property is set to null.");
		}
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{

		// Alter gun rotation to simulate sway
        TargetOffset = new Vector3(LookDelta.Y * SwayAmount, LookDelta.X * SwayAmount, 0.0f);
		CurrentOffset = CurrentOffset.Lerp(TargetOffset, (float)delta * SwaySmoothness);
		Rotation = BaseRotation + CurrentOffset;
		

		if (Input.IsActionJustPressed("Fire") && !Bearer.InventoryOpen)
			Fire();
		if (Bearer.InventoryOpen)
		{
			if (!Bearer.InventoryFocused && Input.IsActionJustPressed("Fire"))
            {
                for (int i = 0; i < Bullets.Length && Bullets[0] == null; i++)
                    RotateBarrelRight();
                if (Bearer.PickUp(Bullets[0]))
					Unload();
            }
        }
		if (!Bearer.InventoryOpen || !Bearer.InventoryFocused)
		{
            if (Input.IsActionJustReleased("ScrollUp"))
                RotateBarrelRight();
            if (Input.IsActionJustReleased("ScrollDown"))
                RotateBarrelLeft(); 
        }

	}
	private void RotateBarrelRight()
	{
		Bullet f = Bullets[0];
		for (int i = 1; i < Bullets.Length; i++)
			Bullets[i - 1] = Bullets[i];
		Bullets[Bullets.Length - 1] = f;
        foreach (BarrelCylinder b in barrelCylinders)
        {
            b.RotateRight();
            GD.Print(b.Name);
        }
    }
    private void RotateBarrelLeft()
    {
        Bullet f = Bullets[Bullets.Length-1];
        for (int i = Bullets.Length - 1; i >= 1; i--)
            Bullets[i] = Bullets[i - 1];
        Bullets[0] = f;
        foreach (BarrelCylinder b in barrelCylinders)
        {
            b.RotateLeft();
            GD.Print(b.Name);
        }
    }
    public void Fire()
	{
        for (int i = 0; i < Bullets.Length && Bullets[0] == null; i++)
            RotateBarrelRight();
        if (Bullets[0] != null)
        {
            GetTree().Root.AddChild(Bullets[0]);
            Fire(Bullets[0]);
            Unload(Bullets[0]);
		}
		RotateBarrelRight();

		foreach (var bullet in Bullets)
		{
			GD.Print("\t- " + (bullet != null ? bullet.Name : "Empty"));
		}
		
    }

    private void Fire(Bullet bullet)
    {
		//if (!GetNode<AnimationPlayer>("AnimationPlayer").IsPlaying())
		//{
			EmitSignal(SignalName.SuccessfulFire);
			bullet.ProcessMode = ProcessModeEnum.Inherit;
			bullet.GlobalPosition = GetNode<Node3D>("BulletEmitter").GlobalPosition;
			bullet.GlobalBasis = GetNode<Node3D>("BulletEmitter").GlobalBasis;

			GetNode<AnimationPlayer>("AnimationPlayer").Stop();
			GetNode<AnimationPlayer>("AnimationPlayer").Play("Fire");


			bullet.Visible = true;

			GD.Print("Gun: FIRED");

			bullet.Fired(this);
		//}
	}

	//public void Load(Loader loader)
	//{
	//	// right now, replace the current bullets with the new ones
	//	if (loader == null)
	//	{
	//		GD.PrintErr("Loader is null, cannot load bullets.");
	//		return;
	//	}
	//	int ocount = Bullets.Count;

	//	// *copy* over the bullets
	//	Bullets = new List<Bullet>(loader.HeldBullets.Length);

	//	for (int i = 0; i < loader.HeldBullets.Length; i++)
	//	{
	//		var bullet = loader.HeldBullets[i];
	//		Bullet clonedBullet = (Bullet)bullet.Duplicate();

	//		Load(clonedBullet);
	//	}

	//	foreach (BarrelCylinder b in barrelCylinders)
	//	{
	//		b.LoadFrom(loader);
	//	}

	//	GD.Print(barrelCylinders);

	//	GD.Print("Loaded from loader. ∆" + (Bullets.Count - ocount) + ". (+" + Bullets.Count + ", -" + ocount + ")");
	//}

    public void Load(Bullet bullet)
    {
		for (int i = 0; i < Bullets.Length && Bullets[0] != null; i++)
            RotateBarrelRight();
		
		// set the owner
		bullet.MyPlayer = Bearer;
		
		// add to clip, to store 'local' references to the bullets
		Bullets[0] = bullet;
       
        RotateBarrelRight();

        updateCylinders();

        GD.Print("loaded up");
    }
	public bool Unload()
	{
        for (int i = 0; i < Bullets.Length && Bullets[0] == null; i++)
            RotateBarrelRight();

        Bullet b = Bullets[0];
		if (Bullets[0] != null)
		{
			return Unload(Bullets[0]);
        }
		return false;

	}

    public bool Unload(Bullet bullet)
    {
        for (int i = 0; i < Bullets.Length; i++)
            if (Bullets[i] == bullet)
				Bullets[i] = null;
		//GD.Print(Bullets.Length);

        updateCylinders();
		return true;
    }
	private void updateCylinders()
	{
        foreach (BarrelCylinder b in barrelCylinders)
        {
            b.Update();
        }
    }
}
