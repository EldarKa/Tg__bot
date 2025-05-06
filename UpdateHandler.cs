using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Types.Enums;
using static Telegram.Bot.TelegramBotClient;
using System.Net.Http.Json;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Text;

namespace Tg__bot
{
    public delegate void MessageHandler(string message);
    public class UpdateHandler : IUpdateHandler
    {
        public event MessageHandler? OnHandleUpdateStarted;
        public event MessageHandler? OnHandleUpdateCompleted;
        public Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, HandleErrorSource source, CancellationToken cancellationToken)
        {
            Console.WriteLine($"Error from {source}: {exception.Message}");
            return Task.CompletedTask;
        }

        public async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            if (update.Type != UpdateType.Message || update.Message is not { Text: var text } message)
                return;

            OnHandleUpdateStarted?.Invoke(text);

            if (text == "/cat")
            {
                using var client = new HttpClient();
                try
                {
                    var catFact = await client.GetFromJsonAsync<CatFactDto>("https://catfact.ninja/fact", cancellationToken);

                    if (catFact is not null)
                    {
                        string englishFact = catFact.Fact;
                        string russianFact = await TranslateToRussian(englishFact);
                        await botClient.SendMessage(
                            chatId: message.Chat.Id,
                            text: $"🐱 Факт о кошках:\n{russianFact}",
                            cancellationToken: cancellationToken
                        );
                    }
                }
                catch (Exception ex)
                {
                    await botClient.SendMessage(
                        chatId: message.Chat.Id,
                        text: $"Произошла ошибка при получении факта о кошках: {ex.Message}",
                        cancellationToken: cancellationToken
                    );
                }
            }
            else
            {
                await botClient.SendMessage(
                    chatId: message.Chat.Id,
                    text: "Сообщение успешно принято, введи /cat и узнаешь интересный факт",
                    cancellationToken: cancellationToken
                );
            }

            OnHandleUpdateCompleted?.Invoke(text);
        }
        async Task<string> TranslateToRussian(string text)
        {
            using var client = new HttpClient();

            var content = new FormUrlEncodedContent(new Dictionary<string, string>
                {
                    { "auth_key", BotConfig.DeepLApiKey },
                    { "text", text },
                    { "target_lang", "RU" }
                });

            var response = await client.PostAsync(BotConfig.DeepLApiUrl, content);
            response.EnsureSuccessStatusCode();

            var resultJson = await response.Content.ReadFromJsonAsync<DeepLResponse>();
            return resultJson?.Translations?.FirstOrDefault()?.Text ?? "[ошибка перевода]";
        }
    }
}
