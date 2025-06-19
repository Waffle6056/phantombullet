using Godot;
using System;

public partial class stamina_display : CsgCylinder3D
{
    private float HeightPerDuration;

    public override void _Ready()
    {
        base._Ready();

        HeightPerDuration = Height / BulletTime.DurationMax;
    }

    public override void _Process(double delta)
    {
        Visible = BulletTime.DurationCurrent < BulletTime.DurationMax;
        Height = BulletTime.DurationCurrent * HeightPerDuration;
    }
}
