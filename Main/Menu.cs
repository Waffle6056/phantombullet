using Godot;
using System;

public partial class Menu : Control
{
    public override void _Ready()
    {
        Hide();
    }

    public override void _Input(InputEvent @event)
    {
        if (@event.IsActionPressed("Pause"))
        {
            GetViewport().SetInputAsHandled();
            GD.Print("Pause action triggered (menu)");
            Hide();
            Input.MouseMode = Input.MouseModeEnum.Captured;
            GetTree().Paused = false;
        }
    }

    public override void _Process(double delta)
    {
        Visible = true;
    }
}
