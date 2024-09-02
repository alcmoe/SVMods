using StardewModdingAPI;

namespace DontStarve;

internal partial class Mod: StardewModdingAPI.Mod
{
    
    private const string BuffHealHealth = "DS_Heal_Health";
    private const string BuffHealStamina = "DS_Heal_Stamina";
    private bool _enabled;

    
    public override void Entry(IModHelper helper)
    {
        Helper.Events.Multiplayer.ModMessageReceived += ModMessageReceivedEvent;
        Helper.Events.GameLoop.GameLaunched += GameLaunchedEvent;
        // if (_config.EnableMod) {
        //     EnableMod();
        // }
    }

    private void EnableMod()
    {
        if (_enabled) {
            return;
        }
        Patcher.PatchAll(this);
        Helper.Events.GameLoop.OneSecondUpdateTicking += OneSecondUpdateTickingForBuffEvent;
        Helper.Events.GameLoop.OneSecondUpdateTicking += OneSecondUpdateTickingEvent;
        _enabled = true;
    }

    private void DisableMod()
    {
        if (!_enabled) {
            return;
        }
        Patcher.UnpatchAll();
        Helper.Events.GameLoop.OneSecondUpdateTicking -= OneSecondUpdateTickingForBuffEvent;
        Helper.Events.GameLoop.OneSecondUpdateTicking -= OneSecondUpdateTickingEvent;
        _enabled = false;
    }
}