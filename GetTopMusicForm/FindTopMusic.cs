
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Threading.Tasks;
    using HtmlAgilityPack;
    using NLog;

namespace GetTopMusicForm
{
    class FindTopMusic
    {
        public static List<Track> listTracks;
        public static Logger logger = LogManager.GetCurrentClassLogger();
        public static bool morgenBlock = false;
        public static string WebPuth
        {
            set
            {
                webPuth = value;
            }
        }
        /// <summary>
        /// Если понадопится скачать музыку с другого сайта,
        /// то нужно просто заменить значения ниже на те, что написаны в коде сайта
        /// 
        /// Да, придется полазить, но на разных сайтах написано по разному
        /// </summary>  
        private static string trackInfoPuth = "Tracks\\TracksInfo.txt";
        private static string webPuth;
        private static string tagNameSinger;
        private static string tagNameTrack;
        private static string tagHref;

        private static void ChangeSite()
        {
            switch(webPuth)
            {
                case @"https://ruv.hotmo.org/songs/top-today":
                    tagNameSinger = "track__desc";
                    tagNameTrack = "track__title";
                    tagHref = "track__download-btn";
                    break;
                case @"https://muzofond.fm/collections/top/топ%20100%20лучших%20русских%20песен":
                    tagNameSinger = "track__desc";
                    tagNameTrack = "track__title";
                    tagHref = "track__download-btn";
                    break;
                case @"https://ru.sefon.pro/top/":
                    tagNameSinger = "artist_name";
                    tagNameTrack = "song_name";
                    tagHref = "download";
                    break;
                case @"https://new.love-music.me/top-100":
                    tagNameSinger = "track__desc";
                    tagNameTrack = "track__title";
                    tagHref = "track__download-btn";
                    break;
                default:
                    break;
            }
        }
        public static void ParsingToList()
        {
            try
            {
                ChangeSite();
                if (webPuth == $"https://ruv.hotmo.org/songs/top-today")
                {
                   
                    HtmlDocument html = new HtmlWeb().Load(webPuth);
                    listTracks = GetListTracks(html);

                    for (int i = 48; i <= 480; i += 48)
                    {
                        html = new HtmlWeb().Load(webPuth + $"/start/" + i);
                        listTracks.AddRange(GetListTracks(html));
                    }
                }
            }
            catch (WebException e)
            {
                logger.Error("Ошибка подключения к сети: " + e.Message);
            }
        }
        private static List<Track> GetListTracks(HtmlDocument html)
        {
            List<Track> listTracks = new List<Track>();
            var namesTracks = html.DocumentNode.SelectNodes("//div[@class='" + tagNameTrack + "']");
            var namesSingers = html.DocumentNode.SelectNodes("//div[@class='" + tagNameSinger + "']");
            var hrefsToTrack = html.DocumentNode.SelectNodes("//a[@class='" + tagHref + "']");

            for (int i = 0; i < namesSingers.Count; i++)
            {
                if (MorgenBlock(namesSingers[i].InnerText))
                {
                    listTracks.Add(new Track(namesSingers[i].InnerText, namesTracks[i].InnerText, hrefsToTrack[i].GetAttributeValue("href", "")));
                }
            }
            return listTracks;
        }
        public static bool MorgenBlock(string name)
        {
            if (morgenBlock)
            {
                if (name.ToUpper().Contains("MORGENSHTERN") || name.ToLower().Contains("моргенштерн"))
                {
                    return false;
                }
            }
            return true;
        }
        public static void SaveToFile()
        {
            try
            {
                using (StreamWriter sw = new StreamWriter(trackInfoPuth, false))
                {
                    foreach (Track track in listTracks)
                    {
                        sw.Write(track.ToString() + "\n\n");
                    }
                    logger.Debug("Информация о песнях записана в файл: " + trackInfoPuth);
                }
            }
            catch (Exception e)
            {
                logger.Error("Не удается записать в файл: " + e.Message);
            }
        }
        public static async Task DownloadTracks(string filePuth)
        {
            try
            {
                WebClient wc = new WebClient();
                Uri uri;
                foreach (Track track in listTracks)
                {
                    if (!FileExist(filePuth, track.nameFile))
                    {
                        uri = new Uri(track.fileLink);
                        await wc.DownloadFileTaskAsync(uri, filePuth + track.nameFile);
                    }
                }
                wc.Dispose();
            }
            catch (WebException e)
            {
                logger.Error("Возможна ошибка подключения к сети: " + e.Message);
            }
        }
        private static bool FileExist(string puth, string nameFile)
        {
            try
            {
                DirectoryInfo dir = new DirectoryInfo(puth);
                foreach (var item in dir.GetFiles())
                {
                    if (item.Name.Equals(nameFile))
                    {
                        return true;
                    }
                }
            }
            catch (DirectoryNotFoundException e)
            {
                logger.Error("Данной дериктории не существует: " + e.Message);
            }
            return false;
        }

    }
}
