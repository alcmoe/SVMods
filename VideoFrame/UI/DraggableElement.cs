using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace VideoFrame.UI;

public class DraggableElement : UiElement
{
    private bool currentlyBeingDragged;
    private UiElement dragArea;

    internal DraggableElement(string name, Rectangle bounds, DrawableType type = DrawableType.Texture,Texture2D? texture = null, Rectangle? sourceRect = null,
        Color? color = null)
        : base(name, bounds, type, texture, sourceRect, color)
    {
    }
}
