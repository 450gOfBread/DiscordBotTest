using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;


namespace DiscordBotTest
{
    class Program
    {
        bool running = true;

        ulong botGuildId;

        private DiscordSocketClient _client;
        private static Dictionary<ulong, BotInstance> instances = new Dictionary<ulong, BotInstance>();
        public static CommandHandler ch;
        
        static void Main(string[] args) => new Program().MainAsync().GetAwaiter().GetResult();

        string TOKEN = "NzE4MzEzMTgxMjc3MzIzMzM0.XtnDjQ.-sRLNTYVNc48qGEI-xIjuqiaMWM";
        //string TEST_TOKEN = "NzIwMTMzNTE2MTc1NzM2ODQz.XuBipg.-yv-tKMJNo6frS7HGRrKUSz8v5A";


        public async Task MainAsync()
        {
            ch = new CommandHandler();
            _client = new DiscordSocketClient();
            _client.Log += Log;
            _client.MessageReceived += MessageRecieved;
            await _client.LoginAsync(Discord.TokenType.Bot, TOKEN);

            

            await _client.StartAsync();

            var console = new Task(() =>
            {
                while (running)
                {
                    string command = Console.ReadLine();

                    

                    if (command == "quit")
                    {
                        running = false;
                        _client.StopAsync();
                        Environment.Exit(0);
                    }
                    else
                    {
                        
                        ResolveCommand(new ConsoleMessage(command));

                    }
                }
            });

            console.Start();

            await Task.Delay(-1);

        }

        private Task Log(LogMessage msg)
        {
            Console.WriteLine(msg.ToString());
            return Task.CompletedTask;
        }

        private async Task MessageRecieved(SocketMessage message)
        {
            var guild = (message.Channel as SocketGuildChannel).Guild;
            botGuildId = guild.Id;
            
            if (!instances.ContainsKey(guild.Id))
            {
                BotInstance inst = new BotInstance(guild);
                instances.Add(guild.Id, inst);
                
            }
            if (!message.Author.IsBot)
            {
                await ResolveCommand(message, instances[guild.Id]);
            }
        }


        private async Task ResolveCommand(ConsoleMessage message)
        {
            var msg = message.Content.Split(' ');
            BotInstance instance = instances[botGuildId];
            Dictionary<string, int> rolls = instance.Rolls;
            
            switch (msg[0])
            {
                case "init":
                    try
                    {
                        string name = CommandHandler.StringFromArray(msg.Skip(1).Take(msg.Length - 2).ToArray());
                        int roll = Convert.ToInt32(msg[msg.Length - 1]);

                        if (rolls.ContainsKey(name))
                        {
                            rolls[name] = roll;
                        }
                        else
                        {
                            rolls.Add(name, roll);

                        }
                    }catch(Exception e)
                    {
                        Console.WriteLine("####################################################\n");
                        Console.WriteLine("Something went wrong with inputting initiative roll.");
                    }
                    break;

                case "order":

                    Console.WriteLine("####################################################\n");
                    Console.WriteLine(CommandHandler.GetOrder(rolls));
                    break;

                case "clear":

                    rolls.Clear();
                    Console.WriteLine("####################################################\n");
                    Console.WriteLine("Rolls cleared.");
                    break;

                case "remove":
                    try
                    {
                        rolls.Remove(CommandHandler.StringFromArray(msg.Skip(1).ToArray()));
                        Console.WriteLine("####################################################\n");
                        Console.WriteLine("Removed.");

                    }
                    catch(Exception e)
                    {
                        Console.WriteLine("####################################################\n");
                        Console.WriteLine("Could not remove name.");
                    }
                    break;

                case "channel":
                    if (msg.Length == 2)
                    {
                        ulong newId = Convert.ToUInt64(msg[1]);
                        if (instances.ContainsKey(newId)) {
                            botGuildId = newId;
                            Console.WriteLine("####################################################\n");
                            Console.WriteLine("Guild changed to: " + instances[newId].Guild.Name);

                        }
                        else
                        {
                            Console.WriteLine("####################################################\n");
                            Console.WriteLine("Channel id does not exist.");
                        }
                    }
                    Console.WriteLine("####################################################\n");
                    Console.WriteLine(instance.Guild.Name);
                    Console.WriteLine(instance.Guild.Id);
                    break;

                case "channels":
                    Console.WriteLine("####################################################\n");
                    foreach(BotInstance inst in instances.Values)
                    {
                        Console.WriteLine(inst.Guild.Name + ": " + inst.Guild.Id);
                    }
                    break;

                case "say":
                    
                    await instance.Channel.SendMessageAsync(CommandHandler.StringFromArray(msg.Skip(1).ToArray()));
                    Console.WriteLine("####################################################\n");
                    Console.WriteLine(CommandHandler.StringFromArray(msg.Skip(1).ToArray()));
                    break;

                default:
                    break;
            }
                
            Console.Write("\n");
            Console.WriteLine("####################################################");
            Console.Write("\n");
            Console.Write("\n");
        }
        private async Task ResolveCommand(SocketMessage message, BotInstance instance)
        {
            ch.Handle(message, instance);
        }

        /*
        public string StringFromArray(string[] array)
        {
            string name = "";

            for (int i = 0; i < array.Length; i++)
            {
                name += (array[i] + " ");
            }
            return name.Trim();
        }

        public string GetOrder(Dictionary<string,int> rolls)
        {
            if(rolls.Count == 0)
            {
                return "The initiative order is blank!";
            }
            var sortedRolls = rolls.OrderByDescending(x => x.Value).ToList();

            string output = "";
            int i = 1;
            foreach (KeyValuePair<string, int> p in sortedRolls)
            {
                output += i.ToString() + ". " + p.Key + ": " + p.Value.ToString() + "\n";
                i++;
            }
            return output;
        }
        */
    }

    

    

    
}
