using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;

namespace GeneralGameBot
{
    class GameDataBase
    {
        public static bool DataBaseContains(string username)
        {

            using (AppContext context = new AppContext())
            {
                foreach (var item in context.Generals)
                {
                    if (item.TUsername == username)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public static void DataBaseAdd(Entities.General general)
        {
            try
            {
                using (AppContext context = new AppContext())
                {
                    context.Add(general);
                    context.SaveChanges();
                }
            }
            catch (Exception ex)
            {

                Console.WriteLine(ex.Message);
            }
            
        }

        public static Entities.General GeneralCreate(string username,long chatId)
        {
            
            
            Entities.General general = new Entities.General() { TUsername = username, Stats = new Entities.Stats { Stamina = 1, Strength = 1,Tactics =1 }, ChatID = chatId};
            return general;


        }

        public static Entities.General GetGeneral(string username)
        {
            using (AppContext context = new AppContext())
            {
                
                foreach (var item in context.Generals)
                {
                    if (item.TUsername == username)
                    {
                        return item;
                    }
                }
            }

            return null;
            
        }

        public static void StatsIncrement(long chatId, TelegramBotClient client, Entities.General general, Entities.Stats GeneralStats, string UpdateStat)
        {
            if (general.Exp != 0)
            {
                using (AppContext db = new AppContext())
                {
                    db.Update(general);
                    db.Update(GeneralStats);
                    general.Exp -= 1;
                    db.SaveChanges();
                    if (UpdateStat == "Strength")
                    {
                        GeneralStats.Strength += 1;
                        client.SendTextMessageAsync(chatId, "Вы прокачали Strength");
                        db.SaveChanges();
                    }
                    else if (UpdateStat == "Stamina")
                    {
                        GeneralStats.Stamina += 1;
                        general.maxHpAmount += 5;
                        client.SendTextMessageAsync(chatId, "Вы прокачали Stamina");
                        db.SaveChanges();
                    }
                    else if(UpdateStat == "Tactics")
                    {
                        GeneralStats.Tactics += 1;
                        client.SendTextMessageAsync(chatId, "Вы прокачали Tactics");
                        db.SaveChanges();
                    }
                    
                }
            }
            else
            {
                client.SendTextMessageAsync(chatId: chatId, "У вас нету опыта");
            }
        }



    }
}
