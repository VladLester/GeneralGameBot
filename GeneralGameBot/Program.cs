using System;
using Telegram.Bot;
using Telegram.Bot.Args;
using System.IO;
using Telegram.Bot.Types.ReplyMarkups;
using System.Threading;
using System.Threading.Tasks;

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
            Task.Run(() => GeneralAnthem());
            
            client.OnMessage += async (object sender, MessageEventArgs e) =>
            {
                
                var msg = e.Message;
                Task.Run(() => HpRestoration(GameDataBase.GetGeneral(msg.From.Username)));
                string FightCall = "";
                if (msg.Text == "/start")
                {

                    await client.SendTextMessageAsync(chatId: msg.Chat.Id, File.ReadAllText(@"C:\GeneralGameBot\GeneralGameStartMessage.txt"), replyMarkup: TelegramButtons.GetButtons());
                }
                else if (msg.Text == "Похоронить")
                {
                    var DeathGeneral = GameDataBase.GetGeneral(msg.From.Username);
                    if (DeathGeneral.HP <= 0)
                    {
                        using (AppContext context = new AppContext())
                        {
                            await client.SendPhotoAsync(chatId: msg.Chat.Id, "https://memepedia.ru/wp-content/uploads/2019/09/7bf9f32d.jpg", "Ваш генерал был так молод и красив 😭");
                            await client.SendAudioAsync(chatId: msg.Chat.Id, "https://rbmk.xyz/GeneralYmer.mp3");
                            await client.SendTextMessageAsync(msg.Chat.Id, "Минута молчания....");
                            Thread.Sleep(5000);
                            context.Remove(DeathGeneral);
                            context.SaveChanges();
                            await client.SendPhotoAsync(chatId: msg.Chat.Id, "https://cdn.discordapp.com/attachments/566347121364828160/909766983044698163/b-6-t.jpg", "Ваш генерал был похоронен с почестями 😭");

                        }
                    }
                    else
                    {
                        await client.SendTextMessageAsync(msg.Chat.Id, "Ты че ебанашка, твой генерал еще в расцвете сил! ", replyToMessageId: msg.MessageId);
                    }
                    
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

                                GameDataBase.DataBaseAdd(GameDataBase.GeneralCreate(msg?.From.Username, msg.Chat.Id));
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
                                if (general2.Name == null || general1.Name == null)
                                {
                                    await client.SendTextMessageAsync(msg.Chat.Id, $"У кого-то из дуэлянтов нету имени, проверьте и дайте имя генералу");

                                }
                                else
                                {
                                    if (DateTime.Now >= general1.CoolDown.AddMinutes(10) && DateTime.Now >= general2.CoolDown.AddMinutes(10))
                                    {
                                        await client.SendTextMessageAsync(chatId: msg.Chat.Id, "Дуэль началась");
                                        await client.SendTextMessageAsync(chatId: general2.ChatID, $"На вас напал {general1.Name}");
                                        if (general1.HP >= 0 && general2.HP >= 0)
                                        {
                                            Thread.Sleep(2000);
                                            AttackGeneralDamage = FightMechanics.GeneralHit(msg.Chat.Id, general1, general2, client, General1Stats, General2Stats);
                                            Thread.Sleep(2000);
                                            DefenseGeneralDamage = FightMechanics.GeneralHit(msg.Chat.Id, general2, general1, client, General2Stats, General1Stats);
                                            if (AttackGeneralDamage > DefenseGeneralDamage)
                                            {
                                                await client.SendTextMessageAsync(msg.Chat.Id, $"Победа генерала {general1.Name}, + 3 exp");
                                                await client.SendTextMessageAsync(general2.ChatID, $"Победа генерала {general1.Name}, + 3 exp");
                                                context.Update(general1);
                                                general1.Exp += 3;
                                                context.SaveChanges();
                                            }
                                            else if (AttackGeneralDamage < DefenseGeneralDamage)
                                            {
                                                await client.SendTextMessageAsync(msg.Chat.Id, $"Победа генерала {general2.Name}, + 3 exp");
                                                await client.SendTextMessageAsync(general2.ChatID, $"Победа генерала {general2.Name}, + 3 exp");
                                                context.Update(general2);
                                                general2.Exp += 3;
                                                context.SaveChanges();
                                            }
                                            else
                                            {
                                                await client.SendTextMessageAsync(msg.Chat.Id, $"Ничья, все получают по 1 exp");
                                                await client.SendTextMessageAsync(general2.ChatID, $"Ничья, все получают по 1 exp");
                                                context.Update(general1);
                                                context.Update(general2);
                                                general1.Exp += 1;
                                                general2.Exp += 1;
                                                context.SaveChanges();
                                            }
                                            context.Update(general1);
                                            context.Update(general2);
                                            general1.CoolDown = DateTime.Now;
                                            general2.CoolDown = DateTime.Now;
                                            context.SaveChanges();
                                        }
                                        else
                                        {
                                            await client.SendTextMessageAsync(msg.Chat.Id, $"У кого-то из вас мертвый генерал");
                                        }


                                    }
                                    else
                                    {
                                        await client.SendTextMessageAsync(msg.Chat.Id, $"Генерал устал ему нужно отдохнуть, кд закончится в {general1.CoolDown.AddMinutes(10)}");
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
                            await client.SendTextMessageAsync(chatId: msg.Chat.Id, $"💪🏿{stats.Stamina} - Стамина Генерала\n👊🏿{stats.Strength} - Сила Генерала\n👺{stats.Tactics} - Тактика Генерала,\nВаша Експа {general.Exp}", replyMarkup: TelegramButtons.StatsButtons());

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

        static async void GeneralAnthem()
        {
            bool isTime = true;
            while (true)
            {
                if (DateTime.Now.Hour == 8 || DateTime.Now.Hour == 21)
                {
                    if (isTime == true)
                    {
                        using (AppContext context = new AppContext())
                        {
                            foreach (var item in context.Generals)
                            {
                                try
                                {
                                    await client.SendTextMessageAsync(item.ChatID, File.ReadAllText(@"C:\GeneralGameBot\GeneralAnthem.txt"));
                                }
                                catch (Exception ex)
                                {

                                    Console.WriteLine(ex.Message);
                                }
                            }
                        }
                        isTime = false;
                    }
                   
                }
                else
                {
                    isTime = true;
                }
                Thread.Sleep(10000);

            }
            
        }

        static async void HpRestoration(Entities.General general)
        {
            try
            {
                using (AppContext context = new AppContext())
                {
                    while (true)
                    {
                        if (DateTime.Now >= general.HpRestorationTime.AddMinutes(20))
                        {
                            if (general.HP != general.maxHpAmount)
                            {
                                context.Update(general);
                                general.HP += general.maxHpAmount - general.HP;
                                general.HpRestorationTime = DateTime.Now;
                                context.SaveChanges();
                            }
                        }


                        Thread.Sleep(10000);
                    }
                }
               
            }
            catch (Exception ex)
            {

                Console.WriteLine(ex.Message);
            }
           
           
        }


    }
}
