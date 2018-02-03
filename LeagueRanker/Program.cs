using System;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using Discord.Net.Providers.WS4Net;
using System.Collections.Generic;
using Discord.Commands;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using System.Configuration;

namespace LeagueRanker
{
    public class Program
    {
        private DiscordSocketClient _client;
        private CommandService _commands;
        private IServiceProvider _services;

        static void Main(string[] args) => new Program().MainAsync().GetAwaiter().GetResult();

        public async Task MainAsync()
        {
            var token = ConfigurationManager.AppSettings["discord_token"];

            _client = new DiscordSocketClient(new DiscordSocketConfig()
            {
                WebSocketProvider = WS4NetProvider.Instance,
            });

            _commands = new CommandService();
            _services = new ServiceCollection()
                .AddSingleton(_client)
                .AddSingleton(_commands)
                .AddSingleton(new Services.GetRankService())
                .BuildServiceProvider();

            await _commands.AddModuleAsync<Modules.UpdateRankModule>();

            _client.Log += Log;
            _client.Ready += _client_Ready;
            
            await InstallCommandsAsync();

            await _client.LoginAsync(TokenType.Bot, token);
            await _client.StartAsync();

            await Task.Delay(-1);
        }

        private async Task _client_Ready()
        {
            foreach (var guild in _client.Guilds)
            {
                List<SocketRole> roles = new List<SocketRole>();
                foreach (var role in guild.Roles)
                {
                    roles.Add(role);
                }

                if(!roles.Exists(x => x.Name == "Bronze"))
                    await guild.CreateRoleAsync("Bronze", color: Color.Orange);
                if (!roles.Exists(x => x.Name == "Silver"))
                    await guild.CreateRoleAsync("Silver", color: Color.LightGrey);
                if (!roles.Exists(x => x.Name == "Gold"))
                    await guild.CreateRoleAsync("Gold", color: Color.Gold);
                if (!roles.Exists(x => x.Name == "Platinum"))
                    await guild.CreateRoleAsync("Platinum", color: Color.Purple);
                if (!roles.Exists(x => x.Name == "Diamond"))
                    await guild.CreateRoleAsync("Diamond", color: Color.Teal);
            }
        }

        public async Task InstallCommandsAsync()
        {
            // Hook the MessageReceived Event into our Command Handler
            _client.MessageReceived += HandleCommandAsync;
            // Discover all of the commands in this assembly and load them.
            await _commands.AddModulesAsync(Assembly.GetEntryAssembly());
        }

        private async Task HandleCommandAsync(SocketMessage messageParam)
        {
            // Don't process the command if it was a System Message
            var message = messageParam as SocketUserMessage;
            if (message == null) return;
            // Create a number to track where the prefix ends and the command begins
            int argPos = 0;
            // Determine if the message is a command, based on if it starts with '!' or a mention prefix
            if (!(message.HasCharPrefix('!', ref argPos) || message.HasMentionPrefix(_client.CurrentUser, ref argPos))) return;
            // Create a Command Context
            var context = new SocketCommandContext(_client, message);
            // Execute the command. (result does not indicate a return value, 
            // rather an object stating if the command executed successfully)
            var result = await _commands.ExecuteAsync(context, argPos, _services);
            if (!result.IsSuccess)
                await context.Channel.SendMessageAsync(result.ErrorReason);
        }

        private Task Log(LogMessage msg)
        {
            Console.WriteLine(msg.ToString());
            return Task.CompletedTask;
        }
    }
}
