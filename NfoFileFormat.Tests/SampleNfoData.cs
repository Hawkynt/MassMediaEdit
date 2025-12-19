namespace Hawkynt.NfoFileFormat.Tests;

internal static class SampleNfoData {

  public const string MovieNfo = """
    <?xml version="1.0" encoding="utf-8"?>
    <movie>
      <title>Test Movie</title>
      <originaltitle>Original Test Movie</originaltitle>
      <year>2024</year>
      <mpaa>PG-13</mpaa>
      <certification>US:PG-13</certification>
      <art>
        <poster>movie_poster.jpg</poster>
        <fanart>movie_fanart.jpg</fanart>
      </art>
    </movie>
    """;

  public const string TVShowNfo = """
    <?xml version="1.0" encoding="utf-8"?>
    <tvshow>
      <title>Test Show</title>
      <originaltitle>Original Test Show</originaltitle>
      <year>2024</year>
      <mpaa>TV-14</mpaa>
      <certification>US:TV-14</certification>
      <art>
        <poster>show_poster.jpg</poster>
        <fanart>show_fanart.jpg</fanart>
      </art>
    </tvshow>
    """;

  public const string SeasonNfo = """
    <?xml version="1.0" encoding="utf-8"?>
    <season>
      <title>Season 1</title>
      <year>2024</year>
      <seasonnumber>1</seasonnumber>
      <art>
        <poster>season_poster.jpg</poster>
        <fanart>season_fanart.jpg</fanart>
      </art>
    </season>
    """;

  public const string EpisodeNfo = """
    <?xml version="1.0" encoding="utf-8"?>
    <episodedetails>
      <title>Test Episode</title>
      <originaltitle>Original Test Episode</originaltitle>
      <year>2024</year>
      <art>
        <poster>episode_poster.jpg</poster>
        <fanart>episode_fanart.jpg</fanart>
      </art>
    </episodedetails>
    """;

}
