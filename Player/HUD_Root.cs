using Godot;
using System;

public partial class HUD_Root : Node3D
{
    public override void _Ready()
    {
        RecurseSetLayer(this, 3<<18);
    }

    private void RecurseSetLayer(Node node, uint layer)
    {
        foreach (Node child in node.GetChildren())
        {
            if (child is VisualInstance3D visual)
            {
                visual.Layers = layer;
            }
            RecurseSetLayer(child, layer);
        }
    }
}
