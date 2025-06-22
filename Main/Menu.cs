using Godot;
using System;

public partial class Menu : Control
{
    [Export]
    public Label StageLabel;

    [Export]
    public Node LevelManager;

    [Export]
    public PackedScene MainMenuScene;

    public override void _Ready()
    {
        Hide();
        if (StageLabel == null)
        {
            GD.PrintErr("Menu's StageLabel is not set. Please set it in the inspector.");
        }
    }

    public override void _Input(InputEvent @event)
    {
        if (@event.IsActionPressed("Pause"))
        {
            GetViewport().SetInputAsHandled();
            ResumeGame();
        }
    }

    public void ResumeGame()
    {
        GD.Print("Resuming game...");
        Hide();
        GetTree().Root.ContentScaleMode = Window.ContentScaleModeEnum.Disabled;
        Input.MouseMode = Input.MouseModeEnum.Captured;
        GetTree().Paused = false;
    }

    public void QuitToMain()
    {
        // GetTree().ChangeSceneToPacked(MainMenuScene);
        GetTree().UnloadCurrentScene();
    }

    public override void _Process(double delta)
    {
        Visible = true;
        StageLabel.Text = $"{(LevelManager as LevelManager).CurrentLevel.Name}";
    }
}
