using Godot;
using System;

public partial class BulletTime : Node
{

	public static float TimeScale = 1f;
	public static bool IndicatorMarked = false;
	[Export]
	public float NormalScale = 1f;
	[Export]
	public float SlowedScale = .25f;

	public static float DurationMax = 3;
    public static float DurationCurrent = 3;

    [Export]
    public float DurationRegen = .5f;
	bool BulletTimeManual = false;
    private bool _manualActivation { get { return BulletTimeManual && DurationCurrent > 0; } }

    [Export]
    public float BlurringPercentVelocity = 1f;
    public float BlurringPercent = 0;
    [Export]
    public ColorRect BlurNode;
	private ShaderMaterial _BlurMaterial {  get { return BlurNode.Material as ShaderMaterial; } }
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{

	}
	public override void _Process(double delta)
	{
		if (Input.IsActionJustPressed("BulletTimeTestBind"))
			BulletTimeManual = true; 
		if (Input.IsActionJustReleased("BulletTimeTestBind"))
            BulletTimeManual = false;
    }

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _PhysicsProcess(double delta)
	{
		
		TimeScale = NormalScale; 
		if (_manualActivation || IndicatorMarked)
        {
            TimeScale = SlowedScale;
			IndicatorMarked = false;
            BlurringPercent = (float) Math.Min(BlurringPercent + BlurringPercentVelocity * delta, 1);
        }
		else
            BlurringPercent = (float) Math.Max(BlurringPercent - BlurringPercentVelocity * delta, 0);
		_BlurMaterial.SetShaderParameter("blurring_percent", BlurringPercent);


        if (_manualActivation)
		{
			DurationCurrent = (float)Math.Max(DurationCurrent - delta, 0);
			if (DurationCurrent <= 0)
				BulletTimeManual = false;

		}
		else
			DurationCurrent = (float)Math.Min(DurationCurrent + DurationRegen * delta, DurationMax);

    }
}
