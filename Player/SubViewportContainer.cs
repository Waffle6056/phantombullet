using Godot;
using System;

public partial class SubViewportContainer : Godot.SubViewportContainer
{
    public override void _Notification(int what)
    {
        if (what == NotificationResized)
        {
            // Resize the viewport when the container is resized
            if (GetViewport() != null)
            {
                var windowSize = GetViewport().GetVisibleRect().Size;
                Size = new Vector2(windowSize.X, windowSize.Y);
            }
        }
    }
}
