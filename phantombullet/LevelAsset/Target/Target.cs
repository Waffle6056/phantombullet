using Godot;
using System;

public partial class Target : Area3D
{
    public bool Hit = false;
	[Export]
	public Node3D HitIndicator;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{

	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (HitIndicator != null)
		{
			HitIndicator.Visible = Hit;
		}

	}
	public void IsShot(Node3D body)
	{
		Hit = true;
	}
}
