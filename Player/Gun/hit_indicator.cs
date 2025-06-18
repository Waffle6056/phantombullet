using Godot;
using System;

public partial class hit_indicator : CsgTorus3D
{
    [Export]
    public RayCast3D targetingRay;

    public override void _PhysicsProcess(double delta)
    {
        if (targetingRay != null && targetingRay.IsColliding())
        {
            var collider = targetingRay.GetCollider();
            Visible = collider is Target;
        }
        else
        {
            Visible = false;
        }
    }
}
