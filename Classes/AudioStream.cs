namespace Classes {
  public class AudioStream : MediaStream {
    internal AudioStream(SectionDictionary values) : base(values) {
    }

    public string Format => this.GetStringOrDefault("format");
    public int Channels => this.GetSomeIntOrDefault("channel(s)");
    public int SamplingRate => this.GetSomeIntOrDefault("sampling rate");
    public double? ReplayGain => this.GetDoubleOrNull("replay gain");
    public double? ReplayPeak => this.GetDoubleOrNull("replay gain peak");
  }
}
