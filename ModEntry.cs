using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Menus;

namespace GoldVault
{
    /// <summary>The main entry point for the Gold Vault mod.</summary>
    public class ModEntry : Mod
    {
        private ModConfig Config;

        /// <summary>Called once when SMAPI loads your mod.</summary>
        public override void Entry(IModHelper helper)
        {
            // Load (or create) your config.json
            Config = helper.ReadConfig<ModConfig>();

            // Hook input and post-launch events
            helper.Events.Input.ButtonPressed += OnButtonPressed;
            helper.Events.GameLoop.GameLaunched  += OnGameLaunched;
        }

        /// <summary>Register with Generic Mod Config Menu (GMCM) after all mods load.</summary>
        private void OnGameLaunched(object sender, GameLaunchedEventArgs e)
        {
            var gmcm = Helper.ModRegistry.GetApi<IGenericModConfigMenuApi>(
                "spacechase0.GenericModConfigMenu"
            );
            if (gmcm == null)
                return; // GMCM not installed

            // Tell GMCM how to reset and save your config
            gmcm.Register(
                mod: ModManifest,
                reset: () => Config = new ModConfig(),
                save:  () => Helper.WriteConfig(Config),
                titleScreenOnly: false
            );

            // Expose the OpenMenuKey binding
            gmcm.AddKeybindOption(
                mod: ModManifest,
                name:    () => "Open Vault Hotkey",
                tooltip: () => "Which key opens the Gold Vault menu.",
                getValue: () => Config.OpenMenuKey,
                setValue: val => Config.OpenMenuKey = val
            );
        }

        /// <summary>Open the vault menu when the user presses the configured key.</summary>
        private void OnButtonPressed(object sender, ButtonPressedEventArgs e)
        {
            if (!Context.IsWorldReady)
                return; // no save loaded
            if (e.Button == Config.OpenMenuKey)
                Game1.activeClickableMenu = new VaultMenu();
        }
    }

    /// <summary>A simple placeholder vault menu.</summary>
    public class VaultMenu : IClickableMenu
    {
        private const int Width  = 800;
        private const int Height = 600;
        private readonly Texture2D Pixel;

        public VaultMenu()
          : base(
              x: (Game1.viewport.Width  - Width ) / 2,
              y: (Game1.viewport.Height - Height) / 2,
              width:  Width,
              height: Height,
              showUpperRightCloseButton: true
            )
        {
            // white-pixel texture used for solid fills
            Pixel = Game1.content.Load<Texture2D>("LooseSprites\\whitePixel");
        }

        public override void draw(SpriteBatch b)
        {
            // background panel (semi-transparent)
            b.Draw(Pixel, new Rectangle(xPositionOnScreen, yPositionOnScreen, width, height),
                   Color.Black * 0.75f);

            // header bar
            var headerRect = new Rectangle(xPositionOnScreen, yPositionOnScreen, width, 56);
            b.Draw(Pixel, headerRect, Color.Gold * 0.5f);

            // header text, centered
            string title = "★ Gold Vault ★";
            Vector2 size = Game1.dialogueFont.MeasureString(title);
            Vector2 pos  = new Vector2(
                xPositionOnScreen + (width - size.X)  / 2,
                yPositionOnScreen + (56    - size.Y)  / 2
            );
            b.DrawString(Game1.dialogueFont, title, pos, Color.Yellow);

            // draw close button and mouse cursor
            base.drawMouse(b);
        }

        public override void receiveLeftClick(int x, int y, bool playSound = true)
        {
            if (readyToClose())
            {
                exitThisMenu();
                if (playSound) Game1.playSound("bigDeSelect");
            }
        }

        public override void performHoverAction(int x, int y) { }
        public override void receiveRightClick(int x, int y, bool playSound = true) { }
    }

    /// <summary>Minimal interface for GMCM keybind options.</summary>
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
