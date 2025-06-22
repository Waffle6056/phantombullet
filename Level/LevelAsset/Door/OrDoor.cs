using Godot;
using System;

public partial class OrDoor : Door
{
    [Export]
    public Target[] ExtraTargets; 
    public override void _Process(double delta)
    {
        Collider.ProcessMode = ProcessModeEnum.Inherit;
        Visible = true;
        base._Process(delta);
        foreach (Target t in ExtraTargets)
        {
            //GD.Print(Target.Hit);
            if (t.Hit)
            {
                Collider.ProcessMode = ProcessModeEnum.Disabled;
                Visible = false;
            }
        }
        
    }
}
