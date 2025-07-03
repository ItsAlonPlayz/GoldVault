using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Menus;

namespace GoldVault
{
    /// <summary>The mod’s entry point.</summary>
    public class ModEntry : Mod
    {
        private ModConfig Config;

        /// <summary>Called once when SMAPI loads your mod.</summary>
        public override void Entry(IModHelper helper)
        {
            // 1) load or create the config
            Config = helper.ReadConfig<ModConfig>();

            // 2) hook input and game-launched
            helper.Events.Input.ButtonPressed += OnButtonPressed;
            helper.Events.GameLoop.GameLaunched += OnGameLaunched;
        }

        /// <summary>Register with GMCM after all mods are loaded.</summary>
        private void OnGameLaunched(object sender, GameLaunchedEventArgs e)
        {
            var gmcm = Helper.ModRegistry.GetApi<IGenericModConfigMenuApi>(
                "spacechase0.GenericModConfigMenu"
            );
            if (gmcm == null)
                return; // no GMCM installed

            // register reset/save
            gmcm.Register(
                mod: ModManifest,
                reset: () => Config = new ModConfig(),
                save:  () => Helper.WriteConfig(Config),
                titleScreenOnly: false
            );

            // let the user rebind the open-menu key
            gmcm.AddKeybindOption(
                mod: ModManifest,
                name:    () => "Open Vault Hotkey",
                tooltip: () => "Which key opens the Gold Vault menu.",
                getValue: () => Config.OpenMenuKey,
                setValue: val => Config.OpenMenuKey = val
            );
        }

        /// <summary>Open the menu when the user presses the bound key.</summary>
        private void OnButtonPressed(object sender, ButtonPressedEventArgs e)
        {
            if (!Context.IsWorldReady)
                return;

            if (e.Button == Config.OpenMenuKey)
                Game1.activeClickableMenu = new VaultMenu();
        }
    }

    /// <summary>A simple vault menu stub.</summary>
    public class VaultMenu : IClickableMenu
    {
        private const int WIDTH = 400;
        private const int HEIGHT = 300;
        private const int PADDING = 16;

        public VaultMenu()
          : base(
              x: Game1.viewport.Width / 2 - WIDTH / 2,
              y: Game1.viewport.Height / 2 - HEIGHT / 2,
              width: WIDTH,
              height: HEIGHT,
              showUpperRightCloseButton: true
            )
        { }

        public override void draw(SpriteBatch b)
        {
            // draw the standard dialog box background
            Game1.drawDialogueBox(xPositionOnScreen, yPositionOnScreen, width, height, speaker: false, drawOnlyBox: true);

            // draw the header
            string header = "💰 Gold Vault 💰";
            var pos = new Vector2(xPositionOnScreen + PADDING, yPositionOnScreen + PADDING);
            b.DrawString(Game1.dialogueFont, header, pos, Color.Gold);

            // draw the close button and the cursor
            base.drawMouse(b);
        }

        public override void receiveLeftClick(int x, int y, bool playSound = true)
        {
            if (this.readyToClose())
            {
                exitThisMenu();
                if (playSound) Game1.playSound("bigDeSelect");
            }
        }
    }

    /// <summary>Minimal GMCM API subset for keybinds.</summary>
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
