using Godot;
using System;

public partial class FinalDoor : Node3D
{
    [Signal]
    public delegate void DoorEnteredEventHandler();

    [Export]
    public bool IsOpen = false;

    private Node3D IdleModel;
    private Node3D OpenModel;
    private Area3D TriggerArea;

    public override void _Ready()
    {
        IdleModel = GetNodeOrNull<Node3D>("Idle");
        OpenModel = GetNodeOrNull<Node3D>("Open");

        if (IdleModel == null || OpenModel == null)
        {
            GD.PrintErr("FinalDoor: IdleModel or OpenModel is not found.");
            return;
        }

        IdleModel.Visible = !IsOpen;
        OpenModel.Visible = IsOpen;
    }

    public void Entered(Node3D body)
    {
        GD.Print($"FinalDoor {Name}: Body entered: {body.Name}");
        if (body is Player)
        {
            GD.Print($"FinalDoor {Name}: Player entered.");

            EmitSignal (SignalName.DoorEntered);
        }
    }

    public override void _Process(double delta)
    {
        IdleModel.Visible = !IsOpen;
        OpenModel.Visible = IsOpen;
    }
}
