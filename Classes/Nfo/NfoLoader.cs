using System.IO;
using System.Xml.Serialization;

namespace MassMediaEdit.Classes.Nfo;
internal class NfoLoader {

  private static readonly XmlSerializer _EPISODE_SERIALIZER = new(typeof(EpisodeDetails));
  private static readonly XmlSerializer _MOVIE_SERIALIZER = new(typeof(Movie));

  public static bool HasNfo(FileInfo mediaFile) => GetNfoFile(mediaFile).Exists;

  private static FileInfo GetNfoFile(FileInfo mediaFile) => mediaFile.WithNewExtension(".nfo");

  public static Movie LoadMovieOrNull(FileInfo mediaFile) {
    var file = GetNfoFile(mediaFile);
    if(file.NotExists())
      return null;

    try {
      using var stream = file.OpenRead();
      return (Movie)_MOVIE_SERIALIZER.Deserialize(stream);
    } catch {
      return null;
    }
  }

  public static EpisodeDetails LoadEpisodeOrNull(FileInfo mediaFile) {
    var file = GetNfoFile(mediaFile);
    if (file.NotExists())
      return null;

    try {
      using var stream = file.OpenRead();
      return (EpisodeDetails)_EPISODE_SERIALIZER.Deserialize(stream);
    } catch {
      return null;
    }
  }

}
