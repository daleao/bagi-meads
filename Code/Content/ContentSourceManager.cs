#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;

namespace DaLion.Meads.Content;

internal class ContentSourceManager
{
    private static readonly ConstructorInfo ArtisanGoodTextureProviderCtor =
        "BetterArtisanGoodIcons.ArtisanGoodTextureProvider".ToType().RequireConstructor(3);

    internal static object? TryLoadContentSource(TextureDataContentSource contentSource, IMonitor monitor)
    {
        var data = contentSource.GetData();
		return TryLoadTextureProvider(contentSource, data.Item1, data.Item2, data.Item3, monitor, out var provider) ? provider : null;
    }

	private static bool TryLoadTextureProvider(IContentSource contentSource, string? imagePath, List<string>? source, object good, IMonitor monitor, out object? provider)
	{
		provider = null;
		if (string.IsNullOrEmpty(imagePath)) return false;
		
        var manifest = contentSource.GetManifest();
		if (source is null || source.Count == 0 || source.Any(string.IsNullOrEmpty))
		{
			monitor.Log($"Couldn't load Mead from {manifest.Name} ({manifest.UniqueID}) because it has an invalid source list (Flowers).", LogLevel.Warn);
			monitor.Log("Flowers must not be null, must not be empty, and cannot have null items inside it.", LogLevel.Warn);
		}
		else
		{
			try
            {
                provider = ArtisanGoodTextureProviderCtor.Invoke(new[] {contentSource.Load<Texture2D>(imagePath), source, good});
				return true;
			}
			catch (Exception)
			{
				monitor.Log($"Couldn't load Mead from {manifest.Name} ({manifest.UniqueID}) because the Mead texture file path is invalid ({imagePath}).", LogLevel.Warn);
			}
		}
		return false;
	}
}
