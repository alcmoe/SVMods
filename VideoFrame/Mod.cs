using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley;
using VideoFrame.Frame;
using VideoFrame.UI;

namespace VideoFrame;

internal partial class Mod: StardewModdingAPI.Mod
{
    private readonly List<Texture2D> Tl = [];
    
    public override void Entry(IModHelper helper)
    {
        Helper.Events.GameLoop.OneSecondUpdateTicking += OneSecondUpdateTickingEvent;
        Helper.Events.Input.ButtonPressed += ButtonPressedEvent;
        ;
        for (var i = 1; i <= 90; i++) {
            Tl.Add(Helper.ModContent.Load<Texture2D>("assets/CG/CG" + i + ".png"));
        }
    }
    
    private  void OneSecondUpdateTickingEvent(object? sender, StardewModdingAPI.Events.OneSecondUpdateTickingEventArgs e)
    {
        if (!Context.IsWorldReady || !Game1.shouldTimePass()) {
            return;
        }
        
    }
    
    private void ButtonPressedEvent(object? sender, StardewModdingAPI.Events.ButtonPressedEventArgs e)
    {
        if (!Context.IsWorldReady) {
            return;
        }
        if (e.Button == SButton.F8) {
            var menu = AlbumFrame.Get(Tl);
            Game1.activeClickableMenu = menu;
            menu.MenuOpened();
        }
    }       
    
}