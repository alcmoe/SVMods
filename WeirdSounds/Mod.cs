using HarmonyLib;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Audio;
using StardewValley.Menus;
using StardewValley.Tools;
using Object = StardewValley.Object;

namespace WeirdSounds;

public class Mod: StardewModdingAPI.Mod
{

    private static Dictionary<string, List<string>> _slb = [];
    
    public override void Entry(IModHelper helper)
    {
        var harmony = new Harmony(ModManifest.UniqueID);
        harmony.Patch(
            original: AccessTools.Method(typeof(SoundsHelper), nameof(SoundsHelper.PlayLocal)),
            prefix: new HarmonyMethod(typeof(Game1Patcher), nameof(Game1Patcher.PlayLocalPrefix))
        );
        harmony.Patch(
            original: AccessTools.Method(typeof(FarmAnimal), nameof(FarmAnimal.pet)),
            prefix: new HarmonyMethod(typeof(Game1Patcher), nameof(Game1Patcher.PetPrefix))
        );
        harmony.Patch(
            original: AccessTools.Method(typeof(Object), nameof(Object.PlaceInMachine)),
            postfix: new HarmonyMethod(typeof(Game1Patcher), nameof(Game1Patcher.PlayEffectsPostfix))
        );
        harmony.Patch(
            original: AccessTools.Method(typeof(ClickableComponent), nameof(ClickableComponent.containsPoint), [typeof(int), typeof(int)]),
            postfix: new HarmonyMethod(typeof(Game1Patcher), nameof(Game1Patcher.ContainsPointPostfix))
        );        
        harmony.Patch(
            original: AccessTools.Method(typeof(GameLocation), nameof(GameLocation.answerDialogueAction)),
            postfix: new HarmonyMethod(typeof(Game1Patcher), nameof(Game1Patcher.AnswerDialogueActionPostfix))
        );
        // harmony.Patch(
        //     original: AccessTools.Method(typeof(Tool), nameof(Tool.endUsing)),
        //     prefix: new HarmonyMethod(typeof(Game1Patcher), nameof(Game1Patcher.EndUsingToolPrefix))
        // );
        LoadWeirdSounds(helper);
        
        Helper.Events.GameLoop.TimeChanged += TimeChangeEvent;
        Helper.Events.Display.MenuChanged += MenuChangedEvent;
        helper.Events.GameLoop.DayStarted += DayStartedEvent;

    }

    private static void TimeChangeEvent(object? sender, TimeChangedEventArgs e)
    {
        switch (e.NewTime) {
            case 2400:
                Game1.playSound(_slb["midnight2400"][0]);
                return;
            case 2500:
                Game1.playSound(_slb["midnight2500"][0]);
                return;
            case 2600:
                Game1.playSound(_slb["goodNight"][0]);
                return;
        }
    }

    private static void LoadWeirdSounds(IModHelper helper)
    {
        var assetsDir = Path.Combine(helper.DirectoryPath, "assets");
        var soundNames = Directory.GetFiles(assetsDir).Where(fileName => fileName.EndsWith(".wav"))
            .Select(Path.GetFileName).ToArray();
        foreach (var soundFileName in soundNames) {// a_1.wav
            if (soundFileName is null) {
                continue;
            }
            var soundFileNameWithoutExt = Path.GetFileNameWithoutExtension(soundFileName);//a_1
            var soundPrefix = soundFileNameWithoutExt.LastIndexOf('_') != -1 ? soundFileNameWithoutExt[..soundFileNameWithoutExt.LastIndexOf('_')] : soundFileNameWithoutExt;//a
            var soundUniquePrefix = string.Join('.', [helper.ModContent.ModID, soundPrefix]);//modId.a
            var soundUniqueFileNameWithoutExt = string.Join('.', [helper.ModContent.ModID, soundFileNameWithoutExt]);//modId.a_1
            var sound = SoundEffect.FromStream(new FileStream(Path.Combine(assetsDir, soundFileName), FileMode.Open));
            var soundDict = new Dictionary<string, SoundEffect> { {soundUniqueFileNameWithoutExt, sound} };
            
            var cue = new CueDefinition
            {
                name = soundUniqueFileNameWithoutExt,
                instanceLimit = 1,
                limitBehavior = CueDefinition.LimitBehavior.ReplaceOldest
            };
            cue.SetSound(sound, Game1.audioEngine.GetCategoryIndex("Sound"));
            Game1.soundBank.AddCue(cue);
            
            if (_slb.ContainsKey(soundPrefix)) {
                _slb[soundPrefix].Add(soundUniqueFileNameWithoutExt);
            } else {
                _slb.Add(soundPrefix, [soundUniqueFileNameWithoutExt]);
            }
        }
    }

