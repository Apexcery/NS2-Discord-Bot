using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;

namespace NS2_Discord_Bot
{
    internal class Program
    {
        public static void Main(string[] args)
            => new Program().MainAsync().GetAwaiter().GetResult();

        private DiscordSocketClient _client;
        private CommandService _commands;
        private IServiceProvider _services;

        public async Task MainAsync()
        {
            _client = new DiscordSocketClient();
            _client.Log += Log;

            _commands = new CommandService();

            _services = new ServiceCollection()
                .AddSingleton(_client)
                .AddSingleton(_commands)
                .BuildServiceProvider();

            var token = Environment.GetEnvironmentVariable("token");

            await RegisterCommandsAsync();

            await _client.LoginAsync(TokenType.Bot, token);
            await _client.StartAsync();

            await Task.Delay(-1);
        }

        private Task Log(LogMessage msg)
        {
            Console.WriteLine(msg.ToString());
            return Task.CompletedTask;
        }

        public async Task RegisterCommandsAsync()
        {
            _client.MessageReceived += HandleCommandAsync;
            await _commands.AddModulesAsync(Assembly.GetEntryAssembly(), _services);
        }

        private async Task HandleCommandAsync(SocketMessage arg)
        {
            var message = arg as SocketUserMessage;
            var context = new SocketCommandContext(_client, message);

            if (message?.Author == null || message.Author.IsBot) return;

            if (context.IsPrivate && message.Channel is SocketDMChannel)
            {
                var idiots = new[]
                {
                    (ulong) 97513010746097664, //izaac
                    (ulong) 127855504440557568, //wiggy
                    (ulong) 131529962908942336 //stuart
                };

                if (idiots.Contains(message.Author.Id))
                {
                    await context.Channel.SendMessageAsync("ur an idiot lol");
                    return;
                }
            }

            var argPos = 0;

            var prefix = Environment.GetEnvironmentVariable("prefix");

            if (message.HasStringPrefix(prefix, ref argPos))
            {
                var result = await _commands.ExecuteAsync(context, argPos, _services);
                if (!result.IsSuccess) Console.WriteLine(result.ErrorReason);
            }
        }
    }
}
