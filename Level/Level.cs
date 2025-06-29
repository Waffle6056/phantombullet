using Godot;
using System;

public partial class Level : Node3D
{
	[Export]
	public Target[] Targets;
    public bool IsCompleted()
    {
		bool isCompleted = true;
		foreach (Target t in Targets)
		{
			isCompleted &= t.Hit;
		}
		return isCompleted;
    }
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{

	}


	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
