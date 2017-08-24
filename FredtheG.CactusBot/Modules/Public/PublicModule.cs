using System;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Text;
using System.IO;
using System.Threading;
using System.Net;
using System.Text.RegularExpressions;
using System.Collections.Generic;

namespace FredtheG.CactusBot.Modules.Public
{
    public class PublicModule : ModuleBase
    {

        #region Strings

        public static string getBetween(string strSource, string strStart, string strEnd)
        {
            int Start, End;
            if (strSource.Contains(strStart) && strSource.Contains(strEnd))
            {
                Start = strSource.IndexOf(strStart, 0) + strStart.Length;
                End = strSource.IndexOf(strEnd, Start);
                return strSource.Substring(Start, End - Start);
            }
            else
            {
                return "";
            }
        }

        private static DiscordSocketClient _client;

        #endregion 

        #region Owner

        [Command("is a god")]
        [Alias("Is a god")]
        [Summary("Returns 'I know I am'")]
        [RequireOwner]
        public async Task God()
        {
            await ReplyAsync("I know I am.");
        }

        [Command("Turnoff")]
        [Alias("TurnOff","turnoff","poweroff","Poweroff")]
        [Summary("Turns off bot")]
        [RequireOwner]
        public async Task TurnOff()
        {
            await Context.Channel.SendMessageAsync(":wave:");
            Environment.Exit(0);
        }

        #endregion

        #region Help

        [Command("help")]
        [Alias("Help", "commands", "Commands")]
        [Summary("List of commands for the bot.")]
        public async Task Help()
        {
            EmbedBuilder embed = new EmbedBuilder()
            {
                Color = new Color(66, 244, 226),
            };

            await ReplyAsync(Context.User.Mention + " I've just sent my commands to your DMs. :grinning:");

            IDMChannel channel = await Context.User.CreateDMChannelAsync();
            embed.Title = "__**Fred the G. Cactus Commands**__";
            embed.Description = "**Moderator**\n" +
                "/verify - Verifies the user mentioned, reason needed.\n" +
                "/mute - Mutes the user mentioned, Usage: user, time, reason(Without ,).\n" +
                "/kick - Kicks the user mentioned, reason needed.\n" +
                "/ban - Bans the user mentioned, reason needed.\n" +
                "**PR2 Staff Member Only**\n" +
                "/arti - Tells users that the artifact has been changed(mentions role)\n" +
                "**PR2 Discussion Only**\n" +
                "/hint - Tell you the current hint for the artifact location.\n" +
                "/view - Gives you info of a PR2 user, type user after command.\n" +
                "/guild - Tells you guild info of the guild named.\n" +
                "/exp - Tells you the exp needed to the next rank from the rank you said.\n" +
                "/hhadd - Adds you to the Happy Hour role.\n" +
                "/hhremove - Removes you from the Happy Hour role.\n" +
                "/artiadd - Adds you to the Arti role\n" +
                "/artiremove - Removes you from the Arti role\n" +
                "/bans - Gives you info on a PR2 ban, type ban id after command.\n" +
                "/fah - Gives you info on a fah user from Team Jiggmin.\n" +
                "/pop - Tells you total number of users on PR2.\n" +
                "**Music Only**\n" +
                "/play - Plays a song, URL only.\n" +
                "/queue - Queue of songs to be played.\n" +
                "/np - Tells you current song that is playing.\n" +
                "/skip - Not currently working.\n" + 
                "**Everyone**\n" +
                "/help - Tells you commands that you can use for me.\n" +
                "/suggest - Lets you add a suggestion for the suggestions channel.\n" +
                "/search - Searches for a video on Youtube with the ID, NOT URL!";
            await channel.SendMessageAsync("", false, embed);
        }

        #endregion

        #region Music

        [Command("Channel")]
        [Alias("channel")]
        [Summary("Gets info about a YouTube channel.")]
        public async Task Channel([Remainder] string id = null)
        {
            if (id == null)
            {
                await ReplyAsync($"{Context.User.Mention} you must enter an ID.");
                return;
            }
            if (!Uri.IsWellFormedUriString(id, UriKind.Absolute))
            {
                string channelId = id;
                YouTubeChannel channel = new YouTubeChannel(channelId);
                string name = channel.name;
                string creationDate = channel.creationDate.ToShortDateString();
                string description = channel.desciption;
                if (creationDate == "01/01/0001")
                {
                    await ReplyAsync($"{Context.User.Mention} that channel does not exist or could not be found.");
                    return;
                }
                else
                {
                    EmbedBuilder embed = new EmbedBuilder()
                    {
                        Color = new Color(235, 66, 244)
                    };
                    embed.Title = "**-=-=-=-=-= Channel =-=-=-=-=-**";
                    embed.WithCurrentTimestamp();
                    embed.Description = $"**Channel Name**\n{name}\n**Created At**\n{creationDate}\n**Description**\n{description}";
                    await Context.Channel.SendMessageAsync("", false, embed);
                }
            }
            else
            {
                await ReplyAsync($"{Context.User.Mention} enter the ID of the channel not the URL.");
                return;
            }
        }
        [Command("Playlist")]
        [Alias("playlist")]
        [Summary("Gets videos from a playlist")]
        public async Task Playlist([Remainder] string id = null)
        {
            if (id == null)
            {
                await ReplyAsync($"{Context.User.Mention} you must enter an ID.");
                return;
            }
            if (!Uri.IsWellFormedUriString(id, UriKind.Absolute))
            {
                string playlistId = id;
                YoutubeVideo[] videos = YoutubeApi.GetPlaylist(playlistId);
                List<string> vids = new List<string>();
                foreach (var video in videos)
                {
                    vids.Add(video.publishedDate.ToShortDateString() + ": " + video.title + " (" + video.id + ")\n");
                }
                EmbedBuilder embed = new EmbedBuilder()
                {
                    Color = new Color(235, 66, 244)
                };
                embed.Title = "**-=-=-=-=-= Playlist =-=-=-=-=-**";
                embed.WithCurrentTimestamp();
                embed.Description = $"{vids}";
                await Context.Channel.SendMessageAsync("", false, embed);
            }
            else
            {
                await ReplyAsync($"{Context.User.Mention} enter the ID of the video not the URL.");
                return;
            }
        }

