using MassMediaEdit.Utilities;
using NUnit.Framework;

namespace MassMediaEdit.Tests.Utilities;

/// <summary>
/// Unit tests for the <see cref="TextManipulation"/> utility class.
/// </summary>
[TestFixture]
public sealed class TextManipulationTests {
  #region ExtractSeasonEpisode Tests

  [Test]
  public void ExtractSeasonEpisode_WithNull_ReturnsNull() {
    var result = TextManipulation.ExtractSeasonEpisode(null);

    Assert.That(result, Is.Null);
  }

  [Test]
  public void ExtractSeasonEpisode_WithEmptyString_ReturnsNull() {
    var result = TextManipulation.ExtractSeasonEpisode(string.Empty);

    Assert.That(result, Is.Null);
  }

  [Test]
  public void ExtractSeasonEpisode_WithWhitespace_ReturnsNull() {
    var result = TextManipulation.ExtractSeasonEpisode("   ");

    Assert.That(result, Is.Null);
  }

  [Test]
  public void ExtractSeasonEpisode_WithNoMatch_ReturnsNull() {
    var result = TextManipulation.ExtractSeasonEpisode("Just a regular title");

    Assert.That(result, Is.Null);
  }

  [TestCase("s05e04", (ushort)5, (ushort)4)]
  [TestCase("S01E02", (ushort)1, (ushort)2)]
  [TestCase("s1e1", (ushort)1, (ushort)1)]
  [TestCase("S10E25", (ushort)10, (ushort)25)]
  public void ExtractSeasonEpisode_WithSimpleFormat_ExtractsCorrectly(
    string input, ushort expectedSeason, ushort expectedEpisode) {
    var result = TextManipulation.ExtractSeasonEpisode(input);

    Assert.That(result, Is.Not.Null);
    Assert.That(result!.Value.Season, Is.EqualTo(expectedSeason));
    Assert.That(result!.Value.Episode, Is.EqualTo(expectedEpisode));
  }

  [TestCase("s05e04 - Episode Title", "Episode Title")]
  [TestCase("S01E02.The.Title", "The.Title")]
  [TestCase("s1e1 - Something - Extra", "Something - Extra")]
  public void ExtractSeasonEpisode_WithTitle_ExtractsTitle(string input, string expectedTitle) {
    var result = TextManipulation.ExtractSeasonEpisode(input);

    Assert.That(result, Is.Not.Null);
    Assert.That(result!.Value.Title, Is.EqualTo(expectedTitle));
  }

  [Test]
  public void ExtractSeasonEpisode_WithNoTitleAfterMatch_ReturnNullTitle() {
    var result = TextManipulation.ExtractSeasonEpisode("s05e04");

    Assert.That(result, Is.Not.Null);
    Assert.That(result!.Value.Title, Is.Null);
  }

  [TestCase("season5episode4", (ushort)5, (ushort)4)]
  [TestCase("Season 1 Episode 10", (ushort)1, (ushort)10)]
  public void ExtractSeasonEpisode_WithLongFormat_ExtractsCorrectly(
    string input, ushort expectedSeason, ushort expectedEpisode) {
    var result = TextManipulation.ExtractSeasonEpisode(input);

    Assert.That(result, Is.Not.Null);
    Assert.That(result!.Value.Season, Is.EqualTo(expectedSeason));
    Assert.That(result!.Value.Episode, Is.EqualTo(expectedEpisode));
  }

  #endregion

  #region FormatSeasonEpisode Tests

  [TestCase((ushort)1, (ushort)1, "s01e01")]
  [TestCase((ushort)5, (ushort)4, "s05e04")]
  [TestCase((ushort)10, (ushort)25, "s10e25")]
  [TestCase((ushort)1, (ushort)100, "s01e100")]
  public void FormatSeasonEpisode_FormatsCorrectly(
    ushort season, ushort episode, string expected) {
    var result = TextManipulation.FormatSeasonEpisode(season, episode);

    Assert.That(result, Is.EqualTo(expected));
  }

  #endregion

  #region RecoverSpaces Tests

  [Test]
  public void RecoverSpaces_WithNull_ReturnsNull() {
    var result = TextManipulation.RecoverSpaces(null);

    Assert.That(result, Is.Null);
  }

  [Test]
  public void RecoverSpaces_WithExistingSpaces_ReturnsUnchanged() {
    const string input = "Already has spaces";

    var result = TextManipulation.RecoverSpaces(input);

    Assert.That(result, Is.EqualTo(input));
  }

