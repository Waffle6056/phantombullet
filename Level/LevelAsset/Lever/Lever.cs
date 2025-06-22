using Godot;

public partial class Lever : Node3D
{
    [Export]
    public bool IsTimed = false;

    [Export]
    public Color UnactivatedColor;

    [Export]
    public Color ActivatedColor;

    [Export]
    private MeshInstance3D Indicator;

    [Export]
    private Node3D LeverArm;
    
    public double Cooldown = 1.0;
    public double TimeRemaining = 0.0;
    private bool Activated;

    private Vector3 ActivatedRotation = new Vector3(0f, 0f, -45f);
    private Vector3 UnactivatedRotation = new Vector3(0f, 0f, 45f);

    public override void _Ready()
    {
        Deactivate();
        Indicator.MaterialOverride = Indicator.MaterialOverride.Duplicate() as Material;
    }

    public override void _Process(double delta)
    {
    }

    public override void _PhysicsProcess(double delta)
    {
        base._PhysicsProcess(delta);

        if (TimeRemaining <= 0)
        {
            Activated = false;
        }
        else
        {
            TimeRemaining -= delta * BulletTime.TimeScale;
            Indicator.MaterialOverride.Set("albedo_color", ActivatedColor.Lerp(UnactivatedColor, (float)(1.0 - TimeRemaining / Cooldown)));
            LeverArm.RotationDegrees = ActivatedRotation.Lerp(UnactivatedRotation, (float)(1.0 - TimeRemaining / Cooldown));
        }
    }

    public bool IsActivated()
    {
        return Activated;
    }

    public void Activate()
    {
        Activated = true;
        Indicator.MaterialOverride.Set(BaseMaterial3D.PropertyName.AlbedoColor, ActivatedColor);
        LeverArm.RotationDegrees = ActivatedRotation;

        if (IsTimed)
            TimeRemaining = Cooldown;
    }

    public void Deactivate()
    {
        Indicator.MaterialOverride.Set(BaseMaterial3D.PropertyName.AlbedoColor, UnactivatedColor);
        LeverArm.RotationDegrees = new Vector3(0f, 0f, 45f);
        Activated = false;
    }
}