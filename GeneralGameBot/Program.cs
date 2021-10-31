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
            client.OnMessage += async (object sender, MessageEventArgs e) =>
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
                                await client.SendPhotoAsync(chatId: msg.Chat.Id, general.PhotoUrl, caption: $"Информация про генерала\nЕго хп: {general.HP}\nЕго имя: {general.Name}\nДата создания Генерала {general.DateOfCreating} ", replyMarkup: TelegramButtons.GetButtons());
                            }
                        }
                    }
                    catch (Exception ex)
                    {

                        Console.WriteLine(ex.Message);
                    }
                }
                if (msg.Text == "О боте")
                {
                    await client.SendTextMessageAsync(chatId: msg.Chat.Id, File.ReadAllText(@"C:\GeneralGameBot\GameInformation.txt"));
                }
                try
                {
                    if (msg.Text.StartsWith("Имя:"))
                    {
                        general = GameDataBase.GetGeneral(msg.From.Username);
                        MessageHandler.ChangeName(general, msg.Text);
                        await client.SendTextMessageAsync(chatId: msg.Chat.Id, "Вы поменяли имя генералу", replyMarkup: TelegramButtons.GetButtons());
                    }
                    else if (msg.Text.StartsWith("Фото:"))
                    {
                        general = GameDataBase.GetGeneral(msg.From.Username);
                        MessageHandler.ChangePhoto(general, msg.Text);
                        await client.SendTextMessageAsync(chatId: msg.Chat.Id, "Вы поменяли фото генералу", replyMarkup: TelegramButtons.GetButtons());
                    }
                }
                catch (Exception ex)
                {

                    Console.WriteLine(ex.Message);
                }


                if (msg.Text.StartsWith("Генерал другого человека:"))
                {
                    try
                    {
                        Entities.General AnotherGeneral = MessageHandler.AnotherGeneralInfo(msg.Text);
                        await client.SendPhotoAsync(chatId: msg.Chat.Id, AnotherGeneral.PhotoUrl, caption: $"Информация про генерала\nЕго хп: {AnotherGeneral.HP}\nЕго имя: {AnotherGeneral.Name}\nДата создания Генерала: {AnotherGeneral.DateOfCreating} ", replyMarkup: TelegramButtons.GetButtons());
                    }
                    catch
                    {
                        await client.SendTextMessageAsync(chatId: msg.Chat.Id, "Генерала с данным именем не существует", replyMarkup: TelegramButtons.GetButtons());
                    }

                }

            };
            Console.ReadLine();
        }




    }
}
