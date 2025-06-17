using Godot;
using System;

public partial class TrajectoryIndicator : Node3D
{
	[Export]
	public Node3D Visuals;
    [Export]
    public RayCast3D Raycast;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		Vector3 start = GlobalPosition;
		Vector3 end =  Raycast.GlobalTransform * Raycast.TargetPosition;
		if (Raycast.IsColliding()) {
			end = Raycast.GetCollisionPoint();
			//GD.Print(Raycast.GetCollider());
			if (Raycast.GetCollider() is Target)
			{
				BulletTime.IndicatorMarked = true;
				//GD.Print("SLOW");
			}
		}
		//GD.Print(start + " " + end);


		Visuals.GlobalPosition = (start + end) / 2;
		Visuals.Scale = new Vector3(1,(end - start).Length(),1);
    }
}
