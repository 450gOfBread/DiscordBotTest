using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordBotTest
{
    class Commands
    {
        [Command("init")]
        public async Task InitCommand(SocketMessage message, BotInstance instance)
        {
            Dictionary<string, int> rolls = instance.Rolls;
            var msg = message.Content.Split(' ');

            try
            {
                if (msg.Length == 2)
                {

                    var chnl = message.Channel as SocketGuildChannel;

                    string name = chnl.GetUser(message.Author.Id).Nickname;
                    if (name == null)
                    {
                        name = message.Author.Username;
                    }
                    int roll = Convert.ToInt32(msg[1]);

                    if (rolls.ContainsKey(name))
                    {
                        rolls[name] = roll;
                    }
                    else
                    {
                        rolls.Add(name, roll);

                    }
                    await message.Channel.SendMessageAsync(name + ": " + roll);
                }
                else
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
                    await message.Channel.SendMessageAsync(name + ": " + roll);
                }
            }
            catch (Exception e)
            {
                await message.Channel.SendMessageAsync("!init [roll]\n!init [name] [roll]");
                Console.WriteLine(e.Message);
            }
        }

        [Command("order")]
        public async Task OrderCommand(SocketMessage message, BotInstance instance)
        {
            Dictionary<string,int> rolls = instance.Rolls;

            if (rolls.Count > 0)
            {


                if (rolls.Count != 0)
                {
                    if (instance.ChannelOrderIds.ContainsKey(message.Channel.Id))
                    {
                        await message.Channel.DeleteMessageAsync(instance.ChannelOrderIds[message.Channel.Id]);
                    }

                    var orderMessage = await message.Channel.SendMessageAsync(CommandHandler.GetOrder(rolls));
                    instance.ChannelOrderIds[message.Channel.Id] = orderMessage.Id;
                    Console.WriteLine(instance.ChannelOrderIds[message.Channel.Id] + ": " + orderMessage.Id);

                }
            }
            else
            {
                await message.Channel.SendMessageAsync("There are no stored rolls.");
            }
        }

        [Command("clear")]
        public async Task ClearCommand(SocketMessage message, BotInstance instance)
        {
            Dictionary<string, int> rolls = instance.Rolls;
            rolls.Clear();
            await message.Channel.SendMessageAsync("Initiative order cleared!");
        }

        [Command("r")]
        public async Task RemoveCommand(SocketMessage message, BotInstance instance)
        {
            var msg = message.Content.Split(' ');
            Dictionary<string, int> rolls = instance.Rolls;
            
            if (rolls.Count != 0)
            {
                string name = StringFromArray(msg.Skip(1).ToArray());
                if (!rolls.Remove(name))
                {

                    await message.Channel.SendMessageAsync("Name not found!");

                }
                else
                {
                    await message.Channel.SendMessageAsync("Removed " + name + "!");

                    if (instance.ChannelOrderIds.ContainsKey(message.Channel.Id))
                    {
                        await message.Channel.DeleteMessageAsync(instance.ChannelOrderIds[message.Channel.Id]);
                    }

                    if (rolls.Count != 0)
                    {
                        instance.ChannelOrderIds[message.Channel.Id] = (await message.Channel.SendMessageAsync(GetOrder(rolls))).Id;
                    }
                    else
                    {
                        await message.Channel.SendMessageAsync("Initiative order is now blank!");
                    }
                }

            }
            else
            {
                await message.Channel.SendMessageAsync("Initiative order is empty!");
            }
        }

        private static string StringFromArray(string[] array)
        {
            string name = "";

            for (int i = 0; i < array.Length; i++)
            {
                name += (array[i] + " ");
            }
            return name.Trim();
        }
        private static string GetOrder(Dictionary<string, int> rolls)
        {
            if (rolls.Count == 0)
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
    }
}
