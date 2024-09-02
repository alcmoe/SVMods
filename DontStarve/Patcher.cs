using HarmonyLib;
using StardewModdingAPI;
using StardewValley;

namespace DontStarve
{
    internal class Patcher
    {
        private static Harmony _harmony = null!;
        private static IMod _mod = null!;
        private static float _stamina;
        private static int _health;
        private static int _timeForSleep1;
        private static bool _exhausted;
        internal static void PatchAll(IMod mod)
        {
            _mod = mod;
            _harmony = new Harmony(mod.ModManifest.UniqueID);
            _harmony.Patch(
                original: AccessTools.Method(typeof(GameLocation), "startSleep"),
                prefix: new HarmonyMethod(typeof(Patcher), nameof(StartSleepPrefix))
            );
            _harmony.Patch(
                original: AccessTools.Method(typeof(Farmer), nameof(Farmer.dayupdate)),
                prefix: new HarmonyMethod(typeof(Patcher), nameof(DayUpdatePrefix))
            );
            _harmony.Patch(
                original: AccessTools.Method(typeof(Farmer), nameof(Farmer.dayupdate)),
                postfix: new HarmonyMethod(typeof(Patcher), nameof(DayUpdatePostfix))
            );
        }

        internal static void UnpatchAll()
        {
            _harmony.UnpatchAll();
        }
            
        private static bool StartSleepPrefix()
        {
            if (Game1.timeOfDay >= 2200) {
                return true;
            }
            Game1.showRedMessage(_mod.Helper.Translation.Get("popup.CancelSleepBefore2200"));
            return false;
        }
        
        private static bool DayUpdatePrefix(int timeWentToSleep)
        {
            _stamina = Game1.player.Stamina;
            _health = Game1.player.health;
            _exhausted = Game1.player.exhausted.Value;
            _timeForSleep1 = timeWentToSleep;
            return true;
        }
        
        private static void DayUpdatePostfix()
        {
            var player = Game1.player;
            int reduceStamina;
            if (_timeForSleep1 >= 2400) {
                reduceStamina = 42;
                _mod.Monitor.Log("reduce " + reduceStamina);
            } else {
                var mul = _exhausted ? 4 : 6;
                var time = 3000 - _timeForSleep1;
                var timeSec = time % 100;
                var timeMin = time / 100;
                reduceStamina = (timeSec / 10 + timeMin * 6) * 7 / mul;
                _mod.Monitor.Log("reduce " + reduceStamina + "on" + time + " for S " + timeSec + " H " + timeMin);
            }
            if (player.isInBed.Value) {
                player.exhausted.Value = true;
            } else {
                reduceStamina = 84;
                _mod.Monitor.Log("reduce " + reduceStamina);
            }
            _stamina = Math.Max(0, _stamina - reduceStamina);
            if (_stamina > 10) {
                var staminaForHeal = Math.Min(_stamina - 10, (player.maxHealth - _health) * 2);
                _stamina -= staminaForHeal;
                if (!_exhausted) {
                    player.health = _health + Math.Min(player.maxHealth - _health, (int)(staminaForHeal / 2));
                }
            }
            player.stamina = _stamina;
        }
    }
}