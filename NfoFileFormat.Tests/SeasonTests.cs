using NUnit.Framework;

namespace Hawkynt.NfoFileFormat.Tests;

[TestFixture]
public class SeasonTests {

  [Test]
  public void Season_DefaultValues_AreNull() {
    var season = new Season();

    Assert.That(season.Title, Is.Null);
    Assert.That(season.Year, Is.EqualTo(0));
    Assert.That(season.Seasonnumber, Is.EqualTo(0));
    Assert.That(season.Art, Is.Null);
  }

  [Test]
  public void Season_SetProperties_ReturnsCorrectValues() {
    var season = new Season {
      Title = "Season 1",
      Year = 2024,
      Seasonnumber = 1,
      Art = new Art { Poster = "season_poster.jpg", Fanart = "season_fanart.jpg" }
    };

    Assert.That(season.Title, Is.EqualTo("Season 1"));
    Assert.That(season.Year, Is.EqualTo(2024));
    Assert.That(season.Seasonnumber, Is.EqualTo(1));
    Assert.That(season.Art, Is.Not.Null);
    Assert.That(season.Art.Poster, Is.EqualTo("season_poster.jpg"));
    Assert.That(season.Art.Fanart, Is.EqualTo("season_fanart.jpg"));
  }

}
