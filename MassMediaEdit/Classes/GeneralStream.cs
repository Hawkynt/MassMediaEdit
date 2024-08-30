namespace Classes;

public class GeneralStream : MediaStream {
  internal GeneralStream(SectionDictionary values) : base(values) {
  }

  public string AlbumArtist => this.GetStringOrDefault("album/performer");
  public string Album => this.GetStringOrDefault("album");
  public string Artist => this.GetStringOrDefault("performer");
  public string Title => this.GetStringOrDefault("title");
  public int? Track => this.GetSomeIntOrNull("track name/position");
  public int? TrackCount => this.GetSomeIntOrNull("track name/total");
  public int? Disc => this.GetSomeIntOrNull("part/position");
  public int? DiscCount => this.GetSomeIntOrNull("part/total");
  public string RecordingDate => this.GetStringOrDefault("recorded date");
  public double? AlbumGain => this.GetDoubleOrNull("album replay gain");
  public double? AlbumPeak => this.GetDoubleOrNull("album replay gain peak");
  public bool IsInterleaved => this.GetBoolOrDefault("interleaved");
  public int OverallBitRateInBitsPerSecond => this.GetIntOrDefault("overall bit rate");
  public long FileSizeInBytes => this.GetLongOrDefault("file size");
}