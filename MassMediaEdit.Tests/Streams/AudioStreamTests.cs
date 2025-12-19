using System;
using System.Globalization;
using Classes;
using MassMediaEdit.Tests.TestData;
using NUnit.Framework;

namespace MassMediaEdit.Tests.Streams;

/// <summary>
/// Unit tests for the <see cref="AudioStream"/> class.
/// Tests use sample MediaInfo data from a typical MKV file.
/// </summary>
[TestFixture]
public sealed class AudioStreamTests {
  private AudioStream _stream = null!;

  [SetUp]
  public void SetUp() => this._stream = StreamFactory.CreateAudioStream();

  #region Channel Information Tests

  [Test]
  public void Channels_Returns2() 
    => Assert.That(this._stream.Channels, Is.EqualTo(2));

  [Test]
  public void ChannelPositions_ReturnsFrontLr() 
    => Assert.That(this._stream.ChannelPositions, Is.EqualTo("Front: L R"));

  [Test]
  public void ChannelLayout_ReturnsLr() 
    => Assert.That(this._stream.ChannelLayout, Is.EqualTo("L R"));

  #endregion

  #region Sampling Information Tests

  [Test]
  public void SamplingRate_Returns48000() 
    => Assert.That(this._stream.SamplingRate, Is.EqualTo(48000));

  [Test]
  public void SamplesPerFrame_Returns1024() 
    => Assert.That(this._stream.SamplesPerFrame, Is.EqualTo(1024));

  [Test]
  public void SamplesCount_ReturnsCorrectCount() 
    => Assert.That(this._stream.SamplesCount, Is.EqualTo(32217072L));

  [Test]
  public void AudioFrameRate_Returns46875() 
    => Assert.That(this._stream.AudioFrameRate, Is.EqualTo(46.875).Within(0.001));

  #endregion

  #region Format Detail Tests

  [Test]
  public void Format_ReturnsAac() 
    => Assert.That(this._stream.Format, Is.EqualTo("AAC"));

  [Test]
  public void FormatInfo_ReturnsAdvancedAudioCodecLowComplexity() 
    => Assert.That(this._stream.FormatInfo, Is.EqualTo("Advanced Audio Codec Low Complexity"));

  [Test]
  public void CommercialName_ReturnsAac() 
    => Assert.That(this._stream.CommercialName, Is.EqualTo("AAC"));

  [Test]
  public void FormatAdditionalFeatures_ReturnsLc() 
    => Assert.That(this._stream.FormatAdditionalFeatures, Is.EqualTo("LC"));

  [Test]
  public void CodecId_ReturnsCorrectCodecId() 
    => Assert.That(this._stream.CodecId, Is.EqualTo("A_AAC-2"));

  [Test]
  public void FormatWithProfile_ReturnsAacLc() 
    => Assert.That(this._stream.FormatWithProfile, Is.EqualTo("AAC LC"));

  #endregion

  #region Compression Tests

  [Test]
  public void CompressionModeValue_ReturnsLossy() 
    => Assert.That(this._stream.CompressionModeValue, Is.EqualTo(CompressionMode.Lossy));

  #endregion

  #region Delay/Sync Tests

  [Test]
  public void DelayInMilliseconds_Returns0() 
    => Assert.That(this._stream.DelayInMilliseconds, Is.EqualTo(0));

  [Test]
  public void DelayRelativeToVideoInMilliseconds_ReturnsMinus80() 
    => Assert.That(this._stream.DelayRelativeToVideoInMilliseconds, Is.EqualTo(-80));

  [Test]
  public void DelayOrigin_ReturnsContainer() 
    => Assert.That(this._stream.DelayOrigin, Is.EqualTo("Container"));

  #endregion

  #region Bitrate and Size Tests

  [Test]
  public void BitRateInBitsPerSecond_ReturnsCorrectValue() 
    => Assert.That(this._stream.BitRateInBitsPerSecond, Is.EqualTo(255908));

  [Test]
  public void SizeInBytes_ReturnsCorrectSize() 
    => Assert.That(this._stream.SizeInBytes, Is.EqualTo(21470405L));

  [Test]
  public void ProportionOfStream_ReturnsCorrectProportion() 
    => Assert.That(this._stream.ProportionOfStream, Is.EqualTo(0.36506).Within(0.00001));

  #endregion

  #region Duration Tests

  [Test]
  public void Duration_ReturnsCorrectDuration() {
    var expectedDuration = TimeSpan.FromMilliseconds(671189);
    Assert.That(this._stream.Duration, Is.EqualTo(expectedDuration));
  }

  [Test]
  public void FrameCount_Returns31462() 
    => Assert.That(this._stream.FrameCount, Is.EqualTo(31462));

