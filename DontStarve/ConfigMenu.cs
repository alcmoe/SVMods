using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewValley;

namespace DontStarve
{
    internal partial class Mod
    {
        private ModConfig _config = null!;
        public class ModConfig
        {
            public bool EnableMod { get; set; } = true;
        }

        private void GameLaunchedEvent(object? sender, StardewModdingAPI.Events.GameLaunchedEventArgs e)
        {
            _config = Helper.ReadConfig<ModConfig>();
            UpdateModStatus();
            var configMenu = Helper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");
            if (configMenu is null)
                return;
            configMenu.Register(
                mod: ModManifest,
                reset: ResetConfig,
                save: WriteConfig
            );
            configMenu.AddBoolOption(
                mod: ModManifest,
                name: () => Helper.Translation.Get("config.EnableMod"),
                getValue: () => _config.EnableMod,
                setValue: value => _config.EnableMod = value
            );
        }

        private void WriteConfig()
        {
            if (Game1.IsMultiplayer) {
                if (!Game1.player.IsMainPlayer) {
                    Game1.showRedMessage(Helper.Translation.Get("config.CannotEditConfigAsAClientPlayer"));
                    Game1.chatBox.addMessage(Helper.Translation.Get("config.CannotEditConfigAsAClientPlayer"), Color.Red);
                    return;
                }
                Helper.Multiplayer.SendMessage(_config, ToggleStatus, modIDs: [ModManifest.UniqueID]);
                Game1.chatBox.addMessage(Helper.Translation.Get("config.BroadcastConfigToAllPlayers"), Color.Blue);
            }
            UpdateModStatus();
            Helper.WriteConfig(_config);
        }

        private void ResetConfig()
        {
            if (Game1.IsMultiplayer) {
                if (!Game1.player.IsMainPlayer) {
                    return;
                }
            }
            _config = new ModConfig();
            UpdateModStatus();
        }
    }

    public interface IGenericModConfigMenuApi
    {
        void Register(IManifest mod, Action reset, Action save, bool titleScreenOnly = false);
        
        void AddBoolOption(IManifest mod, Func<bool> getValue, Action<bool> setValue, Func<string> name, Func<string>? tooltip = null, string? fieldId = null);
    }
}