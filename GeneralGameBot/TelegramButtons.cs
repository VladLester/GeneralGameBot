using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types.ReplyMarkups;

namespace GeneralGameBot
{
    static class TelegramButtons
    {
        public static IReplyMarkup GetButtons()
        {
            return new ReplyKeyboardMarkup
            {
                Keyboard = new List<List<KeyboardButton>>
                {
                    new List<KeyboardButton> {new KeyboardButton { Text = "Информация про генерала" }, new KeyboardButton{ Text = "О боте" }, new KeyboardButton { Text = "Навыки"} }
                }
            };
        }
        public static IReplyMarkup StatsButtons()
        {
            return new ReplyKeyboardMarkup
            {
                Keyboard = new List<List<KeyboardButton>>
                {
                    new List<KeyboardButton> {new KeyboardButton { Text = "Качнуть силу" }, new KeyboardButton { Text = "Назад"} }
                }
            };
        }
    }
}
