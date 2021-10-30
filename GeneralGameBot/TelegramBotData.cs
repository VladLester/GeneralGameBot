using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using System.IO;

namespace GeneralGameBot
{
    static class TelegramBotData
    {
        public static readonly string token = File.ReadAllText(@"C:\GeneralGameBot\BotInfo.txt");


        

    }
}
