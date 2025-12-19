using System;
using System.IO;
using System.Xml.Serialization;
using NUnit.Framework;

namespace Hawkynt.NfoFileFormat.Tests;

[TestFixture]
public class NfoFileTests {

  private string _testDirectory = null!;

  [SetUp]
  public void SetUp() {
    this._testDirectory = Path.Combine(Path.GetTempPath(), $"NfoFileTests_{Guid.NewGuid():N}");
    Directory.CreateDirectory(this._testDirectory);
  }

  [TearDown]
  public void TearDown() {
    if (Directory.Exists(this._testDirectory))
      Directory.Delete(this._testDirectory, true);
  }

  #region HasNfo Tests

  [Test]
  public void HasNfo_WhenNfoExists_ReturnsTrue() {
    var mediaFile = this._CreateTestFile("test.mp4");
    this._CreateNfoFile("test.nfo", SampleNfoData.MovieNfo);

    var result = NfoFile.HasNfo(mediaFile);

    Assert.That(result, Is.True);
  }

  [Test]
  public void HasNfo_WhenNfoDoesNotExist_ReturnsFalse() {
    var mediaFile = this._CreateTestFile("test.mp4");

    var result = NfoFile.HasNfo(mediaFile);

    Assert.That(result, Is.False);
  }

  #endregion

  #region LoadMovieOrNull Tests

  [Test]
  public void LoadMovieOrNull_WhenValidNfoExists_ReturnsMovie() {
    var mediaFile = this._CreateTestFile("movie.mp4");
    this._CreateNfoFile("movie.nfo", SampleNfoData.MovieNfo);

    var result = NfoFile.LoadMovieOrNull(mediaFile);

    Assert.That(result, Is.Not.Null);
    Assert.That(result.Title, Is.EqualTo("Test Movie"));
    Assert.That(result.Year, Is.EqualTo(2024));
  }

  [Test]
  public void LoadMovieOrNull_WhenNfoDoesNotExist_ReturnsNull() {
    var mediaFile = this._CreateTestFile("movie.mp4");

    var result = NfoFile.LoadMovieOrNull(mediaFile);

    Assert.That(result, Is.Null);
  }

  [Test]
  public void LoadMovieOrNull_WhenNfoIsInvalid_ReturnsNull() {
    var mediaFile = this._CreateTestFile("movie.mp4");
    this._CreateNfoFile("movie.nfo", "invalid xml content");

    var result = NfoFile.LoadMovieOrNull(mediaFile);

    Assert.That(result, Is.Null);
  }

  #endregion

  #region LoadShowOrNull Tests

  [Test]
  public void LoadShowOrNull_WhenValidNfoExists_ReturnsTVShow() {
    var mediaFile = this._CreateTestFile("tvshow.mp4");
    this._CreateNfoFile("tvshow.nfo", SampleNfoData.TVShowNfo);

    var result = NfoFile.LoadShowOrNull(mediaFile);

    Assert.That(result, Is.Not.Null);
    Assert.That(result.Title, Is.EqualTo("Test Show"));
    Assert.That(result.Year, Is.EqualTo(2024));
  }

  [Test]
  public void LoadShowOrNull_WhenNfoDoesNotExist_ReturnsNull() {
    var mediaFile = this._CreateTestFile("tvshow.mp4");

    var result = NfoFile.LoadShowOrNull(mediaFile);

    Assert.That(result, Is.Null);
  }

  #endregion

  #region LoadSeasonOrNull Tests

  [Test]
  public void LoadSeasonOrNull_WhenValidNfoExists_ReturnsSeason() {
    var mediaFile = this._CreateTestFile("season.mp4");
    this._CreateNfoFile("season.nfo", SampleNfoData.SeasonNfo);

    var result = NfoFile.LoadSeasonOrNull(mediaFile);

    Assert.That(result, Is.Not.Null);
    Assert.That(result.Title, Is.EqualTo("Season 1"));
    Assert.That(result.Seasonnumber, Is.EqualTo(1));
  }

  [Test]
  public void LoadSeasonOrNull_WhenNfoDoesNotExist_ReturnsNull() {
    var mediaFile = this._CreateTestFile("season.mp4");

    var result = NfoFile.LoadSeasonOrNull(mediaFile);

    Assert.That(result, Is.Null);
  }

  #endregion

  #region LoadEpisodeOrNull Tests

  [Test]
  public void LoadEpisodeOrNull_WhenValidNfoExists_ReturnsEpisodeDetails() {
    var mediaFile = this._CreateTestFile("episode.mp4");
    this._CreateNfoFile("episode.nfo", SampleNfoData.EpisodeNfo);

    var result = NfoFile.LoadEpisodeOrNull(mediaFile);

    Assert.That(result, Is.Not.Null);
    Assert.That(result.Title, Is.EqualTo("Test Episode"));
    Assert.That(result.Year, Is.EqualTo(2024));
  }

  [Test]
  public void LoadEpisodeOrNull_WhenNfoDoesNotExist_ReturnsNull() {
    var mediaFile = this._CreateTestFile("episode.mp4");

    var result = NfoFile.LoadEpisodeOrNull(mediaFile);

    Assert.That(result, Is.Null);
  }

  #endregion

  #region Update Tests

  [Test]
  public void Update_WhenNfoExists_UpdatesValues() {
    var nfoFile = this._CreateNfoFile("movie.nfo", SampleNfoData.MovieNfo);
    var updatedMovie = new Movie { Title = "Updated Movie Title" };

    NfoFile.Update(nfoFile, updatedMovie);

    var content = File.ReadAllText(nfoFile.FullName);
    Assert.That(content, Does.Contain("Updated Movie Title"));
  }

  [Test]
  public void Update_WhenNfoDoesNotExist_ThrowsFileNotFoundException() {
    var nfoFile = new FileInfo(Path.Combine(this._testDirectory, "nonexistent.nfo"));
    var movie = new Movie { Title = "Test" };

    Assert.Throws<FileNotFoundException>(() => NfoFile.Update(nfoFile, movie));
  }

  [Test]
  public void Update_WhenRootElementNotFound_ThrowsInvalidOperationException() {
    var nfoFile = this._CreateNfoFile("wrong.nfo", SampleNfoData.TVShowNfo);
    var movie = new Movie { Title = "Test" };

    Assert.Throws<InvalidOperationException>(() => NfoFile.Update(nfoFile, movie));
  }

  #endregion

  #region Helper Methods

  private FileInfo _CreateTestFile(string fileName) {
    var filePath = Path.Combine(this._testDirectory, fileName);
    File.WriteAllText(filePath, string.Empty);
    return new FileInfo(filePath);
  }

  private FileInfo _CreateNfoFile(string fileName, string content) {
    var filePath = Path.Combine(this._testDirectory, fileName);
    File.WriteAllText(filePath, content);
    return new FileInfo(filePath);
  }

  #endregion

}
