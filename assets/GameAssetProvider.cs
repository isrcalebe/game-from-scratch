using System.Reflection;

namespace miniblocks.Game.Assets;

/// <summary>
/// A static class that provides access to the assembly that contains the game's assets.
/// </summary>
public static class GameAssetProvider
{
    /// <summary>
    /// The assembly that contains the game's assets.
    /// </summary>
    public static Assembly AssetAssembly => typeof(GameAssetProvider).Assembly;
}
