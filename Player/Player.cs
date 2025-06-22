using Godot;
using System;
using System.Collections.Generic;
using static Godot.HttpRequest;

public partial class Player : CharacterBody3D
{
	[Export]
	public Gun Gun;

	[Export]
	public float Speed = 5.0f;
	[Export]
	public float JumpVelocity = 4.5f;
	public static Player Instance;
    public int Ind;
    public Bullet[] Inventory = new Bullet[12];
	public Dictionary<Bullet, Node3D> DupedVisuals = new Dictionary<Bullet, Node3D>();
	
	public bool InventoryOpen = false;

    public bool InventoryFocused = true;
    [Export]
    public float InventoryDistance = .1f;
    [Export]
	public Node3D AmmoBand;
	[Export]
	public Node3D Cursor;
    [Export]
    public Node3D CylinderHUD;
    [Export]
    public Node3D CylinderCursor;
    [Export]
    public Node3D SwapLabel;
    [Export]
    public Label3D SlotsLabel;
    [Export]
    public AnimationPlayer Animator;
	[Export]
	public bool Dead = false;
	public override void _Ready()
	{
		base._Ready();
		Instance = this;

		if (Gun == null)
		{
			GD.PrintErr("Player's 'Gun' property is set to null.");
		}
		if (Cursor == null)
			GD.PrintErr("Player's Cursor is not set. Please set it in the inspector.");
		if (CylinderCursor == null)
			GD.PrintErr("Players' CylinderCursor is not set. Please set it in the inspector.");
		if (Animator == null)
			GD.PrintErr("Player's Animator is not set. Please set it in the inspector.");
		if (AmmoBand == null)
			GD.PrintErr("Player's AmmoBand is not set. Please set it in the inspector.");
		if (CylinderHUD == null)
			GD.PrintErr("Player's CylinderHUD is not set. Please set it in the inspector.");
	}
    public override void _Process(double delta)
    {
        base._Process(delta);
		if (Input.IsActionJustPressed("Reset"))
		{
			Animator.Play("Reset");
		}
		if (Input.IsActionJustPressed("ToggleInventory"))
		{
            InventoryFocused = SwapLabel.Visible = Cursor.Visible = InventoryOpen = !InventoryOpen;

			if (InventoryOpen)
				Animator.Play("ToggleInventory");
			else
				Animator.PlayBackwards("ToggleInventory");
        }

		if (InventoryOpen && Input.IsActionJustReleased("ToggleFocus"))
		{
			InventoryFocused = !InventoryFocused;
            if (InventoryFocused)
                Animator.PlayBackwards("ToggleFocus");
            else
                Animator.Play("ToggleFocus");
        }

        if (InventoryOpen && InventoryFocused)
        {
            if (Input.IsActionJustReleased("ScrollUp"))
                ScrollLeft();
			if (Input.IsActionJustReleased("ScrollDown"))
				ScrollRight();
			if (Input.IsActionJustPressed("Fire"))
			{
				Bullet b = Inventory[Ind];
				Inventory[Ind] = null;
                LoadGun(b);
                ScrollRight();
            }
		}
		if (InventoryOpen)
		{
			if (InventoryFocused)
			{
				Cursor.Position = IndToPosition(Ind);
				Cursor.Basis = Basis.Identity;
				Cursor.Visible = true;
                CylinderCursor.Visible = false;
            }
			else
			{
				Cursor.Visible = false;
                CylinderCursor.Visible = true;
            }
			int count = 0;
			foreach (var item in Inventory)
				if (item != null)
					count++;
			SlotsLabel.Text = count + "/" + Inventory.Length;
            //else
            //{
            //	Cursor.GlobalPosition = CylinderHUD.GlobalPosition;
            //	Cursor.GlobalBasis = CylinderHUD.GlobalBasis.Rotated(CylinderHUD.GlobalBasis[0], (float)Math.PI/2);
            //             Cursor.SetDisableScale(true);
            //         }
        }


    }
	public Vector3 IndToPosition(int i)
	{
         return new Vector3(InventoryDistance * i, 0, 0);
    }
    public override void _PhysicsProcess(double delta)//this is the builtin template btw
	{
		Vector3 velocity = Velocity;

		// Add the gravity.
		if (!IsOnFloor())
		{
			velocity += GetGravity() * (float)delta * BulletTime.TimeScale;
		}

		// Handle Jump.
		if (Input.IsActionJustPressed("Jump") && IsOnFloor())
		{
			velocity.Y = JumpVelocity;
		}

		// Get the input direction and handle the movement/deceleration.
		// As good practice, you should replace UI actions with custom gameplay actions.
		Vector2 inputDir = Input.GetVector("Left", "Right", "Up", "Down");
		Vector3 direction = (Transform.Basis * new Vector3(inputDir.X, 0, inputDir.Y)).Normalized();
		if (direction != Vector3.Zero)
		{
			velocity.X = direction.X * Speed;
			velocity.Z = direction.Z * Speed;
		}
		else
		{
			velocity.X = Mathf.MoveToward(Velocity.X, 0, Speed);
			velocity.Z = Mathf.MoveToward(Velocity.Z, 0, Speed);
		}
		//GD.Print(velocity);
		Velocity = velocity;
		Velocity *= BulletTime.TimeScale;
		MoveAndSlide();
		Velocity /= BulletTime.TimeScale;
	}
	public bool PickUp(Bullet b)
	{
		GD.Print("Picked up");
        int i = 0;
		for (; i < Inventory.Length && Inventory[i] != null; i++)
			continue;
		if (i == Inventory.Length)
		{
			GD.Print("inventory full");
			return false;
        }
		Inventory[i] = b;

        Node3D vis = b.Visual.Duplicate() as Node3D;
        AmmoBand.AddChild(vis);
        vis.Position = IndToPosition(i);

		if (DupedVisuals.ContainsKey(b))
            DupedVisuals.Remove(b);
        DupedVisuals.Add(b, vis);
		return true;
		
	}
	public bool LoadGun(Bullet b)
    {
        if (b == null)
            return false;
		if (Gun.ChambersFilled && !(PickUp(Gun.Bullets[0]) && Gun.Unload()))
			return false;
        GD.Print("Picked down");
        for (int i = 0; i < Inventory.Length; i++)
            if (Inventory[i] == b)
                Inventory[i] = null;
		if (DupedVisuals.ContainsKey(b))
		{
			DupedVisuals[b].QueueFree();
			DupedVisuals.Remove(b);

        }
        Gun.Load(b);
		return true;
	}
	public void ClearInventory()
	{
        for (int i = 0; i < Inventory.Length; i++)
            Inventory[i] = null;
		foreach (Node3D vis in DupedVisuals.Values)
			vis.QueueFree();
		DupedVisuals.Clear();
		Gun.ClearInventory();
    }
    int FloorMod(int a, int b)
    {
        return ((a % b) + b) % b;
    }
    public void ScrollRight()
	{
		int i = 1;
        for (; i < Inventory.Length && Inventory[FloorMod(Ind + i, Inventory.Length)] == null; i++)
            continue;
		Ind = FloorMod(Ind + i, Inventory.Length);
    }
    public void ScrollLeft()
    {
        int i = 1;
        for (; i < Inventory.Length && Inventory[FloorMod(Ind - i, Inventory.Length)] == null; i++)
            continue;
        Ind = FloorMod(Ind - i, Inventory.Length);
    }
	public void _on_gun_successful_fire()
	{
		GetNode<AnimationPlayer>("AnimationPlayer").Stop();
		GetNode<AnimationPlayer>("AnimationPlayer").Play("Revolver Recoil");
	}
}
