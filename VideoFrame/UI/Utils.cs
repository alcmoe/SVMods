using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI.Framework;
using StardewValley;

namespace VideoFrame.UI;

public static class Utils
{
    public static void DrawBox(SpriteBatch batch, Texture2D? texture, Rectangle sourceRect, Rectangle bounds,
        int topEdgeHeight = 16, int leftEdgeWidth = 12, int rightEdgeWidth = 16, int bottomEdgeHeight = 12)
    {
        DrawBox(
            batch,
            texture,
            sourceRect,
            bounds.X,
            bounds.Y,
            bounds.Width,
            bounds.Height,
            topEdgeHeight,
            leftEdgeWidth,
            rightEdgeWidth,
            bottomEdgeHeight
        );
    }

    private static void DrawBox(SpriteBatch batch, Texture2D? texture, Rectangle sourceRect, int xPos, int yPos,
        int width,
        int height, int topEdgeHeight = 16, int leftEdgeWidth = 12, int rightEdgeWidth = 16, int bottomEdgeHeight = 12)
    {
        var topEdgeWidth = width - (leftEdgeWidth + rightEdgeWidth);
        var leftEdgeHeight = height - (topEdgeHeight + bottomEdgeHeight);
        // We don't need a bottomEdgeWidth or a rightEdgeHeight, because we're not doing bloody trapezoids.

        // Get our corner destination rects...
        var topLeftCorner = new Rectangle(
            xPos,
            yPos,
            leftEdgeWidth,
            topEdgeHeight);
        var topRightCorner = new Rectangle(
            xPos + width - rightEdgeWidth,
            yPos,
            rightEdgeWidth,
            topEdgeHeight);
        var bottomLeftCorner = new Rectangle(
            xPos,
            yPos + leftEdgeHeight + topEdgeHeight,
            leftEdgeWidth,
            bottomEdgeHeight);
        var bottomRightCorner = new Rectangle(
            xPos + leftEdgeWidth + topEdgeWidth,
            yPos + topEdgeHeight + leftEdgeHeight,
            rightEdgeWidth,
            bottomEdgeHeight);

        // ...and our edge destination rects.
        var topEdge = new Rectangle(
            xPos + leftEdgeWidth,
            yPos,
            topEdgeWidth,
            topEdgeHeight);
        var leftEdge = new Rectangle(
            xPos,
            yPos + topEdgeHeight,
            leftEdgeWidth,
            leftEdgeHeight);
        var rightEdge = new Rectangle(
            xPos + leftEdgeWidth + topEdgeWidth,
            yPos + topEdgeHeight,
            rightEdgeWidth,
            leftEdgeHeight);
        var bottomEdge = new Rectangle(
            xPos + leftEdgeWidth,
            yPos + height - bottomEdgeHeight,
            topEdgeWidth,
            bottomEdgeHeight);

        // Finally, our centre destination rect.
        var centreRect = new Rectangle(
            xPos + leftEdgeWidth,
            yPos + topEdgeHeight,
            width - (leftEdgeWidth + rightEdgeWidth),
            height - (topEdgeHeight + bottomEdgeHeight));

        // Top left corner.
        batch.Draw(
            texture,
            topLeftCorner,
            new Rectangle(
                sourceRect.X,
                sourceRect.Y,
                leftEdgeWidth,
                topEdgeHeight),
            Color.White);

        // Top right corner.
        batch.Draw(
            texture,
            topRightCorner,
            new Rectangle(
                sourceRect.X + (sourceRect.Width - rightEdgeWidth),
                sourceRect.Y,
                rightEdgeWidth,
                topEdgeHeight),
            Color.White);

        // Bottom left corner.
        batch.Draw(
            texture,
            bottomLeftCorner,
            new Rectangle(
                sourceRect.X,
                sourceRect.Y + (sourceRect.Height - bottomEdgeHeight),
                leftEdgeWidth,
                bottomEdgeHeight),
            Color.White);

        // Bottom right corner.
        batch.Draw(
            texture,
            bottomRightCorner,
            new Rectangle(
                sourceRect.X + (sourceRect.Width - rightEdgeWidth),
                sourceRect.Y + (sourceRect.Height - bottomEdgeHeight),
                rightEdgeWidth,
                bottomEdgeHeight),
            Color.White);

        // Top edge
        batch.Draw(
            texture,
            topEdge,
            new Rectangle(
                sourceRect.X + leftEdgeWidth,
                sourceRect.Y,
                sourceRect.Width - (leftEdgeWidth + rightEdgeWidth),
                topEdgeHeight),
            Color.White);

        // Bottom edge
        batch.Draw(
            texture,
            bottomEdge,
            new Rectangle(
                sourceRect.X + leftEdgeWidth,
                sourceRect.Y + sourceRect.Height - bottomEdgeHeight,
                sourceRect.Width - (leftEdgeWidth + rightEdgeWidth),
                bottomEdgeHeight),
            Color.White);

        // Left edge
        batch.Draw(
            texture,
            leftEdge,
            new Rectangle(
                sourceRect.X,
                sourceRect.Y + topEdgeHeight,
                leftEdgeWidth,
                sourceRect.Height - (topEdgeHeight + bottomEdgeHeight)),
            Color.White);

        // Right edge
        batch.Draw(
            texture,
            rightEdge,
            new Rectangle(
                sourceRect.X + sourceRect.Width - rightEdgeWidth,
                sourceRect.Y + topEdgeHeight,
                rightEdgeWidth,
                sourceRect.Height - (topEdgeHeight + bottomEdgeHeight)),
            Color.White);

        // And, finally, our centre piece.
        batch.Draw(
            texture,
            centreRect,
            new Rectangle(
                sourceRect.X + leftEdgeWidth,
                sourceRect.Y + topEdgeHeight,
                sourceRect.Width - (leftEdgeWidth + rightEdgeWidth),
                sourceRect.Height - (topEdgeHeight + bottomEdgeHeight)),
            Color.White);
    }
    
    public static void TryPlaySound(string soundCue)
    {
        try
        {
            Game1.soundBank.GetCue(soundCue);
        }
        catch (Exception e) {
            return;
        }

        Game1.playSound(soundCue);
    }

    public static bool TimeIsMultipleOf (uint number){
        return  Game1.ticks % number == 0U;
    }

}