  [Test]
  public void RecoverSpaces_WithUrlEncodedSpaces_RecoversThem() {
    const string input = "Some%20Title%20Here";

    var result = TextManipulation.RecoverSpaces(input);

    Assert.That(result, Is.EqualTo("Some Title Here"));
  }

  [TestCase("Some_Title_Here", "Some Title Here")]
  [TestCase("Some.Title.Here", "Some Title Here")]
  [TestCase("Some-Title-Here", "Some Title Here")]
  [TestCase("Some+Title+Here", "Some Title Here")]
  public void RecoverSpaces_WithCommonReplacements_RecoversThem(string input, string expected) {
    var result = TextManipulation.RecoverSpaces(input);

    Assert.That(result, Is.EqualTo(expected));
  }

  [Test]
  public void RecoverSpaces_WithNoRecoverablePattern_ReturnsUnchanged() {
    const string input = "NoSpacesNoPattern";

    var result = TextManipulation.RecoverSpaces(input);

    Assert.That(result, Is.EqualTo(input));
  }

  #endregion

  #region RemoveBracketContent Tests

  [Test]
  public void RemoveBracketContent_WithNull_ReturnsNull() {
    var result = TextManipulation.RemoveBracketContent(null);

    Assert.That(result, Is.Null);
  }

  [Test]
  public void RemoveBracketContent_WithNoBrackets_ReturnsUnchanged() {
    const string input = "No brackets here";

    var result = TextManipulation.RemoveBracketContent(input);

    Assert.That(result, Is.EqualTo(input));
  }

  [TestCase("Title (2023)", "Title")]
  [TestCase("Title [720p]", "Title")]
  [TestCase("Title {HD}", "Title")]
  [TestCase("Title <info>", "Title")]
  public void RemoveBracketContent_WithSingleBrackets_RemovesThem(string input, string expected) {
    var result = TextManipulation.RemoveBracketContent(input);

    Assert.That(result, Is.EqualTo(expected));
  }

  [Test]
  public void RemoveBracketContent_WithMultipleBrackets_RemovesAll() {
    const string input = "Title (2023) [720p] {rip}";

    var result = TextManipulation.RemoveBracketContent(input);

    Assert.That(result, Is.EqualTo("Title"));
  }

  [Test]
  public void RemoveBracketContent_WithNestedBrackets_HandlesCorrectly() {
    const string input = "Title (info (nested))";

    var result = TextManipulation.RemoveBracketContent(input);

    // Non-greedy matching means inner content is preserved partially
    Assert.That(result, Does.Not.Contain("("));
  }

  #endregion

  #region GetFilenameWithoutExtension Tests

  [TestCase("C:\\folder\\file.mkv", "file")]
  [TestCase("file.mp4", "file")]
  [TestCase("/path/to/movie.avi", "movie")]
  [TestCase("file.with.dots.mkv", "file.with.dots")]
  public void GetFilenameWithoutExtension_ExtractsCorrectly(string path, string expected) {
    var result = TextManipulation.GetFilenameWithoutExtension(path);

    Assert.That(result, Is.EqualTo(expected));
  }

  #endregion

  #region Integration Tests

  [Test]
  public void ExtractSeasonEpisode_AndFormat_Roundtrip() {
    const string input = "s05e04 - The Episode";

    var extracted = TextManipulation.ExtractSeasonEpisode(input);
    Assert.That(extracted, Is.Not.Null);

    var formatted = TextManipulation.FormatSeasonEpisode(extracted!.Value.Season, extracted!.Value.Episode);

    Assert.That(formatted, Is.EqualTo("s05e04"));
  }

  [Test]
  public void TypicalFilenameProcessing_WorksCorrectly() {
    const string filename = "Show.Name.S02E05.Episode.Title.720p.BluRay.x264-GROUP";

    // First recover spaces
    var withSpaces = TextManipulation.RecoverSpaces(filename);
    Assert.That(withSpaces, Is.EqualTo("Show Name S02E05 Episode Title 720p BluRay x264-GROUP"));

    // Then extract season/episode
    var extracted = TextManipulation.ExtractSeasonEpisode(withSpaces);
    Assert.That(extracted, Is.Not.Null);
    Assert.That(extracted!.Value.Season, Is.EqualTo(2));
    Assert.That(extracted!.Value.Episode, Is.EqualTo(5));
  }

  #endregion
}
