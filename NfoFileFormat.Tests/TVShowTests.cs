using NUnit.Framework;

namespace Hawkynt.NfoFileFormat.Tests;

[TestFixture]
public class TVShowTests {

  [Test]
  public void TVShow_DefaultValues_AreNull() {
    var tvShow = new TVShow();

    Assert.That(tvShow.Title, Is.Null);
    Assert.That(tvShow.OriginalTitle, Is.Null);
    Assert.That(tvShow.Year, Is.EqualTo(0));
    Assert.That(tvShow.MPAA, Is.Null);
    Assert.That(tvShow.Certification, Is.Null);
    Assert.That(tvShow.Art, Is.Null);
  }

  [Test]
  public void TVShow_SetProperties_ReturnsCorrectValues() {
    var tvShow = new TVShow {
      Title = "Test Show",
      OriginalTitle = "Original Test Show",
      Year = 2024,
      MPAA = "TV-14",
      Certification = "US:TV-14",
      Art = new Art { Poster = "show_poster.jpg", Fanart = "show_fanart.jpg" }
    };

    Assert.That(tvShow.Title, Is.EqualTo("Test Show"));
    Assert.That(tvShow.OriginalTitle, Is.EqualTo("Original Test Show"));
    Assert.That(tvShow.Year, Is.EqualTo(2024));
    Assert.That(tvShow.MPAA, Is.EqualTo("TV-14"));
    Assert.That(tvShow.Certification, Is.EqualTo("US:TV-14"));
    Assert.That(tvShow.Art, Is.Not.Null);
    Assert.That(tvShow.Art.Poster, Is.EqualTo("show_poster.jpg"));
    Assert.That(tvShow.Art.Fanart, Is.EqualTo("show_fanart.jpg"));
  }

}