    private static void MenuChangedEvent(object? sender, MenuChangedEventArgs e)
    {
        var a = e.OldMenu;
        var b = e.NewMenu;
        if ((e.NewMenu is GameMenu or ItemGrabMenu or JunimoNoteMenu) &&  e.OldMenu?.GetType() != e.NewMenu.GetType()) {
            Game1.playSound(_slb["inventory"][0]);
        }

        if (e.NewMenu is BobberBar { treasure: true } bBar) {
            DelayedAction.playSoundAfterDelay(_slb["treasureBox"][0], (int) bBar.treasureAppearTimer);
        }

        if (e.NewMenu is ItemGrabMenu { context: FishingRod } igm) {
            var price = 0;
            var gem = false;
            string[] gemList = ["797", "62", "72", "60", "82", "84", "70", "74", "64", "68", "66"];
            foreach (var tr in igm.ItemsToGrabMenu.actualInventory) {
                price += tr.sellToStorePrice() * tr.Stack;
                if (gemList.Any(g => tr.ItemId == g)) {
                    gem = true;
                }
            }
            if (price >= 700) {
                Game1.playSound(_slb["treasure"][0]);
            } else if (gem){
                Game1.playSound(_slb["gems"][0]);
            }
        }
        //
        // if (Game1.player.CurrentEmote == 24 && Game1.player.isEmoting && e.OldMenu is DialogueBox) {
        //     Game1.playSound(_slb["goodNight"][0]);
        // }
        
    }

    private static void DayStartedEvent(object? sender, DayStartedEventArgs e)
    {
        if (Game1.dayTimeMoneyBox.moneyDial.previousTargetValue - Game1.dayTimeMoneyBox.moneyDial.currentValue > 500) {
            DelayedAction.playSoundAfterDelay(_slb["million"][0], 1500);
        }
        
    }
    private class Game1Patcher
    {
        public static bool PlayLocalPrefix(ISoundsHelper __instance, ref bool __result, ref string cueName, GameLocation location, Vector2? position, int? pitch, StardewValley.Audio.SoundContext context)
        {
            string[] toolSounds = {"woodyHit", "hammer", "axchop", "swordswipe", "clubswipe", "daggerswipe", "clubSmash"};
            var cnc = cueName;
            if (toolSounds.Any(n => n == cnc)) {
                if (Game1.player.UsingTool) {
                    var toolCueName = ToolSlbCueName("tool");
                    __instance.PlayLocal(toolCueName, location, position, pitch, context, out _);
                }
            }
            if (cnc == "cancel") {
                var menu = Game1.activeClickableMenu;
                if (menu is PurchaseAnimalsMenu animalsMenu) {
                    var buildingAt = animalsMenu.TargetLocation.getBuildingAt(new Vector2((int) ((Utility.ModifyCoordinateFromUIScale(Game1.getMouseX()) + (double) Game1.viewport.X) / 64.0), (int) ((Utility.ModifyCoordinateFromUIScale(Game1.getMouseY()) + (double) Game1.viewport.Y) / 64.0)));
                    if (animalsMenu.animalBeingPurchased.CanLiveIn(buildingAt)) {
                        __instance.PlayLocal(_slb["noMoreAnimals"][0], location, position, pitch, context, out _);
                    } else {
                        var cancelCueName = CancelSlbCueName("cancel");
                        __instance.PlayLocal(cancelCueName, location, position, pitch, context, out _);
                    }
                } else {
                    var cancelCueName = CancelSlbCueName("cancel");
                    __instance.PlayLocal(cancelCueName, location, position, pitch, context, out _);
                }
            }
            if (cueName == "sell") {
                var menu = Game1.activeClickableMenu;
                if (menu is ShopMenu) {
                    __instance.PlayLocal(_slb["sell"][0], location, position, pitch, context, out _);
                }
            }            
            if (cueName == "trashcan") {
                __instance.PlayLocal(_slb["trashcan"][0], location, position, pitch, context, out _);
            }

            if (cueName == "batFlap") {
                var player = Game1.player;
                if (player.ActiveItem is MeleeWeapon sword && sword.type.Value == MeleeWeapon.defenseSword) {
                    int[] an = [252, 243, 259, 234];
                    if (an.Any(p => p == player.FarmerSprite.currentSingleAnimation)) { 
                        __instance.PlayLocal(_slb["defense"][0], location, position, pitch, context, out _);
                        var toolLocation = player.GetToolLocation();
                        var zero1 = Vector2.Zero;
                        var zero2 = Vector2.Zero;
                        var areaOfEffect = sword.getAreaOfEffect((int)toolLocation.X, (int)toolLocation.Y, player.FacingDirection, ref zero1, ref zero2, player.GetBoundingBox(), 1);
                        areaOfEffect.Inflate(50, 50);
                        if (player.currentLocation.getAllFarmAnimals().Any(animal => animal.GetBoundingBox().Intersects(areaOfEffect))) {
                            __instance.PlayLocal(_slb["defenseHa"][0], location, position, pitch, context, out _);
                        }
                    }
                }
            }

            if (cueName == "death") {
                if (Game1.player.health <= 0) {
                    __instance.PlayLocal(_slb["death"][0], location, position, pitch, context, out _);
                }
            }
            if (cueName == "breathout") {
                if (Game1.player.health == 10 && Game1.player.Sprite.currentFrame == 5) {
                    _hospitalFinished = true;
                }
            }
            if (cueName == "bigDeSelect") {
                if (Game1.activeClickableMenu is LevelUpMenu { isProfessionChooser: false }) {
                    Game1.playSound(_slb["ok"][0]);
                }   
            }
            // if (cueName == "batFlap") {
            //     System.Diagnostics.StackTrace st = new System.Diagnostics.StackTrace();
            //     System.Diagnostics.StackFrame[] sfs = st.GetFrames();
            //     var a = 1;
            // }
            return true;
        }

