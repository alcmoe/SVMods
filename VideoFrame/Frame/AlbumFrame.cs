using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using VideoFrame.UI;

namespace VideoFrame.Frame;

public static class AlbumFrame
{
    public static MenuBase Get(List<Texture2D> album)
    {
        List<MenuPage> pages = [];
        foreach (var picture in album) {
            var picElement = new ImageElement(picture.Name, picture.Bounds, DrawableType.Texture, picture, picture.Bounds);
            var text = new TextElement("page text title", Rectangle.Empty, null, 1000, picture.Name);
            pages.Add(new MenuPage(picElement,text));
        }
        var albumMenu = new AlbumMenu("album", pages, new Rectangle(Game1.uiViewport.X, Game1.uiViewport.Y, Game1.uiViewport.Width, Game1.uiViewport.Height), 0.5f);
        var menu = new MenuBase(albumMenu, "menu");
        albumMenu.SetParent(menu);
        return menu;
    }
    
}