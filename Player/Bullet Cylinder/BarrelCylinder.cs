using Godot;
using System;
using System.Collections.Generic;

public partial class BarrelCylinder : Node3D
{
    [Export]
    public Gun myGun;

    private int acctime = 0;
    private Tween tween;

    private enum PossibleSlots
    {
        N,
        NE,
        SE,
        S,
        SW,
        NW
    }

    private PossibleSlots CurrentSlot = PossibleSlots.N;

    private Dictionary<PossibleSlots, bool> SlotMap = new Dictionary<PossibleSlots, bool>
    {
        { PossibleSlots.N, false },
        { PossibleSlots.NE, false },
        { PossibleSlots.SE, false },
        { PossibleSlots.S, false },
        { PossibleSlots.SW, false },
        { PossibleSlots.NW, false }
    };

    public override void _Ready()
    {
        base._Ready();
        if (myGun == null) GD.PrintErr(Name + " Barrel's Gun set to null. barrel will not be connected.");
        myGun.RegisterBarrell(this);
    }

    // public override void _Process(double delta)
    // {
    //     acctime += (int)(delta * 1000); // Convert delta to milliseconds
    //     if (acctime >= 2000)
    //     {
    //         GD.Print("Cylinder: 2 seconds have passed.");
    //         acctime = 0; // reset time
    //         FireAnimation();
    //     }
    // }

    public void Fire()
    {
        GD.Print("Firing!");
        FireAnimation();
        HideAllButXFromCluster("", GetNode<Node3D>($"%{CurrentSlot.ToString()}_Cluster"));
        // cycle through slots backwards
        CurrentSlot = (CurrentSlot == PossibleSlots.N) ? 
                      PossibleSlots.NW : 
                      (PossibleSlots)((int)CurrentSlot - 1);
    }

    private void HideAllButXFromCluster(string lookingFor, Node3D Cluster)
    {
        foreach (Node3D bullet in Cluster.GetChildren())
        {
            bullet.Visible = bullet.Name == lookingFor;
        }
    }

    public void LoadFrom(Loader loader)
    {
        // Start from current slot, load bullets backwards till we hit 6
        int slotIndex = (int)CurrentSlot;
        if (loader.HeldBullets.Length > 6) GD.PrintErr(Name + " Loader holds more than 6 bullets- only the first 6 will be rendered in the barrel.");
        PossibleSlots slot = CurrentSlot;
        for (int i = 0; i < 6 && i < loader.HeldBullets.Length; i++)
        {

            // get current bullet
            Bullet bullet = loader.HeldBullets[i];

            Node3D Cluster = GetNode<Node3D>($"%{slot.ToString()}_Cluster");

            if (bullet is PhotonBullet)
            {
                GD.Print("Loading photon into slot " + slot.ToString());
                HideAllButXFromCluster("PhotonBullet", Cluster);
            }
            else if (bullet is TeleporterBullet)
            {
                GD.Print("Loading tp into slot " + slot.ToString());
                HideAllButXFromCluster("TeleportBullet", Cluster);
            }
            else if (bullet is OnTriggerDeadshotBullet)
            {
                GD.Print("Loading ds into slot " + slot.ToString());
                HideAllButXFromCluster("DeadshotBullet", Cluster);
            }
            else if (bullet is OnTriggerOrCollisionBoomBullet)
            {
                GD.Print("Loading boom into slot " + slot.ToString());
                HideAllButXFromCluster("BoomBullet", Cluster);
            }
            else if (bullet is OnTriggerTurningBullet)
            {
                GD.Print("Loading turny into slot " + slot.ToString());
                HideAllButXFromCluster("TurnyBullet", Cluster);
            }
            else
            {
                GD.Print("Loading normie into slot " + slot.ToString());
                HideAllButXFromCluster("NormalBullet", Cluster);
            }

            slot = (slot == PossibleSlots.N) ? 
                      PossibleSlots.NW : 
                      (PossibleSlots)((int)slot - 1);
        }
    }

    private void FireAnimation()
    {
        // get initial rotation
        Vector3 initialRotation = RotationDegrees;
        Vector3 stage1Rotation = initialRotation + new Vector3(0, 0, -80);
        Vector3 stage2Rotation = initialRotation + new Vector3(0, 0, -60);

        // kill any existing tween and replace it with our own, new, better, grander tween
        if (tween != null)
            tween.Kill();

        tween = CreateTween();

        // wait for .3 sec
        tween.TweenInterval(0.3f);

        // move -80deg in z axis over 0.25 seconds
        tween.TweenProperty(this, "rotation_degrees", stage1Rotation, 0.25f);

        // move +20deg in z axis over 0.09 seconds
        tween.TweenProperty(this, "rotation_degrees", stage2Rotation, 0.09f);

        // idle
        tween.TweenInterval(0.06f);
    }
}
