using System;
using System.Text.RegularExpressions;

namespace MassMediaEdit.Utilities;

/// <summary>
/// Provides text manipulation utilities for media metadata.
/// </summary>
public static class TextManipulation {
  // Pattern to match season and episode numbers in various formats
  // Supports: s05e04, S01E02, season5episode4, st2ep1, ep1st2, "staffel 5 episode 13", etc.
  private static readonly Regex SeasonEpisodeRegex = new(
    @"(?:s(?:\w*?)[\s.\\_-]*?(?<season>\d+).*?e(?:\w*?)[\s.\\_-]*?(?<episode>\d+))|(?:e(?:\w*?)[\s.\\_-]*?(?<episode>\d+).*?s(?:\w*?)[\s.\\_-]*?(?<season>\d+))",
    RegexOptions.IgnoreCase | RegexOptions.Compiled);

  private static readonly Regex BracketContentRegex = new(
    @"\s*(?:\([^)]*\)|\[[^\]]*\]|\{[^}]*\}|<[^>]*>)\s*",
    RegexOptions.Compiled);

  /// <summary>
  /// Extracts season and episode information from text.
  /// </summary>
  /// <param name="text">The text to parse.</param>
  /// <returns>A tuple containing the season number, episode number, and remaining title text, or <see langword="null"/> if not found.</returns>
  public static (ushort Season, ushort Episode, string? Title)? ExtractSeasonEpisode(string? text) {
    if (string.IsNullOrWhiteSpace(text))
      return null;

    var match = SeasonEpisodeRegex.Match(text);
    if (!match.Success)
      return null;

    var season = ushort.Parse(match.Groups["season"].Value);
    var episode = ushort.Parse(match.Groups["episode"].Value);

    var titleStart = match.Index + match.Length;
    var title = text[titleStart..].Trim().TrimStart('-', '.', '_', ' ').Trim();

    return (season, episode, string.IsNullOrWhiteSpace(title) ? null : title);
  }

  /// <summary>
  /// Formats a season and episode number in the standard format (e.g., "s02e05").
  /// </summary>
  /// <param name="season">The season number.</param>
  /// <param name="episode">The episode number.</param>
  /// <returns>The formatted string.</returns>
  public static string FormatSeasonEpisode(ushort season, ushort episode) => $"s{season:D2}e{episode:D2}";

  /// <summary>
  /// Attempts to recover spaces in text that may have been replaced with other characters.
  /// </summary>
  /// <param name="text">The text to process.</param>
  /// <returns>The text with recovered spaces, or the original text if no recovery was possible.</returns>
  public static string? RecoverSpaces(string? text) {
    if (text is null)
      return null;

    // If already has spaces, don't modify
    if (text.Contains(' '))
      return text;

    // Try URL-encoded spaces
    if (text.Contains("%20"))
      return text.Replace("%20", " ");

    // Try common space replacement characters
    ReadOnlySpan<char> spaceReplacements = ['_', '.', '%', '-', '+'];
    foreach (var c in spaceReplacements)
      if (text.Contains(c))
        return text.Replace(c, ' ');

    return text;
  }

  /// <summary>
  /// Removes content within brackets (parentheses, square brackets, curly braces, angle brackets) from text.
  /// </summary>
  /// <param name="text">The text to process.</param>
  /// <returns>The text with bracket content removed.</returns>
  public static string? RemoveBracketContent(string? text) =>
    text is null ? null : BracketContentRegex.Replace(text, string.Empty).Trim();

  /// <summary>
  /// Gets the filename without extension from a file path.
  /// </summary>
  /// <param name="filePath">The full file path.</param>
  /// <returns>The filename without extension.</returns>
  public static string GetFilenameWithoutExtension(string filePath) =>
    System.IO.Path.GetFileNameWithoutExtension(filePath);
}
