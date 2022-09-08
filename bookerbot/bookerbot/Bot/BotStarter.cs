using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace bookerbot.Bot;

public class BotStarter
{
    private readonly ReceiverOptions _receiverOptions = new()
    {
        AllowedUpdates = new[]
        {
            UpdateType.Unknown, UpdateType.Message, UpdateType.CallbackQuery, UpdateType.Poll, UpdateType.ChannelPost, UpdateType.ChatMember, UpdateType.EditedMessage,
            UpdateType.InlineQuery, UpdateType.ShippingQuery, UpdateType.ChosenInlineResult, UpdateType.MyChatMember, UpdateType.PreCheckoutQuery, UpdateType.PollAnswer,
        },
        ThrowPendingUpdates = true
    };
    
    public BotStarter()
    {
    }

    public async Task Run()
    {
        ITelegramBotClient bot = new TelegramBotClient("5186218806:AAEe2niWFOV3m9_U1pI95n7OrpjlB0z8gMY");

        using var cts = new CancellationTokenSource();
        var cancellationToken = cts.Token;

        await bot.ReceiveAsync(
            HandleUpdateAsync,
            HandleErrorAsync,
            _receiverOptions,
            cancellationToken
        );
    }

    private async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
    {
        if (update.Message is Message message)
        {
        }
    }

    private async Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
    {
        if (exception is ApiRequestException apiRequestException)
        {
            await botClient.SendTextMessageAsync(123, apiRequestException.ToString(), cancellationToken: cancellationToken);
        }
    }
}