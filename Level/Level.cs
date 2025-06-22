using Godot;
using System;
using System.IO;

public partial class Level : Node3D
{
	[Signal]
	public delegate void LevelCompletedEventHandler();

	[Signal]
	public delegate void LevelFailedEventHandler();

	[Export]
	public Target[] Targets = [];

	[Export]
	public FinalDoor FinishDoor;

	public int targetsLeft = 0;
	public bool targetsComputed = false;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		targetsLeft = 0;
		foreach (Target t in Targets)
		{
			t.IsWatching = true;
			targetsLeft++;
			t.TargetCompleted += TargetReportedComplete;
		}

		targetsComputed = true;

		foreach (Node3D node in GetChildren())
		{
			if (node is Target target)
			{
				// connect all targets in the scene
				if (target.CanTriggerDeath)
					target.TargetTriggered += TargetTriggered;
				target.Hit = false;
			}
		}

		if (FinishDoor == null)
		{
			GD.PrintErr($"Level {Name}: FinishDoor is not set. Please set it in the inspector.");
		}
		else
		{
			FinishDoor.DoorEntered += FinishAttempted;
			FinishDoor.IsOpen = false;
		}

		Player.Instance.GlobalPosition = Vector3.Zero; // Assuming Player is a singleton
	}

	public void FinishAttempted()
	{
		if (IsCompleted())
		{
			OnLevelComplete();
		}
		else
		{
			GD.Print($"Level {Name}: Finish door entered, but level is not completed.");
			GD.Print($"Targets left: {targetsLeft}");
		}
	}

	private void OnLevelComplete()
	{
		// handle level completion logic here
		GD.Print($"Level {Name}: Level completed successfully!");
		// Player.Instance.GlobalPosition = Vector3.Zero; // Reset player position
		EmitSignal(SignalName.LevelCompleted);
	}

    private bool IsCompleted()
	{
		return targetsComputed && targetsLeft <= 0;
	}

	private void CheckForCompletion()
	{
		if (IsCompleted())
		{
			FinishDoor.IsOpen = true;
		}
	}

	public void TargetTriggered(Target target)
	{
		GD.Print($"Level {Name}: target {target.Name} triggered.");
		EmitSignal (SignalName.LevelFailed);
	}

	public void TargetReportedComplete(Target target)
	{
		if (target == null)
		{
			GD.PrintErr($"Level {Name}: TargetReportedComplete: target is null.");
			return;
		}

		if (!target.Hit)
		{
			GD.PrintErr($"Level {Name}: TargetReportedComplete: target {target.Name} was not hit.");
			return;
		}

		GD.Print($"Level {Name}: target {target.Name} completed.");

		targetsLeft--;
	}


	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		CheckForCompletion();
	}
}
