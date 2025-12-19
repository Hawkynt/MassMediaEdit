namespace MassMediaEdit.Tests.TestData;

/// <summary>
/// Provides sample MediaInfo output data for unit testing.
/// This data is based on a real MKV file analysis and allows testing without requiring actual media files.
/// </summary>
internal static class SampleMediaInfoData {
  /// <summary>
  /// Sample General stream data from MediaInfo -full output.
  /// Container: Matroska, Duration: 11:11, Size: 56.1 MiB
  /// </summary>
  public const string GeneralStreamData = """
    Count                                    : 349
    Count of stream of this kind             : 1
    Kind of stream                           : General
    Kind of stream                           : General
    Stream identifier                        : 0
    Unique ID                                : 161479925783552580734818772237588442284
    Unique ID                                : 161479925783552580734818772237588442284 (0x797BE706DBB0789E00108B4568A12CAC)
    Count of video streams                   : 1
    Count of audio streams                   : 1
    Video_Format_List                        : AVC
    Video_Format_WithHint_List               : AVC
    Codecs Video                             : AVC
    Audio_Format_List                        : AAC LC
    Audio_Format_WithHint_List               : AAC LC
    Audio codecs                             : AAC LC
    Audio_Language_List                      : German
    Audio_Channels_Total                     : 2
    Complete name                            : C:\Media\Videos\sample_video.mkv
    Folder name                              : C:\Media\Videos
    File name extension                      : sample_video.mkv
    File name                                : sample_video
    File extension                           : mkv
    Format                                   : Matroska
    Format                                   : Matroska
    Format/Url                               : https://matroska.org/downloads/windows.html
    Format/Extensions usually used           : mkv mk3d mka mks
    Commercial name                          : Matroska
    Format version                           : Version 4
    File size                                : 58812558
    File size                                : 56.1 MiB
    File size                                : 56 MiB
    File size                                : 56 MiB
    File size                                : 56.1 MiB
    File size                                : 56.09 MiB
    Duration                                 : 671189
    Duration                                 : 11 min 11 s
    Duration                                 : 11 min 11 s 189 ms
    Duration                                 : 11 min 11 s
    Duration                                 : 00:11:11.189
    Duration                                 : 00:11:10:15
    Duration                                 : 00:11:11.189 (00:11:10:15)
    Overall bit rate                         : 700995
    Overall bit rate                         : 701 kb/s
    Frame rate                               : 25.000
    Frame rate                               : 25.000 FPS
    Frame count                              : 16765
    Stream size                              : 192401
    Stream size                              : 188 KiB (0%)
    Stream size                              : 188 KiB
    Stream size                              : 188 KiB
    Stream size                              : 188 KiB
    Stream size                              : 187.9 KiB
    Stream size                              : 188 KiB (0%)
    Proportion of this stream                : 0.00327
    IsStreamable                             : Yes
    Encoded date                             : 2024-01-15 12:00:00 UTC
    File creation date                       : 2024-01-15 12:00:00.000 UTC
    File creation date (local)               : 2024-01-15 13:00:00.000
    File last modification date              : 2024-01-15 14:00:00.000 UTC
    File last modification date (local)      : 2024-01-15 15:00:00.000
    Writing application                      : mkvmerge v88.0 ('All I Know') 64-bit
    Writing application                      : mkvmerge v88.0 ('All I Know') 64-bit
    Writing library                          : libebml v1.4.5 + libmatroska v1.7.1 / Lavf61.7.100
    Writing library                          : libebml v1.4.5 + libmatroska v1.7.1 / Lavf61.7.100
    """;

