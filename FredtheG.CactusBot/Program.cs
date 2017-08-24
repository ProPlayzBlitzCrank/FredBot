using System;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using Discord.Commands;
using System.Timers;
using System.Net;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.IO;

namespace FredtheG.CactusBot
{
    public class Program
    {

        #region Fields

        private DiscordSocketClient _client = new DiscordSocketClient(new DiscordSocketConfig
        {
            LogLevel = LogSeverity.Verbose
        });
        private CommandHandler _commands = new CommandHandler();

        #endregion

        #region Startup

        // Convert our sync main to an async main.
        public static void Main(string[] args) =>
           new Program().Start().GetAwaiter().GetResult();

        public async Task Start()
        {
            await _client.LoginAsync(TokenType.Bot, "MzA2ODExNjE5NDI4Nzk0MzY5.C-JX0Q.mUuKgeB_cyQwjnswcD0HIga8f0Q");
            await _client.StartAsync();

            _client.Log += Log;

            //_client.MessageReceived += ReceivedMessage;
            await _commands.Install(_client);

            System.Timers.Timer aTimer = new System.Timers.Timer();
            aTimer.Elapsed += new ElapsedEventHandler(OnTimedEventAsync);

            //1000 = 1 second
            aTimer.Interval = 5000;
            aTimer.Enabled = true;

            await Task.Delay(-1);
        }

        #endregion

        #region Timer Loop

        private async void OnTimedEventAsync(object sender, ElapsedEventArgs e)
        {
            //Do something on timer tick
            WebClient web = new WebClient();
            String status = web.DownloadString("http://pr2hub.com/files/server_status_2.txt");
            String artifactHint = web.DownloadString("http://pr2hub.com/files/server_status_2.txt");

            #region Update Status
            string[] serverInfo = status.Split(':');

            #region Check Derron
            string derronHappyHour = serverInfo[10].Substring(1);
            derronHappyHour = derronHappyHour.Substring(0, derronHappyHour.Length - 15);
            bool bDerron = derronHappyHour.Equals("1");
            await CommandHandler.checkStatusAsync(bDerron, "Derron");
            #endregion

            #region Check Carina
            string carinaHappyHour = serverInfo[19].Substring(1);
            carinaHappyHour = carinaHappyHour.Substring(0, carinaHappyHour.Length - 15);
            bool bCarina = carinaHappyHour.Equals("1");
            await CommandHandler.checkStatusAsync(bCarina, "Carina");
            #endregion

            #region Check Grayan
            string grayanHappyHour = serverInfo[28].Substring(1);
            grayanHappyHour = grayanHappyHour.Substring(0, grayanHappyHour.Length - 15);
            bool bGrayan = grayanHappyHour.Equals("1");
            await CommandHandler.checkStatusAsync(bGrayan, "Grayan");
            #endregion

            #region Check Fitz
            string fitzHappyHour = serverInfo[37].Substring(1);
            fitzHappyHour = fitzHappyHour.Substring(0, fitzHappyHour.Length - 15);
            bool bFitz = fitzHappyHour.Equals("1");
            await CommandHandler.checkStatusAsync(bFitz, "Fitz");
            #endregion

            #region Check Loki
            string lokiHappyHour = serverInfo[46].Substring(1);
            lokiHappyHour = lokiHappyHour.Substring(0, lokiHappyHour.Length - 15);
            bool bLoki = lokiHappyHour.Equals("1");
            await CommandHandler.checkStatusAsync(bLoki, "Loki");
            #endregion

            #region Check Promie
            string promieHappyHour = serverInfo[56].Substring(1);
            promieHappyHour = promieHappyHour.Substring(0, promieHappyHour.Length - 15);
            bool bPromie = promieHappyHour.Equals("1");
            await CommandHandler.checkStatusAsync(bPromie, "Promie");
            #endregion

            #region Check Morgana
            string morganaHappyHour = serverInfo[65].Substring(1);
            morganaHappyHour = morganaHappyHour.Substring(0, morganaHappyHour.Length - 15);
            bool bMorgana = morganaHappyHour.Equals("1");
            await CommandHandler.checkStatusAsync(bMorgana, "Morgana");
            #endregion

            #region Check Andres
            string andresHappyHour = serverInfo[74].Substring(1);
            andresHappyHour = andresHappyHour.Substring(0, andresHappyHour.Length - 15);
            bool bAndres = andresHappyHour.Equals("1");
            await CommandHandler.checkStatusAsync(bAndres, "Andres");
            #endregion

            #region Check Isabel
            string isabelHappyHour = serverInfo[83].Substring(1);
            isabelHappyHour = isabelHappyHour.Substring(0, isabelHappyHour.Length - 15);
            bool bIsabel = isabelHappyHour.Equals("1");
            await CommandHandler.checkStatusAsync(bIsabel, "Isabel");
            #endregion

            #endregion


            CommandHandler.checkHint = artifactHint;

        }

        #endregion

        #region Log

        private Task Log(LogMessage msg)
        {
            ConsoleColor log = Console.ForegroundColor;
            switch (msg.Severity)
            {
                case LogSeverity.Critical:
                    Console.ForegroundColor = ConsoleColor.Red;
                    break;
                case LogSeverity.Error:
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    break;
                case LogSeverity.Warning:
                    Console.ForegroundColor = ConsoleColor.Green;
                    break;
                case LogSeverity.Info:
                    Console.ForegroundColor = ConsoleColor.Blue;
                    break;
                case LogSeverity.Verbose:
                    Console.ForegroundColor = ConsoleColor.DarkMagenta;
                    break;
                case LogSeverity.Debug:
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    break;
            }

            Console.WriteLine($"{DateTime.Now,-19} [{msg.Severity,8}] {msg.Source}: {msg.Message}");
                Console.ForegroundColor = log;

            return Task.CompletedTask;
        }

        internal static Task Start(object workingDirectly, object friendlyName)
        {
            throw new NotImplementedException();
        }

        internal static Task Start(string v)
        {
            throw new NotImplementedException();
        }

        #endregion

    }
}
