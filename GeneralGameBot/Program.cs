using System;
using Telegram.Bot;
using Telegram.Bot.Args;
using System.IO;
using Telegram.Bot.Types.ReplyMarkups;

namespace GeneralGameBot
{
    class Program
    {
        public static TelegramBotClient client { get; private set; }
        private static Entities.General general;


        static void Main(string[] args)
        {
            client = new TelegramBotClient(TelegramBotData.token);
            client.StartReceiving();
            client.OnMessage += async(object sender, MessageEventArgs e) => 
            {
                var msg = e.Message;
               
                if (msg.Text == "/start")
                {

                    await client.SendTextMessageAsync(chatId: msg.Chat.Id, File.ReadAllText(@"C:\GeneralGameBot\GeneralGameStartMessage.txt"), replyMarkup: TelegramButtons.GetButtons());
                }
                if (msg.Text == "Информация про генерала")
                {
                    try
                    {
                        using (AppContext context = new AppContext())
                        {
                            if (GameDataBase.DataBaseContains(msg.From.Username) == false)
                            {

                                GameDataBase.DataBaseAdd(GameDataBase.GeneralCreate(msg?.From.Username));                             
                                await client.SendPhotoAsync(chatId: msg.Chat.Id, MessageHandler.DefaultGeneralPhotoUrl, caption: "Это ваш первый генерал\nЕго хп: 100", replyMarkup: TelegramButtons.GetButtons());
                                
                            }
                            else 
                            {
                                general = GameDataBase.GetGeneral(msg.From.Username);
                                await client.SendPhotoAsync(chatId: msg.Chat.Id, general.PhotoUrl, caption: $"У вас уже есть генерал\nЕго хп: {general.HP}", replyMarkup: TelegramButtons.GetButtons());
                            }
                        }
                    }
                    catch (Exception ex)
                    {

                        Console.WriteLine(ex.Message);
                    }
                }
                
            };
            Console.ReadLine();
        }



        
    }
}