  /// <summary>
  /// Sample Video stream data from MediaInfo -full output.
  /// Format: AVC High@L3, 720x404, 25fps, 443 kb/s
  /// </summary>
  public const string VideoStreamData = """
    Count                                    : 390
    Count of stream of this kind             : 1
    Kind of stream                           : Video
    Kind of stream                           : Video
    Stream identifier                        : 0
    StreamOrder                              : 0
    ID                                       : 1
    ID                                       : 1
    Unique ID                                : 518158616342279724
    Format                                   : AVC
    Format                                   : AVC
    Format/Info                              : Advanced Video Codec
    Format/Url                               : http://developers.videolan.org/x264.html
    Commercial name                          : AVC
    Format profile                           : High@L3
    Format settings                          : CABAC / 4 Ref Frames
    Format settings, CABAC                   : Yes
    Format settings, CABAC                   : Yes
    Format settings, Reference frames        : 4
    Format settings, Reference frames        : 4 frames
    Internet media type                      : video/H264
    Codec ID                                 : V_MPEG4/ISO/AVC
    Codec ID/Url                             : http://ffdshow-tryout.sourceforge.net/
    Duration                                 : 670600.000000
    Duration                                 : 11 min 10 s
    Duration                                 : 11 min 10 s 600 ms
    Duration                                 : 11 min 10 s
    Duration                                 : 00:11:10.600
    Duration                                 : 00:11:10:15
    Duration                                 : 00:11:10.600 (00:11:10:15)
    Bit rate                                 : 443182
    Bit rate                                 : 443 kb/s
    Width                                    : 720
    Width                                    : 720 pixels
    Height                                   : 404
    Height                                   : 404 pixels
    Stored_Height                            : 416
    Sampled_Width                            : 720
    Sampled_Height                           : 404
    Pixel aspect ratio                       : 1.000
    Display aspect ratio                     : 1.782
    Display aspect ratio                     : 16:9
    Frame rate mode                          : CFR
    Frame rate mode                          : Constant
    FrameRate_Mode_Original                  : VFR
    Frame rate                               : 25.000
    Frame rate                               : 25.000 FPS
    FrameRate_Num                            : 25
    FrameRate_Den                            : 1
    Frame count                              : 16765
    Color space                              : YUV
    Chroma subsampling                       : 4:2:0
    Chroma subsampling                       : 4:2:0
    Bit depth                                : 8
    Bit depth                                : 8 bits
    Scan type                                : Progressive
    Scan type                                : Progressive
    Bits/(Pixel*Frame)                       : 0.061
    Delay                                    : 80
    Delay                                    : 80 ms
    Delay                                    : 80 ms
    Delay                                    : 80 ms
    Delay                                    : 00:00:00.080
    Delay                                    : 00:00:00:02
    Delay                                    : 00:00:00.080 (00:00:00:02)
    Delay, origin                            : Container
    Delay, origin                            : Container
    Stream size                              : 37149752
    Stream size                              : 35.4 MiB (63%)
    Stream size                              : 35 MiB
    Stream size                              : 35 MiB
    Stream size                              : 35.4 MiB
    Stream size                              : 35.43 MiB
    Stream size                              : 35.4 MiB (63%)
    Proportion of this stream                : 0.63166
    Writing library                          : x264 - core 164 r3191 4613ac3
    Writing library                          : x264 core 164 r3191 4613ac3
    Encoded_Library_Name                     : x264
    Encoded_Library_Version                  : core 164 r3191 4613ac3
    Encoding settings                        : cabac=1 / ref=2 / deblock=1:0:0 / analyse=0x3:0x113 / me=hex / subme=6 / psy=1 / psy_rd=1.00:0.00 / mixed_ref=1 / me_range=16 / chroma_me=1 / trellis=1 / 8x8dct=1 / cqm=0 / deadzone=21,11 / fast_pskip=1 / chroma_qp_offset=-2 / threads=20 / lookahead_threads=3 / sliced_threads=0 / nr=0 / decimate=1 / interlaced=0 / bluray_compat=0 / constrained_intra=0 / bframes=3 / b_pyramid=2 / b_adapt=1 / b_bias=0 / direct=1 / weightb=1 / open_gop=0 / weightp=1 / keyint=400 / keyint_min=200 / scenecut=40 / intra_refresh=0 / rc_lookahead=200 / rc=crf / mbtree=1 / crf=23.0 / qcomp=0.60 / qpmin=0 / qpmax=69 / qpstep=4 / vbv_maxrate=820 / vbv_bufsize=8280 / crf_max=0.0 / nal_hrd=none / filler=0 / ip_ratio=1.40 / aq=1:1.00
    Default                                  : Yes
    Default                                  : Yes
    Forced                                   : No
    Forced                                   : No
    colour_description_present               : Yes
    colour_description_present_Source        : Stream
    Color range                              : Limited
    colour_range_Source                      : Stream
    colour_primaries_Source                  : Stream
    transfer_characteristics_Source          : Stream
    Matrix coefficients                      : BT.709
    matrix_coefficients_Source               : Stream
    """;

