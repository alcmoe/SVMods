using StardewModdingAPI;
using StardewModdingAPI.Events;

namespace DontStarve
{
    internal partial class Mod
    {
        private const string ToggleStatus = "ToggleStatus";
        private const string AskModStatus = "AskStatus";
        private void ModMessageReceivedEvent(object? sender, ModMessageReceivedEventArgs e)
        {
            if (e.FromModID != ModManifest.UniqueID) {
                return;
            }
            if (Context.IsMainPlayer) {
                if (e.Type == AskModStatus) {
                    Helper.Multiplayer.SendMessage(_config, ToggleStatus, modIDs: [ModManifest.UniqueID]);
                }
            } else {
                if (e.Type == ToggleStatus) {
                    _config = e.ReadAs<ModConfig>();
                    UpdateModStatus();
                }
            }
        }
        
        private void SaveLoadedEvent(object? sender, SaveLoadedEventArgs e)
        {
            if (!Context.IsMainPlayer) {
                Helper.Multiplayer.SendMessage(_config, AskModStatus, modIDs: [ModManifest.UniqueID]);
            }
        }
    }
}