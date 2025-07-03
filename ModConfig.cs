using StardewModdingAPI;

namespace GoldVault
{
    /// <summary>Configuration settings persisted to config.json.</summary>
    public class ModConfig
    {
        /// <summary>The key to open the vault menu.</summary>
        public SButton OpenMenuKey { get; set; } = SButton.F5;
    }
}