        [Command("Search")]
        [Alias("search")]
        [Summary("Searches for a video on YouTube.")]
        public async Task Search([Remainder] string id = null)
        {
            if (id == null)
            {
                await ReplyAsync($"{Context.User.Mention} you must enter an ID.");
                return;
            }
            if (!Uri.IsWellFormedUriString(id, UriKind.Absolute))
            {
                string videoId = id;
                YoutubeVideo video = new YoutubeVideo(videoId);
                string title = video.title;
                string description = video.description;
                string publishedDate = video.publishedDate.ToShortDateString();
                string channel = video.channel;
                //string stuff = video.duration;
                if (publishedDate == "01/01/0001")
                {
                    await ReplyAsync($"{Context.User.Mention} that video does not exist or could not be found.");
                    return;
                }
                else
                {
                    string thumbnail = "http://img.youtube.com/vi/" + id + "/0.jpg";
                    EmbedBuilder embed = new EmbedBuilder()
                    {
                        Color = new Color(235, 66, 244)
                    };
                    embed.Title = "**-=-=-=-=-= Search =-=-=-=-=-**";
                    embed.ThumbnailUrl = thumbnail;
                    embed.WithCurrentTimestamp();
                    embed.Description = $"**Title**\n{title}\n**Description**\n{description}\n**Published Date**\n{publishedDate}\n**Channel**\n{channel}";
                    await Context.Channel.SendMessageAsync("", false, embed);
                }
            }
            else
            {
                await ReplyAsync($"{Context.User.Mention} enter the ID of the video not the URL.");
                return;
            }
        }

        #endregion

        #region Everyone

        [Command("Suggest")]
        [Alias("suggest","Suggestion","suggestion")]
        [Summary("Adds suggestion to suggestion channel")]
        public async Task Suggest([Remainder] string suggestion = null)
        {
            if (suggestion == null)
            {
                IUserMessage reply = await ReplyAsync($"{Context.User.Mention} you need to enter a suggestion.");
                return;
            }
            if (suggestion.Length < 18 || suggestion.Length > 800)
            {
                await ReplyAsync($"{Context.User.Mention} your suggestion must be between at least 18 and no more than 800 characters long.");
                return;
            }
            EmbedBuilder embed = new EmbedBuilder()
            {
                Color = new Color(66, 244, 226)
            };
            embed.WithCurrentTimestamp();
            embed.Description = $"**Suggestion:** {suggestion}\n**Suggested by:** {Context.User.Mention}";
            SocketTextChannel channel = await Context.Guild.GetChannelAsync(249684395454234624) as SocketTextChannel;
            IUserMessage msg = await channel.SendMessageAsync("", false, embed);
            await msg.AddReactionAsync("<:thumbsup:319499737747292170>");
            await msg.AddReactionAsync("<:thumbsdown:319500952925110272>");
        }

        [Command("Steam")]
        [Alias("steam")]
        [Summary("Steam account info.")]
        public async Task Steam(string ID = null)
        {
            if (ID == null)
            {
                 await ReplyAsync($"{Context.User.Mention} you need to specify a profile Id");
                 return;
            }
            WebClient web = new WebClient();
            //String text = web.DownloadString("https://steamdb.info/calculator/" + ID + "/?cc=us");
            web.Proxy.Credentials = CredentialCache.DefaultCredentials;
            String text = web.DownloadString("https://steamdb.info/calculator/76561198115121330/?cc=us");
            web.UseDefaultCredentials = true;
            string[] info = text.Split('>');
            string name = info[316].TrimEnd(new char[] { 'a', '/', '<', ' ' });
            string status = info[320].TrimEnd(new char[] { 'i', 'l', '/', '<', ' ' });
            string created = info[327].TrimEnd(new char[] { 'n', 'a', 'p', 's', '/', '<', ' ' });
            string value = info[347].TrimEnd(new char[] { 'b', '/', '<', ' ' });
            string value2 = info[353].TrimEnd(new char[] { 'b', '/', '<', ' ' });
            string level = info[359].Substring(0, 3);
            string avgvalue = info[369].TrimEnd(new char[] { 'b', '/', '<', ' ' });
            string avgvalueph = info[375].TrimEnd(new char[] { 'b', '/', '<', ' ' });
            string games = info[381].TrimEnd(new char[] { 'b', '/', '<', ' ' });
            string hours = info[391].TrimEnd(new char[] { 'b', '/', '<', ' ' });
            string avghours = info[397].TrimEnd(new char[] { 'b', '/', '<', ' ' });
            string notplayed = info[405].TrimEnd(new char[] { 'b', '/', '<', ' ' });
            string pnotplayed = info[411].TrimEnd(new char[] { 'b', '/', '<', ' ' });
            string vurl = info[427].TrimEnd(new char[] { 'n', 'a', 'p', 's', '/', '<', ' ' });
            string SteamID = ID;
            string Steam2ID = info[443].TrimEnd(new char[] { 'n', 'a', 'p', 's', '/', '<', ' ' });
            string Steam3ID = info[451].TrimEnd(new char[] { 'n', 'a', 'p', 's', '/', '<', ' ' });
            string GameBans = info[464].TrimEnd(new char[] { 'd', 't', '/', '<', ' ' });
            string VACBans = info[470].TrimEnd(new char[] { 'd', 't', '/', '<', ' ' });
            string CommunityBans = info[476].TrimEnd(new char[] { 'd', 't', '/', '<', ' ' });
            string TradeBans = info[482].TrimEnd(new char[] { 'd', 't', '/', '<', ' ' });
            string[] img = text.Split('"');
            string url = img[687];
            EmbedBuilder embed = new EmbedBuilder()
            {
                Color = new Color(66, 244, 226)
            };
            embed.Title = $"Steam info of {name}";
            embed.WithCurrentTimestamp();
            embed.ThumbnailUrl = url;
            embed.Description = $"**Status:** {status}               | **Creation Date:** {created}\n" +
                                $"**Current account value:** {value} | **Total cost with sales:** {value2}     | **Steam level:** {level} | **Hours on record:** {hours}      | **Average playtime:** {avghours}\n" +
                                $"**Average price:** {avgvalue}      | **Average price per hour:**{avgvalueph} | **Games owned:** {games} | **Games not played:** {notplayed} | **Games not played:** {pnotplayed}\n\n" +
                                $"**Vanity URL:** {vurl}                                                       | **Game Bans: {GameBans}\n" +
                                $"**SteamID:** {ID}                                                            | **VAC Bans: {VACBans}\n" +
                                $"**Steam2 ID:** {Steam2ID}                                                    | **Community Bans:** {CommunityBans}\n" +
                                $"**Steam3 ID:** {Steam3ID}                                                    | **Trade Bans: {TradeBans}";
            await Context.Channel.SendMessageAsync("", false, embed);
        }
        

