using System;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using Microsoft.Xna.Framework;    // for Vector2, Color
using StardewValley.Menus;        // SpriteText, IClickableMenu
using Microsoft.Xna.Framework.Graphics;

namespace GoldVault
{
    /// <summary>Main entry point for the GoldVault mod.</summary>
    public class ModEntry : Mod
    {
        private ModConfig Config;

        public override void Entry(IModHelper helper)
        {
            // 1) load or create config.json
            Config = helper.ReadConfig<ModConfig>();

            // 2) open vault menu when hotkey is pressed
            helper.Events.Input.ButtonPressed += OnButtonPressed;

            // 3) register GMCM UI (if installed)
            helper.Events.GameLoop.GameLaunched += OnGameLaunched;
        }

        private void OnButtonPressed(object sender, ButtonPressedEventArgs e)
        {
            if (!Context.IsWorldReady)
                return;
            if (e.Button == Config.Hotkey)
                Game1.activeClickableMenu = new VaultMenu();
        }

        private void OnGameLaunched(object sender, GameLaunchedEventArgs e)
        {
            var gmcm = Helper.ModRegistry.GetApi<IGenericModConfigMenuApi>(
                "spacechase0.GenericModConfigMenu"
            );
            if (gmcm == null)
                return;

            // register reset/save
            gmcm.Register(
                mod: ModManifest,
                reset: () => Config = new ModConfig(),
                save:  () => Helper.WriteConfig(Config),
                titleScreenOnly: false
            );

            // allow user to change the vault hotkey in-game
            gmcm.AddKeybindOption(
                mod: ModManifest,
                name:    () => "Open Vault Hotkey",
                tooltip: () => "Which button opens the Gold Vault menu.",
                getValue: () => Config.Hotkey,
                setValue: val => Config.Hotkey = val
            );
        }
    }

    /// <summary>A simple vault menu stub – you'll flesh this out next.</summary>
    public class VaultMenu : IClickableMenu
    {
public VaultMenu()
  : base(0, 0, 400, 300, showUpperRightCloseButton: true)
{ }


public override void draw(SpriteBatch b)
{
    base.draw(b);

    // draw our header text using the game's small font
    string header = "💰 Gold Vault 💰";
    var pos = new Vector2(xPositionOnScreen + 20, yPositionOnScreen + 20);
    b.DrawString(Game1.smallFont, header, pos, Color.Gold);

    drawMouse(b);
}

    }

    /// <summary>Minimal subset of the GMCM API we need.</summary>
    public interface IGenericModConfigMenuApi
    {
        void Register(IManifest mod, Action reset, Action save, bool titleScreenOnly);
        void AddKeybindOption(
            IManifest mod,
            Func<string> name,
            Func<string> tooltip,
            Func<SButton> getValue,
            Action<SButton> setValue
        );
    }
}
