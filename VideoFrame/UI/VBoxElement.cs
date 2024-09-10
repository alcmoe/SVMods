using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;

namespace VideoFrame.UI;

public class VBoxElement : ContainerElement
{
    private Alignment alignment;
    private int childSpacing;

    public VBoxElement(string name, Rectangle bounds, DrawableType type = DrawableType.Texture, Texture2D? texture = null, Rectangle? sourceRect = null,
        Color? color = null,
        int topEdgeSize = 4, int bottomEdgeSize = 4, int leftEdgeSize = 4, int rightEdgeSize = 4, int childSpacing = 4)
        : base(name, bounds, type, texture, sourceRect, color,
            topEdgeSize, bottomEdgeSize, leftEdgeSize, rightEdgeSize)
    {
        this.childSpacing = childSpacing;
        Bounds = bounds;
    }

    internal override void OrganiseChildren()
    {
        var widestChild = 0;
        var cumulativeChildHeight = 0;

        // Figure out our widest child element, and total height.
        foreach (UiElement child in ChildElements)
        {
            if (child.Bounds.Width > widestChild)
                widestChild = child.Bounds.Width;

            cumulativeChildHeight += child.Bounds.Height + childSpacing;
        }

        var totalWidth = widestChild + LeftEdgeSize + RightEdgeSize;
        var totalHeight = cumulativeChildHeight + TopEdgeSize + BottomEdgeSize - childSpacing;
        Bounds.Width = totalWidth;
        Bounds.Height = totalHeight;
        Bounds.Width -= Bounds.Width % 4 * 4;
        Bounds.Height -= Bounds.Height % 4 * 4;
        var centrePosition = Utility.getTopLeftPositionForCenteringOnScreen(Width, Height);
        X = (int)centrePosition.X;
        Y = (int)centrePosition.Y;
        var previousChildBottom = 0;
        foreach (var child in ChildElements)
        {
            if (previousChildBottom == 0)
            {
                child.Bounds.Y = Bounds.Y + TopEdgeSize;
            }
            else
            {
                child.Bounds.Y = previousChildBottom + childSpacing;
            }

            previousChildBottom = child.Bounds.Bottom;
        }
        foreach (var child in ChildElements)
        {
            child.Bounds.X = Bounds.X + Bounds.Width / 2 - child.Bounds.Width / 2;
        }
        base.OrganiseChildren();
    }
}
