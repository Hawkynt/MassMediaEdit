using System.Xml.Serialization;

namespace Hawkynt.NfoFileFormat;
[XmlRoot("tvshow")]
public class TVShow {

  [XmlElement("title")]
  public string Title { get; set; }

  [XmlElement("originaltitle")]
  public string OriginalTitle { get; set; }

  [XmlElement("year")]
  public int Year { get; set; }

  [XmlElement("mpaa")]
  public string MPAA { get; set; }
  
  [XmlElement("certification")]
  public string Certification { get; set; }

  [XmlElement("art")]
  public Art Art { get; set; }

}