        private static bool _hospitalFinished;

        private static int _toolSlbIndex;
        private static long _toolSlbTime;
        
        private static string ToolSlbCueName(string key)
        {
            var currentTimestamp = (long)(DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalSeconds;
            if (currentTimestamp - _toolSlbTime > 5) {
                _toolSlbIndex = 0;
            } else {
                _toolSlbIndex++;
                _toolSlbIndex = _toolSlbIndex >= _slb[key].Count ? 0 : _toolSlbIndex;
            }
            _toolSlbTime = currentTimestamp;
            return _slb[key][_toolSlbIndex];
        }

        private static int _cancelSlbIndex;
        
        private static string CancelSlbCueName(string key)
        {
            var cueName = _slb[key][_cancelSlbIndex]; 
            _cancelSlbIndex++;
            _cancelSlbIndex = _cancelSlbIndex >= _slb[key].Count ? 0 : _cancelSlbIndex;
            return cueName;
        }

        public static bool PetPrefix(FarmAnimal __instance, bool is_auto_pet)
        {
            if (is_auto_pet || !__instance.type.Value.EndsWith("Chicken") || __instance.wasPet.Value || Game1.options.muteAnimalSounds) {
                return true;
            }
            var cueName = ChickenSlbCueName("cluck");
            Game1.playSound(cueName);
            return true;
        }
        
        private static int _cluckSlbIndex;

        private static string ChickenSlbCueName(string key)
        {
            var cueName = _slb[key][_cluckSlbIndex]; 
            _cluckSlbIndex++;
            _cluckSlbIndex = _cluckSlbIndex >= _slb[key].Count ? 0 : _cluckSlbIndex;
            return cueName;
        }

        public static void AnswerDialogueActionPostfix(string questionAndAnswer, string[] questionParams)
        {
            if (questionAndAnswer == "TroutDerbyBooth_Rewards") {
                if (Game1.delayedActions.Any(p => p.stringData == "getNewSpecialItem")) {
                    Game1.playSound(_slb["reward"][0]);
                }
            } else if (questionAndAnswer.ToLower().Contains("rewards")) {
                Game1.playSound(_slb["reward"][0]);
            }
            if (questionAndAnswer.ToLower() == "sleep_yes") {
                Game1.playSound(_slb["goodNight"][0]);
            }
        }

        public static void PlayEffectsPostfix(Object __instance, ref bool __result, bool probe)
        {
            if (__result && !probe) {
                __instance.Location.playSound(MachineSlbCueName("machine"), __instance.TileLocation);
            }
        }
        
        private static int _machineSlbIndex;

        private static string MachineSlbCueName(string key)
        {
            var cueName = _slb[key][_machineSlbIndex]; 
            _machineSlbIndex++;
            _machineSlbIndex = _machineSlbIndex >= _slb[key].Count ? 0 : _machineSlbIndex;
            return cueName;
        }

        public static void ContainsPointPostfix(ClickableComponent __instance, ref bool __result)
        {
            if (__result && __instance is ClickableTextureComponent ok) {
                if (ok.texture.Name.StartsWith("LooseSprites/Cursors") && ok.sourceRect is { Left: 128, Top: 256 , Height: 64, Width:64} && Game1.didPlayerJustLeftClick()) {
                    if (_hospitalFinished) {
                        Game1.playSound(_slb["hospitalFinished"][0]);
                        _hospitalFinished = false;
                    } else {
                        Game1.playSound(_slb["ok"][0]);
                    }
                }
            }
        }        
    }
}