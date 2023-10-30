namespace MusicHub
{
    using System;
    using System.Globalization;
    using System.Linq;
    using System.Text;
    using Data;
    using Initializer;

    public class StartUp
    {
        public static void Main(string[] args)
        {
            MusicHubDbContext context = 
                new MusicHubDbContext();

            DbInitializer.ResetDatabase(context);

            Console.WriteLine(ExportSongsAboveDuration(context,4));
        }

        public static string ExportAlbumsInfo(MusicHubDbContext context, int producerId)
        {
            StringBuilder result = new StringBuilder();
            var albums = context.Albums
                .Where(x => x.ProducerId == producerId)
                .Select(x=> new
                { 
                    x.Name,
                    x.ReleaseDate,
                    x.Producer,
                    x.Songs,
                    x.Price
                }
                );
            foreach (var album in albums.OrderByDescending(x => x.Songs.Sum(x => x.Price)))
            {
                result.AppendLine($"-AlbumName: {album.Name}");
                result.AppendLine($"-ReleaseDate: {album.ReleaseDate.ToString("MM/dd/yyyy",CultureInfo.InvariantCulture)}");
                result.AppendLine($"-ProducerName: {album.Producer.Name}");
                result.AppendLine($"-Songs:");
                int i = 1;
                foreach (var song in album.Songs.OrderByDescending(x => x.Name).ThenBy(x => x.Writer.Name))
                {
                    result.AppendLine($"---#{i}");
                    result.AppendLine($"---SongName: {song.Name}");
                    result.AppendLine($"---Price: {song.Price:f2}");
                    result.AppendLine($"---Writer: {song.Writer.Name}");
                    i++;
                }
                result.AppendLine($"-AlbumPrice: {album.Price:f2}");
            }
            return result.ToString().TrimEnd();
        }

        public static string ExportSongsAboveDuration(MusicHubDbContext context, int duration)
        {
            StringBuilder result = new StringBuilder();
            var songs = context.Songs.AsEnumerable()
                .Where(x => x.Duration.TotalSeconds > duration)
                .Select(x => new
                {
                    x.Name,
                    x.SongPerformers,
                    x.Writer,
                    x.Album,
                    x.Duration
                })
                .OrderBy(x => x.Name)
                .ThenBy(x => x.Writer.Name)
                .ToList();
            int i = 1;
            foreach (var song in songs)
            {
                result.AppendLine($"-Song #{i}");
                result.AppendLine($"---SongName: {song.Name}");
                result.AppendLine($"---Writer: {song.Writer.Name}");
                if (song.SongPerformers != null)
                {
                    foreach (var perormer in song.SongPerformers.OrderBy(x => x.Performer.FirstName))
                    {
                        result.AppendLine($"---Performer: {perormer.Performer.FirstName} {perormer.Performer.LastName}");
                    }
                }
                result.AppendLine($"---AlbumProducer: {song.Album.Producer.Name}");
                result.AppendLine($"---Duration: {song.Duration.ToString("c")}");
                i++;
            }

            return result.ToString().TrimEnd();
        }
    }
}
