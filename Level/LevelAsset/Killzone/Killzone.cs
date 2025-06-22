using Godot;
using System;

public partial class Killzone : Area3D
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
	public void Collide(Node3D node)
	{
		if (node is Player)
			(node as Player).Dead = true;
	}
}
