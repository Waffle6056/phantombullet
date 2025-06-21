using Godot;
using System;

public partial class FirstPersonCamera : Camera3D
{
    [Export]
    public float DPI = 1;
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
    }

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{

        GlobalPosition = Player.Instance.GlobalPosition;
    }
    public override void _Input(InputEvent @event)
    {
        base._Input(@event);
        if (@event is InputEventMouseMotion)
            rotateCamera((@event as InputEventMouseMotion).ScreenRelative);

    }
    void rotateCamera(Vector2 motion)
    {
        Vector2 MouseVelo = motion * DPI * (float)(Math.PI / 180);

        GlobalBasis = GlobalBasis.Rotated(Vector3.Up, -(float)(MouseVelo.X));

        Player.Instance.GlobalBasis = Player.Instance.GlobalBasis.Rotated(Vector3.Up, -(float)(MouseVelo.X));

        float YDelta = -(float)(MouseVelo.Y);
        YDelta = Math.Clamp(YDelta, -(-GlobalBasis[2]).AngleTo(Vector3.Down) + .1f, (-GlobalBasis[2]).AngleTo(Vector3.Up) - .1f);
        //GD.Print(YDelta);
        GlobalBasis = GlobalBasis.Rotated(GlobalBasis[0].Normalized(), YDelta);
    }
}
