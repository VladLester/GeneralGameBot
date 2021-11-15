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

        public static int GeneralHit(long chatId, Entities.General AttackGeneral, Entities.General DefenceGeneral, TelegramBotClient client, Entities.Stats AttackGeneralStats, Entities.Stats DefenceGeneralStats)
        {
            using (AppContext context = new AppContext())
            {
                Random rdm = new Random();
                int Damage = 0;
                Thread.Sleep(2000);
                client.SendTextMessageAsync(chatId: AttackGeneral.ChatID, $"Удар генерала {AttackGeneral.Name}");
                client.SendTextMessageAsync(chatId: DefenceGeneral.ChatID, $"Удар генерала {AttackGeneral.Name}");
                Thread.Sleep(2000);
                Damage = FightMechanics.DamageCalculate(AttackGeneralStats.Strength, DefenceGeneralStats.Tactics);
                client.SendTextMessageAsync(chatId: AttackGeneral.ChatID, $"Генерал {AttackGeneral.Name} попадает и наносит {Damage}");
                client.SendTextMessageAsync(chatId: DefenceGeneral.ChatID, $"Генерал {AttackGeneral.Name} попадает и наносит {Damage}");
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
