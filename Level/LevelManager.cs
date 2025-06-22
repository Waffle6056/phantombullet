	using Godot;
using System;

public partial class LevelManager : Node
{
	public static LevelManager Instance { get; private set; }
	[Export]
	PackedScene[] LevelList;
	int ind = -1;
	public Level CurrentLevel;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Instance = this;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (CurrentLevel == null || CurrentLevel.IsCompleted())
		{
			ind++;
			ind %= LevelList.Length;
            Load(LevelList[ind]);
		}
		if (Player.Instance.Dead)
			Load(LevelList[ind]);

	}
	
	public void Load(PackedScene scene)
	{
		if (CurrentLevel != null)
			CurrentLevel.QueueFree();
		AddChild(CurrentLevel = scene.Instantiate() as Level);
		Player.Instance.GlobalPosition = Vector3.Zero;
        Player.Instance.Dead = false;
        Player.Instance.ClearInventory();
    }
}
