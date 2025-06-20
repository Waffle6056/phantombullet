using Godot;
using System;
using System.Reflection;

public partial class Ammo : Area3D
{
    public bool Used = false;

    private long CollectedTime = 0;

    [Export]
    public Bullet[] Storage;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        // check storage and build freq map
        if (Storage == null)
        {
            GD.PrintErr($"Ammo {Name}: Ammo's 'Storage' property is set to null.");
        }
        var freqMap = new System.Collections.Generic.Dictionary<string, int>();
        foreach (Bullet child in Storage)
        {
            string typeName = (child as Bullet).GetBulletType();
            if (freqMap.ContainsKey(typeName))
            {
                freqMap[typeName]++;
            }
            else
            {
                freqMap[typeName] = 1;
            }
        }

        // order in pairs of 2, concantate with newlines, and print
        string output = "";
        int i = 0;
        foreach (var kvp in freqMap)
        {
            output += $"{kvp.Value}x {kvp.Key}";
            i++;
            // put a comma if not the last item and if i is even
            if (i < freqMap.Count)
            {
                output += (i % 2 == 0) ? ", " : "\n";
            }
        }

        GetNode<Label3D>("Contents").Text = output;
        GetNode<Label3D>("Title").Text = $"Ammo ({Storage.Length})";
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
        Visible = !Used;
        if (Used)
        {
            CollectedTime += (long)(delta * 1000);
        }

        if (CollectedTime > 5000 && Used)
        {
            GD.Print("Ammo resetting");
            Used = false;
        }
	}

    public void IsEntered(Node3D body)
    {
        CollectedTime = 0;
        if (Used) return;
        if (body is Player)
        {
            // load bullets

            if (Storage != null)
            {
                GD.Print("\nPicking up ammo.");
                Player p = body as Player;
                foreach (Bullet b in Storage)
                {
                    Bullet d = b.Duplicate() as Bullet;
                    p.PickUp(d);

                }

                // mark as used
                Used = true;
            }
        }
	}

}
