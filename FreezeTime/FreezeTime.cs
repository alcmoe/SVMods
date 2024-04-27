using HarmonyLib;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Monsters;
using Object = StardewValley.Object;

namespace FreezeTime;

public partial class FreezeTime: Mod
{
    private const string BroadcastStatusMessageType = "_FREEZE";
    private const string AskStatusMessageType = "_ASK_FREEZE";
    private static Texture2D? _frame, _blackBlock, _frozenBlock,_unfrozenBlock,_unloadedButFrozenBlock,_unloadedButUnfrozenBlock;

    private FreezeTimeChecker _checker = new(new ModConfig());
    private static bool _lastFreezeStatus;
    private static bool _forcePassTime;
    private static readonly Vector2 FramePosition = new(44, 240);
    public override void Entry(IModHelper helper)
    {
        _frame = helper.ModContent.Load<Texture2D>("assets/Frame.png");
        _blackBlock = helper.ModContent.Load<Texture2D>("assets/BlackBlock.png");
        _frozenBlock = helper.ModContent.Load<Texture2D>("assets/FrozenBlock.png");
        _unfrozenBlock = helper.ModContent.Load<Texture2D>("assets/UnfrozenBlock.png");        
        _unloadedButFrozenBlock = helper.ModContent.Load<Texture2D>("assets/UnloadedButFrozenBlock.png");
        _unloadedButUnfrozenBlock = helper.ModContent.Load<Texture2D>("assets/UnloadedButUnfrozenBlock.png");
        Helper.Events.Display.RenderingHud += PreRenderHudEvent;
        Helper.Events.GameLoop.UpdateTicked += GameTickEvent;
        Helper.Events.Multiplayer.PeerConnected += PlayerConnectedEvent;
        Helper.Events.Multiplayer.PeerDisconnected += PlayerDisconnectedEvent;
        Helper.Events.Multiplayer.ModMessageReceived += ModMessageReceivedEvent;
        Helper.Events.Input.ButtonReleased += ButtonReleasedEvent;
        Helper.Events.GameLoop.SaveLoaded += SaveLoadedEvent;
        Helper.Events.GameLoop.GameLaunched += GameLaunchedEvent;
        
        var harmony = new Harmony(ModManifest.UniqueID);
        harmony.Patch(
            original: AccessTools.Method(typeof(Game1), nameof(Game1.shouldTimePass)),
            prefix: new HarmonyMethod(typeof(Game1Patcher), nameof(Game1Patcher.ShouldTimePass))
        );
        harmony.Patch(
            original: AccessTools.Method(typeof(Game1), "Update"),
            postfix: new HarmonyMethod(typeof(Game1Patcher), nameof(Game1Patcher.ForceNetTimePause))
        );        
        harmony.Patch(
            original: AccessTools.Method(typeof(GameLocation), nameof(GameLocation.performTouchAction), [typeof(string[]), typeof(Vector2)]),
            prefix: new HarmonyMethod(typeof(Game1Patcher), nameof(Game1Patcher.PerformSleepCheck))
        );        
        harmony.Patch(
            original: AccessTools.Method(typeof(GameLocation), nameof(GameLocation.performTouchAction), [typeof(string[]), typeof(Vector2)]),
            postfix: new HarmonyMethod(typeof(Game1Patcher), nameof(Game1Patcher.PerformSleepChecked))
        );        
        harmony.Patch(
            original: AccessTools.Method(typeof(Object), "CheckForActionOnFeedHopper"),
            prefix: new HarmonyMethod(typeof(Game1Patcher), nameof(Game1Patcher.FeedHopperPrefix))
        );
    }

    private class Game1Patcher
    {
        public static bool ShouldTimePass(ref bool __result)
        {
            if (!Context.IsMultiplayer) {
                return true;
            }
            if (_lastFreezeStatus && _forcePassTime) {
                __result = true;
                return false;
            }
            if (!Context.IsMainPlayer) {
                return true;
            }
            __result = !_lastFreezeStatus;
            return false;
        }
        public static void ForceNetTimePause()
        {
            if (Context.IsMainPlayer && Context.IsMultiplayer) {
                Game1.netWorldState.Value.IsTimePaused = _lastFreezeStatus;
            }
        }
        public static void PerformSleepCheck()
        {
            _forcePassTime = true;
        }        
        public static void PerformSleepChecked()
        {
            _forcePassTime = false;
        }

