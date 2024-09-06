using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;

namespace VideoFrame.UI;

public class HBoxElement : ContainerElement
{
    private readonly int _childSpacing;

    public HBoxElement(string name, Rectangle bounds, DrawableType type = DrawableType.Texture, Texture2D? texture = null, Rectangle? sourceRect = null,
        Color? color = null,
        int topEdgeSize = 4, int bottomEdgeSize = 4, int leftEdgeSize = 4, int rightEdgeSize = 4, int childSpacing = 4)
        : base(name, bounds, type, texture, sourceRect, color,
            topEdgeSize, bottomEdgeSize, leftEdgeSize, rightEdgeSize)
    {
        _childSpacing = childSpacing;
        Bounds = bounds;
    }

    internal override void OrganiseChildren()
    {
        var tallestChild = 0;
        var cumulativeChildWidth = 0;
        foreach (UiElement child in ChildElements)
        {
            if (child.Bounds.Height > tallestChild)
                tallestChild = child.Bounds.Height;

            cumulativeChildWidth += child.Bounds.Width + _childSpacing;
        }
        var totalHeight = tallestChild + TopEdgeSize + BottomEdgeSize;
        var totalWidth = cumulativeChildWidth + LeftEdgeSize + RightEdgeSize - _childSpacing;
        Bounds.Width = totalWidth;
        Bounds.Height = totalHeight;
        Bounds.Width -= Bounds.Width % 4 * 4;
        Bounds.Height -= Bounds.Height % 4 * 4;
        var centrePosition = Utility.getTopLeftPositionForCenteringOnScreen(Width, Height);
        X = (int)centrePosition.X;
        Y = (int)centrePosition.Y;

        var previousChildRight = 0;
        
        foreach (var child in ChildElements) {
            if (previousChildRight == 0) {
                child.Bounds.X = Bounds.X + LeftEdgeSize;
            } else {
                child.Bounds.X = previousChildRight + _childSpacing;
            }
            previousChildRight = child.Bounds.Right;
        }
        // Then, for now, just vertically centre them.
        foreach (var child in ChildElements)
        {
            child.Bounds.Y = Bounds.Y + Bounds.Height / 2 - child.Bounds.Height / 2;
        }
    }
}
