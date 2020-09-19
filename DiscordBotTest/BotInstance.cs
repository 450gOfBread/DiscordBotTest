using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Text;

namespace DiscordBotTest
{
    public class BotInstance
    {
        public BotInstance(SocketGuild Guild)
        {

            this.Guild = Guild;

            Rolls = new Dictionary<string, int>();
            ChannelOrderIds = new Dictionary<ulong, ulong>();

            OrderId = 0;
        }

        public SocketGuild Guild { get; set; }
        public ISocketMessageChannel Channel { get; set; }

        public Dictionary<string, int> Rolls { get; set; }

        public Dictionary<ulong, ulong> ChannelOrderIds { get; set; }
        public ulong OrderId { get; set; }

    }
}
