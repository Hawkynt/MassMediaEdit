using System;
using Classes;
using MassMediaEdit.Tests.TestData;
using NUnit.Framework;

namespace MassMediaEdit.Tests.Streams;

/// <summary>
/// Unit tests for the <see cref="VideoStream"/> class.
/// Tests use sample MediaInfo data from a real MKV file (Adventure Time s03e15.mkv).
/// </summary>
[TestFixture]
public sealed class VideoStreamTests {
  private VideoStream _stream = null!;

  [SetUp]
  public void SetUp() => this._stream = StreamFactory.CreateVideoStream();

  #region Dimension Tests

  [Test]
  public void WidthInPixels_Returns720() 
    => Assert.That(this._stream.WidthInPixels, Is.EqualTo(720));

  [Test]
  public void HeightInPixels_Returns404() 
    => Assert.That(this._stream.HeightInPixels, Is.EqualTo(404));

  [Test]
  public void StoredHeightInPixels_Returns416() 
    => Assert.That(this._stream.StoredHeightInPixels, Is.EqualTo(416));

  [Test]
  public void SampledWidthInPixels_Returns720() 
    => Assert.That(this._stream.SampledWidthInPixels, Is.EqualTo(720));

  [Test]
  public void SampledHeightInPixels_Returns404() 
    => Assert.That(this._stream.SampledHeightInPixels, Is.EqualTo(404));

  [Test]
  public void PixelAspectRatio_Returns1() 
    => Assert.That(this._stream.PixelAspectRatio, Is.EqualTo(1.0));

  [Test]
  public void DisplayAspectRatio_ReturnsCorrectValue() 
    => Assert.That(this._stream.DisplayAspectRatio, Is.EqualTo(1.782).Within(0.001));

  [Test]
  public void DisplayAspectRatioString_Returns16By9() 
    => Assert.That(this._stream.DisplayAspectRatioString, Is.EqualTo("16:9"));

  #endregion

  #region Frame Rate Tests

  [Test]
  public void FramesPerSecond_Returns25() 
    => Assert.That(this._stream.FramesPerSecond, Is.EqualTo(25.0));

  [Test]
  public void FrameRateModeValue_ReturnsConstant() 
    => Assert.That(this._stream.FrameRateModeValue, Is.EqualTo(FrameRateMode.Constant));

  [Test]
  public void OriginalFrameRateMode_ReturnsVfr() 
    => Assert.That(this._stream.OriginalFrameRateMode, Is.EqualTo("VFR"));

  [Test]
  public void FrameRateNumerator_Returns25() 
    => Assert.That(this._stream.FrameRateNumerator, Is.EqualTo(25));

  [Test]
  public void FrameRateDenominator_Returns1() 
    => Assert.That(this._stream.FrameRateDenominator, Is.EqualTo(1));

  [Test]
  public void FrameCount_Returns16765() 
    => Assert.That(this._stream.FrameCount, Is.EqualTo(16765));

  #endregion

  #region Format Detail Tests

  [Test]
  public void Format_ReturnsAvc() 
    => Assert.That(this._stream.Format, Is.EqualTo("AVC"));

  [Test]
  public void FormatInfo_ReturnsAdvancedVideoCodec() 
    => Assert.That(this._stream.FormatInfo, Is.EqualTo("Advanced Video Codec"));

  [Test]
  public void FormatProfile_ReturnsHighL3() 
    => Assert.That(this._stream.FormatProfile, Is.EqualTo("High@L3"));

  [Test]
  public void FormatSettings_ReturnsCabacAndRefFrames() 
    => Assert.That(this._stream.FormatSettings, Is.EqualTo("CABAC / 4 Ref Frames"));

  [Test]
  public void UsesCabac_ReturnsTrue() 
    => Assert.That(this._stream.UsesCabac, Is.True);

  [Test]
  public void ReferenceFrames_Returns4() 
    => Assert.That(this._stream.ReferenceFrames, Is.EqualTo(4));

  [Test]
  public void InternetMediaType_ReturnsVideoH264() 
    => Assert.That(this._stream.InternetMediaType, Is.EqualTo("video/H264"));

  [Test]
  public void CodecId_ReturnsCorrectCodecId() 
    => Assert.That(this._stream.CodecId, Is.EqualTo("V_MPEG4/ISO/AVC"));

  #endregion

  #region Color Information Tests

  [Test]
  public void ColorSpace_ReturnsYuv() 
    => Assert.That(this._stream.ColorSpace, Is.EqualTo("YUV"));