        #endregion

        #region PR2 Commands

        [Command("hint")]
        [Alias("Hint")]
        [Summary("Tells the user the current artifact hint.")]
        public async Task Hint()
        {
            if (Context.Channel.Id == 249678944956055562)
            {
                WebClient web = new WebClient();
                String text = web.DownloadString("http://pr2hub.com/files/artifact_hint.txt");

                string hint1 = text.Substring(9);
                string[] split = hint1.Split(',');
                string levelname = split[0].TrimEnd(new Char[] { '"', ' ' });
                string user = split[1].Substring(15);
                string person = user.TrimEnd(new Char[] { '}', '"', ' ' });
                if (person.Length < 1)
                {
                    await Context.Channel.SendMessageAsync($"Here's what I remember: `{levelname}`. Maybe I can remember more later!!");
                }
                else
                {
                    await Context.Channel.SendMessageAsync($"Here's what I remember: `{levelname}`. Maybe I can remember more later!!\nThe first person to find this artifact was {person}!!");
                }
            }
            else
            {
                return;
            }
        }

        [Command("view")]
        [Alias("View")]
        [Summary("Tells information about pr2 name put after the command.")]
        public async Task View([Remainder] string pr2name = null)
        {
            if (Context.Channel.Id == 249678944956055562)
            {
                WebClient web = new WebClient();
                String pr2info = web.DownloadString("https://pr2hub.com/get_player_info_2.php?name=" + pr2name);

                if (pr2name == null)
                {
                    await ReplyAsync($"{Context.User.Mention} you must enter a PR2 user to view.");
                    return;
                }

                if (pr2info.Contains("Could not find a user with that name."))
                {
                    await ReplyAsync($"{Context.User.Mention} the user {pr2name} does not exist or could not be found.");
                    return;
                }

                EmbedBuilder embed = new EmbedBuilder()
                {
                    Color = new Color(66, 244, 226),
                };

                string[] userinfo = pr2info.Split(',');
                string rank = userinfo[0].Substring(8);
                string hats = userinfo[1].Substring(7);
                int group = Int32.Parse(userinfo[2].Substring(9).TrimEnd(new Char[] { '"', ' ' }));
                string createdat = userinfo[7].Substring(16).TrimEnd(new Char[] { '"', ' ' });
                string lastlogin = userinfo[6].Substring(13).TrimEnd(new Char[] { '"', ' ' });
                string status = userinfo[5].Substring(10).TrimEnd(new Char[] { '"', ' ' });
                string guild = userinfo[17].Substring(13).TrimEnd(new Char[] { '"', ' ' });
                if (group == 1)
                {
                    string Group = "Member";
                    embed.Title = $"-- {pr2name} --";
                    embed.WithCurrentTimestamp();

                    if (createdat.Contains("1970"))
                    {
                        embed.Description = $"{status}\n**Group:** {Group}\n**Guild:** {guild}\n**Rank:** {rank}\n**Hats:** {hats}\n**Joined:** Age of Heroes\n**Active:** {lastlogin}";
                        await Context.Channel.SendMessageAsync("", false, embed);
                    }
                    else
                    {
                        embed.Description = $"{status}\n**Group:** {Group}\n**Guild:** {guild}\n**Rank:** {rank}\n**Hats:** {hats}\n**Joined:** {createdat}\n**Active:** {lastlogin}";
                        await Context.Channel.SendMessageAsync("", false, embed);
                    }
                }
                if (group == 2)
                {
                    string Group = "Moderator";
                    embed.Title = $"-- {pr2name} --";
                    embed.WithCurrentTimestamp();

                    if (createdat.Contains("1970"))
                    {
                        embed.Description = $"{status}\n**Group:** {Group}\n**Guild:** {guild}\n**Rank:** {rank}\n**Hats:** {hats}\n**Joined:** Age of Heroes\n**Active:** {lastlogin}";
                        await Context.Channel.SendMessageAsync("", false, embed);
                    }
                    else
                    {
                        embed.Description = $"{status}\n**Group:** {Group}\n**Guild:** {guild}\n**Rank:** {rank}\n**Hats:** {hats}\n**Joined:** {createdat}\n**Active:** {lastlogin}";
                        await Context.Channel.SendMessageAsync("", false, embed);
                    }
                }
                if (group == 3)
                {
                    string Group = "Admin";
                    embed.Title = $"-- {pr2name} --";
                    embed.WithCurrentTimestamp();

                    if (createdat.Contains("1970"))
                    {
                        embed.Description = $"{status}\n**Group:** {Group}\n**Guild:** {guild}\n**Rank:** {rank}\n**Hats:** {hats}\n**Joined:** Age of Heroes\n**Active:** {lastlogin}";
                        await Context.Channel.SendMessageAsync("", false, embed);
                    }
                    else
                    {
                        embed.Description = $"{status}\n**Group:** {Group}\n**Guild:** {guild}\n**Rank:** {rank}\n**Hats:** {hats}\n**Joined:** {createdat}\n**Active:** {lastlogin}";
                        await Context.Channel.SendMessageAsync("", false, embed);
                    }
                }
            }
            else
            {
                return;
            }
        }