        public static bool FeedHopperPrefix(Object __instance, ref bool __result, Farmer who, bool justCheckingForActivity)
        {
            __result = CheckForActionOnFeedHopper();
            return false;
            bool CheckForActionOnFeedHopper() {
                if (justCheckingForActivity) {
                    return true;
                }
                if (who.ActiveObject != null) {
                    return false;
                }
                if (who.freeSpotsInInventory() > 0) {
                    var location = __instance.Location;
                    var rootLocation = location.GetRootLocation();
                    var piecesHay = rootLocation.piecesOfHay.Value;
                    if (piecesHay > 0) {
                        if (location is AnimalHouse i) {
                            var piecesOfHayToRemove = Math.Min(i.animalsThatLiveHere.Count, piecesHay);
                            piecesOfHayToRemove = Math.Max(1, piecesOfHayToRemove);
                            var alreadyHay = i.numberOfObjectsWithName("Hay");
                            piecesOfHayToRemove = alreadyHay == i.animalLimit.Value ? Math.Min(i.animalLimit.Value, piecesHay) : Math.Min(piecesOfHayToRemove, i.animalLimit.Value - alreadyHay);
                            if (piecesOfHayToRemove != 0 && Game1.player.couldInventoryAcceptThisItem("(O)178", piecesOfHayToRemove))
                            {
                                rootLocation.piecesOfHay.Value -= Math.Max(1, piecesOfHayToRemove);
                                who.addItemToInventoryBool(ItemRegistry.Create("(O)178", piecesOfHayToRemove));
                                Game1.playSound("shwip");
                            }
                        } else if (Game1.player.couldInventoryAcceptThisItem("(O)178", 1)) {
                            rootLocation.piecesOfHay.Value--;
                            who.addItemToInventoryBool(ItemRegistry.Create("(O)178"));
                            Game1.playSound("shwip");
                        }
                        if (rootLocation.piecesOfHay.Value <= 0) {
                            __instance.showNextIndex.Value = false;
                        }
                        return true;
                    }
                    Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\StringsFromCSFiles:Object.cs.12942"));
                } else {
                    Game1.showRedMessage(Game1.content.LoadString("Strings\\StringsFromCSFiles:Crop.cs.588"));
                }
                return true;
            }
        }
    }

    private void SaveLoadedEvent(object? sender, SaveLoadedEventArgs e)
    {
        _checker = new FreezeTimeChecker(_config);
        if (Context.IsMainPlayer) {
            _lastFreezeStatus = false;
            if (_checker.HasPlayer(Game1.player)) {
                return;
            }
            _checker.AddPlayer(Game1.player);
            _checker.SetPlayerLoaded(Game1.player, true);
        } else {
            AskFreezeTimeStatus();
        }
    }
    
    private void ModMessageReceivedEvent(object? sender, ModMessageReceivedEventArgs e)
    {
        if (e.FromModID != ModManifest.UniqueID) {
            return;
        }
        if (Context.IsMainPlayer) {
            if (e.Type == AskStatusMessageType) {
                BroadcastStatus();
            }
        } else {
            if (e.Type != BroadcastStatusMessageType) {
                return;
            }
            var status = e.ReadAs<Dictionary<long, Dictionary<string, bool>>>();
            _checker.LoadFromStatus(status);
        }
    }

    private void AskFreezeTimeStatus()
    {
        Helper.Multiplayer.SendMessage(true, AskStatusMessageType);
    }
    
    private void PreRenderHudEvent(object? sender, EventArgs e)
    {
        if (Game1.displayHUD && !Game1.freezeControls) {
            DrawStatusBar();
        }
    }
    
    private void ButtonReleasedEvent(object? sender, ButtonReleasedEventArgs e)
    {
        if (e.Button != SButton.MouseLeft) {
            return;
        }

        if (!new Rectangle((int)((Game1.dayTimeMoneyBox.position.X + FramePosition.X + 24) * Game1.options.uiScale),(int)((Game1.dayTimeMoneyBox.position.Y + FramePosition.Y + 24) * Game1.options.uiScale),(int)(108 * Game1.options.uiScale), (int)(24 * Game1.options.uiScale)).Contains(Game1.getMouseX(), Game1.getMouseY())) { 
            return;
        }

        _lastFreezeStatus = !_lastFreezeStatus;
        Game1.chatBox.addMessage(_checker.GetFreezeTimeMessage(), Color.Aqua);
        Helper.Input.Suppress(SButton.MouseLeft);
    }
    
    private static bool PlayerFrozen(Farmer player)
    {
        return player is { CanMove: false, UsingTool: false } || player.hasMenuOpen.Value || player.Sprite.currentFrame == 84;
    }
    
