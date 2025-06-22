	using Godot;
using System;

public partial class LevelManager : Node
{
	[Export]
	PackedScene[] LevelList;
	int ind = 0;
	public static Level CurrentLevel;
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

	private void OnLevelFailed()
	{
		// reload
		Load(LevelList[ind-1]);
		// maybe later, show a failure screen
	}

	public void Load(PackedScene scene)
	{
		if (CurrentLevel != null)
			CurrentLevel.QueueFree();
		AddChild(CurrentLevel = scene.Instantiate() as Level);
		CurrentLevel.LevelCompleted += OnLevelComplete;
		CurrentLevel.LevelFailed += OnLevelFailed;

		// later, force player to reinstantiate (or reparent bullets)
		Player.Instance.Reset();
	}
}
