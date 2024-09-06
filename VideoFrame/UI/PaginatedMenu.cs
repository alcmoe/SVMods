using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;

namespace VideoFrame.UI;

public class PaginatedMenu : ContainerElement
{
    private UiElement? _previousArrow;
    private UiElement? _nextArrow;
    private readonly List<MenuPage> _pages;
    private readonly List<ContainerElement> _assembledPages = [];
    private readonly Orientation _orientation;
    private readonly Rectangle _upArrowSourceRect = new(76, 72, 40, 44);
    private readonly Rectangle _downArrowSourceRect = new(12, 76, 40, 44);
    private readonly Rectangle _leftArrowSourceRect = new(8, 268, 44, 40);
    private readonly Rectangle _rightArrowSourceRect = new(12, 204, 44, 40);
    private int _currentIndex;
    private string _pageTurnCueName;
    // private Logger logger;

    private int Index
    {
        get => _currentIndex;
        set
        {
            if (value < 0) {
                value = 0;
            }
            if (value > _pages.Count - 1) {
                value = _pages.Count;
            }
            _currentIndex = value;
        }
    }

    public PaginatedMenu(string name, List<MenuPage> pages, Rectangle bounds, DrawableType type = DrawableType.Texture, string pageTurnCue = "bigSelect", Texture2D? texture = null,
        Rectangle? sourceRect = null,
        Color? color = null, int topEdgeSize = 4, int bottomEdgeSize = 4, int leftEdgeSize = 4, int rightEdgeSize = 4,
        Orientation orientation = Orientation.Horizontal) :
        base(name, bounds, type, texture, sourceRect, color, topEdgeSize, bottomEdgeSize, leftEdgeSize, rightEdgeSize)
    {
        _pages = pages;
        _orientation = orientation;
        _pageTurnCueName = pageTurnCue;
        SetupPages(_orientation);
        OrganiseUi(_orientation);
    }

    private void SetupPages(Orientation orientation)
    {
        var widestPage = 0;
        var tallestPage = 0;
        _assembledPages.Clear();

        foreach (MenuPage page in _pages)
        {
            if (widestPage < page.TotalWidth)
                widestPage = page.TotalWidth;

            if (tallestPage < page.TotalHeight)
                tallestPage = page.TotalHeight;

            if (orientation == Orientation.Horizontal)
            {
                var horizontalPage = new VBoxElement(
                    "Page",
                    Rectangle.Empty
                );
                horizontalPage.AddChild(page.Page);
                horizontalPage.AddChild(page.PageText);
                _assembledPages.Add(horizontalPage);
            }
            else
            {
                var verticalPage = new VBoxElement(
                    "Page",
                    Rectangle.Empty
                );
                verticalPage.AddChild(page.Page);
                verticalPage.AddChild(page.PageText);
                _assembledPages.Add(verticalPage);
            }
        }

        // Now we set our new width and height.
        Width = widestPage;
        Height = tallestPage;

        // And centre ourselves on the screen.
        var centrePosition = Utility.getTopLeftPositionForCenteringOnScreen(Width, Height);
        X = (int)centrePosition.X;
        Y = (int)centrePosition.Y;
    }