        [Command("guild")]
        [Alias("Guild")]
        [Summary("Info about the guild named after.")]
        public async Task Guild([Remainder] string guildname = null)
        {
            if (Context.Channel.Id == 249678944956055562)
            {
                WebClient web = new WebClient();
                String pr2info = web.DownloadString("http://pr2hub.com/guild_info.php?getMembers=yes&name=" + guildname);

                if (guildname == null)
                {
                    await ReplyAsync($"{Context.User.Mention} you must enter a PR2 guild to view.");
                    return;
                }

                if (pr2info.Contains("Could not find a guild by that name."))
                {
                    await ReplyAsync($"{Context.User.Mention} the guild {guildname} does not exist or could not be found.");
                    return;
                }

                EmbedBuilder embed = new EmbedBuilder()
                {
                    Color = new Color(66, 244, 226),
                };

                string[] guildinfo = pr2info.Split(',');
                string createdat = guildinfo[2].Substring(17).TrimEnd(new Char[] { '"', ' ' });
                string members = guildinfo[4].Substring(16).TrimEnd(new Char[] { '"', ' ' });
                string gptotal = Int32.Parse(guildinfo[6].Substring(12).TrimEnd(new Char[] { '"', ' ' })).ToString("N0"); ;
                string gptoday = Int32.Parse(guildinfo[7].Substring(12).TrimEnd(new Char[] { '"', ' ' })).ToString("N0"); ;
                string note = guildinfo[9].Substring(8).TrimEnd(new Char[] { '"', ' ' });
                string active = guildinfo[10].Substring(16).TrimEnd(new Char[] { '}', '"', ' ' });
                string guildpic = guildinfo[5].Substring(10).TrimEnd(new Char[] { '"', ' ' });

                embed.Title = $"-- {guildname} --";
                embed.WithCurrentTimestamp();
                embed.ThumbnailUrl = "http://pr2hub.com/emblems/" + guildpic;
                embed.Description = $"**Created At:** {createdat}\n**Members:** {members}\n**GP Total:** {gptotal}\n**GP Today:** {gptoday}\n**Description:** {note}";
                await Context.Channel.SendMessageAsync("", false, embed);
            }
            else
            {
                return;
            }
        }

        [Command("exp")]
        [Alias("EXP", "Exp", "EXp", "ExP")]
        [Summary("Tells exp needed to rank up at that rank.")]
        public async Task EXP(string lvl)
        {
            if (Context.Channel.Id == 249678944956055562)
            {
                if (string.IsNullOrEmpty(lvl))
                {
                    await ReplyAsync($"{Context.User.Mention} you need to enter a level.");
                    return;
                }

                else if (!int.TryParse(lvl, out int level_))
                {
                    await ReplyAsync($"{Context.User.Mention} that does not seem to be a number.");
                    return;
                }
                else if (level_ < 0 || level_ > 99)
                {
                    await ReplyAsync($"{Context.User.Mention} you can only do a level between 0 and 100");
                    return;
                }
                else
                {
                    double pow = Math.Pow(1.25, level_);
                    double exp = pow * 30;
                    string exp2 = Math.Round(exp).ToString("N0");

                    EmbedBuilder embed = new EmbedBuilder()
                    {
                        Color = new Color(66, 244, 226),
                    };

                    embed.Title = $"EXP needed to rank up from {lvl}";
                    embed.WithCurrentTimestamp();
                    embed.Description = $"**From rank {lvl} to rank {level_ + 1} you need {exp2} EXP.**";
                    await Context.Channel.SendMessageAsync("", false, embed);
                }
            }
            else
            {
                return;
            }
        }

        [Command("hhadd")]
        [Alias("HHAdd", "HH+", "hh+")]
        [Summary("Adds user to role that gets mentioned when there is happy hour.")]
        [RequireBotPermission(GuildPermission.ManageRoles)]
        public async Task HHAdd()
        {
            if (Context.Channel.Id == 249678944956055562)
            {
                SocketGuildUser userino = Context.User as SocketGuildUser;
                if (userino.Roles.Any(e => e.Name == "HH"))
                {
                    await ReplyAsync($"{Context.User.Mention} you already have the HH role.");
                }
                else
                {
                    EmbedBuilder embed = new EmbedBuilder()
                    {
                        Color = new Color(66, 244, 226),
                    };

                    SocketGuildUser user = Context.User as SocketGuildUser;
                    string roleName = "HH";
                    IEnumerable<SocketRole> role = user.Guild.Roles.Where(input => input.Name.ToUpper() == roleName.ToUpper());
                    await user.AddRolesAsync(role);

                    IRole RoleM = Context.Guild.GetRole(307631922094407682);

                    embed.Title = $"Happy Hour Add";
                    embed.WithCurrentTimestamp();
                    embed.Description = $"**{Context.User.Mention} has been added to the happy hour role({RoleM.Mention}).**";
                    await Context.Channel.SendMessageAsync("", false, embed);
                }
            }
            else
            {
                return;
            }
        }


        [Command("hhremove")]
        [Alias("HHRemove", "HH-", "hh-")]
        [Summary("Removes role from user that gets mentioned when there is happy hour.")]
        [RequireBotPermission(GuildPermission.ManageRoles)]
        public async Task HHRemove()
        {
            if (Context.Channel.Id == 249678944956055562)
            {
                SocketGuildUser userino = Context.User as SocketGuildUser;
                if (userino.Roles.Any(e => e.Name == "HH"))
                {
                    EmbedBuilder embed = new EmbedBuilder()
                    {
                        Color = new Color(66, 244, 226),
                    };

                    SocketGuildUser user = Context.User as SocketGuildUser;
                    string roleName = "HH";
                    IEnumerable<SocketRole> role = user.Guild.Roles.Where(input => input.Name.ToUpper() == roleName.ToUpper());
                    await user.RemoveRolesAsync(role);

                    IRole RoleM = Context.Guild.GetRole(307631922094407682);

                    embed.Title = $"Happy Hour Remove";
                    embed.WithCurrentTimestamp();
                    embed.Description = $"**{Context.User.Mention} has been removed from the happy hour role({RoleM.Mention}).**";
                    await Context.Channel.SendMessageAsync("", false, embed);
                }
                else
                {
                    await ReplyAsync($"{Context.User.Mention} you do not have the HH role so I cannot remove it.");
                }
            }
            else
            {
                return;
            }
        }

        [Command("artiadd")]
        [Alias("ArtiAdd", "Arti+", "arti+")]
        [Summary("Adds user to role that gets mentioned when the artifact has changed.")]
        [RequireBotPermission(GuildPermission.ManageRoles)]
        public async Task ArtiAdd()
        {
            if (Context.Channel.Id == 249678944956055562)
            {
                SocketGuildUser userino = Context.User as SocketGuildUser;
                if (userino.Roles.Any(e => e.Name == "Arti"))
                {
                    await ReplyAsync($"{Context.User.Mention} you already have the Arti role.");
                }
                else
                {
                    EmbedBuilder embed = new EmbedBuilder()
                    {
                        Color = new Color(66, 244, 226),
                    };

                    SocketGuildUser user = Context.User as SocketGuildUser;
                    string roleName = "Arti";
                    IEnumerable<SocketRole> role = user.Guild.Roles.Where(input => input.Name.ToUpper() == roleName.ToUpper());
                    await user.AddRolesAsync(role);

                    IRole RoleM = Context.Guild.GetRole(347312071618330626);

                    embed.Title = $"Arti Add";
                    embed.WithCurrentTimestamp();
                    embed.Description = $"**{Context.User.Mention} has been added to the Arti role({RoleM.Mention}).**";
                    await Context.Channel.SendMessageAsync("", false, embed);
                }
            }
            else
            {
                return;
            }
        }


