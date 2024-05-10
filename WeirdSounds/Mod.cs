using StardewModdingAPI;

namespace WeirdSounds;

internal partial class Mod: StardewModdingAPI.Mod
{
    public override void Entry(IModHelper helper)
    {
        WeirdSoundsLibrary.Load(this);
        Helper.Events.Input.ButtonPressed += ButtonPressedEvent;
    }

    private bool EnableMod()
    {
        Patcher.PatchAll(this);
        Helper.Events.Player.Warped += WarpedEvent;
        Helper.Events.Display.MenuChanged += MenuChangedEvent;
        Helper.Events.GameLoop.DayStarted += DayStartedEvent;
        Helper.Events.GameLoop.TimeChanged += TimeChangeEvent;
        Helper.Events.GameLoop.UpdateTicked += UpdateTickedEvent;
        Helper.Events.GameLoop.OneSecondUpdateTicking += OneSecondUpdateTickingEvent;
        return true;
    }

    private bool DisableMod()
    {
        Patcher.UnpatchAll();
        Helper.Events.Player.Warped -= WarpedEvent;
        Helper.Events.Display.MenuChanged -= MenuChangedEvent;
        Helper.Events.GameLoop.DayStarted -= DayStartedEvent;
        Helper.Events.GameLoop.TimeChanged -= TimeChangeEvent;
        Helper.Events.GameLoop.UpdateTicked -= UpdateTickedEvent;
        Helper.Events.GameLoop.OneSecondUpdateTicking -= OneSecondUpdateTickingEvent;
        return true;
    }
}

internal struct mutex
{
    internal static bool DisableMod;

    internal static bool ToolMutex;
        
    internal static bool DeathMutex;

    internal static readonly Dictionary<int, bool> CluckMutex = [];
        
    internal static readonly Dictionary<int, bool> CatFlopDictionary = [];
        
    internal static readonly Dictionary<int, bool> SerpentBarkDictionary = [];

    internal static void DailyClearCache()
    {
        CluckMutex.Clear();
        SerpentBarkDictionary.Clear();
    }
}