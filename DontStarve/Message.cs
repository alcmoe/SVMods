using StardewModdingAPI;
using StardewModdingAPI.Events;

namespace DontStarve
{
    internal partial class Mod
    {
        private const string ToggleStatus = "ToggleStatus";
        private void ModMessageReceivedEvent(object? sender, ModMessageReceivedEventArgs e)
        {
            if (e.FromModID != ModManifest.UniqueID) {
                return;
            }
            if (!Context.IsMainPlayer) {
                if (e.Type == ToggleStatus) {
                    var toggle = e.ReadAs<bool>();
                    switch (toggle) {
                        case true when !_enabled:
                            EnableMod();
                            break;
                        case false when _enabled:
                            DisableMod();
                            break;
                    }
                }
            } 
        }
    }
}