using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Tg__bot;

namespace Tg__bot
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            var botClient = new TelegramBotClient("7596278638:AAHaRXgzYRPgyCU-DJF2cmveeJBe_Y3HMR0");
            var receiverOptions = new ReceiverOptions
            {
                AllowedUpdates = [UpdateType.Message],
                DropPendingUpdates = true
            };

            using var cts = new CancellationTokenSource();



            var handler = new UpdateHandler();

            handler.OnHandleUpdateStarted += (message) =>
            {
                Console.WriteLine($"Началась обработка сообщения '{message}'");
            };

            handler.OnHandleUpdateCompleted += (message) =>
            {
                Console.WriteLine($"Закончилась обработка сообщения '{message}'");
            };

            var me = await botClient.GetMe();
            Console.WriteLine($"Бот запущен: @{me.Username}");

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
                handler.OnHandleUpdateStarted -= null;
                handler.OnHandleUpdateCompleted -= null;
            }

            Console.ReadLine();
            cts.Cancel();
        }
    }
}
