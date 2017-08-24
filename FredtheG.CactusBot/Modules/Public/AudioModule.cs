using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using Discord;
using Discord.Commands;
using Discord.Audio;
using Discord.WebSocket;
using System.Runtime.InteropServices;
using System.Diagnostics;

namespace FredtheG.CactusBot
{
    public class AudioModule : ModuleBase
    {
        private Process CreateStream(string url)
        {
            Process currentsong = new Process();

            currentsong.StartInfo = new ProcessStartInfo
            {
                FileName = "cmd.exe",
                Arguments = $"/C youtube-dl.exe -o - {url} | ffmpeg -i pipe:0 -ac 2 -f s16le -ar 48000 pipe:1",
                UseShellExecute = false,
                RedirectStandardOutput = true,
                CreateNoWindow = true
            };

            currentsong.Start();
            return currentsong;
        }
        static Random rnd = new Random();
        static IAudioClient client;
        static List<string> songList = new List<string>();
        static List<string> queue = new List<string>();
        static List<string> autoplay = new List<string>()
        {
            "https://www.youtube.com/watch?v=opXUzxWMRG8",
            "https://www.youtube.com/watch?v=3f7vylUl3Qo",
            "https://www.youtube.com/watch?v=jqwXY8h9c5I",
            "https://www.youtube.com/watch?v=ywPQqRWjhOs",
            "https://www.youtube.com/watch?v=k7mkdb5y3As",
            "https://www.youtube.com/watch?v=Ess0tB2obZo",
            "https://www.youtube.com/watch?v=YfRLJQlpMNw",
            "https://www.youtube.com/watch?v=gsvKF2ojUzs",
            "https://www.youtube.com/watch?v=NrchHYnXB3w"
        };


        static bool playing = false;

        [Command("play", RunMode = RunMode.Async)]
        [Alias("Play","PLAY")]
        public async Task Play([Remainder] string url)
        {
            try
            {
                if (Context.Channel.Id != 257682684405481472)
                {
                    return;
                }
                if (!Uri.IsWellFormedUriString(url, UriKind.Absolute))
                {
                    await ReplyAsync($"{Context.User.Mention} use the video URL not name.");
                    return;
                }
                if (client == null)
                {
                    IVoiceChannel channel = (Context.User as IVoiceState).VoiceChannel;
                    client = await channel.ConnectAsync();
                }
                IVoiceChannel VC = (Context.User as IVoiceState).VoiceChannel;
                if (VC.Id != 259900874204119054)
                {
                    await ReplyAsync($"{Context.User.Mention} I can only play music in the Music voice channel.");
                    return;
                }
                if (songList.Contains(url))
                    await ReplyAsync($"{Context.User.Mention} that song is already in the queue!");
                else
                {
                    string id = url.Substring(32);
                    string videoId = id;
                    YoutubeVideo video = new YoutubeVideo(videoId);
                    string title = video.title;
                    string channel = video.channel;
                    string thumbnail = "http://img.youtube.com/vi/" + id + "/0.jpg";
                    songList.Add(url);
                    queue.Add($"__**1.**__ **{title}** - Requested by **{Context.User.Username}**");
                    EmbedBuilder embed = new EmbedBuilder()
                    {
                        Color = new Color(235, 66, 244)
                    };
                    embed.Title = "-=-=-=-=-= Queued =-=-=-=-=-";
                    embed.AddField(y =>
                    {
                        y.Name = "Song URL";
                        y.Value = $"{url}";
                        y.IsInline = false;
                    });
                    embed.AddField(y =>
                    {
                        y.Name = "Song Name";
                        y.Value = $"{title}";
                        y.IsInline = false;
                    });
                    embed.AddField(y =>
                    {
                        y.Name = "Channel";
                        y.Value = $"Uploaded by **{channel}**";
                        y.IsInline = false;
                    });
                    EmbedFooterBuilder Footer = new EmbedFooterBuilder()
                    {
                        IconUrl = Context.User.GetAvatarUrl(),
                        Text = ($"Queued By {Context.User.Username}")
                    };
                    embed.WithCurrentTimestamp();
                    embed.WithFooter(Footer);
                    embed.ThumbnailUrl = thumbnail;
                    await Context.Channel.SendMessageAsync("", false, embed);
                }

                if (!playing)
                    await LaunchStream(songList[0], client);
            }
            catch (NullReferenceException)
            {
                await ReplyAsync($"{Context.User.Mention} you must be in a voice channel to use this command.");
            }
        }

        [Command("queue")]
        [Alias("Queue","q","Q")]
        public async Task Queue()
        {
            if (queue.Count == 0)
                await ReplyAsync("Nothing queued.");
            else
            {
                string res = "";
                foreach (string s in queue)
                    res += s + "\n";
                EmbedBuilder embed = new EmbedBuilder()
                {
                    Color = new Color(235, 66, 244)
                };
                embed.WithCurrentTimestamp();
                embed.Description = $"{res}";
                await ReplyAsync("", false, embed);
            }
        }

        [Command("skip")]
        [Alias("Skip","SKIP")]
        [RequireUserPermission(GuildPermission.KickMembers)]
        public async Task Skip()
        {
            int no = songList.Count;
            if (no <= 0)
            {
                await ReplyAsync($"{Context.User.Mention} there is nothing to skip.");
                return;
            }
            songList.Remove(songList[0]);
            await LaunchStream(songList[0], client);
            EmbedBuilder embed = new EmbedBuilder()
            {
                Color = new Color(235, 66, 244)
            };
            embed.Description = $"The song was successfully skipped by {Context.User.Mention} .";
            embed.WithCurrentTimestamp();
            await Context.Channel.SendMessageAsync("", false, embed);
        }

        [Command("np")]
        [Alias("NP","nowplaying","NowPlaying")]
        public async Task NP()
        {
            string song = songList[0];
            EmbedBuilder embed = new EmbedBuilder()
            {
                Color = new Color(235, 66, 244)
            };
            embed.Description = $"Now playing: {song} .";
            embed.WithCurrentTimestamp();
            await Context.Channel.SendMessageAsync("", false, embed);
        }

        public async Task LaunchStream(string url, IAudioClient client)
        {
            playing = true;
            EmbedBuilder embed = new EmbedBuilder()
            {
                Color = new Color(235, 66, 244)
            };
            embed.Description = $"Now playing: {queue[0]}";
            await ReplyAsync("", false, embed);
            queue.RemoveAt(0);
            var output = CreateStream(url).StandardOutput.BaseStream;
            var stream = client.CreatePCMStream(AudioApplication.Music, 480);
            await output.CopyToAsync(stream);
            await stream.FlushAsync();
            if (songList.Count == 0)
            {
                playing = false;
                await client.StopAsync();
            }
            else
            {
                songList.Remove(url);
                await LaunchStream(songList[0], client);
            }
        }

        [Command("stop", RunMode = RunMode.Async)]
        [Alias("Stop", "STOP")]
        [RequireOwner]
        public async Task Stop()
        {
            await client.StopAsync();
            songList.Clear();
            queue.Clear();
            await ReplyAsync($"The music was successfully stopped by {Context.User.Mention} .");
            playing = false;
        }
    }

}