using Godot;
using System;

public partial class Cover : Control
{
    [Export]
    public PackedScene Main;

    public override void _Ready()
    {
        base._Ready();
        Input.MouseMode = Input.MouseModeEnum.Visible;
        GetTree().Root.ContentScaleMode = Window.ContentScaleModeEnum.CanvasItems;
    }

    public void Play()
    {
        GetTree().ChangeSceneToPacked(Main);
    }
}
