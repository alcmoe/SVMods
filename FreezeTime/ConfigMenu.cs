using Microsoft.Xna.Framework;
using StardewModdingAPI;

namespace FreezeTime
{
    public partial class FreezeTime
    {
        private ModConfig _config = new();
        public class ModConfig
        {
            public string PauseLogic { get; set; } = "All";

            public bool Any()
            {
                return PauseLogic == "Any";
            }
        }

        private void GameLaunchedEvent(object? sender, StardewModdingAPI.Events.GameLaunchedEventArgs e)
        {
            _config = Helper.ReadConfig<ModConfig>();
            var configMenu = Helper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");
            if (configMenu is null)
                return;
            configMenu.Register(
                mod: ModManifest,
                reset: () => _config = new ModConfig(),
                save: WriteConfig
            );
            configMenu.AddTextOption(
                mod: this.ModManifest,
                name: () => "PauseLogic",
                getValue: () => _config.PauseLogic,
                setValue: value => _config.PauseLogic = value,
                allowedValues: ["All", "Any"]
            );
        }

        private void WriteConfig()
        {
            if (StardewValley.Game1.IsMultiplayer) {
                if (!StardewValley.Game1.player.IsMainPlayer) {
                    StardewValley.Game1.chatBox.addMessage("u can not change pauseLogic as a client player!", Color.Red);
                    return;
                }
                BroadcastConfig();
                StardewValley.Game1.chatBox.addMessage("Broadcasting config to  all clients", Color.Blue);
            }
            Helper.WriteConfig(_config);
        }
    }

    public interface IGenericModConfigMenuApi
    {
        void Register(IManifest mod, Action reset, Action save, bool titleScreenOnly = false);
        void AddTextOption(IManifest mod, Func<string> getValue, Action<string> setValue, Func<string> name, Func<string>? tooltip = null, string[]? allowedValues = null, Func<string, string>? formatAllowedValue = null, string? fieldId = null);
    }
}