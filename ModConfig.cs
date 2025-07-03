using StardewModdingAPI;

namespace GoldVault
{
    /// <summary>Persisted settings for GoldVault.</summary>
    public class ModConfig
    {
        /// <summary>Key to open the vault menu.</summary>
        public SButton Hotkey { get; set; } = SButton.F5;
    }
}
