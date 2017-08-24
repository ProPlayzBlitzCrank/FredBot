//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using Discord;
//using Discord.Commands;
//using Discord.WebSocket;

//namespace FredtheG.CactusBot.Modules.Public
//{
//    public class WaitressModule : ModuleBase
//    {
//        [Command("mute")]
//        [Alias("Mute")]
//        [Summary("Mutes the user mentioned")]
//        [RequireBotPermission(GuildPermission.ManageRoles)]
//        [RequireUserPermission(GuildPermission.KickMembers)]
//        public async Task Mute(IGuildUser user = null, string time = null, [Remainder] string reason = null)
//        {
//            EmbedBuilder embed = new EmbedBuilder()
//            {
//                Color = new Color(235, 66, 244)
//            };

//            if (user == Context.User)
//            {
//                await ReplyAsync($"{Context.User.Mention} you cannot mute yourself.");
//                return;
//            }

//            if (user == null)
//            {
//                await ReplyAsync($"{Context.User.Mention} you need to mention a user.");
//                return;
//            }
//            double num;
//            if (!double.TryParse(time, out num))
//            {
//                await ReplyAsync($"{Context.User.Mention} time needs to be a number.");
//                return;
//            }

//            if (reason == null)
//            {
//                await ReplyAsync($"{Context.User.Mention} you must provide a reason.");
//                return;
//            }

//            string roleName = "Muted";
//            IEnumerable<IRole> role = user.Guild.Roles.Where(input => input.Name.ToUpper() == roleName.ToUpper());
//            await user.AddRolesAsync(role);
//            embed.WithCurrentTimestamp();
//            embed.Title = "__**Add User Mute**__";
//            embed.Description = $"**{user.Mention} has been muted by {Context.User.Mention} for {time} minute(s) with reason {reason}.**";
//            await Context.Channel.SendMessageAsync("", false, embed);

//            int x = Int32.Parse(time);
//            int mutetime = x * 60000;
//            Task task = Task.Run(async () => {
//                await Task.Delay(mutetime);
//                await user.RemoveRolesAsync(role);
//            });
//        }
//    }
//}