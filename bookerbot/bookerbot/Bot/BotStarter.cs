using bookerbot.State;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.InputFiles;
using Telegram.Bot.Types.ReplyMarkups;

namespace bookerbot.Bot;

public class BotStarter
{
    private readonly BotMessageHelper _botMessageHelper;
    private readonly ReceiverOptions _receiverOptions = new()
    {
        AllowedUpdates = new[]
        {
            UpdateType.Unknown, UpdateType.Message, UpdateType.CallbackQuery, UpdateType.Poll, UpdateType.ChannelPost, UpdateType.ChatMember, UpdateType.EditedMessage,
            UpdateType.InlineQuery, UpdateType.ShippingQuery, UpdateType.ChosenInlineResult, UpdateType.MyChatMember, UpdateType.PreCheckoutQuery, UpdateType.PollAnswer,
        },
        ThrowPendingUpdates = true
    };
    
    public BotStarter(BotMessageHelper botMessageHelper)
    {
        _botMessageHelper = botMessageHelper;
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
            if (message.Text == "/start")
            {
                if (message.From != null)
                {
                    await _botMessageHelper.TryAddUser(message.From.Id);
                }
            }
            else
            {
                await botClient.DeleteMessageAsync(message.Chat, message.MessageId, cancellationToken: cancellationToken);
            }

            if (message.From != null && message.Text != null)
            {
                ResponseMessage responseMessage = await _botMessageHelper.NextState(message.From.Id, message.Text); //await userContext.Change(message.Text ?? "");

                ReplyKeyboardMarkup replyKeyboardMarkup = new(new[]
                {
                    responseMessage.UpButtons.Select(x => new KeyboardButton(x)),
                    responseMessage.DownButtons.Select(x => new KeyboardButton(x)),
                    responseMessage.Buttons.Select(x => new KeyboardButton(x))
                })
                {

                    ResizeKeyboard = true,
                    OneTimeKeyboard = false,

                };

                if (responseMessage.ResponseMessageType == EResponseMessageType.Photo)
                {
                    using (var fileStream = new FileStream("/Users/vadimarduanov/RiderProjects/booker/bookerbot/bookerbot/Common/a65eb6a075f4360811e178f53d9dfcab.jpg",
                               FileMode.Open, FileAccess.Read, FileShare.Read))
                    {
                        await botClient.SendPhotoAsync(message.Chat,
                            photo: new InputOnlineFile(
                                fileStream), // new InputMedia("https://ndc.book24.ru/resize/220x270/iblock/f57/f570377d06e49196c0cffa31bd8e62df/a65eb6a075f4360811e178f53d9dfcab.jpg"),// ,
                            caption: responseMessage.Text,
                            replyMarkup: replyKeyboardMarkup, cancellationToken: cancellationToken);
                    }
                }
                else
                {
                    await botClient.SendTextMessageAsync(message.Chat, responseMessage.Text, replyMarkup: replyKeyboardMarkup);
                }
            }
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