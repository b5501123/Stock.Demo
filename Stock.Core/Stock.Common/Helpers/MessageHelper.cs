using System.Text;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace Stock.Common.Helpers
{
    public class MessageHelper //MS950
    {
        public static async Task SendMessageAsync(long chatID,string message)
        {
            var botClient = new TelegramBotClient(ConfigHelper.TelegramBotApi);

            await botClient.SendTextMessageAsync(chatId: chatID, message);
        }

        public static async Task SendMessageAsync(List<long> chatID, string message)
        {

            var botClient = new TelegramBotClient(ConfigHelper.TelegramBotApi);

            foreach(var chat in chatID)
            {
               await botClient.SendTextMessageAsync(chatId: chat, message);
            }
        }

    }
}
