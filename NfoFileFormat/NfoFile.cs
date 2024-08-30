using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace Hawkynt.NfoFileFormat;
public static class NfoFile {

  private static readonly XmlSerializer _EPISODE_SERIALIZER = new(typeof(EpisodeDetails));
  private static readonly XmlSerializer _SHOW_SERIALIZER = new(typeof(TVShow));
  private static readonly XmlSerializer _SEASON_SERIALIZER = new(typeof(Season));
  private static readonly XmlSerializer _MOVIE_SERIALIZER = new(typeof(Movie));

  public static bool HasNfo(FileInfo mediaFile) => GetNfoFile(mediaFile).Exists;

  private static FileInfo GetNfoFile(FileInfo mediaFile) => mediaFile.WithNewExtension(".nfo");

  public static Movie LoadMovieOrNull(FileInfo mediaFile)
    => _LoadNfoOrNull<Movie>(mediaFile, _MOVIE_SERIALIZER)
    ;

  public static TVShow LoadShowOrNull(FileInfo mediaFile)
    => _LoadNfoOrNull<TVShow>(mediaFile, _SHOW_SERIALIZER)
  ;

  public static Season LoadSeasonOrNull(FileInfo mediaFile)
    => _LoadNfoOrNull<Season>(mediaFile, _SEASON_SERIALIZER)
  ;

  public static EpisodeDetails LoadEpisodeOrNull(FileInfo mediaFile)
    => _LoadNfoOrNull<EpisodeDetails>(mediaFile, _EPISODE_SERIALIZER)
    ;
  
  private static TResult _LoadNfoOrNull<TResult>(FileInfo mediaFile,XmlSerializer deserializer) {
    var file = GetNfoFile(mediaFile);
    if (file.NotExists())
      return default;

    try {
      using var stream = file.OpenRead();
      return (TResult)deserializer.Deserialize(stream);
    } catch {
      return default;
    }
  }

  public static void Update<T>(FileInfo nfoFile, T dataToUpdate) where T : class {
    if (!nfoFile.Exists)
      throw new FileNotFoundException("No NFO file found to update.");

    var doc = XDocument.Load(nfoFile.FullName);
    var rootAttribute = typeof(T).GetCustomAttribute<XmlRootAttribute>();
    var containerElementName = rootAttribute?.ElementName ?? typeof(T).Name.ToLower();

    var containerElement = doc.Descendants(containerElementName).FirstOrDefault();
    if (containerElement == null)
      throw new InvalidOperationException("The specified root element is not found in the XML document.");

    UpdateElement(containerElement, typeof(T), dataToUpdate);

    doc.Save(nfoFile.FullName);
  }

  private static void UpdateElement(XElement containerElement, Type dataType, object dataToUpdate) {
    foreach (var prop in dataType.GetProperties()) {
      var elementAttribute = prop.GetCustomAttribute<XmlElementAttribute>();
      if (elementAttribute == null)
        continue;

      var elementName = elementAttribute.ElementName;
      var element = containerElement.Element(elementName);
      var propValue = prop.GetValue(dataToUpdate);
      
      if (prop.PropertyType.IsClass && prop.PropertyType != typeof(string)) {
        if(propValue == null)
          continue;

        // It's a nested object, handle recursively
        if (element == null) {
          element = new XElement(elementName);
          containerElement.Add(element);
        }
        UpdateElement(element, prop.PropertyType, propValue);

      } else {
        
        // It's a simple property, update or create the element
        if (element == null) {
          element = new XElement(elementName);
          containerElement.Add(element);
        }
        element.Value = propValue?.ToString() ?? string.Empty;
      }
    }
  }

}
