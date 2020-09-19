using System;
using System.Collections.Generic;
using System.Text;
using Discord;
using Discord.WebSocket;

namespace DiscordBotTest
{
    class ConsoleMessage
    {
        public String Content { get; set; }
        
        public ConsoleMessage(String message)
        {
            Content = message;
        }

        public ConsoleMessage(SocketMessage message)
        {

        }

        


    }
}
