using NUnit.Framework;

namespace Hawkynt.NfoFileFormat.Tests;

[TestFixture]
public class ArtTests {

  [Test]
  public void Art_DefaultValues_AreNull() {
    var art = new Art();

    Assert.That(art.Poster, Is.Null);
    Assert.That(art.Fanart, Is.Null);
  }

  [Test]
  public void Art_SetProperties_ReturnsCorrectValues() {
    var art = new Art {
      Poster = "poster.jpg",
      Fanart = "fanart.jpg"
    };

    Assert.That(art.Poster, Is.EqualTo("poster.jpg"));
    Assert.That(art.Fanart, Is.EqualTo("fanart.jpg"));
  }

}
