using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;

namespace VideoFrame.UI;

public class TextElement : UiElement
{
    private readonly string _text;
    private readonly SpriteFont _font;
    private readonly int _widthConstraint;

    public TextElement(
        string name, Rectangle bounds, SpriteFont? font1 = null, int widthConstraint = 1000, string text = "",
        SpriteFont? font = null, DrawableType type = DrawableType.SlicedBox, Texture2D? texture = null, Rectangle? sourceRect = null, Color? color = null,
        int topEdgeSize = 16, int bottomEdgeSize = 16, int leftEdgeSize = 16, int rightEdgeSize = 16)
        : base(name, bounds, type, texture, sourceRect, color, false,
            topEdgeSize, bottomEdgeSize, leftEdgeSize, rightEdgeSize)
    {
        _font = font1 ?? Game1.dialogueFont;
        _widthConstraint = widthConstraint;
        _text = text;
        var wrappedText = Game1.parseText(_text, Game1.dialogueFont, _widthConstraint);
        var stringSize = Game1.dialogueFont.MeasureString(wrappedText);
        Bounds = new Rectangle(bounds.X, bounds.Y, (int)stringSize.X + leftEdgeSize + rightEdgeSize, (int)stringSize.Y + topEdgeSize + bottomEdgeSize);
    }

    public override void Draw(SpriteBatch spriteBatch, Color? colourTint = null)
    {
        base.Draw(spriteBatch, colourTint);
        Utility.drawTextWithShadow(
            spriteBatch,
            Game1.parseText(_text, Game1.dialogueFont, _widthConstraint),
            _font,
            new Vector2(Bounds.X + LeftEdgeSize, Bounds.Y + TopEdgeSize),
            Game1.textColor
            );
    }
}
