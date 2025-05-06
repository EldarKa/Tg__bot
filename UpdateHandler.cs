using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Types.Enums;

namespace Tg__bot
{
    public class UpdateHandler : IUpdateHandler
    {

        public async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            if (update.Message is { Text: var text } message)
            {
                Console.WriteLine($"Получено сообщение: {text}");

                //await botClient.SendTextMessageAsync(
                //    chatId: message.Chat.Id,
                //    text: $"Вы написали: {text}",
                //    cancellationToken: cancellationToken
                //);
            }
        }

        public Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
        {
            var errorMessage = exception switch
            {
                ApiRequestException apiEx => $"Ошибка Telegram API: {apiEx.ErrorCode} — {apiEx.Message}",
                _ => exception.ToString()
            };

            Console.WriteLine(errorMessage);
            return Task.CompletedTask;
        }
    }
}
