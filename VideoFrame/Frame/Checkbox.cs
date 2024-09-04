using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace VideoFrame.Frame;

public class Checkbox : UiElement
{


    public Checkbox(string name, Rectangle bounds, DrawableType type = DrawableType.Texture, Texture2D texture = null, Rectangle? sourceRect = null, Color? color = null, bool drawShadow = false, int topEdgeSize = 16, int bottomEdgeSize = 12, int leftEdgeSize = 12, int rightEdgeSize = 16, int scale = 4) : base(name, bounds, type, texture, sourceRect, color, drawShadow, topEdgeSize, bottomEdgeSize, leftEdgeSize, rightEdgeSize, scale)
    {

    }
}
