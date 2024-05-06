using StardewModdingAPI;

namespace WeirdSounds;

internal partial class Mod: StardewModdingAPI.Mod
{
    public override void Entry(IModHelper helper)
    {
        Patcher.PatchAll(this);
        WeirdSoundsLibrary.Load(this);
        Helper.Events.GameLoop.TimeChanged += TimeChangeEvent;
        Helper.Events.Display.MenuChanged += MenuChangedEvent;
        Helper.Events.GameLoop.DayStarted += DayStartedEvent;
    }
}