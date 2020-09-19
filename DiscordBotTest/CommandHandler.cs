using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace DiscordBotTest
{
    class CommandHandler
    {
        private static Dictionary<string, MethodInfo> commands = new Dictionary<string, MethodInfo>();
        private Commands cons = new Commands();
        public CommandHandler()
        {
            Type type = typeof(Commands);
            
            
            MemberInfo[] members = type.GetMethods();

            foreach(MethodInfo info in members)
            {
                Attribute[] attrs = Attribute.GetCustomAttributes(info);

                foreach(Attribute attr in attrs)

                if (attr is CommandAttribute) {

                    commands.Add(((CommandAttribute)attr).GetName(), info);

                }
            }
            
        }
        public void Handle(SocketMessage message, BotInstance instance)
        {

            var msg = message.Content.Split(' ');

            msg[0] = msg[0].Substring(1, msg[0].Length - 1);

            commands[msg[0]].Invoke(cons, new object[] { message, instance });

        }

        public static string StringFromArray(string[] array)
        {
            string name = "";

            for (int i = 0; i < array.Length; i++)
            {
                name += (array[i] + " ");
            }
            return name.Trim();
        }
        public static string GetOrder(Dictionary<string, int> rolls)
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



    [System.AttributeUsage(System.AttributeTargets.Method)]
    public class CommandAttribute : Attribute
    {
        private string name;

        public CommandAttribute(string name)
        {
            this.name = name;

        }

        public string GetName()
        {
            return name;
        }

        
    }

}
