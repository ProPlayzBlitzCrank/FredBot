using System;

namespace FredtheG.CactusBot
{
    public class YoutubeVideo
    {
        public string id, title, description, channel;
        public DateTime publishedDate;

        public YoutubeVideo(string id)
        {
            this.id = id;
            YoutubeApi.GetVideoInfo(this);
        }
    }
}