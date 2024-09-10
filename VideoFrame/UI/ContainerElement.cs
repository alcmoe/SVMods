using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace VideoFrame.UI;

public class ContainerElement : UiElement
{
    internal readonly List<UiElement> ChildElements = [];
    internal int ContainerMargin;
    private MenuBase? _parentMenu;
    protected bool DrawUi = true;
    
    internal ContainerElement(string name, Rectangle bounds, DrawableType type = DrawableType.SlicedBox, Texture2D? texture = null, Rectangle? sourceRect = null,
        Color? color = null,
        int topEdgeSize = 16, int bottomEdgeSize = 12, int leftEdgeSize = 12, int rightEdgeSize = 16,
        int containerMargin = 4)
        : base(name, bounds, type, texture, sourceRect, color, false,
            topEdgeSize, bottomEdgeSize, leftEdgeSize, rightEdgeSize)
    {
        Bounds = bounds;
        TextureTint = color ?? Color.White;

        ContainerMargin = containerMargin;
    }

    internal virtual void AddChild(UiElement child)
    {
        if (!ChildElements.Contains(child))
        {
            ChildElements.Add(child);
            child.Parent = this;
        }
        OrganiseChildren();
    }

    internal virtual void Draw(SpriteBatch spriteBatch)
    {
        base.Draw(spriteBatch);

        foreach (UiElement child in ChildElements)
        {
            child.Draw(spriteBatch);
        }
    }

    public override void ReceiveLeftClick(int x, int y)
    {
        foreach (UiElement child in ChildElements)
        {
            child.ReceiveLeftClick(x, y);
        }

        base.ReceiveLeftClick(x, y);
    }

    public override void ReceiveRightClick(int x, int y)
    {
        foreach (UiElement child in ChildElements)
        {
            child.ReceiveRightClick(x, y);
        }

        base.ReceiveRightClick(x, y);
    }
    
    

    public void SetParent(MenuBase parent)
    {
        _parentMenu = parent;
        OrganiseChildren();
    }

    public void SetDrawUi(bool value)
    {
        DrawUi = value;
        _parentMenu!.SetDrawUi(value);
    }

    internal override void OrganiseChildren()
    {
        SourceRect = Bounds;
        _parentMenu?.UpdateCloseButton(TopRightCorner);
    }
}
