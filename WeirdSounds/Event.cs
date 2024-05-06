using StardewValley;
using StardewValley.Menus;

namespace WeirdSounds
{
    internal partial class Mod
    {
        private static string CueName(string key)
        {
            return WeirdSoundsLibrary.GetCueName(key);
        }
        
        private static void TimeChangeEvent(object? sender, StardewModdingAPI.Events.TimeChangedEventArgs e)
        {
            switch (e.NewTime) {
                case 2400:
                    Game1.playSound(CueName("midnight2400"));
                    return;
                case 2500:
                    Game1.playSound(CueName("midnight2500"));
                    return;
                case 2600:
                    Game1.playSound(CueName("goodNight"));
                    return;
            }
        }
        private static readonly string[] JewelList = ["797", "62", "72", "60", "82", "84", "70", "74", "64", "68", "66"];
        
        private static void MenuChangedEvent(object? sender, StardewModdingAPI.Events.MenuChangedEventArgs e)
        {
            switch (e.NewMenu) {
                case GameMenu or ItemGrabMenu or JunimoNoteMenu when e.OldMenu?.GetType() != e.NewMenu.GetType():
                    Game1.playSound(CueName("inventory"));
                    return;
                case BobberBar { treasure: true } bBar:
                    DelayedAction.playSoundAfterDelay(CueName("treasureBox"), (int) bBar.treasureAppearTimer);
                    return;
                case ItemGrabMenu { context: StardewValley.Tools.FishingRod } igm: {
                    var price = 0;
                    var jewel = false;
                    foreach (var tr in igm.ItemsToGrabMenu.actualInventory) {
                        price += tr.sellToStorePrice() * tr.Stack;
                        if (JewelList.Any(jewelId => tr.ItemId == jewelId)) {
                            jewel = true;
                        }
                    }
                    if (price >= 700) {
                        Game1.playSound(CueName("treasure"));
                    } else if (jewel){
                        Game1.playSound(CueName("jewel"));
                    }
                    break;
                }
            }
        }
        
        private static void DayStartedEvent(object? sender, StardewModdingAPI.Events.DayStartedEventArgs e)
        {
            if (Game1.dayTimeMoneyBox.moneyDial.previousTargetValue - Game1.dayTimeMoneyBox.moneyDial.currentValue >= 1000000) {
                DelayedAction.playSoundAfterDelay(CueName("million"), 1500);
            }
        }
    }
}