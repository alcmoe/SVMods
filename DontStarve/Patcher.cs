using HarmonyLib;
using StardewModdingAPI;
using StardewValley;

namespace DontStarve
{
    internal class Patcher
    {
        private static Harmony _harmony = null!;
        private static IMod _mod = null!;

        private struct PlayerStatus
        {
            internal static float SleepStamina;
            internal static int SleepHealth;
            internal static int SleepTime;
            internal static bool SleepExhausted;
            internal static int SwimHealth;
            internal static float RestStamina;
            internal static int RestHealth;
        }
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
            _harmony.Patch(
                original: AccessTools.Method(typeof(Farmer), "updateCommon"),
                prefix: new HarmonyMethod(typeof(Patcher), nameof(FarmerUpdateCommonPrefix))
            );
            _harmony.Patch(
                original: AccessTools.Method(typeof(Farmer), "updateCommon"),
                postfix: new HarmonyMethod(typeof(Patcher), nameof(FarmerUpdateCommonPostfix))
            );
            _harmony.Patch(
                original: AccessTools.Method(typeof(Farmer), nameof(Farmer.Update)),
                prefix: new HarmonyMethod(typeof(Patcher), nameof(FarmerUpdatePrefix))
            );
            _harmony.Patch(
                original: AccessTools.Method(typeof(Farmer), nameof(Farmer.Update)),
                postfix: new HarmonyMethod(typeof(Patcher), nameof(FarmerUpdatePostfix))
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
            PlayerStatus.SleepStamina = Game1.player.Stamina;
            PlayerStatus.SleepHealth = Game1.player.health;
            PlayerStatus.SleepExhausted = Game1.player.exhausted.Value;
            PlayerStatus.SleepTime = timeWentToSleep;
            return true;
        }
        
        private static void DayUpdatePostfix()
        {
            var player = Game1.player;
            int reduceStamina;
            if (PlayerStatus.SleepTime >= 2400) {
                reduceStamina = 42;
                _mod.Monitor.Log("reduce " + reduceStamina);
            } else {
                var mul = PlayerStatus.SleepExhausted ? 4 : 6;
                var time = 3000 - PlayerStatus.SleepTime;
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
            PlayerStatus.SleepStamina = Math.Max(0, PlayerStatus.SleepStamina - reduceStamina);
            if (PlayerStatus.SleepStamina > 10) {
                var staminaForHeal = Math.Min(PlayerStatus.SleepStamina - 10, (player.maxHealth - PlayerStatus.SleepHealth) * 2);
                PlayerStatus.SleepStamina -= staminaForHeal;
                if (!PlayerStatus.SleepExhausted) {
                    player.health = PlayerStatus.SleepHealth + Math.Min(player.maxHealth - PlayerStatus.SleepHealth, (int)(staminaForHeal / 2));
                }
            }
            player.stamina = PlayerStatus.SleepStamina;
        }        
        
        private static bool FarmerUpdateCommonPrefix(Farmer __instance)
        {
            PlayerStatus.SwimHealth = __instance.health;
            return true;
        }
        
        private static void FarmerUpdateCommonPostfix(Farmer __instance)
        {
            if (__instance.swimming.Value && __instance.swimTimer == 100) {
                __instance.swimTimer = 1000;
                __instance.health = PlayerStatus.SwimHealth;
            }
        }
        
        private static bool FarmerUpdatePrefix(Farmer __instance)
        {
            PlayerStatus.RestStamina = __instance.Stamina;
            PlayerStatus.RestHealth = __instance.health;
            return true;
        }
        
        private static void FarmerUpdatePostfix(Farmer __instance)
        {
            if (__instance.regenTimer != 500) {
                return;
            }
            __instance.stamina = PlayerStatus.RestStamina;
            __instance.health = PlayerStatus.RestHealth;
        }
    }
}