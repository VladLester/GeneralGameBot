using System;
using Telegram.Bot;
using Telegram.Bot.Args;
using System.IO;
using Telegram.Bot.Types.ReplyMarkups;
using System.Threading;

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
                    await client.SendTextMessageAsync(chatId: msg.Chat.Id, "Генералу Аналу слава!", replyToMessageId: msg.MessageId);
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
                        if (FightCall == msg.From.Username)
                        {
                            await client.SendTextMessageAsync(chatId: msg.Chat.Id, "Ты че ахуел сам с собой пиздится? ", replyToMessageId: msg.MessageId);
                        }
                        else
                        {
                            using (AppContext context = new AppContext())
                            {
                                Entities.General general1 = GameDataBase.GetGeneral(msg.From.Username);
                                Entities.General general2 = GameDataBase.GetGeneral(FightCall);
                                Entities.Stats General1Stats = context.Stats.Find(GameDataBase.GetGeneral(msg.From.Username).Id);
                                Entities.Stats General2Stats = context.Stats.Find(GameDataBase.GetGeneral(FightCall).Id);
                                int AttackGeneralDamage = 0;
                                int DefenseGeneralDamage = 0;
                                if (general2.Name == null)
                                {
                                    await client.SendTextMessageAsync(msg.Chat.Id, $"Дай имя своему генералу а потом уже дерись");

                                }
                                else
                                {
                                    await client.SendTextMessageAsync(chatId: msg.Chat.Id, "Дуэль началась");
                                    if (general1.HP >= 0 && general2.HP >= 0)
                                    {
                                        Thread.Sleep(5000);
                                        AttackGeneralDamage = FightMechanics.GeneralHit(msg.Chat.Id, general1, general2, client, General1Stats, General2Stats);
                                        Thread.Sleep(5000);
                                        DefenseGeneralDamage = FightMechanics.GeneralHit(msg.Chat.Id, general2, general1, client, General2Stats, General1Stats);
                                        if (AttackGeneralDamage > DefenseGeneralDamage)
                                        {
                                            await client.SendTextMessageAsync(msg.Chat.Id, $"Победа генерала {general1.Name}, + 3 exp");
                                            context.Update(general1);
                                            general1.Exp += 3;
                                            context.SaveChanges();
                                        }
                                        else if (AttackGeneralDamage < DefenseGeneralDamage)
                                        {
                                            await client.SendTextMessageAsync(msg.Chat.Id, $"Победа генерала {general2.Name}, + 3 exp");
                                            context.Update(general2);
                                            general2.Exp += 3;
                                            context.SaveChanges();
                                        }
                                        else
                                        {
                                            await client.SendTextMessageAsync(msg.Chat.Id, $"Ничья, все получают по 1 exp");
                                            context.Update(general1);
                                            context.Update(general2);
                                            general1.Exp += 1;
                                            general2.Exp += 1;
                                            context.SaveChanges();
                                        }
                                    }

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


                #region Stats 
                try
                {

                    if (msg.Text == "Навыки")
                    {
                        using (AppContext db = new AppContext())
                        {
                            Entities.Stats stats = db.Stats.Find(GameDataBase.GetGeneral(msg.From.Username).Id);
                            general = GameDataBase.GetGeneral(msg.From.Username);
                            await client.SendTextMessageAsync(chatId: msg.Chat.Id, $"{stats.Stamina} - Стамина Генерала\n{stats.Strength} - Сила Генерала\n{stats.Tactics} - Тактика Генерала,\nВаша Експа {general.Exp}", replyMarkup: TelegramButtons.StatsButtons());

                        }
                    }
                    else if (msg.Text == "Назад")
                    {
                        await client.SendTextMessageAsync(chatId: msg.Chat.Id, "Возвращение на начальный экран", replyMarkup: TelegramButtons.GetButtons());
                    }
                    using (AppContext db = new AppContext())
                    {
                        switch (msg.Text)
                        {

                            case "Качнуть силу":
                                Entities.Stats stats = db.Stats.Find(GameDataBase.GetGeneral(msg.From.Username).Id);
                                general = GameDataBase.GetGeneral(msg.From.Username);
                                GameDataBase.StatsIncrement(msg.Chat.Id, client, general, stats, "Strength");
                                break;
                            case "Качнуть Тактику":
                                Entities.Stats stats1 = db.Stats.Find(GameDataBase.GetGeneral(msg.From.Username).Id);
                                general = GameDataBase.GetGeneral(msg.From.Username);
                                GameDataBase.StatsIncrement(msg.Chat.Id, client, general, stats1, "Tactics");
                                break;
                            case "Качнуть Стамину":
                                Entities.Stats stats2 = db.Stats.Find(GameDataBase.GetGeneral(msg.From.Username).Id);
                                general = GameDataBase.GetGeneral(msg.From.Username);
                                GameDataBase.StatsIncrement(msg.Chat.Id, client, general, stats2, "Stamina");
                                break;


                        }
                    }
                }
                catch (Exception ex)
                {

                    Console.WriteLine(ex.Message);
                }
                #endregion

            };        
         Console.ReadLine();
        }





    }
}
