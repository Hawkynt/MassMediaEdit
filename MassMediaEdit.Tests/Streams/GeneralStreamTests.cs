using System;
using Classes;
using MassMediaEdit.Tests.TestData;
using NUnit.Framework;

namespace MassMediaEdit.Tests.Streams;

/// <summary>
/// Unit tests for the <see cref="GeneralStream"/> class.
/// Tests use sample MediaInfo data from a typical MKV file.
/// </summary>
[TestFixture]
public sealed class GeneralStreamTests {
  private GeneralStream _stream = null!;

  [SetUp]
  public void SetUp() => this._stream = StreamFactory.CreateGeneralStream();

  #region File Information Tests

  [Test]
  public void CompleteName_ReturnsFullPath() 
    => Assert.That(this._stream.CompleteName, Is.EqualTo(@"C:\Media\Videos\sample_video.mkv"));

  [Test]
  public void FolderName_ReturnsFolderPath() 
    => Assert.That(this._stream.FolderName, Is.EqualTo(@"C:\Media\Videos"));

  [Test]
  public void FileNameWithExtension_ReturnsFileNameWithExtension() 
    => Assert.That(this._stream.FileNameWithExtension, Is.EqualTo("sample_video.mkv"));

  [Test]
  public void FileName_ReturnsFileNameWithoutExtension() 
    => Assert.That(this._stream.FileName, Is.EqualTo("sample_video"));

  [Test]
  public void FileExtension_ReturnsExtension() 
    => Assert.That(this._stream.FileExtension, Is.EqualTo("mkv"));

  #endregion

  #region Format Information Tests

  [Test]
  public void ContainerFormat_ReturnsMatroska() 
    => Assert.That(this._stream.ContainerFormat, Is.EqualTo("Matroska"));

  [Test]
  public void FormatUrl_ReturnsUrl() 
    => Assert.That(this._stream.FormatUrl, Is.EqualTo("https://matroska.org/downloads/windows.html"));

  [Test]
  public void FormatExtensions_ReturnsExpectedExtensions() 
    => Assert.That(this._stream.FormatExtensions, Is.EqualTo("mkv mk3d mka mks"));

  [Test]
  public void FormatVersion_ReturnsVersion4() 
    => Assert.That(this._stream.FormatVersion, Is.EqualTo("Version 4"));

  #endregion

  #region Stream Count Tests

  [Test]
  public void VideoStreamCount_Returns1() 
    => Assert.That(this._stream.VideoStreamCount, Is.EqualTo(1));

  [Test]
  public void AudioStreamCount_Returns1() 
    => Assert.That(this._stream.AudioStreamCount, Is.EqualTo(1));

  [Test]
  public void TotalAudioChannels_Returns2() 
    => Assert.That(this._stream.TotalAudioChannels, Is.EqualTo(2));

  #endregion

  #region Stream Format List Tests

  [Test]
  public void VideoFormatList_ReturnsAvc() 
    => Assert.That(this._stream.VideoFormatList, Is.EqualTo("AVC"));

  [Test]
  public void AudioFormatList_ReturnsAacLc() 
    => Assert.That(this._stream.AudioFormatList, Is.EqualTo("AAC LC"));

  [Test]
  public void AudioLanguageList_ReturnsGerman() 
    => Assert.That(this._stream.AudioLanguageList, Is.EqualTo("German"));

  #endregion

  #region File Size and Bitrate Tests

  [Test]
  public void FileSizeInBytes_ReturnsCorrectSize() 
    => Assert.That(this._stream.FileSizeInBytes, Is.EqualTo(58812558L));

  [Test]
  public void OverallBitRateInBitsPerSecond_ReturnsCorrectBitrate() 
    => Assert.That(this._stream.OverallBitRateInBitsPerSecond, Is.EqualTo(700995));

  #endregion

  #region Duration Tests

  [Test]
  public void Duration_ReturnsCorrectDuration() {
    var expectedDuration = TimeSpan.FromMilliseconds(671189);
    Assert.That(this._stream.Duration, Is.EqualTo(expectedDuration));
  }

  [Test]
  public void Duration_InMinutesAndSeconds_IsCorrect() {
    Assert.That(this._stream.Duration.Minutes, Is.EqualTo(11));
    Assert.That(this._stream.Duration.Seconds, Is.EqualTo(11));
  }

  #endregion

  #region Timing Tests

  [Test]
  public void FrameRate_Returns25() 
    => Assert.That(this._stream.FrameRate, Is.EqualTo(25.0));

  [Test]
  public void FrameCount_Returns16765() 
    => Assert.That(this._stream.FrameCount, Is.EqualTo(16765));

  #endregion

  #region Streamability Tests

  [Test]
  public void IsStreamable_ReturnsTrue() 
    => Assert.That(this._stream.IsStreamable, Is.True);

  #endregion

  #region Date Tests

  [Test]
  public void EncodedDate_ReturnsCorrectDate() 
    => Assert.That(this._stream.EncodedDate, Is.EqualTo("2024-01-15 12:00:00 UTC"));

  [Test]
  public void WritingApplication_ReturnsMkvmergeInfo() 
    => Assert.That(this._stream.WritingApplication, Does.Contain("mkvmerge"));

  [Test]
  public void WritingLibrary_ReturnsLibebmlInfo() 
    => Assert.That(this._stream.WritingLibrary, Does.Contain("libebml"));

  #endregion

  #region Common Stream Properties Tests

  [Test]
  public void UniqueId_ReturnsCorrectId() 
    => Assert.That(this._stream.UniqueId, Is.EqualTo("161479925783552580734818772237588442284"));

  [Test]
  public void StreamIdentifier_Returns0() 
    => Assert.That(this._stream.StreamIdentifier, Is.EqualTo(0));

  [Test]
  public void SizeInBytes_ReturnsOverheadSize() 
    => Assert.That(this._stream.SizeInBytes, Is.EqualTo(192401L));

  [Test]
  public void ProportionOfStream_ReturnsSmallValue() 
    => Assert.That(this._stream.ProportionOfStream, Is.EqualTo(0.00327).Within(0.00001));

  #endregion
}
