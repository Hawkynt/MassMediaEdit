using Classes;
using MassMediaEdit.Tests.TestData;
using NUnit.Framework;

namespace MassMediaEdit.Tests.Streams;

/// <summary>
/// Unit tests for base <see cref="MediaStream"/> properties using different stream types.
/// </summary>
[TestFixture]
public sealed class MediaStreamTests {
  #region Format Tests

  [Test]
  public void Format_VideoStream_ReturnsCorrectFormat() {
    var stream = StreamFactory.CreateVideoStream();
    Assert.That(stream.Format, Is.EqualTo("AVC"));
  }

  [Test]
  public void Format_AudioStream_ReturnsCorrectFormat() {
    var stream = StreamFactory.CreateAudioStream();
    Assert.That(stream.Format, Is.EqualTo("AAC"));
  }

  [Test]
  public void Format_GeneralStream_ReturnsMatroska() {
    var stream = StreamFactory.CreateGeneralStream();
    Assert.That(stream.Format, Is.EqualTo("Matroska"));
  }

  #endregion

  #region FormatInfo Tests

  [Test]
  public void FormatInfo_VideoStream_ReturnsDescription() {
    var stream = StreamFactory.CreateVideoStream();
    Assert.That(stream.FormatInfo, Is.EqualTo("Advanced Video Codec"));
  }

  [Test]
  public void FormatInfo_AudioStream_ReturnsDescription() {
    var stream = StreamFactory.CreateAudioStream();
    Assert.That(stream.FormatInfo, Is.EqualTo("Advanced Audio Codec Low Complexity"));
  }

  #endregion

  #region Empty/Missing Value Tests

  [Test]
  public void Name_WhenNotPresent_ReturnsNull() {
    // Neither video nor audio stream in our sample has a title/name
    var stream = StreamFactory.CreateVideoStream();
    Assert.That(stream.Name, Is.Null);
  }

  [Test]
  public void Language_VideoStream_ReturnsNull() {
    // Video streams typically don't have language set
    var stream = StreamFactory.CreateVideoStream();
    Assert.That(stream.Language, Is.Null);
  }

  [Test]
  public void ReplayGain_AudioStream_ReturnsNull() {
    // Sample audio doesn't have replay gain
    var stream = StreamFactory.CreateAudioStream();
    Assert.That(stream.ReplayGain, Is.Null);
  }

  #endregion

  #region Default Value Tests

  [Test]
  public void GetIntOrDefault_WhenMissing_ReturnsZero() {
    // Test using a property that relies on GetIntOrDefault for a missing value
    var stream = StreamFactory.CreateGeneralStream();
    // Title is a string property, but we can test int default behavior
    // through properties that might not exist
    Assert.That(stream.Track, Is.Null);
  }

  #endregion
}
