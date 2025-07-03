using StardewModdingAPI;

namespace GoldVault
{
    /// <summary>Persistent user settings (saved to config.json).</summary>
    public class ModConfig
    {
        /// <summary>
        /// The button which opens the vault menu.
        /// Defaults to F5.
        /// </summary>
        public SButton OpenMenuKey { get; set; } = SButton.F5;
    }
}
