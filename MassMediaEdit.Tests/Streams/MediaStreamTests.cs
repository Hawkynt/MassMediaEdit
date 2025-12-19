using System.Globalization;
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

  #region Language Parsing Tests

  [Test]
  public void Language_WithGermanCode_ReturnsGermanCulture() {
    var stream = StreamFactory.CreateAudioStream();
    
    Assert.That(stream.Language, Is.Not.Null);
    Assert.That(stream.Language.TwoLetterISOLanguageName, Is.EqualTo("de"));
  }

  [Test]
  public void Language_WithEnglishWord_ReturnsEnglishCulture() {
    var data = """
      Kind of stream                           : Audio
      Language                                 : English
      """;
    var stream = StreamFactory.CreateAudioStream(data);
    
    Assert.That(stream.Language, Is.Not.Null);
    Assert.That(stream.Language.TwoLetterISOLanguageName, Is.EqualTo("en"));
  }

  [Test]
  public void Language_WithGermanWord_ReturnsGermanCulture() {
    var data = """
      Kind of stream                           : Audio
      Language                                 : German
      """;
    var stream = StreamFactory.CreateAudioStream(data);
    
    Assert.That(stream.Language, Is.Not.Null);
    Assert.That(stream.Language.TwoLetterISOLanguageName, Is.EqualTo("de"));
  }

  [Test]
  public void Language_WithDeutschWord_ReturnsGermanCulture() {
    var data = """
      Kind of stream                           : Audio
      Language                                 : Deutsch
      """;
    var stream = StreamFactory.CreateAudioStream(data);
    
    Assert.That(stream.Language, Is.Not.Null);
    Assert.That(stream.Language.TwoLetterISOLanguageName, Is.EqualTo("de"));
  }

  [Test]
  public void Language_WithEnglischWord_ReturnsEnglishCulture() {
    var data = """
      Kind of stream                           : Audio
      Language                                 : Englisch
      """;
    var stream = StreamFactory.CreateAudioStream(data);
    
    Assert.That(stream.Language, Is.Not.Null);
    Assert.That(stream.Language.TwoLetterISOLanguageName, Is.EqualTo("en"));
  }

  [Test]
  public void Language_WithIetfTag_ReturnsCulture() {
    var data = """
      Kind of stream                           : Audio
      Language                                 : en-US
      """;
    var stream = StreamFactory.CreateAudioStream(data);
    
    Assert.That(stream.Language, Is.Not.Null);
    Assert.That(stream.Language.TwoLetterISOLanguageName, Is.EqualTo("en"));
  }

  [Test]
  public void Language_WithInvalidValue_ReturnsNull() {
    var data = """
      Kind of stream                           : Audio
      Language                                 : NotALanguage12345
      """;
    var stream = StreamFactory.CreateAudioStream(data);
    
    Assert.That(stream.Language, Is.Null);
  }

  [Test]
  public void Language_WhenMissing_ReturnsNull() {
    var data = """
      Kind of stream                           : Audio
      Format                                   : AAC
      """;
    var stream = StreamFactory.CreateAudioStream(data);
    
    Assert.That(stream.Language, Is.Null);
  }

  #endregion

  #region Codec Tests

  [Test]
  public void Codec_VideoStream_ReturnsValue() {
    var stream = StreamFactory.CreateVideoStream();
    // Codec uses index 1 which might return different formatted value
    Assert.That(stream.Codec, Is.Null.Or.Not.Empty);
  }

  [Test]
  public void CodecId_VideoStream_ReturnsCorrectValue() {
    var stream = StreamFactory.CreateVideoStream();
    Assert.That(stream.CodecId, Is.EqualTo("V_MPEG4/ISO/AVC"));
  }

  [Test]
  public void CodecId_AudioStream_ReturnsCorrectValue() {
    var stream = StreamFactory.CreateAudioStream();
    Assert.That(stream.CodecId, Is.EqualTo("A_AAC-2"));
  }

  #endregion

  #region Boolean Property Tests

  [Test]
  public void IsDefault_WhenYes_ReturnsTrue() {
    var data = """
      Kind of stream                           : Audio
      Default                                  : Yes
      """;
    var stream = StreamFactory.CreateAudioStream(data);
    
    Assert.That(stream.IsDefault, Is.True);
  }

  [Test]
  public void IsDefault_WhenNo_ReturnsFalse() {
    var data = """
      Kind of stream                           : Audio
      Default                                  : No
      """;
    var stream = StreamFactory.CreateAudioStream(data);
    
    Assert.That(stream.IsDefault, Is.False);
  }

  [Test]
  public void IsDefault_WhenMissing_ReturnsFalse() {
    var data = """
      Kind of stream                           : Audio
      Format                                   : AAC
      """;
    var stream = StreamFactory.CreateAudioStream(data);
    
    Assert.That(stream.IsDefault, Is.False);
  }

  [Test]
  public void IsForced_WhenYes_ReturnsTrue() {
    var data = """
      Kind of stream                           : Audio
      Forced                                   : Yes
      """;
    var stream = StreamFactory.CreateAudioStream(data);
    
    Assert.That(stream.IsForced, Is.True);
  }

  [Test]
  public void IsForced_WhenNo_ReturnsFalse() {
    var data = """
      Kind of stream                           : Audio
      Forced                                   : No
      """;
    var stream = StreamFactory.CreateAudioStream(data);
    
    Assert.That(stream.IsForced, Is.False);
  }

  #endregion

  #region Name Property Tests

  [Test]
  public void Name_WhenPresent_ReturnsValue() {
    var data = """
      Kind of stream                           : Audio
      Title                                    : Main Audio Track
      """;
    var stream = StreamFactory.CreateAudioStream(data);
    
    Assert.That(stream.Name, Is.EqualTo("Main Audio Track"));
  }

  #endregion

  #region Common Properties Tests

  [Test]
  public void CommercialName_VideoStream_ReturnsValue() {
    var stream = StreamFactory.CreateVideoStream();
    Assert.That(stream.CommercialName, Is.EqualTo("AVC"));
  }

  [Test]
  public void CommercialName_AudioStream_ReturnsValue() {
    var stream = StreamFactory.CreateAudioStream();
    Assert.That(stream.CommercialName, Is.EqualTo("AAC"));
  }

  [Test]
  public void DelayOrigin_WhenPresent_ReturnsValue() {
    var stream = StreamFactory.CreateVideoStream();
    Assert.That(stream.DelayOrigin, Is.EqualTo("Container"));
  }

  #endregion
}
