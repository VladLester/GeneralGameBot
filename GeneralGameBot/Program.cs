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
                string FightCall = "";

                if (msg.Text == "/start")
                {

                    await client.SendTextMessageAsync(chatId: msg.Chat.Id, File.ReadAllText(@"C:\GeneralGameBot\GeneralGameStartMessage.txt"), replyMarkup: TelegramButtons.GetButtons());
                }
                #region InformationAboutGeneral
                if (msg.Text == "Информация про генерала")
                {
                    try
                    {
                        using (AppContext context = new AppContext())
                        {
                            if (GameDataBase.DataBaseContains(msg.From.Username) == false)
                            {

                                GameDataBase.DataBaseAdd(GameDataBase.GeneralCreate(msg?.From.Username));
                                await client.SendPhotoAsync(chatId: msg.Chat.Id, MessageHandler.DefaultGeneralPhotoUrl, caption: $"Это ваш первый генерал {msg.From.Username}\nЕго хп: 100", replyMarkup: TelegramButtons.GetButtons());

                            }
                            else
                            {
                                general = GameDataBase.GetGeneral(msg.From.Username);
                                await client.SendPhotoAsync(chatId: msg.Chat.Id, general.PhotoUrl, caption: $"Информация про генерала {msg.From.Username}\nЕго хп: {general.HP}\nЕго имя: {general.Name}\nДата создания Генерала: {general.DateOfCreating} ", replyMarkup: TelegramButtons.GetButtons());
                            }
                        }
                    }
                    catch (Exception ex)
                    {

                        Console.WriteLine(ex.Message);
                    }
                }
                #endregion
                
                
                if (msg.Text == "О боте")
                {
                    await client.SendTextMessageAsync(chatId: msg.Chat.Id, File.ReadAllText(@"C:\GeneralGameBot\GameInformation.txt"));
                }
                else if (msg.Text == "Слава Аналу!")
                {
                    await client.SendTextMessageAsync(chatId: msg.Chat.Id, "Генералу Аналу слава!", replyToMessageId:msg.MessageId);
                }
                #region Change GeneralName and Photo
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
                #endregion
                
                #region AnotherGeneral
                try
                {
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
                }
                catch (Exception ex)
                {

                    Console.WriteLine(ex.Message);
                }
                #endregion
                
                #region Duel
                try
                {
                    if (msg.Text.StartsWith("Дуэль:"))
                    {
                        
                        FightCall = msg.Text.Replace("Дуэль:", "");
                        using (AppContext context = new AppContext())
                        {
                            Entities.General general1 = GameDataBase.GetGeneral(msg.From.Username);
                            Entities.General general2 = GameDataBase.GetGeneral(FightCall);
                            if (general2.Name == null)
                            {
                                await client.SendTextMessageAsync(msg.Chat.Id, $"Дай имя своему генералу а потом уже дерись");
                                
                            }
                            else
                            {
                                Random rdm = new Random();
                                int FirstGeneralDice = rdm.Next(0, 6);
                                int SecondGeneralDice = rdm.Next(0, 6);

                                await client.SendTextMessageAsync(msg.Chat.Id, $"Генерал с именем {general1.Name} кидает кости и получает {FirstGeneralDice}");
                                
                                await client.SendTextMessageAsync(msg.Chat.Id, $"Генерал с именем {general2.Name} кидает кости и получает {SecondGeneralDice}");

                                if (FirstGeneralDice < SecondGeneralDice)
                                {
                                    await client.SendTextMessageAsync(msg.Chat.Id, $"Победил генерал {general2.Name}, первый генерал получает минус одно очко здоровья");
                                    context.Update(general1);
                                    general1.HP -= 1;
                                    context.SaveChanges();
                                }
                                else if (FirstGeneralDice == SecondGeneralDice)
                                {
                                    await client.SendTextMessageAsync(msg.Chat.Id, $"Ничья ебать, всем похуй");
                                }
                                else
                                {
                                    await client.SendTextMessageAsync(msg.Chat.Id, $"Победил генерал {general1.Name}, второй генерал получает минус одно очко здоровья");
                                    context.Update(general2);
                                    general2.HP -= 1;
                                    context.SaveChanges();
                                }
                            }
                            
                        }
                    }
                }
                catch (Exception ex)
                {

                    await client.SendTextMessageAsync(msg.Chat.Id, "Такого генерала не существует", replyToMessageId: msg.MessageId);
                }
                #endregion
                try
                {
                    if (msg.Text == "Навыки")
                    {
                        general = GameDataBase.GetGeneral(msg.From.Username);
                        await client.SendTextMessageAsync(chatId: msg.Chat.Id, $"{general.Stats.Stamina} - Стамина Генерала\n{general.Stats.Strength} - Сила Генерала\n{general.Stats.Tactics} - Тактика Генерала", replyMarkup: TelegramButtons.StatsButtons());
                    }

                }
                catch (Exception ex)
                {

                    Console.WriteLine(ex.Message);
                }
                
            };        
         Console.ReadLine();
        }




    }
}
