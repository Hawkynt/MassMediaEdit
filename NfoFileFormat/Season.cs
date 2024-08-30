using System.Xml.Serialization;

namespace Hawkynt.NfoFileFormat;
[XmlRoot("season")]
public class Season {

  [XmlElement("title")]
  public string Title { get; set; }

  [XmlElement("year")]
  public int Year { get; set; }

  [XmlElement("seasonnumber")]
  public int Seasonnumber { get; set; }

  [XmlElement("art")]
  public Art Art { get; set; }
  
}