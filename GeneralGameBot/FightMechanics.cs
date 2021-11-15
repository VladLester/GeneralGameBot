using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using System.Threading;

namespace GeneralGameBot
{
    static class FightMechanics
    {
        private static int DamageCalculate(int Strength, int Tactics)
        {
            Random rdm = new Random();
            int Damage = 0;
            int StrMultiplier = 2;
            if (rdm.Next(0, 2) == 1)
            {
                return Damage = Strength * StrMultiplier + Strength * Tactics;
            }
            else
            {
                return Damage = Strength * StrMultiplier;
            }


        }

        public  static int GeneralHit(long chatId, Entities.General AttackGeneral, Entities.General DefenceGeneral, TelegramBotClient client, Entities.Stats AttackGeneralStats, Entities.Stats DefenceGeneralStats)
        {
            using (AppContext context = new AppContext())
            {
                Random rdm = new Random();
                int Damage = 0;
                Thread.Sleep(5000);
                client.SendTextMessageAsync(chatId: chatId, $"Удар генерала {AttackGeneral.Name}");
                Thread.Sleep(5000);
                Damage = FightMechanics.DamageCalculate(AttackGeneralStats.Strength, DefenceGeneralStats.Tactics);
                client.SendTextMessageAsync(chatId: chatId, $"Генерал {AttackGeneral.Name} попадает и наносит {Damage}");
                context.Update(AttackGeneral);
                context.Update(DefenceGeneral);
                DefenceGeneral.HP -= Damage;
                context.SaveChanges();
                return Damage;
            }
        }

        public static bool FightTurn(int AttackGeneralDice, int DefenceGeneralDice)
        {
            if (AttackGeneralDice > DefenceGeneralDice)
            {
                return true;
            }
            else if(AttackGeneralDice < DefenceGeneralDice)
            {
                return false;
            }
            return false;
            
        }
        
    }
}
