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
		Load(LevelList[ind++]);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		// if (CurrentLevel == null || false)
		// {
		// 	Load(LevelList[ind++]);
		// 	ind %= LevelList.Length;
		// }
	}

	private void OnLevelComplete()
	{
		CurrentLevel.LevelCompleted -= OnLevelComplete;
		if (ind < LevelList.Length)
			Load(LevelList[ind++]);
		else
		{
			GD.Print("All levels completed!");
			Load(LevelList[ind = 0]);
		}
	}

	public void Load(PackedScene scene)
	{
		if (CurrentLevel != null)
			CurrentLevel.QueueFree();
		AddChild(CurrentLevel = scene.Instantiate() as Level);
		CurrentLevel.LevelCompleted += OnLevelComplete;
		Player.Instance.Reset();
	}
}
