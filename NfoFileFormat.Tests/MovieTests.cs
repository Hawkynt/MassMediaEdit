using NUnit.Framework;

namespace Hawkynt.NfoFileFormat.Tests;

[TestFixture]
public class MovieTests {

  [Test]
  public void Movie_DefaultValues_AreNull() {
    var movie = new Movie();

    Assert.That(movie.Title, Is.Null);
    Assert.That(movie.OriginalTitle, Is.Null);
    Assert.That(movie.Year, Is.EqualTo(0));
    Assert.That(movie.MPAA, Is.Null);
    Assert.That(movie.Certification, Is.Null);
    Assert.That(movie.Art, Is.Null);
  }

  [Test]
  public void Movie_SetProperties_ReturnsCorrectValues() {
    var movie = new Movie {
      Title = "Test Movie",
      OriginalTitle = "Original Test Movie",
      Year = 2024,
      MPAA = "PG-13",
      Certification = "US:PG-13",
      Art = new Art { Poster = "poster.jpg", Fanart = "fanart.jpg" }
    };

    Assert.That(movie.Title, Is.EqualTo("Test Movie"));
    Assert.That(movie.OriginalTitle, Is.EqualTo("Original Test Movie"));
    Assert.That(movie.Year, Is.EqualTo(2024));
    Assert.That(movie.MPAA, Is.EqualTo("PG-13"));
    Assert.That(movie.Certification, Is.EqualTo("US:PG-13"));
    Assert.That(movie.Art, Is.Not.Null);
    Assert.That(movie.Art.Poster, Is.EqualTo("poster.jpg"));
    Assert.That(movie.Art.Fanart, Is.EqualTo("fanart.jpg"));
  }

}
