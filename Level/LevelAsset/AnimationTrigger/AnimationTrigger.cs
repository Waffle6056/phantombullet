using Godot;
using System;

public partial class AnimationTrigger : Area3D
{
	[Export]
	public AnimationPlayer Animator;
    [Export]
    public String AnimationName = "";
    [Export]
    public bool Oneshot = true;
    public bool Activated = false;
    public void Entered(Node3D node)
    {
        if (node is Player && (!Oneshot || !Activated))
        {
            Animator?.Play(AnimationName);
            Activated = true;
        }

    }
}
