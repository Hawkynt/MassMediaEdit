using System.Xml.Serialization;

namespace Hawkynt.NfoFileFormat;

[XmlRoot("episodedetails")]
public class EpisodeDetails {
  
  [XmlElement("title")]
  public string Title { get; set; }
  
  [XmlElement("originaltitle")]
  public string OriginalTitle { get; set; }

  [XmlElement("year")]
  public int Year { get; set; }

  [XmlElement("art")]
  public Art Art { get; set; }

}