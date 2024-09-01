using StardewModdingAPI;

namespace DontStarve;

internal partial class Mod: StardewModdingAPI.Mod
{
    
    private const string BuffHealHealth = "DS_Heal_Health";
    private const string BuffHealStamina = "DS_Heal_Stamina";

    
    public override void Entry(IModHelper helper)
    {
        EnableMod();
        Patcher.PatchAll(this);
    }

    private void EnableMod()
    {
        Helper.Events.GameLoop.OneSecondUpdateTicking += OneSecondUpdateTickingForBuffEvent;
        Helper.Events.GameLoop.OneSecondUpdateTicking += OneSecondUpdateTickingEvent;
    }
}