  [Test]
  public void ChromaSubsampling_Returns420() 
    => Assert.That(this._stream.ChromaSubsampling, Is.EqualTo("4:2:0"));

  [Test]
  public void BitDepth_Returns8() 
    => Assert.That(this._stream.BitDepth, Is.EqualTo(8));

  [Test]
  public void ColorRange_ReturnsLimited() 
    => Assert.That(this._stream.ColorRange, Is.EqualTo("Limited"));

  [Test]
  public void HasColorDescription_ReturnsTrue() 
    => Assert.That(this._stream.HasColorDescription, Is.True);

  [Test]
  public void MatrixCoefficients_ReturnsBt709() 
    => Assert.That(this._stream.MatrixCoefficients, Is.EqualTo("BT.709"));

  #endregion

  #region Scan Type Tests

  [Test]
  public void ScanTypeValue_ReturnsProgressive() 
    => Assert.That(this._stream.ScanTypeValue, Is.EqualTo(ScanType.Progressive));

  #endregion

  #region Encoding Information Tests

  [Test]
  public void BitsPerPixelFrame_ReturnsCorrectValue() 
    => Assert.That(this._stream.BitsPerPixelFrame, Is.EqualTo(0.061).Within(0.001));

  [Test]
  public void WritingLibrary_ReturnsX264() 
    => Assert.That(this._stream.WritingLibrary, Does.Contain("x264"));

  [Test]
  public void EncodingLibraryName_ReturnsX264() 
    => Assert.That(this._stream.EncodingLibraryName, Is.EqualTo("x264"));

  [Test]
  public void EncodingLibraryVersion_ReturnsVersionInfo() 
    => Assert.That(this._stream.EncodingLibraryVersion, Does.Contain("core 164"));

  [Test]
  public void EncodingSettings_ContainsCrf() 
    => Assert.That(this._stream.EncodingSettings, Does.Contain("crf=23.0"));

  [Test]
  public void EncodingSettings_ContainsCabacEnabled() 
    => Assert.That(this._stream.EncodingSettings, Does.Contain("cabac=1"));

  #endregion

  #region Bitrate and Size Tests

  [Test]
  public void BitRateInBitsPerSecond_ReturnsCorrectValue() 
    => Assert.That(this._stream.BitRateInBitsPerSecond, Is.EqualTo(443182));

  [Test]
  public void SizeInBytes_ReturnsCorrectSize() 
    => Assert.That(this._stream.SizeInBytes, Is.EqualTo(37149752L));

  [Test]
  public void ProportionOfStream_ReturnsCorrectProportion() 
    => Assert.That(this._stream.ProportionOfStream, Is.EqualTo(0.63166).Within(0.00001));

  #endregion

  #region Duration Tests

  [Test]
  public void Duration_ReturnsCorrectDuration() {
    var expectedDuration = TimeSpan.FromMilliseconds(670600);
    Assert.That(this._stream.Duration, Is.EqualTo(expectedDuration));
  }

  #endregion

  #region Delay Tests

  [Test]
  public void DelayInMilliseconds_Returns80() 
    => Assert.That(this._stream.DelayInMilliseconds, Is.EqualTo(80));

  [Test]
  public void DelayOrigin_ReturnsContainer() 
    => Assert.That(this._stream.DelayOrigin, Is.EqualTo("Container"));

  #endregion

  #region Common Stream Property Tests

  [Test]
  public void IsDefault_ReturnsTrue() 
    => Assert.That(this._stream.IsDefault, Is.True);

  [Test]
  public void IsForced_ReturnsFalse() 
    => Assert.That(this._stream.IsForced, Is.False);

  [Test]
  public void StreamOrder_Returns0() 
    => Assert.That(this._stream.StreamOrder, Is.EqualTo(0));

  [Test]
  public void Id_Returns1() 
    => Assert.That(this._stream.Id, Is.EqualTo(1));

  [Test]
  public void UniqueId_ReturnsCorrectId() 
    => Assert.That(this._stream.UniqueId, Is.EqualTo("518158616342279724"));

  [Test]
  public void CommercialName_ReturnsAvc() 
    => Assert.That(this._stream.CommercialName, Is.EqualTo("AVC"));

  #endregion

  #region Stereoscopic Tests

  [Test]
  public void IsStereoscopic_ReturnsFalse() 
    => Assert.That(this._stream.IsStereoscopic, Is.False);

  [Test]
  public void StereoscopicMode_ReturnsNone() 
    => Assert.That(this._stream.StereoscopicMode, Is.EqualTo(StereoscopicMode.None));

  #endregion
}
