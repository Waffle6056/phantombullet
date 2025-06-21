using Godot;
using System;

public partial class Main : Node3D
{
    public override void _Ready()
    {
        base._Ready();

        Input.MouseMode = Input.MouseModeEnum.Captured;
    }

    public override void _Notification(int what)
    {
        if (what == NotificationApplicationFocusOut)
        { Pause(); }
    }

    public override void _Input(InputEvent @event)
    {
        if (@event.IsActionPressed("Pause"))
        {
            GetViewport().SetInputAsHandled();
            Pause();
        }
    }

    public void Pause()
    {
        GD.Print("Pause action triggered (main)");
        GetTree().Paused = true;
        GetTree().Root.ContentScaleMode = Window.ContentScaleModeEnum.CanvasItems;
        Input.MouseMode = Input.MouseModeEnum.Visible;
    }

    public override void _Process(double delta)
    {
    }
}