  /// <summary>
  /// Sample Audio stream data from MediaInfo -full output.
  /// Format: AAC LC, 2 channels, 48kHz, 256 kb/s, German language
  /// </summary>
  public const string AudioStreamData = """
    Count                                    : 285
    Count of stream of this kind             : 1
    Kind of stream                           : Audio
    Kind of stream                           : Audio
    Stream identifier                        : 0
    StreamOrder                              : 1
    ID                                       : 2
    ID                                       : 2
    Unique ID                                : 10515736078130736169
    Format                                   : AAC
    Format                                   : AAC LC
    Format/Info                              : Advanced Audio Codec Low Complexity
    Commercial name                          : AAC
    Format_AdditionalFeatures                : LC
    Codec ID                                 : A_AAC-2
    Duration                                 : 671189.000000
    Duration                                 : 11 min 11 s
    Duration                                 : 11 min 11 s 189 ms
    Duration                                 : 11 min 11 s
    Duration                                 : 00:11:11.189
    Duration                                 : 00:11:11.189
    Bit rate                                 : 255908
    Bit rate                                 : 256 kb/s
    Channel(s)                               : 2
    Channel(s)                               : 2 channels
    Channel positions                        : Front: L R
    Channel positions                        : 2/0/0
    Channel layout                           : L R
    Samples per frame                        : 1024
    Sampling rate                            : 48000
    Sampling rate                            : 48.0 kHz
    Samples count                            : 32217072
    Frame rate                               : 46.875
    Frame rate                               : 46.875 FPS (1024 SPF)
    Frame count                              : 31462
    Compression mode                         : Lossy
    Compression mode                         : Lossy
    Delay                                    : 0
    Delay                                    : 00:00:00.000
    Delay                                    : 00:00:00.000
    Delay, origin                            : Container
    Delay, origin                            : Container
    Delay relative to video                  : -80
    Delay relative to video                  : -80 ms
    Delay relative to video                  : -80 ms
    Delay relative to video                  : -80 ms
    Delay relative to video                  : -00:00:00.080
    Delay relative to video                  : -00:00:00.080
    Stream size                              : 21470405
    Stream size                              : 20.5 MiB (37%)
    Stream size                              : 20 MiB
    Stream size                              : 20 MiB
    Stream size                              : 20.5 MiB
    Stream size                              : 20.48 MiB
    Stream size                              : 20.5 MiB (37%)
    Proportion of this stream                : 0.36506
    Language                                 : de
    Language                                 : German
    Language                                 : German
    Language                                 : de
    Language                                 : deu
    Language                                 : de
    Default                                  : Yes
    Default                                  : Yes
    Forced                                   : No
    Forced                                   : No
    """;

  /// <summary>
  /// Converts a multi-line string to an array of lines, trimming each line and removing carriage returns.
  /// </summary>
  public static string[] ToLines(string data) 
    => data.Replace("\r", "").Split('\n', System.StringSplitOptions.RemoveEmptyEntries);
}
