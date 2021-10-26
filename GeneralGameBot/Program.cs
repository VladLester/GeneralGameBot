using System;
using Telegram.Bot;
using Telegram.Bot.Args;
using System.IO;

namespace GeneralGameBot
{
    class Program
    {
        public static TelegramBotClient client { get; private set; }
       
        static void Main(string[] args)
        {
            client = new TelegramBotClient(TelegramBotData.token);
            client.StartReceiving();
            client.OnMessage += async (object sender, MessageEventArgs e) =>
            {
                var msg = e.Message;

                if (msg.Text != "Информация про генерала" && msg.Text != "Правила генеральской битвы")
                {
                    try
                    {

                        await client.SendTextMessageAsync(chatId: msg.Chat.Id, File.ReadAllText(@"C:\Users\xlegolazxx\Desktop\general.txt"));
                    }
                    catch (Exception exc)
                    {

                        Console.WriteLine(exc.Message);
                    }
                }

            };
            Console.ReadLine();          
        }

       
    }
}
