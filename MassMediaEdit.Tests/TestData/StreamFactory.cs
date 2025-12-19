using System;
using System.Collections.Generic;
using System.Linq;
using Classes;

namespace MassMediaEdit.Tests.TestData;

/// <summary>
/// Helper class to create stream objects from sample MediaInfo data for testing.
/// </summary>
internal static class StreamFactory {
  /// <summary>
  /// Creates a GeneralStream from the sample data.
  /// </summary>
  public static GeneralStream CreateGeneralStream() 
    => CreateGeneralStream(SampleMediaInfoData.GeneralStreamData);

  /// <summary>
  /// Creates a GeneralStream from the provided MediaInfo output lines.
  /// </summary>
  public static GeneralStream CreateGeneralStream(string data) {
    var dict = ParseToSectionDictionary(data);
    return new GeneralStream(dict);
  }

  /// <summary>
  /// Creates a VideoStream from the sample data.
  /// </summary>
  public static VideoStream CreateVideoStream() 
    => CreateVideoStream(SampleMediaInfoData.VideoStreamData);

  /// <summary>
  /// Creates a VideoStream from the provided MediaInfo output lines.
  /// </summary>
  public static VideoStream CreateVideoStream(string data) {
    var dict = ParseToSectionDictionary(data);
    return new VideoStream(dict);
  }

  /// <summary>
  /// Creates an AudioStream from the sample data.
  /// </summary>
  public static AudioStream CreateAudioStream() 
    => CreateAudioStream(SampleMediaInfoData.AudioStreamData);

  /// <summary>
  /// Creates an AudioStream from the provided MediaInfo output lines.
  /// </summary>
  public static AudioStream CreateAudioStream(string data) {
    var dict = ParseToSectionDictionary(data);
    return new AudioStream(dict);
  }

  /// <summary>
  /// Parses MediaInfo output text into a SectionDictionary.
  /// </summary>
  private static SectionDictionary ParseToSectionDictionary(string data) {
    var lines = SampleMediaInfoData.ToLines(data);
    return SectionDictionary.FromLines(lines);
  }
}