        [Command("artiremove")]
        [Alias("ArtiRemove", "Arti-", "arti-")]
        [Summary("Removes role from user that gets mentioned when the artifact has been changed.")]
        [RequireBotPermission(GuildPermission.ManageRoles)]
        public async Task ArtiRemove()
        {
            if (Context.Channel.Id == 249678944956055562)
            {
                SocketGuildUser userino = Context.User as SocketGuildUser;
                if (userino.Roles.Any(e => e.Name == "Arti"))
                {
                    EmbedBuilder embed = new EmbedBuilder()
                    {
                        Color = new Color(66, 244, 226),
                    };

                    SocketGuildUser user = Context.User as SocketGuildUser;
                    string roleName = "Arti";
                    IEnumerable<SocketRole> role = user.Guild.Roles.Where(input => input.Name.ToUpper() == roleName.ToUpper());
                    await user.RemoveRolesAsync(role);

                    IRole RoleM = Context.Guild.GetRole(347312071618330626);

                    embed.Title = $"Arti Remove";
                    embed.WithCurrentTimestamp();
                    embed.Description = $"**{Context.User.Mention} has been removed from the Arti role({RoleM.Mention}).**";
                    await Context.Channel.SendMessageAsync("", false, embed);
                }
                else
                {
                    await ReplyAsync($"{Context.User.Mention} you do not have the Arti role so I cannot remove it.");
                }
            }
            else
            {
                return;
            }
        }

        [Command("topguilds")]
        [Alias("TopGuilds","Topguilds")]
        [Summary("Returns current top 10 guild on pr2.")]
        public async Task TopGuilds()
        {
            if (Context.Channel.Id == 249678944956055562)
            {
                WebClient web = new WebClient();
                String text = web.DownloadString("http://pr2hub.com/guilds_top.php?");

                string[] guildlist = text.Split(',');
                string guild1name = guildlist[2].Substring(14).TrimEnd(new Char[] { '"', ' ' });
                string guild1gp = Int32.Parse(guildlist[3].Substring(12).TrimEnd(new Char[] { '"', ' ' })).ToString("N0");
                string guild2name = guildlist[6].Substring(14).TrimEnd(new Char[] { '"', ' ' });
                string guild2gp = Int32.Parse(guildlist[7].Substring(12).TrimEnd(new Char[] { '"', ' ' })).ToString("N0");
                string guild3name = guildlist[10].Substring(14).TrimEnd(new Char[] { '"', ' ' });
                string guild3gp = Int32.Parse(guildlist[11].Substring(12).TrimEnd(new Char[] { '"', ' ' })).ToString("N0");
                string guild4name = guildlist[14].Substring(14).TrimEnd(new Char[] { '"', ' ' });
                string guild4gp = Int32.Parse(guildlist[15].Substring(12).TrimEnd(new Char[] { '"', ' ' })).ToString("N0");
                string guild5name = guildlist[18].Substring(14).TrimEnd(new Char[] { '"', ' ' });
                string guild5gp = Int32.Parse(guildlist[19].Substring(12).TrimEnd(new Char[] { '"', ' ' })).ToString("N0");
                string guild6name = guildlist[22].Substring(14).TrimEnd(new Char[] { '"', ' ' });
                string guild6gp = Int32.Parse(guildlist[23].Substring(12).TrimEnd(new Char[] { '"', ' ' })).ToString("N0");
                string guild7name = guildlist[26].Substring(14).TrimEnd(new Char[] { '"', ' ' });
                string guild7gp = Int32.Parse(guildlist[27].Substring(12).TrimEnd(new Char[] { '"', ' ' })).ToString("N0");
                string guild8name = guildlist[30].Substring(14).TrimEnd(new Char[] { '"', ' ' });
                string guild8gp = Int32.Parse(guildlist[31].Substring(12).TrimEnd(new Char[] { '"', ' ' })).ToString("N0");
                string guild9name = guildlist[34].Substring(14).TrimEnd(new Char[] { '"', ' ' });
                string guild9gp = Int32.Parse(guildlist[35].Substring(12).TrimEnd(new Char[] { '"', ' ' })).ToString("N0");
                string guild10name = guildlist[38].Substring(14).TrimEnd(new Char[] { '"', ' ' });
                string guild10gp = Int32.Parse(guildlist[39].Substring(12).TrimEnd(new Char[] { '"', ' ' })).ToString("N0");

                EmbedBuilder embed = new EmbedBuilder();
                embed.WithColor(new Color(66, 244, 226));
                embed.AddField(y =>
                {
                    y.Name = "Guild";
                    y.Value = $"{guild1name}\n" +
                              $"{guild2name}\n" +
                              $"{guild3name}\n" +
                              $"{guild4name}\n" +
                              $"{guild5name}\n" +
                              $"{guild6name}\n" +
                              $"{guild7name}\n" +
                              $"{guild8name}\n" +
                              $"{guild9name}\n" +
                              $"{guild10name}";
                    y.IsInline = true;
                });
                embed.AddField(y =>
                {
                    y.Name = "GP Today";
                    y.Value = $"{guild1gp}\n" +
                              $"{guild2gp}\n" +
                              $"{guild3gp}\n" +
                              $"{guild4gp}\n" +
                              $"{guild5gp}\n" +
                              $"{guild6gp}\n" +
                              $"{guild7gp}\n" +
                              $"{guild8gp}\n" +
                              $"{guild9gp}\n" +
                              $"{guild10gp}";
                    y.IsInline = true;
                });
                embed.Title = $"PR2 Top 10 Guilds";
                embed.WithCurrentTimestamp();
                await Context.Channel.SendMessageAsync("", false, embed);
            }
            else
            {
                return;
            }
        }

