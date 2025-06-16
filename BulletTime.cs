using Godot;
using System;

public partial class BulletTime : Node
{
	public static float TimeScale = 1f;
	[Export]
	public float NormalScale = 1f;
	[Export]
	public float SlowedScale = .25f;
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (Input.IsActionJustPressed("BulletTimeTestBind"))
		{
			if (TimeScale == NormalScale)
				TimeScale = SlowedScale;
			else
				TimeScale = NormalScale;
		}
	}
}
