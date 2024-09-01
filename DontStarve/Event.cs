using StardewModdingAPI;
using StardewValley;

namespace DontStarve
{
    internal partial class Mod
    {
        private static void OneSecondUpdateTickingForBuffEvent(object? sender, StardewModdingAPI.Events.OneSecondUpdateTickingEventArgs e)
        {
            if (!Context.IsWorldReady || !Game1.shouldTimePass()) {
                return;
            }
            var player = Game1.player;
            if (player.hasBuff(BuffHealHealth)) {
                var heal = Math.Min(2, player.maxHealth - player.health);
                if (e.IsMultipleOf(120)) {
                    player.health += heal;
                }
            }
            if (player.hasBuff(BuffHealStamina)) {
                var heal = Math.Min(2, player.MaxStamina - player.stamina);
                if (e.IsMultipleOf(120)) {
                    player.stamina += heal;
                }
            }
        }       
        
        private static void OneSecondUpdateTickingEvent(object? sender, StardewModdingAPI.Events.OneSecondUpdateTickingEventArgs e)
        {
            if (!Context.IsWorldReady || !Game1.shouldTimePass()) {
                return;
            }
            var player = Game1.player;
            if (player.stamina > 0f) {
                if (e.IsMultipleOf(240)) {
                    player.stamina -= 1f;
                }
            }
            if (player is { stamina: <= 0f, health: > 0 }) {
                if (e.IsMultipleOf(120)) {
                    player.health -= 1;
                }
            }
        }
    }
}