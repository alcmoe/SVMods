using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Menus;

namespace VideoFrame.UI;

public class MenuBase : IClickableMenu
{
    // Every base menu needs a container, and only a container.
    private readonly ContainerElement _uiContainer;
    private readonly string _menuName;
    private string _openSound;

    public MenuBase(ContainerElement uiContainer, string name, string openSound = "bigSelect")
    {
        _uiContainer = uiContainer;
        xPositionOnScreen = 0;
        yPositionOnScreen = 0;
        width = Game1.uiViewport.Width;
        height = Game1.uiViewport.Height;
        _uiContainer.TextureTint = Color.White;
        _menuName = name;
        _openSound = openSound;
        UpdateCloseButton(_uiContainer.TopRightCorner);
    }

    public void MenuOpened()
    {
        // if (!Utilities.Sound.TryPlaySound(this.openSound))
            // this.logger.Error($"Oops! I failed while trying to play sound cue {this.openSound} in {Game1.currentLocation.Name}. Is it a valid cue?");
    }

    public override void draw(SpriteBatch b)
    {
        _uiContainer.Draw(b);
        upperRightCloseButton.draw(b);
        drawMouse(b);
    }

    public override void gameWindowSizeChanged(Rectangle oldBounds, Rectangle newBounds)
    {
        xPositionOnScreen = 0;
        yPositionOnScreen = 0;
        width = Game1.uiViewport.Width;
        height = Game1.uiViewport.Height;
        _uiContainer.OrganiseChildren();
    }

    public override void receiveLeftClick(int x, int y, bool playSound = true)
    {
        if (upperRightCloseButton.containsPoint(x, y))
            exitThisMenu();

        _uiContainer.ReceiveLeftClick(x, y);
    }

    public override void receiveRightClick(int x, int y, bool playSound = true)
    {
        _uiContainer.ReceiveRightClick(x, y);
    }

    public override void receiveScrollWheelAction(int direction)
    {
        _uiContainer.ReceiveScrollWheel(direction);
    }

    public void UpdateCloseButton(Vector2 topRightCorner)
    {
        upperRightCloseButton = new ClickableTextureComponent(new Rectangle((int)topRightCorner.X, (int)topRightCorner.Y, 48, 48), Game1.mouseCursors, new Rectangle(337, 494, 12, 12), 4f);
    }

    public override void releaseLeftClick(int x, int y)
    {
        base.releaseLeftClick(x, y);
    }

    public override void emergencyShutDown()
    {
        base.emergencyShutDown();
    }
}
