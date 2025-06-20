using Godot;
using System;

public partial class hit_indicator : Node3D
{
    [Export]
    public RayCast3D targetingRay;

    [Export]
    public Node3D idle;

    public override void _PhysicsProcess(double delta)
    {
        if (targetingRay != null && targetingRay.IsColliding())
        {
            var collider = targetingRay.GetCollider() as Node3D;
            Visible = collider.IsInGroup("target");
        }
        else
        {
            Visible = false;
        }
        // idle.Visible = !Visible;
    }
}
