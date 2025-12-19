using NUnit.Framework;

namespace Hawkynt.NfoFileFormat.Tests;

[TestFixture]
public class EpisodeDetailsTests {

  [Test]
  public void EpisodeDetails_DefaultValues_AreNull() {
    var episode = new EpisodeDetails();

    Assert.That(episode.Title, Is.Null);
    Assert.That(episode.OriginalTitle, Is.Null);
    Assert.That(episode.Year, Is.EqualTo(0));
    Assert.That(episode.Art, Is.Null);
  }

  [Test]
  public void EpisodeDetails_SetProperties_ReturnsCorrectValues() {
    var episode = new EpisodeDetails {
      Title = "Test Episode",
      OriginalTitle = "Original Test Episode",
      Year = 2024,
      Art = new Art { Poster = "episode_poster.jpg", Fanart = "episode_fanart.jpg" }
    };

    Assert.That(episode.Title, Is.EqualTo("Test Episode"));
    Assert.That(episode.OriginalTitle, Is.EqualTo("Original Test Episode"));
    Assert.That(episode.Year, Is.EqualTo(2024));
    Assert.That(episode.Art, Is.Not.Null);
    Assert.That(episode.Art.Poster, Is.EqualTo("episode_poster.jpg"));
    Assert.That(episode.Art.Fanart, Is.EqualTo("episode_fanart.jpg"));
  }

}
