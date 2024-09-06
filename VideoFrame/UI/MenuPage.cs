using Microsoft.Xna.Framework.Graphics;

namespace VideoFrame.UI;

public class MenuPage(UiElement page, TextElement pageText)
{
    public readonly UiElement Page = page;
    public readonly TextElement PageText = pageText;

    public int TotalHeight => Page.Height + PageText.Height;

    public int TotalWidth => Math.Max(PageText.Width, Page.Width);

    public void Draw(SpriteBatch spriteBatch)
    {
        Page.Draw(spriteBatch);
        PageText.Draw(spriteBatch);
    }
}
