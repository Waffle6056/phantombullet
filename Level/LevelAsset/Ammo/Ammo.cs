using Godot;
using System;

public partial class Ammo : Area3D
{
    public bool Used = false;

    private long CollectedTime = 0;

    [Export]
    public Loader Storage;

	// Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {

    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
        Visible = !Used;
        if (Used)
        {
            CollectedTime += (long)(delta * 1000);
        }

        if (CollectedTime > 5000 && Used)
        {
            GD.Print("Ammo resetting");
            Used = false;
        }
	}

    public void IsEntered(Node3D body)
    {
        CollectedTime = 0;
        if (Used) return;
        if (body is Player)
        {
            // load bullets

            if (Storage != null)
            {
                GD.Print("\nLoading ammo into gun.");
                var gun = ((Player)body).Gun;
                if (gun == null) GD.PrintErr("WARNING: Player's 'gun' property is set to null. Cannot load ammo.");
                gun.Load(Storage);
            }

            // mark as used
            Used = true;
        }
	}

}
