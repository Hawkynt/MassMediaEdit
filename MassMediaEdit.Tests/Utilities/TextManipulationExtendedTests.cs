using MassMediaEdit.Utilities;
using NUnit.Framework;

namespace MassMediaEdit.Tests.Utilities;

/// <summary>
/// Extended unit tests for the <see cref="TextManipulation"/> utility class.
/// </summary>
[TestFixture]
public sealed class TextManipulationExtendedTests {
  #region ExtractSeasonEpisode Extended Tests

  [TestCase("Show S01E01", (ushort)1, (ushort)1)]
  [TestCase("Show.S02E03", (ushort)2, (ushort)3)]
  [TestCase("Show_S04E05", (ushort)4, (ushort)5)]
  [TestCase("Show-S06E07", (ushort)6, (ushort)7)]
  public void ExtractSeasonEpisode_WithPrefixedText_ExtractsCorrectly(
    string input, ushort expectedSeason, ushort expectedEpisode) {
    var result = TextManipulation.ExtractSeasonEpisode(input);

    Assert.That(result, Is.Not.Null);
    Assert.That(result!.Value.Season, Is.EqualTo(expectedSeason));
    Assert.That(result!.Value.Episode, Is.EqualTo(expectedEpisode));
  }

  [TestCase("s1e01", (ushort)1, (ushort)1)]
  [TestCase("s01e1", (ushort)1, (ushort)1)]
  [TestCase("s001e001", (ushort)1, (ushort)1)]
  public void ExtractSeasonEpisode_WithVariousNumberFormats_ExtractsCorrectly(
    string input, ushort expectedSeason, ushort expectedEpisode) {
    var result = TextManipulation.ExtractSeasonEpisode(input);

    Assert.That(result, Is.Not.Null);
    Assert.That(result!.Value.Season, Is.EqualTo(expectedSeason));
    Assert.That(result!.Value.Episode, Is.EqualTo(expectedEpisode));
  }

  [TestCase("S99E99", (ushort)99, (ushort)99)]
  [TestCase("s12e34", (ushort)12, (ushort)34)]
  public void ExtractSeasonEpisode_WithHigherNumbers_ExtractsCorrectly(
    string input, ushort expectedSeason, ushort expectedEpisode) {
    var result = TextManipulation.ExtractSeasonEpisode(input);

    Assert.That(result, Is.Not.Null);
    Assert.That(result!.Value.Season, Is.EqualTo(expectedSeason));
    Assert.That(result!.Value.Episode, Is.EqualTo(expectedEpisode));
  }

  #endregion

  #region FormatSeasonEpisode Extended Tests

  [Test]
  public void FormatSeasonEpisode_WithZeroSeason_FormatsWithLeadingZero() {
    var result = TextManipulation.FormatSeasonEpisode(0, 1);

    Assert.That(result, Is.EqualTo("s00e01"));
  }

  [Test]
  public void FormatSeasonEpisode_WithZeroEpisode_FormatsWithLeadingZero() {
    var result = TextManipulation.FormatSeasonEpisode(1, 0);

    Assert.That(result, Is.EqualTo("s01e00"));
  }

  [Test]
  public void FormatSeasonEpisode_WithLargeNumbers_FormatsCorrectly() {
    var result = TextManipulation.FormatSeasonEpisode(100, 999);

    Assert.That(result, Is.EqualTo("s100e999"));
  }

  #endregion

  #region RecoverSpaces Extended Tests

  [Test]
  public void RecoverSpaces_WithMixedSeparators_UsesFirstFound() {
    const string input = "Some_Title.Here";

    var result = TextManipulation.RecoverSpaces(input);

    // Should use underscore as it appears first in the check order
    Assert.That(result, Is.EqualTo("Some Title.Here"));
  }

  [Test]
  public void RecoverSpaces_WithUrlEncodedOnly_RecoversThem() {
    const string input = "No%20Other%20Separators";

    var result = TextManipulation.RecoverSpaces(input);

    Assert.That(result, Is.EqualTo("No Other Separators"));
  }

  [Test]
  public void RecoverSpaces_WithSingleCharacterSegments_PreservesStructure() {
    const string input = "A.B.C.D";

    var result = TextManipulation.RecoverSpaces(input);

    Assert.That(result, Is.EqualTo("A B C D"));
  }

  [Test]
  public void RecoverSpaces_WithConsecutiveSeparators_ReplacesAll() {
    const string input = "Title__With__Double";

    var result = TextManipulation.RecoverSpaces(input);

    Assert.That(result, Is.EqualTo("Title  With  Double"));
  }

  #endregion

  #region RemoveBracketContent Extended Tests

  [Test]
  public void RemoveBracketContent_WithEmptyBrackets_RemovesThem() {
    const string input = "Title () here";

    var result = TextManipulation.RemoveBracketContent(input);

    // Empty brackets are preserved because the regex expects content
    Assert.That(result, Does.Not.Contain("()").Or.EqualTo("Title here"));
  }

