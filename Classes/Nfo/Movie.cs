using System.Xml.Serialization;

namespace MassMediaEdit.Classes.Nfo;
[XmlRoot("movie")]
public class Movie {

  [XmlElement("title")]
  public string Title { get; set; }

  [XmlElement("originaltitle")]
  public string OriginalTitle { get; set; }

  [XmlElement("year")]
  public int Year { get; set; }

}