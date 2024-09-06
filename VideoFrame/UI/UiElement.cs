using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;

namespace VideoFrame.UI;

public class UiElement
{
    internal string ElementName;
    internal Rectangle Bounds;
    internal Rectangle SourceRect;
    internal readonly Texture2D? Texture;
    internal Color TextureTint;
    internal readonly int TopEdgeSize, BottomEdgeSize, LeftEdgeSize, RightEdgeSize;
    internal int Scale;
    internal UiElement? Parent;
    internal bool drawBox;
    internal DrawableType DrawableType;
    internal Action? LeftClickCallback;
    internal Action? RightClickCallback;
    internal bool DrawShadow;

    public bool DrawBox
    {
        get { return drawBox; }
    }

    public Vector2 TopLeftCorner => new(Bounds.Left, Bounds.Top);

    public Vector2 BottomLeftCorner => new(Bounds.Left, Bounds.Bottom);

    public Vector2 TopRightCorner => new(Bounds.Right, Bounds.Top);

    public Vector2 BottomRightCorner => new Vector2(Bounds.Right, Bounds.Bottom);

    /// <summary>
    /// Sets a new width for this <see cref="UiElement"/>'s bounds.
    /// </summary>
    public int Width
    {
        get => Bounds.Width;
        set => Bounds = new Rectangle( Bounds.X, Bounds.Y, value, Bounds.Height);
    }

    /// <summary>
    /// Sets a new height for this <see cref="UiElement"/>'s bounds.
    /// </summary>
    public int Height
    {
        get => Bounds.Height;
        set =>  Bounds = new Rectangle(Bounds.X, Bounds.Y, Bounds.Width, value);
    }

    /// <summary>
    /// Sets a new X co-ordinate for this <see cref="UiElement"/>'s bounds.
    /// </summary>
    public int X
    {
        get => Bounds.X;
        set => Bounds = new Rectangle(value, Bounds.Y, Bounds.Width, Bounds.Height);
    }

    /// <summary>
    /// Sets a new Y co-ordinate for this <see cref="UiElement"/>'s bounds.
    /// </summary>
    public int Y
    {
        get => Bounds.Y;
        set => Bounds = new Rectangle(Bounds.X, value, Bounds.Width, Bounds.Height);
    }

    public UiElement(string name, Rectangle bounds, DrawableType type = DrawableType.Texture, Texture2D? texture = null, Rectangle? sourceRect = null,
        Color? color = null, bool drawShadow = false, int topEdgeSize = 16, int bottomEdgeSize = 12, int leftEdgeSize = 12, int rightEdgeSize = 16, int scale = 4)
    {
        ElementName = name;
        Texture = texture;
        Bounds = bounds;
        DrawableType = type;
        Scale = scale;
        DrawShadow = drawShadow;
        if (sourceRect.HasValue) {
            SourceRect = sourceRect.Value;
            Bounds.Width = sourceRect.Value.Width;
            Bounds.Height = sourceRect.Value.Height;
        }
        if (DrawableType == DrawableType.SlicedBox && Texture == null) {
            // In this situation, we need sane defaults.
            Texture = Game1.menuTexture;
            SourceRect = new Rectangle(0, 256, 60, 60);
        }
        TextureTint = color ?? Color.White;
        TopEdgeSize = topEdgeSize;
        BottomEdgeSize = bottomEdgeSize;
        LeftEdgeSize = leftEdgeSize;
        RightEdgeSize = rightEdgeSize;
        Bounds = new Rectangle {X = Bounds.X, Y = Bounds.Y, Width = Bounds.Width * scale, Height = Bounds.Height * scale};
    }

    public void RegisterLeftClickCallback(Action left)
    {
        LeftClickCallback = left;
    }

    public void RegisterRightClickCallback(Action right)
    {
        RightClickCallback = right;
    }

    internal void UpdatePosition(int xPos, int yPos)
    {
        BoundsChanged();
    }

    internal void UpdateSize(int width, int height)
    {
        BoundsChanged();
    }

    public virtual void Draw(SpriteBatch spriteBatch, Color? colourTint = null)
    {
        if (DrawableType == DrawableType.Texture) {
            if (Texture is null) {
                return;
            }
            if (DrawShadow) {
                spriteBatch.Draw(Texture, new Rectangle(Bounds.X + 4, Bounds.Y + 4, Bounds.Width, Bounds.Height), SourceRect, TextureTint);
            }
            spriteBatch.Draw(Texture, Bounds, SourceRect, colourTint ?? TextureTint);
        } else if (DrawableType == DrawableType.SlicedBox) {
            Utils.DrawBox(spriteBatch, Texture, SourceRect, Bounds, TopEdgeSize, LeftEdgeSize, RightEdgeSize, BottomEdgeSize);
        }
    }

    internal void BoundsChanged()
    {
    }

    public virtual void ReceiveLeftClick(int x, int y)
    {
        LeftClickCallback?.Invoke();
    }

    public virtual void ReceiveRightClick(int x, int y)
    {
        RightClickCallback?.Invoke();
    }

    public virtual void ReceiveScrollWheel(int direction)
    {

    }

    public void SetParent(UiElement parent)
    {
        Parent = parent;
    }

    internal virtual void OrganiseChildren()
    {

    }
}
