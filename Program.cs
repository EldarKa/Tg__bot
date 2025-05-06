using System.Threading;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Tg__bot;

namespace Tg__bot
{
    record CatFactDto(string Fact, int length);
    internal class Program
    {
        static async Task Main(string[] args)
        {
            var botClient = new TelegramBotClient(BotConfig.TelegramToken);
            var receiverOptions = new ReceiverOptions
            {
                AllowedUpdates = [UpdateType.Message],
                DropPendingUpdates = true
            };

            using var cts = new CancellationTokenSource();



            var handler = new UpdateHandler();

            MessageHandler startedHandler = (message) => Console.WriteLine($"Началась обработка сообщения '{message}'");
            MessageHandler completedHandler = (message) => Console.WriteLine($"Закончилась обработка сообщения '{message}'");

            handler.OnHandleUpdateStarted += startedHandler;
            handler.OnHandleUpdateCompleted += completedHandler;

            await botClient.DeleteWebhook(cancellationToken: cts.Token);
            var me = await botClient.GetMe();
            Console.WriteLine($"Бот запущен: @{me.Username}");

            await botClient.SetMyCommands(new[]
            {
                new BotCommand { Command = "cat", Description = "Прислать случайный факт о кошках" }
            });

            botClient.StartReceiving(
                handler.HandleUpdateAsync,
                handler.HandleErrorAsync,
                receiverOptions,
                cancellationToken: cts.Token
            );

            try
            {
                Console.WriteLine("Нажмите клавишу A для выхода");
                var key = Console.ReadKey(intercept: true).Key;

                if (key == ConsoleKey.A)
                {
                    cts.Cancel();
                    Console.WriteLine("\nОстановка бота...");
                }
                else
                {
                    me = await botClient.GetMe();
                    Console.WriteLine($"\nИнформация о боте: @{me.Username}, id: {me.Id}");
                }
            }
            finally
            {
                handler.OnHandleUpdateStarted -= startedHandler;
                handler.OnHandleUpdateCompleted -= completedHandler;
            }

            Console.ReadLine();
            cts.Cancel();
        }
    }
}