        [Command("f@h")]
        [Alias("F@H", "fah", "FAH", "F@h", "Fah")]
        [Summary("Gets the specified names f@h points")]
        public async Task Fah([Remainder] string fahuser = null)
        {
            if (Context.Channel.Id == 249678944956055562)
            {
                if (fahuser == null)
                {
                    await ReplyAsync($"{Context.User.Mention} you need to enter a user.");
                    return;
                }
                try
                {
                    WebClient web = new WebClient();
                    if (fahuser.Contains(' '))
                    {
                        try
                        {
                            string fahuser2 = fahuser.Replace(' ', '_');
                            String text = web.DownloadString("http://fah-web2.stanford.edu/cgi-bin/main.py?qtype=userpage&username=" + fahuser2 + "&teamnum=143016");
                            string[] fahuserinfo = text.Split('>');
                            string donor = fahuserinfo[120].TrimEnd(new char[] { 'A', '/', '<', ' ' });
                            string score = Int32.Parse(fahuserinfo[143].Substring(1).TrimEnd(new char[] { 'b', '/', '<', ' ' })).ToString("N0");
                            string rank = Int32.Parse(fahuserinfo[155].Substring(1).TrimEnd(new char[] { 'b', '/', '<', ' ' })).ToString("N0");
                            string wu = Int32.Parse(fahuserinfo[165].Substring(1).TrimEnd(new char[] { 'b', '/', '<', ' ' })).ToString("N0");

                            EmbedBuilder embed = new EmbedBuilder()
                            {
                                Color = new Color(66, 244, 226),
                            };
                            embed.AddField(y =>
                            {
                                y.Name = "Score";
                                y.Value = $"{score}";
                                y.IsInline = true;
                            });
                            embed.AddField(y =>
                            {
                                y.Name = "Overall Rank";
                                y.Value = $"{rank}";
                                y.IsInline = true;
                            });
                            embed.AddField(y =>
                            {
                                y.Name = "Completed WUs";
                                y.Value = $"{wu}";
                                y.IsInline = true;
                            });
                            embed.Title = $"F@H info of {fahuser}";
                            embed.WithCurrentTimestamp();
                            await Context.Channel.SendMessageAsync("", false, embed);
                        }
                        catch (IndexOutOfRangeException)
                        {
                            await ReplyAsync($"{Context.User.Mention} the user {fahuser} does not exist or could not be found.");
                            return;
                        }
                    }
                    else
                    {
                        try
                        {
                            String text = web.DownloadString("http://fah-web2.stanford.edu/cgi-bin/main.py?qtype=userpage&username=" + fahuser + "&teamnum=143016");
                            string[] fahuserinfo = text.Split('>');
                            string donor = fahuserinfo[120].TrimEnd(new char[] { 'A', '/', '<', ' ' });
                            string score = Int32.Parse(fahuserinfo[143].Substring(1).TrimEnd(new char[] { 'b', '/', '<', ' ' })).ToString("N0");
                            string rank = Int32.Parse(fahuserinfo[155].Substring(1).TrimEnd(new char[] { 'b', '/', '<', ' ' })).ToString("N0");
                            string wu = Int32.Parse(fahuserinfo[165].Substring(1).TrimEnd(new char[] { 'b', '/', '<', ' ' })).ToString("N0");

                            EmbedBuilder embed = new EmbedBuilder()
                            {
                                Color = new Color(66, 244, 226),
                            };
                            embed.AddField(y =>
                            {
                                y.Name = "Score";
                                y.Value = $"{score}";
                                y.IsInline = true;
                            });
                            embed.AddField(y =>
                            {
                                y.Name = "Overall Rank";
                                y.Value = $"{rank}";
                                y.IsInline = true;
                            });
                            embed.AddField(y =>
                            {
                                y.Name = "Completed WUs";
                                y.Value = $"{wu}";
                                y.IsInline = true;
                            });
                            embed.Title = $"F@H info of {fahuser}";
                            embed.WithCurrentTimestamp();
                            await Context.Channel.SendMessageAsync("", false, embed);
                        }
                        catch (IndexOutOfRangeException)
                        {
                            await ReplyAsync($"{Context.User.Mention} that user {fahuser} does not exist or could not be found.");
                            return;
                        }
                    }
                }
                catch (WebException e) when (e.Status == WebExceptionStatus.Timeout)
                {
                    await ReplyAsync($"{Context.User.Mention} Folding @ Home is currently updating.");
                    return;
                }
            }
            else
            {
                return;
            }
        }

        [Command("bans")]
        [Alias("Bans")]
        [Summary("Gets a PR2 Ban with ID from ban log.")]
        public async Task Bans([Remainder] string id = null)
        {
            if (Context.Channel.Id == 249678944956055562)
            {
                if (id == null)
                {
                    await ReplyAsync($"{Context.User.Mention} you need to specify a ban Id.");
                    return;
                }

                WebClient web = new WebClient();
                String text = web.DownloadString("http://pr2hub.com/bans/show_record.php?ban_id=" + id);

                if (text.Contains("banned for 0 seconds on Jan 1, 1970 12:00 AM."))
                {
                    await ReplyAsync($"{Context.User.Mention} the ban with the Id {id} does not exist or could not be found.");
                    return;
                }

                if (text.Contains("This ban has been lifted"))
                {
                    string[] ban1 = text.Split('<');
                    string lifted = ban1[18].Substring(6).TrimEnd(new char[] { '-', '-', '-', ' ' });
                    string reason2 = ban1[20].Substring(14).TrimEnd(new char[] { '-', '-', '-', ' ' });
                    string bantext1 = ban1[27].Substring(2);
                    string reason1 = ban1[29].Substring(10);
                    string expire1 = ban1[31].Substring(26);

                    EmbedBuilder embed2 = new EmbedBuilder()
                    {
                        Color = new Color(66, 244, 226),
                    };
                    embed2.AddField(y =>
                    {
                        y.Name = "Lifted Ban";
                        y.IsInline = false;
                    });
                    embed2.AddField(y =>
                    {
                        y.Name = "Info";
                        y.Value = $"{lifted}";
                        y.IsInline = true;
                    });
                    embed2.AddField(y =>
                    {
                        y.Name = "Reason";
                        y.Value = $"{reason2}";
                        y.IsInline = true;
                    });
                    embed2.AddField(y =>
                    {
                        y.Name = "Ban";
                        y.IsInline = false;
                    });
                    embed2.AddField(y =>
                    {
                        y.Name = "Ban Info";
                        y.Value = $"{bantext1}";
                        y.IsInline = true;
                    });
                    embed2.AddField(y =>
                    {
                        y.Name = "Reason";
                        y.Value = $"{reason1}";
                        y.IsInline = true;
                    });
                    embed2.AddField(y =>
                    {
                        y.Name = "Expires";
                        y.Value = $"{expire1}";
                        y.IsInline = true;
                    });
                    embed2.Title = $"Ban info of ban Id {id}";
                    embed2.WithCurrentTimestamp();
                    await Context.Channel.SendMessageAsync("", false, embed2);
                    return;
                }

                string[] ban = text.Split('<');
                string bantext = ban[15].Substring(2);
                string reason = ban[17].Substring(10);
                string expire = ban[19].Substring(26);

                EmbedBuilder embed = new EmbedBuilder()
                {
                    Color = new Color(66, 244, 226),
                };
                embed.AddField(y =>
                {
                    y.Name = "Ban Info";
                    y.Value = $"{bantext}";
                    y.IsInline = true;
                });
                embed.AddField(y =>
                {
                    y.Name = "Reason";
                    y.Value = $"{reason}";
                    y.IsInline = true;
                });
                embed.AddField(y =>
                {
                    y.Name = "Expires";
                    y.Value = $"{expire}";
                    y.IsInline = true;
                });
                embed.Title = $"Ban info of ban Id {id}";
                embed.WithCurrentTimestamp();
                await Context.Channel.SendMessageAsync("", false, embed);
            }
            else
            {
                return;
            }
        }