    private void OrganiseUi(Orientation orientation)
    {
        switch (orientation)
        {
            case Orientation.Horizontal:
                var horizArrowWidth = _leftArrowSourceRect.Width;
                var horizArrowHeight = _leftArrowSourceRect.Height;
                var leftArrowBounds = new Rectangle(
                    (int)BottomLeftCorner.X - horizArrowWidth, (int)BottomLeftCorner.Y,
                    horizArrowWidth, horizArrowHeight
                );
                var rightArrowBounds = new Rectangle(
                    (int)BottomRightCorner.X, (int)BottomRightCorner.Y,
                    horizArrowWidth, horizArrowHeight
                );

                if (_previousArrow == null || _nextArrow == null)
                {
                    _previousArrow = new UiElement(
                        "Previous Arrow",
                        leftArrowBounds,
                        DrawableType.Texture,
                        Game1.mouseCursors,
                        _leftArrowSourceRect,
                        scale: 1,
                        drawShadow: true);
                    _nextArrow = new UiElement(
                        "Previous Arrow",
                        rightArrowBounds,
                        DrawableType.Texture,
                        Game1.mouseCursors,
                        _rightArrowSourceRect,
                        scale: 1,
                        drawShadow: true);

                    _previousArrow.LeftClickCallback = PreviousArrowClicked;
                    _nextArrow.LeftClickCallback = NextArrowClicked;
                }
                else
                {
                    _previousArrow.Bounds = leftArrowBounds;
                    _nextArrow.Bounds = rightArrowBounds;
                }

                break;
            case Orientation.Vertical:
                int vertArrowWidth = _upArrowSourceRect.Width;
                int vertArrowHeight = _upArrowSourceRect.Height;
                Rectangle upArrowBounds = new Rectangle(
                    (int)TopRightCorner.X + vertArrowWidth, (int)TopRightCorner.Y - vertArrowHeight,
                    vertArrowWidth, vertArrowHeight
                );
                Rectangle downArrowBounds = new Rectangle(
                    (int)BottomRightCorner.X + vertArrowWidth, (int)BottomRightCorner.Y,
                    vertArrowWidth, vertArrowHeight
                );

                if (_previousArrow == null || _nextArrow == null)
                {
                    _previousArrow = new UiElement(
                        "Previous Arrow",
                        upArrowBounds,
                        DrawableType.Texture,
                        Game1.mouseCursors,
                        _upArrowSourceRect,
                        scale: 1);
                    _nextArrow = new UiElement(
                        "Previous Arrow",
                        downArrowBounds,
                        DrawableType.Texture,
                        Game1.mouseCursors,
                        _downArrowSourceRect,
                        scale: 1);
                    _previousArrow.LeftClickCallback = PreviousArrowClicked;
                    _nextArrow.LeftClickCallback = NextArrowClicked;
                }
                else
                {
                    _previousArrow.Bounds = upArrowBounds;
                    _nextArrow.Bounds = downArrowBounds;
                }
                break;
        }

        var centrePosition = Utility.getTopLeftPositionForCenteringOnScreen(Width, Height);
        X = (int)centrePosition.X;
        Y = (int)centrePosition.Y;
    }

    private void PreviousArrowClicked()
    {
        
        if (_currentIndex > 0)
        {
            // if (!Utilities.Sound.TryPlaySound(this.pageTurnCueName))
                // this.logger.Error($"Oops! I failed while trying to play sound cue {this.pageTurnCueName} in {Game1.currentLocation.Name}. Is it a valid cue?");
        }

        Index--;
    }

    private void NextArrowClicked()
    {
        if (_currentIndex < _pages.Count - 1)
        {
            // if (!Utilities.Sound.TryPlaySound(this.pageTurnCueName))
                // this.logger.Error($"Oops! I failed while trying to play sound cue {this.pageTurnCueName} in {Game1.currentLocation.Name}. Is it a valid cue?");
        }

        Index++;
    }

    internal override void OrganiseChildren()
    {
        SetupPages(_orientation);
        OrganiseUi(_orientation);

        base.OrganiseChildren();
    }

    public override void ReceiveLeftClick(int x, int y)
    {
        if (_previousArrow!.Bounds.Contains(x, y))
            _previousArrow.ReceiveLeftClick(x, y);

        if (_nextArrow!.Bounds.Contains(x, y))
            _nextArrow!.ReceiveLeftClick(x, y);

        base.ReceiveLeftClick(x, y);
    }

    public override void ReceiveScrollWheel(int direction)
    {
        if (direction < 0)
            _nextArrow!.LeftClickCallback?.Invoke();
        else
            _previousArrow!.LeftClickCallback?.Invoke();
    }

    internal override void Draw(SpriteBatch spriteBatch)
    {
        _previousArrow!.Draw(spriteBatch, _currentIndex == 0 ? Color.DarkGray : Color.White);

        _nextArrow!.Draw(spriteBatch, _currentIndex == _pages.Count - 1 ? Color.DarkGray : Color.White);

        if (_pages.Count >= 1)
            _pages[_currentIndex].Draw(spriteBatch);

        base.Draw(spriteBatch);
    }
}
