﻿using System;

namespace FredtheG.CactusBot
{
    public class YouTubeChannel
    {
        public string id, name, desciption;
        public DateTime creationDate;

        public YouTubeChannel(string id)
        {
            this.id = id;
            YoutubeApi.GetChannelInfo(this);
        }
    }
}
