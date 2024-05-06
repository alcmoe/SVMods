using HarmonyLib;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Audio;
using StardewValley.Menus;
using SObject = StardewValley.Object;

namespace WeirdSounds
{
    internal class Patcher
    {
        internal static void PatchAll(IMod mod)
        {
            var harmony = new Harmony(mod.ModManifest.UniqueID);
            harmony.PatchAll();
            harmony.Patch(
                original: AccessTools.Method(typeof(SoundsHelper), nameof(SoundsHelper.PlayLocal)),
                prefix: new HarmonyMethod(typeof(Patcher), nameof(PlayLocalPrefix))
            );
            harmony.Patch(
                original: AccessTools.Method(typeof(FarmAnimal), nameof(FarmAnimal.pet)),
                prefix: new HarmonyMethod(typeof(Patcher), nameof(PetPrefix))
            );
            harmony.Patch(
                original: AccessTools.Method(typeof(SObject), nameof(SObject.PlaceInMachine)),
                postfix: new HarmonyMethod(typeof(Patcher), nameof(PlaceInMachinePostfix))
            );
            harmony.Patch(
                original: AccessTools.Method(typeof(ClickableComponent), nameof(ClickableComponent.containsPoint), [typeof(int), typeof(int)]),
                postfix: new HarmonyMethod(typeof(Patcher), nameof(ContainsPointPostfix))
            );        
            harmony.Patch(
                original: AccessTools.Method(typeof(GameLocation), nameof(GameLocation.answerDialogueAction)),
                postfix: new HarmonyMethod(typeof(Patcher), nameof(AnswerDialogueActionPostfix))
            );
        }

        private static bool _rescuedAfterDeath;
        private static string CueName(string key)
        {
            return WeirdSoundsLibrary.GetCueName(key);
        }
        
        private static bool PlayLocalPrefix(ISoundsHelper __instance, ref string cueName, GameLocation location, Vector2? position, int? pitch, SoundContext context)
        {
            switch (cueName) {
                case "hammer":
                case "axchop":
                case "woodyHit":
                case "clubswipe":
                case "clubSmash":
                case "swordswipe":
                case "daggerswipe":
                    if (Game1.player.UsingTool) {
                        __instance.PlayLocal(CueName("tool"), location, position, pitch, context, out _);
                    }
                    break;
                case "cancel":
                    if (Game1.activeClickableMenu is PurchaseAnimalsMenu animalsMenu) {
                        var buildingAt = animalsMenu.TargetLocation.getBuildingAt(new Vector2((int) ((Utility.ModifyCoordinateFromUIScale(Game1.getMouseX()) + (double) Game1.viewport.X) / 64.0), (int) ((Utility.ModifyCoordinateFromUIScale(Game1.getMouseY()) + (double) Game1.viewport.Y) / 64.0)));
                        if (animalsMenu.animalBeingPurchased.CanLiveIn(buildingAt)) {
                            __instance.PlayLocal(CueName("noMoreAnimals"), location, position, pitch, context, out _); // building is full
                        } else {
                            __instance.PlayLocal(CueName("cancel"), location, position, pitch, context, out _); //wrong building for animal
                        }
                    } else {
                        __instance.PlayLocal(CueName("cancel"), location, position, pitch, context, out _);
                    }
                    break;
                case "sell": 
                    if (Game1.activeClickableMenu is ShopMenu) { //debug
                        __instance.PlayLocal(CueName("sell"), location, position, pitch, context, out _);
                    }
                    break;
                case "trashcan":
                    __instance.PlayLocal(CueName("trashcan"), location, position, pitch,context, out _);
                    break;
                case "batFlap":
                    var player = Game1.player;
                    if (player.ActiveItem is StardewValley.Tools.MeleeWeapon sword && sword.type.Value == StardewValley.Tools.MeleeWeapon.defenseSword) {
                        int[] an = [252, 243, 259, 234];
                        if (an.Any(p => p == player.FarmerSprite.currentSingleAnimation)) { 
                            __instance.PlayLocal(CueName("defense"), location, position, pitch, context, out _);
                            var toolLocation = player.GetToolLocation();
                            var zero1 = Vector2.Zero;
                            var zero2 = Vector2.Zero;
                            var areaOfEffect = sword.getAreaOfEffect((int)toolLocation.X, (int)toolLocation.Y, player.FacingDirection, ref zero1, ref zero2, player.GetBoundingBox(), 1);
                            areaOfEffect.Inflate(50, 50);
                            if (player.currentLocation.getAllFarmAnimals().Any(animal => animal.GetBoundingBox().Intersects(areaOfEffect))) {
                                __instance.PlayLocal(CueName("defenseHa"), location, position, pitch, context, out _);
                            }
                        }
                    }
                    break;
                case "death":
                    if (Game1.player.health <= 0) {
                        __instance.PlayLocal(CueName("death"), location, position, pitch, context, out _);
                    }
                    break;
                case "breathout":
                    if (Game1.player.health == 10 && Game1.player.Sprite.currentFrame == 5) {
                        _rescuedAfterDeath = true;
                    }
                    break;
                case "bigDeSelect":
                    if (Game1.activeClickableMenu is LevelUpMenu { isProfessionChooser: false }) { //selected profession
                        Game1.playSound(CueName("ok"));
                    }   
                    break;
            }
            return true;
        }
        
        private static bool PetPrefix(FarmAnimal __instance, bool is_auto_pet)
        {
            if (is_auto_pet || !__instance.type.Value.EndsWith("Chicken") || __instance.wasPet.Value || Game1.options.muteAnimalSounds) {
                return true;
            }
            Game1.playSound(CueName("cluck"));
            return true;
        }
        
        private static void PlaceInMachinePostfix(SObject __instance, ref bool __result, bool probe)
        {
            if (__result && !probe) {
                __instance.Location.playSound(CueName("machine"), __instance.TileLocation);
            }
        }
        
        private static void ContainsPointPostfix(ClickableComponent __instance, ref bool __result)
        {
            if (!__result || __instance is not ClickableTextureComponent ok || !Game1.didPlayerJustLeftClick()) {
                return;
            }
            if (!ok.texture.Name.StartsWith("LooseSprites/Cursors")) {
                return;
            }
            if (ok.sourceRect is not { Left: 128, Top: 256, Height: 64, Width: 64 }){
                return;
            }
            if (_rescuedAfterDeath) {
                Game1.playSound(CueName("back2fight"));
                _rescuedAfterDeath = false;
            } else {
                Game1.playSound(CueName("ok"));
            }
        } 
        
        private static void AnswerDialogueActionPostfix(string questionAndAnswer)
        {
            if (questionAndAnswer == "TroutDerbyBooth_Rewards") {
                if (Game1.delayedActions.Any(action => action.stringData == "getNewSpecialItem")) {
                    Game1.playSound(CueName("reward"));
                }
            } else if (questionAndAnswer.ToLower().Contains("rewards")) {
                Game1.playSound(CueName("reward"));
            }
            if (questionAndAnswer.ToLower() == "sleep_yes") {
                Game1.playSound(CueName("goodNight"));
            }
        }
    }
}