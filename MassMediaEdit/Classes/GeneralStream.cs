using System;

namespace Classes;

public class GeneralStream : MediaStream {
  internal GeneralStream(SectionDictionary values) : base(values) { }

  #region File Information

  /// <summary>Gets the complete file name with full path.</summary>
  public string CompleteName => this.GetStringOrDefault("complete name");
  
  /// <summary>Gets the folder name containing the file.</summary>
  public string FolderName => this.GetStringOrDefault("folder name");
  
  /// <summary>Gets the file name with extension.</summary>
  public string FileNameWithExtension => this.GetStringOrDefault("file name extension");
  
  /// <summary>Gets the file name without extension.</summary>
  public string FileName => this.GetStringOrDefault("file name");
  
  /// <summary>Gets the file extension.</summary>
  public string FileExtension => this.GetStringOrDefault("file extension");

  #endregion

  #region Format Information

  /// <summary>Gets the container format (e.g., Matroska, MP4).</summary>
  public string ContainerFormat => this.GetStringOrDefault("format");
  
  /// <summary>Gets the format URL for more information.</summary>
  public string FormatUrl => this.GetStringOrDefault("format/url");
  
  /// <summary>Gets the usually used file extensions for this format.</summary>
  public string FormatExtensions => this.GetStringOrDefault("format/extensions usually used");
  
  /// <summary>Gets the format version.</summary>
  public string FormatVersion => this.GetStringOrDefault("format version");

  #endregion

  #region Stream Counts

  /// <summary>Gets the count of video streams.</summary>
  public int VideoStreamCount => this.GetIntOrDefault("count of video streams");
  
  /// <summary>Gets the count of audio streams.</summary>
  public int AudioStreamCount => this.GetIntOrDefault("count of audio streams");
  
  /// <summary>Gets the total count of audio channels across all streams.</summary>
  public int TotalAudioChannels => this.GetIntOrDefault("audio_channels_total");

  #endregion

  #region Stream Format Lists

  /// <summary>Gets the list of video formats in the file.</summary>
  public string VideoFormatList => this.GetStringOrDefault("video_format_list");
  
  /// <summary>Gets the list of audio formats in the file.</summary>
  public string AudioFormatList => this.GetStringOrDefault("audio_format_list");
  
  /// <summary>Gets the list of audio languages in the file.</summary>
  public string AudioLanguageList => this.GetStringOrDefault("audio_language_list");

  #endregion

  #region File Size and Bitrate

  /// <summary>Gets the file size in bytes.</summary>
  public long FileSizeInBytes => this.GetLongOrDefault("file size");
  
  /// <summary>Gets the overall bit rate in bits per second.</summary>
  public int OverallBitRateInBitsPerSecond => this.GetSomeIntOrDefault("overall bit rate");

  #endregion

  #region Timing Information

  /// <summary>Gets the overall frame rate.</summary>
  public double FrameRate => this.GetDoubleOrDefault("frame rate");

  #endregion

  #region Streamability

  /// <summary>Gets whether the file is streamable (can be played while downloading).</summary>
  public bool IsStreamable => this.GetBoolOrDefault("isstreamable");
  
  /// <summary>Gets whether the file is interleaved.</summary>
  public bool IsInterleaved => this.GetBoolOrDefault("interleaved");

  #endregion

  #region Dates

  /// <summary>Gets the encoded date as string.</summary>
  public string EncodedDate => this.GetStringOrDefault("encoded date");
  
  /// <summary>Gets the file creation date as string.</summary>
  public string FileCreationDate => this.GetStringOrDefault("file creation date");
  
  /// <summary>Gets the file last modification date as string.</summary>
  public string FileModificationDate => this.GetStringOrDefault("file last modification date");

  #endregion

  #region Writing/Encoding Information

  /// <summary>Gets the writing application used to create the file.</summary>
  public string WritingApplication => this.GetStringOrDefault("writing application");
  
  /// <summary>Gets the writing library used to create the file.</summary>
  public string WritingLibrary => this.GetStringOrDefault("writing library");

  #endregion

  #region Album/Music Metadata

  /// <summary>Gets the album artist.</summary>
  public string AlbumArtist => this.GetStringOrDefault("album/performer");
  
  /// <summary>Gets the album name.</summary>
  public string Album => this.GetStringOrDefault("album");
  
  /// <summary>Gets the artist/performer name.</summary>
  public string Artist => this.GetStringOrDefault("performer");
  
  /// <summary>Gets the title.</summary>
  public string Title => this.GetStringOrDefault("title");
  
  /// <summary>Gets the track position.</summary>
  public int? Track => this.GetSomeIntOrNull("track name/position");
  
  /// <summary>Gets the total track count.</summary>
  public int? TrackCount => this.GetSomeIntOrNull("track name/total");
  
  /// <summary>Gets the disc/part position.</summary>
  public int? Disc => this.GetSomeIntOrNull("part/position");
  
  /// <summary>Gets the total disc/part count.</summary>
  public int? DiscCount => this.GetSomeIntOrNull("part/total");
  
  /// <summary>Gets the recording date.</summary>
  public string RecordingDate => this.GetStringOrDefault("recorded date");
  
  /// <summary>Gets the album replay gain value.</summary>
  public double? AlbumGain => this.GetDoubleOrNull("album replay gain");
  
  /// <summary>Gets the album replay gain peak value.</summary>
  public double? AlbumPeak => this.GetDoubleOrNull("album replay gain peak");

  #endregion
}