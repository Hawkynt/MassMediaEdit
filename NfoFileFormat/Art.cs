
using System.Xml.Serialization;

namespace Hawkynt.NfoFileFormat;

public class Art {

  [XmlElement("poster")]
  public string Poster { get; set; }

  [XmlElement("fanart")]
  public string Fanart { get; set; }

}