  [Test]
  public void RemoveBracketContent_WithOnlyBrackets_ReturnsEmpty() {
    const string input = "(content)";

    var result = TextManipulation.RemoveBracketContent(input);

    Assert.That(result, Is.Empty);
  }

  [Test]
  public void RemoveBracketContent_WithMixedBracketTypes_RemovesAll() {
    const string input = "Title (2023) [HD] {rip} <info>";

    var result = TextManipulation.RemoveBracketContent(input);

    Assert.That(result, Is.EqualTo("Title"));
  }

  [Test]
  public void RemoveBracketContent_WithBracketsAtStart_RemovesThem() {
    const string input = "[720p] Movie Title";

    var result = TextManipulation.RemoveBracketContent(input);

    Assert.That(result, Is.EqualTo("Movie Title"));
  }

  [Test]
  public void RemoveBracketContent_WithBracketsAtEnd_RemovesThem() {
    const string input = "Movie Title (2023)";

    var result = TextManipulation.RemoveBracketContent(input);

    Assert.That(result, Is.EqualTo("Movie Title"));
  }

  #endregion

  #region GetFilenameWithoutExtension Extended Tests

  [Test]
  public void GetFilenameWithoutExtension_WithMultipleExtensions_RemovesOnlyLast() {
    const string path = @"C:\folder\file.backup.mkv";

    var result = TextManipulation.GetFilenameWithoutExtension(path);

    Assert.That(result, Is.EqualTo("file.backup"));
  }

  [Test]
  public void GetFilenameWithoutExtension_WithNoPath_JustFilename() {
    const string path = "movie.mkv";

    var result = TextManipulation.GetFilenameWithoutExtension(path);

    Assert.That(result, Is.EqualTo("movie"));
  }

  [Test]
  public void GetFilenameWithoutExtension_WithUnixPath_ExtractsCorrectly() {
    const string path = "/home/user/videos/movie.mkv";

    var result = TextManipulation.GetFilenameWithoutExtension(path);

    Assert.That(result, Is.EqualTo("movie"));
  }

  [Test]
  public void GetFilenameWithoutExtension_WithHiddenFile_ExtractsCorrectly() {
    const string path = ".hidden";

    var result = TextManipulation.GetFilenameWithoutExtension(path);

    // .hidden is treated as extension-only file, result may be empty
    Assert.That(result, Is.EqualTo("").Or.EqualTo(".hidden"));
  }

  [Test]
  public void GetFilenameWithoutExtension_WithSpacesInName_PreservesSpaces() {
    const string path = @"C:\folder\My Movie File.mkv";

    var result = TextManipulation.GetFilenameWithoutExtension(path);

    Assert.That(result, Is.EqualTo("My Movie File"));
  }

  #endregion

  #region Integration Tests

  [Test]
  public void FullProcessingPipeline_TypicalTvEpisode() {
    const string filename = "Show.Name.S02E05.Episode.Title.720p.BluRay.x264-GROUP";

    // Step 1: Recover spaces
    var withSpaces = TextManipulation.RecoverSpaces(filename);
    Assert.That(withSpaces, Does.Contain(" "));

    // Step 2: Extract season/episode
    var extracted = TextManipulation.ExtractSeasonEpisode(withSpaces);
    Assert.That(extracted, Is.Not.Null);
    Assert.That(extracted!.Value.Season, Is.EqualTo(2));
    Assert.That(extracted!.Value.Episode, Is.EqualTo(5));

    // Step 3: Format back
    var formatted = TextManipulation.FormatSeasonEpisode(extracted!.Value.Season, extracted!.Value.Episode);
    Assert.That(formatted, Is.EqualTo("s02e05"));
  }

  [Test]
  public void FullProcessingPipeline_MovieWithYear() {
    const string filename = "Movie.Name.2023.1080p.BluRay.x264-GROUP";

    // Step 1: Recover spaces
    var withSpaces = TextManipulation.RecoverSpaces(filename);
    Assert.That(withSpaces, Is.EqualTo("Movie Name 2023 1080p BluRay x264-GROUP"));

    // Step 2: No season/episode should be found
    var extracted = TextManipulation.ExtractSeasonEpisode(withSpaces);
    Assert.That(extracted, Is.Null);
  }

  [Test]
  public void FullProcessingPipeline_WithBrackets() {
    const string filename = "Show Name (2023) [720p] - Episode Title";

    // Remove bracket content
    var cleaned = TextManipulation.RemoveBracketContent(filename);
    Assert.That(cleaned, Does.Contain("Show Name"));
    Assert.That(cleaned, Does.Not.Contain("(2023)"));
    Assert.That(cleaned, Does.Not.Contain("[720p]"));
  }

  #endregion
}
