using System.Threading.Tasks;
using System.Reflection;
using Discord.Commands;
using Discord.WebSocket;
using Discord;
using System;
using System.Net;
using System.Linq;

namespace FredtheG.CactusBot
{
    public class CommandHandler
    {
        private CommandService _cmds;
        private static DiscordSocketClient _client;

        public static string Name;

        public static string hint;

        public static string checkHint
        {
            get{return hint;}
            set
            {
                if (value == hint)
                    return;

                if (value.Contains("\"happy_hour\":\"1\""))
                    hint = value;
            }
        }
        public static bool justConnected;

        public static string DerronStatus = "";
        public static string CarinaStatus = "";
        public static string GrayanStatus = "";
        public static string FitzStatus = "";
        public static string LokiStatus = "";
        public static string PromieStatus = "";
        public static string MorganaStatus = "";
        public static string AndresStatus = "";
        public static string IsabelStatus = "";

        public static async Task checkStatusAsync(bool isOn = false, string serverName = null)
        {
            string compare = "";
            compare = isOn + serverName;

            switch (serverName)
            {
                case "Derron":
                    if (DerronStatus == compare)
                        return;

                    DerronStatus = compare;
                    if (justConnected == true)
                    {
                        justConnected = false;
                        return;
                    }
                    await updateHappyHourAsync(serverName, isOn);
                    break;

                case "Carina":
                    if (CarinaStatus == compare)
                        return;

                    CarinaStatus = compare;
                    if (justConnected == true)
                    {
                        justConnected = false;
                        return;
                    }
                    await updateHappyHourAsync(serverName, isOn);
                    break;

                case "Grayan":
                    if (GrayanStatus == compare)
                        return;

                    GrayanStatus = compare;
                    if (justConnected == true)
                    {
                        justConnected = false;
                        return;
                    }
                    await updateHappyHourAsync(serverName, isOn);
                    break;

                case "Fitz":
                    if (FitzStatus == compare)
                        return;

                    FitzStatus = compare;
                    if (justConnected == true)
                    {
                        justConnected = false;
                        return;
                    }
                    await updateHappyHourAsync(serverName, isOn);
                    break;

                case "Loki":
                    if (LokiStatus == compare)
                        return;

                    LokiStatus = compare;
                    if (justConnected == true)
                    {
                        justConnected = false;
                        return;
                    }
                    await updateHappyHourAsync(serverName, isOn);
                    break;

                case "Promie":
                    if (PromieStatus == compare)
                        return;

                    PromieStatus = compare;
                    if (justConnected == true)
                    {
                        justConnected = false;
                        return;
                    }
                    await updateHappyHourAsync(serverName, isOn);
                    break;

                case "Morgana":
                    if (MorganaStatus == compare)
                        return;

                    MorganaStatus = compare;
                    if (justConnected == true)
                    {
                        justConnected = false;
                        return;
                    }
                    await updateHappyHourAsync(serverName, isOn);
                    break;

                case "Andres":
                    if (AndresStatus == compare)
                        return;

                    AndresStatus = compare;
                    if (justConnected == true)
                    {
                        justConnected = false;
                        return;
                    }
                    await updateHappyHourAsync(serverName, isOn);
                    break;

                case "Isabel":
                    if (IsabelStatus == compare)
                        return;

                    IsabelStatus = compare;
                    if (justConnected == true)
                    {
                        justConnected = false;
                        return;
                    }
                    await updateHappyHourAsync(serverName, isOn);
                    break;
            }
        }

        private static async Task updateHappyHourAsync(string Name = null, bool isOn = false)
        {
            if (isOn)
            {
                SocketGuild Guild = _client.GetGuild(249657315576381450);
                SocketRole RoleM = Guild.GetRole(307631922094407682);
                SocketTextChannel channel = _client.GetChannel(249678944956055562) as SocketTextChannel;
                await channel.SendMessageAsync($"{RoleM.Mention} A happy hour has just started on Server: {Name}");
            }
        }

        public async Task Install(DiscordSocketClient c)
        {
            _client = c;
            _cmds = new CommandService();

            await _cmds.AddModulesAsync(Assembly.GetEntryAssembly());

            _client.MessageReceived += HandleCommand;

            _client.UserJoined += AnnounceUserJoined;
            _client.UserLeft += AnnounceUserLeft;
            _client.Ready += async () =>
            {
                await _client.SetGameAsync($"/help | pr2hub.com");
            };
        }

        public async Task AnnounceUserJoined(SocketGuildUser user)
        {
            if (user.Guild.Id != 249657315576381450)
            {
                return;
            }
            SocketTextChannel channel = _client.GetChannel(249657315576381450) as SocketTextChannel;

            EmbedBuilder embed = new EmbedBuilder()
            {
                Color = new Color(235, 66, 244)
            };

            embed.WithCurrentTimestamp();
            embed.Title = "__**User Joined**__";
            embed.WithCurrentTimestamp();

            System.Collections.Generic.IEnumerable<SocketRole> role = user.Guild.Roles.Where(has => has.Name.ToUpper() == "Members".ToUpper());
            await user.AddRolesAsync(role);

            embed.Description = $"**Welcome to the Platform Racing Group {user.Username}. Make sure to read the #rules. :grinning: **";
            await channel.SendMessageAsync("", false, embed);
        }

        public async Task AnnounceUserLeft(SocketGuildUser user)
        {
            if (user.Guild.Id != 249657315576381450)
            {
                return;
            }
            SocketTextChannel channel = _client.GetChannel(249657315576381450) as SocketTextChannel;

            EmbedBuilder embed = new EmbedBuilder()
            {
                Color = new Color(235, 66, 244)
            };

            embed.WithCurrentTimestamp();
            embed.Title = "__**User Left**__";
            embed.WithCurrentTimestamp();

            embed.Description = $"**{user.Username} left the Platform Racing Group. :frowning: **";
            await channel.SendMessageAsync("", false, embed);
        }

        public async Task HandleCommand(SocketMessage s)
        {
            SocketUserMessage msg = s as SocketUserMessage;
            if (msg == null) return;
            
            CommandContext context = new CommandContext(_client, msg);

            int argPos = 0;
            if (msg.HasStringPrefix("/", ref argPos) || msg.HasMentionPrefix(_client.CurrentUser, ref argPos))
            {
                IResult result = await _cmds.ExecuteAsync(context, argPos);
            }
        }
    }
}