        [Command("Pop")]
        [Alias("pop","population","Population")]
        [Summary("Tells you the number of users on pr2. Does not include private servers.")]
        public async Task Pop()
        {
            if (Context.Channel.Id == 249678944956055562)
            {
                WebClient web = new WebClient();
                String text = web.DownloadString("http://pr2hub.com/files/server_status_2.txt");

                string[] pops = text.Split('}');
                int pop = 0;
                foreach (string server in pops)
                {
                    if (server.Length > 5)
                    {
                        int population = Convert.ToInt32(getBetween(server, "population\":\"", "\","));
                        pop += population;
                    }
                }

                EmbedBuilder embed = new EmbedBuilder()
                {
                    Color = new Color(66, 244, 226),
                };
                embed.Title = "__PR2 Total Online Users__";
                embed.WithCurrentTimestamp();
                embed.Description = $"The total number of users on PR2 currently is {pop}";
                await Context.Channel.SendMessageAsync("", false, embed);
            }
            else
            {
                return;
            }
        }

        [Command("stats")]
        [Alias("Stats","s","S")]
        [Summary("Gets stats of a server on PR2.")]
        public async Task Stats(string server = null)
        {
            if (server == null)
            {
                await ReplyAsync($"{Context.User.Mention} you need to enter a server");
                return;
            }
            WebClient web = new WebClient();
            String text = web.DownloadString("http://pr2hub.com/files/server_status_2.txt");
            string[] servers = text.Split('}');
            
        }

        #endregion

        #region Moderator

        [Command("Arti")]
        [Alias("arti","artifact")]
        [Summary("Command for bls to let people know of new arti.")]
        public async Task Arti()
        {
            SocketGuildUser userino = Context.User as SocketGuildUser;
            if (userino.Roles.Any(e => e.Name == "PR2 Staff Member"))
            {
                WebClient web = new WebClient();
                String text = web.DownloadString("http://pr2hub.com/files/artifact_hint.txt");

                string hint1 = text.Substring(9);
                string[] split = hint1.Split(',');
                string levelname = split[0].TrimEnd(new Char[] { '"', ' ' });
                IRole RoleM = Context.Guild.GetRole(347312071618330626);
                SocketTextChannel channel = await Context.Guild.GetChannelAsync(249678944956055562) as SocketTextChannel;
                await channel.SendMessageAsync($"{RoleM.Mention} Hmm... I seem to have misplaced the artifact. Maybe you can help me find it?\n" +
                    $"Here's what I remember: `{levelname}`. Maybe I can remember more later!!");
            }
            else
            {
                return;
            }
        }

        [Command("Restart")]
        [Alias("restart")]
        [Summary("Restarts bot")]
        [RequireOwner]
        public async Task Restart()
        {
            await Context.Channel.SendMessageAsync("Restarting.....");
            await Program.Start(Directory.GetCurrentDirectory() +
                AppDomain.CurrentDomain.FriendlyName);
        }

        [Command("userinfo")]
        [Alias("Userinfo", "UserInfo")]
        [Summary("Returns information about the mentioned user")]
        [Name("userinfo `<user>`")]
        [RequireUserPermission(GuildPermission.KickMembers)]
        public async Task UserInfo(IGuildUser user = null)
        {
            {
                if (user == null)
                {
                    await ReplyAsync($"{Context.User.Mention} you need to mention a user.");
                    return;
                }
                IApplication application = await Context.Client.GetApplicationInfoAsync(); // Gets The Client's info
                //var thumbnailurl = user.GetAvatarUrl; // Pulls the User's Avatar
                string date = $"{user.CreatedAt.Month}/{user.CreatedAt.Day}/{user.CreatedAt.Year}"; // Shows the date the account was made
                EmbedAuthorBuilder auth = new EmbedAuthorBuilder() // Shows the Name of the user

                {
                    Name = user.Username,
                };

                EmbedBuilder embed = new EmbedBuilder()

                {
                    Color = new Color(0, 255, 12),
                    Author = auth
                };

                SocketGuildUser us = user as SocketGuildUser;

                String D = us.Username;

                String A = us.Discriminator; //Pulls the Discriminator
                ulong T = us.Id; //Gets the user's Id
                String S = date; //Pulls the Date the User's accound was created
                UserStatus C = us.Status; //Pulls The Status of the user
                DateTimeOffset? CC = us.JoinedAt; //Pulls the date the user joined the Guild/Server.
                Game? O = us.Game; //Tells the game the bot is playing
                embed.Title = $"**{us.Username}** Information";
                embed.ThumbnailUrl = user.GetAvatarUrl();
                embed.Description = $"Username: **{D}**\nDiscriminator: **{A}**\nUser ID: **{T}**\nCreated at: **{S}**\nCurrent Status: **{C}**\nJoined server at: **{CC}**\nPlaying: **{O}**";

                await ReplyAsync("", false, embed.Build());
            }
        }