    private void DrawStatusBar()
    {
        const int totalBlockWidth = 108;
        const int separationLineWidth = 4;
        Vector2 offset = new(24, 24);
        var blockWidth = (totalBlockWidth - (Game1.getOnlineFarmers().Count - 1) * separationLineWidth) / Game1.getOnlineFarmers().Count;
        var counter = 0;
        Game1.spriteBatch.Draw(_frame, Game1.dayTimeMoneyBox.position + FramePosition , null, Color.White, 0.0f, Vector2.Zero, 4, SpriteEffects.None, 0.99f);
        foreach (var status in _checker.GetCollection()) {
            Texture2D? bar;
            if (status.Value.Loaded) {
                bar = status.Value.Frozen ? _frozenBlock : _unfrozenBlock;
            } else {
                bar = status.Value.Frozen ? _unloadedButFrozenBlock : _unloadedButUnfrozenBlock;
            }
            Game1.spriteBatch.Draw(bar, Game1.dayTimeMoneyBox.position + FramePosition + offset +  new Vector2(counter * (blockWidth + separationLineWidth), 0), new Rectangle(0, 0, blockWidth, 24), Color.White, 0.0f, Vector2.Zero, 1, SpriteEffects.None, 0.99f);
            if (counter != 0) {
                Game1.spriteBatch.Draw(_blackBlock, Game1.dayTimeMoneyBox.position + FramePosition + offset + new Vector2(counter * (blockWidth + separationLineWidth) - separationLineWidth, 0), new Rectangle(0, 0, separationLineWidth, 24), Color.White, 0.0f, Vector2.Zero, 1, SpriteEffects.None, 0.99f);
            }
            counter++;
        }
    }
    
    //main player
    private void PlayerConnectedEvent(object? sender, PeerConnectedEventArgs e)
    {
        if (!Context.IsMainPlayer) {
            return;
        }
        foreach (var farmer in Game1.getOnlineFarmers().Where(farmer => farmer.UniqueMultiplayerID == e.Peer.PlayerID)) {
            _checker.AddPlayer(farmer);
            string message;
            Color color;
            if (e.Peer.GetMod(ModManifest.UniqueID) == null) { 
                message = farmer.Name + " doesn't have " + ModManifest.Name + " mod.";
                color = Color.Red;
            } else {
                _checker.SetPlayerLoaded(farmer, true);
                message = farmer.Name + " has " + ModManifest.Name + " mod.";
                color = Color.Blue;
            }
            if (Context.IsMainPlayer) {
                Game1.chatBox.addMessage(message, color);
            }
        }
    }

    private void PlayerDisconnectedEvent(object? sender, PeerDisconnectedEventArgs e)
    {
        if (!Context.IsMainPlayer) {
            return;
        }
        if (_checker.HasPlayer(e.Peer.PlayerID)) {
            _checker.DelPlayer(e.Peer.PlayerID);
        }
    }
    
    private void GameTickEvent(object? sender, EventArgs e)
    {
        if (!Context.IsWorldReady) {
            return;
        }
        if(Context.IsMainPlayer) {
            UpdateChecker();
            ApplyFreezing();
            ApplyUnFreezing();
        }
        ApplyMonsterFreezing();
    }
    
    private void UpdateChecker()
    {
        foreach (var farmer in Game1.getOnlineFarmers()) {
            if (!_checker.HasPlayer(farmer)) {
                return;
            }
            if (PlayerFrozen(farmer)) {
                if (_checker.GetPlayer(farmer).Frozen) {
                    continue;
                }
                _checker.SetPlayerFrozen(farmer, true);
                BroadcastStatus();
            } else {
                if (!_checker.GetPlayer(farmer).Frozen) {
                    continue;
                }
                _checker.SetPlayerFrozen(farmer, false);
                BroadcastStatus();
            }
        }
    }

    private void ApplyFreezing()
    {
        if (!_checker.IsFrozen()) {
            return;
        }
        if (!_lastFreezeStatus) {
             _lastFreezeStatus = true;
        }
    }

    private static void ApplyMonsterFreezing()
    {
        if (!Game1.netWorldState.Value.IsTimePaused) {
            return;
        }
        foreach (var character in Game1.player.currentLocation.characters) {
            if (character is not Monster monster) {
                continue;
            }
            if (monster.invincibleCountdown > 0) {
                monster.invincibleCountdown = 0;
            }
        }
    }
    
    private void ApplyUnFreezing()
    {
        if (!_lastFreezeStatus || _checker.IsFrozen()) {
            return;
        }
        _lastFreezeStatus = false;
    }
    
    private void BroadcastStatus()
    {   
        Helper.Multiplayer.SendMessage(_checker.FreezeTimeStatus(), BroadcastStatusMessageType, modIDs: [ModManifest.UniqueID]);
    }
}