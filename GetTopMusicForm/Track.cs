using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GetTopMusicForm
{
    class Track
    {
        public string singerName;
        public string trackName;
        public string fileLink;
        public string nameFile;

        public Track()
        {
            singerName = "";
            trackName = "";
            fileLink = "";
            nameFile = "";
        }

        public Track(string SingerName, string TrackName, string FileLink)
        {
            singerName = FixString(SingerName);
            trackName = FixString(TrackName);
            fileLink = FileLink;
            nameFile = singerName + "_" + trackName + ".mp3";
        }

        public override string ToString()
        {
            return string.Format(singerName + "\n" + trackName + "\n" + fileLink);
        }
        private string FixString(string str)
        {

            str = str.Replace('\n', ' ');
            str = str.Replace('?', ' ');

            return str.Trim();
        }
    }
}
