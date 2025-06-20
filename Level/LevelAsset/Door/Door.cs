using Godot;
using System;

public partial class Door : Node3D
{
	[Export]
	public Target Target;
    [Export]
    public PhysicsBody3D Collider;
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{
		if (Target == null)
		{
			GD.PrintErr($"Door {Name}: Target is not set. Please set it in the inspector.");
		}
		if (Collider == null)
		{
			GD.PrintErr($"Door {Name}: Collider is not set. Please set it in the inspector.");
		}
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		//GD.Print(Target.Hit);
		if (Target.Hit)
		{
            Collider.ProcessMode = ProcessModeEnum.Disabled;
            Visible = false;
        }
		else
        {
            Collider.ProcessMode = ProcessModeEnum.Inherit;
            Visible = true;
		}
	}
}
