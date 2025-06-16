	using Godot;
using System;

public partial class LevelManager : Node
{
	[Export]
	PackedScene[] LevelList;
	int ind = 0;
	public Level CurrentLevel;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (CurrentLevel == null || CurrentLevel.IsCompleted())
		{
			Load(LevelList[ind++]);
			ind %= LevelList.Length;
		}

	}
	
	public void Load(PackedScene scene)
	{
		if (CurrentLevel != null)
			CurrentLevel.QueueFree();
		AddChild(CurrentLevel = scene.Instantiate() as Level);
		Player.Instance.GlobalPosition = Vector3.Zero;
	}
}
