using Godot;

public partial class CustomTimer : Node
{
    [Signal]
    public delegate void TimeoutEventHandler();

    public double TimeElapsed { get; private set; } = 0;
    public bool IsRunning { get; private set; } = false;
    public double TotalTime { get; private set; } = 0;

    public void Start(double time)
    {
        GD.Print($"CustomTimer started for {time} milliseconds.");
        if (IsRunning)
        {
            GD.PrintErr("CustomTimer is already running.");
            return;
        }

        TotalTime = time;
        TimeElapsed = 0;
        IsRunning = true;
    }

    public void Kill()
    {
        GD.Print("CustomTimer killed.");
        IsRunning = false;
        TimeElapsed = 0;
    }

    public override void _Process(double delta)
    {
        if (IsRunning)
        {
            // Convert delta to milliseconds, scaling by BulletTime.TimeScale
            TimeElapsed += delta * 1000 * BulletTime.TimeScale;
            if (TimeElapsed >= TotalTime)
            {
                GD.Print($"CustomTimer completed after {TimeElapsed} milliseconds.");
                IsRunning = false;
                EmitSignal(SignalName.Timeout);
            }
        }
    }
}