  #endregion

  #region Language Tests

  [Test]
  public void Language_ReturnsGerman() {
    Assert.That(this._stream.Language, Is.Not.Null);
    Assert.That(this._stream.Language.TwoLetterISOLanguageName, Is.EqualTo("de"));
  }

  [Test]
  public void Language_ThreeLetterCode_ReturnsGerman() {
    Assert.That(this._stream.Language, Is.Not.Null);
    Assert.That(this._stream.Language.ThreeLetterISOLanguageName, Is.EqualTo("deu"));
  }

  #endregion

  #region Common Stream Property Tests

  [Test]
  public void IsDefault_ReturnsTrue() 
    => Assert.That(this._stream.IsDefault, Is.True);

  [Test]
  public void IsForced_ReturnsFalse() 
    => Assert.That(this._stream.IsForced, Is.False);

  [Test]
  public void StreamOrder_Returns1() 
    => Assert.That(this._stream.StreamOrder, Is.EqualTo(1));

  [Test]
  public void Id_Returns2() 
    => Assert.That(this._stream.Id, Is.EqualTo(2));

  [Test]
  public void UniqueId_ReturnsCorrectId() 
    => Assert.That(this._stream.UniqueId, Is.EqualTo("10515736078130736169"));

  [Test]
  public void StreamIdentifier_Returns0() 
    => Assert.That(this._stream.StreamIdentifier, Is.EqualTo(0));

  #endregion

  #region Compression Mode Edge Cases

  [Test]
  public void CompressionModeValue_WithLossless_ReturnsLossless() {
    var data = """
      Kind of stream                           : Audio
      Compression mode                         : Lossless
      """;
    var stream = StreamFactory.CreateAudioStream(data);
    
    Assert.That(stream.CompressionModeValue, Is.EqualTo(CompressionMode.Lossless));
  }

  [Test]
  public void CompressionModeValue_WithUnknownValue_ReturnsUnknown() {
    var data = """
      Kind of stream                           : Audio
      Compression mode                         : SomethingElse
      """;
    var stream = StreamFactory.CreateAudioStream(data);
    
    Assert.That(stream.CompressionModeValue, Is.EqualTo(CompressionMode.Unknown));
  }

  [Test]
  public void CompressionModeValue_WhenMissing_ReturnsUnknown() {
    var data = """
      Kind of stream                           : Audio
      Format                                   : AAC
      """;
    var stream = StreamFactory.CreateAudioStream(data);
    
    Assert.That(stream.CompressionModeValue, Is.EqualTo(CompressionMode.Unknown));
  }

  #endregion

  #region Replay Gain Tests

  [Test]
  public void ReplayGain_WhenPresent_ReturnsValue() {
    var data = """
      Kind of stream                           : Audio
      Replay gain                              : -3.5
      """;
    var stream = StreamFactory.CreateAudioStream(data);
    
    Assert.That(stream.ReplayGain, Is.EqualTo(-3.5).Within(0.01));
  }

  [Test]
  public void ReplayGain_WhenMissing_ReturnsNull() {
    var stream = StreamFactory.CreateAudioStream();
    
    Assert.That(stream.ReplayGain, Is.Null);
  }

  [Test]
  public void ReplayPeak_WhenPresent_ReturnsValue() {
    var data = """
      Kind of stream                           : Audio
      Replay gain peak                         : 0.95
      """;
    var stream = StreamFactory.CreateAudioStream(data);
    
    Assert.That(stream.ReplayPeak, Is.EqualTo(0.95).Within(0.01));
  }

  [Test]
  public void ReplayPeak_WhenMissing_ReturnsNull() {
    var stream = StreamFactory.CreateAudioStream();
    
    Assert.That(stream.ReplayPeak, Is.Null);
  }

  #endregion

  #region Format With Profile Edge Cases

  [Test]
  public void FormatWithProfile_WithoutAdditionalFeatures_ReturnsFormatOnly() {
    var data = """
      Kind of stream                           : Audio
      Format                                   : FLAC
      """;
    var stream = StreamFactory.CreateAudioStream(data);
    
    Assert.That(stream.FormatWithProfile, Is.EqualTo("FLAC"));
  }

  [Test]
  public void FormatWithProfile_WithEmptyAdditionalFeatures_ReturnsFormatOnly() {
    var data = """
      Kind of stream                           : Audio
      Format                                   : FLAC
      Format_AdditionalFeatures                : 
      """;
    var stream = StreamFactory.CreateAudioStream(data);
    
    Assert.That(stream.FormatWithProfile, Is.EqualTo("FLAC"));
  }

  #endregion
}
