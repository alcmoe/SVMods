using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace VideoFrame.UI;

public class Checkbox(
    string name,
    Rectangle bounds,
    DrawableType type = DrawableType.Texture,
    Texture2D? texture = null,
    Rectangle? sourceRect = null,
    Color? color = null,
    bool drawShadow = false,
    int topEdgeSize = 16,
    int bottomEdgeSize = 12,
    int leftEdgeSize = 12,
    int rightEdgeSize = 16,
    int scale = 4)
    : UiElement(name, bounds, type, texture, sourceRect, color, drawShadow, topEdgeSize, bottomEdgeSize, leftEdgeSize,
        rightEdgeSize, scale);