        [Command("guildinfo")]
        [Alias("GuildInfo", "Guildinfo")]
        [Summary("Information about current server")]
        [RequireUserPermission(GuildPermission.KickMembers)]
        public async Task ServerInfo()
        {
            var gld = Context.Guild as SocketGuild;
            var embed = new EmbedBuilder()
            {
                Color = new Color(235, 66, 244)
            };
            var GuildName = gld.Name;
            var GuildId = gld.Id;
            var GuildPic = gld.IconUrl;
            var GuildOwnerId = gld.OwnerId;
            var GuildOwner = await Context.Guild.GetUserAsync(Context.Guild.OwnerId);
            var GuildCreatedAt = gld.CreatedAt;
            var GuildRegion = gld.VoiceRegionId;
            var GuildUsers = gld.MemberCount;
            var GuildRoles = gld.Roles.Count;
            var GuildChannels = gld.Channels.Count;
            var GuildVoiceChannels = gld.VoiceChannels.Count;
            var GuildVerificationLevel = gld.VerificationLevel;
            embed.WithCurrentTimestamp();
            embed.Title = "__**Server Info**__";
            embed.ThumbnailUrl = Context.Guild.IconUrl;
            embed.Description = $"**Name: **{GuildName}\n**Id: **{GuildId}\n**Owner: **{GuildOwner.Mention}\n**Owner Id: **{GuildOwnerId}\n**Created At: **{GuildCreatedAt}\n**Region: **{GuildRegion}\n**Number Of Members: **{GuildUsers}\n**Number Of Roles: **{GuildRoles}\n**Number Of Text Channels: **{GuildChannels}\n**Number Of Voice Channels: **{GuildVoiceChannels}\n**Verification Level: **{GuildVerificationLevel}";
            await Context.Channel.SendMessageAsync("", false, embed);
        }

        //[Command("Kick")]
        //[Alias("kick")]
        //[Summary("Kicks the user mentioned")]
        //[RequireBotPermission(GuildPermission.KickMembers)] ///Needed BotPerms///
        //[RequireUserPermission(GuildPermission.KickMembers)] ///Needed User Perms///
        //public async Task KickAsync(SocketGuildUser user = null, [Remainder] string reason = null)
        //{
        //    if (user == Context.User)
        //    {
        //        await ReplyAsync($"{Context.User.Mention} you cannot kick yourself.");
        //        return;
        //    }

        //    if (user == null) 
        //    {
        //        await ReplyAsync($"{Context.User.Mention} you must mention a user");
        //        return;
        //    }

        //    if (string.IsNullOrWhiteSpace(reason))
        //    {
        //        await ReplyAsync($"{Context.User.Mention} you must provide a reason");
        //        return;
        //    }
        //    try
        //    {
        //        SocketGuild gld = Context.Guild as SocketGuild;
        //        EmbedBuilder embed = new EmbedBuilder(); ///starts embed///
        //        embed.WithCurrentTimestamp();
        //        embed.WithColor(new Color(0x4900ff)); ///hexacode colours ///
        //        embed.Title = $" {user.Username} has been kicked from {user.Guild.Name}"; ///who was kicked///
        //        embed.Description = $"**Username: **{user.Username}\n**Guild Name: **{user.Guild.Name}\n**Kicked by: **{Context.User.Mention}!\n**Reason: **{reason}"; ///embed values///

        //        Discord.Rest.RestDMChannel dmban = await user.CreateDMChannelAsync();
        //        await dmban.SendMessageAsync($"You have been kicked from {user.Guild.Name} by {Context.User.Mention} for {reason}.\nFeel free to rejoin when you feel as though you have learned your lesson.");

        //        await user.KickAsync(); ///kicks selected user///
        //        await Context.Channel.SendMessageAsync("", false, embed); ///sends embed///
        //    }
        //    catch (Discord.Net.HttpException)
        //    {
        //        await ReplyAsync($"{Context.User.Mention} that user is of higher role than me.");
        //        return;
        //    }
        //}

        //[Command("ban")]
        //[Alias("Ban")]
        //[Summary("Bans the user mentioned, needs reason.")]
        //[RequireBotPermission(GuildPermission.BanMembers)]
        //[RequireUserPermission(GuildPermission.BanMembers)]
        //public async Task Ban(SocketGuildUser user = null, [Remainder] string reason = null)
        //{
        //    if (user == Context.User)
        //    {
        //        await ReplyAsync($"{Context.User.Mention} you cannot ban yourself.");
        //        return;
        //    }
        //    if (user == null) 
        //    {
        //        await ReplyAsync($"{Context.User.Mention} you must mention a user");
        //        return;
        //    }
        //    if (string.IsNullOrWhiteSpace(reason)) 
        //    {
        //        await ReplyAsync($"{Context.User.Mention} you must provide a reason");
        //        return;
        //    }
        //    try
        //    {
        //        SocketGuild gld = Context.Guild as SocketGuild;
        //        EmbedBuilder embed = new EmbedBuilder(); //starts embed
        //        embed.WithCurrentTimestamp();
        //        embed.WithColor(new Color(0x4900ff)); //hexacode colours
        //        embed.Title = $"**{user.Username}** has been banned from {user.Guild.Name}"; //Who was banned
        //        embed.Description = $"**Username: **{user.Username}\n**Guild Name: **{user.Guild.Name}\n**Banned by: **{Context.User.Mention}!\n**Reason: **{reason}"; //Embed values

        //        Discord.Rest.RestDMChannel dmban = await user.CreateDMChannelAsync(); //creates dm channel with user who was banned
        //        await dmban.SendMessageAsync($"You have been banned from {user.Guild.Name} by {Context.User.Mention} for {reason}."); //tells them about their ban

        //        IMessageChannel channel = Context.Guild.GetChannelAsync(263474494327226388) as IMessageChannel;
        //        EmbedBuilder embed2 = new EmbedBuilder(); //starts embed
        //        embed2.WithCurrentTimestamp();
        //        embed2.WithColor(new Color(0x4900ff)); //hexacode colours
        //        embed2.Title = $"**User Banned**"; //Who was banned
        //        embed2.Description = $"**Username: **{user.Username}\n**Guild Name: **{user.Guild.Name}\n**Banned by: **{Context.User.Mention}!\n**Reason: **{reason}"; //Embed values

        //        await gld.AddBanAsync(user); //bans selected user
        //        await Context.Channel.SendMessageAsync("", false, embed); //sends embed
        //        await channel.SendMessageAsync("", false, embed2);
        //    }
        //    catch(Discord.Net.HttpException)
        //    {
        //        await ReplyAsync($"{Context.User.Mention} that user is of higher role than me.");
        //        return;
        //    }
        //}

        #endregion
    }
}

