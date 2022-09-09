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
        try
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
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
    }

    private async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
    {
        try
        {
            if (update.Message is Message message)
            {
                if (message.Text == "/start")
                {
                    if (message.From != null)
                    {
                        await _botMessageHelper.TryAddUser(message.From.Id, message.From.Username);
                    }
                }
                else
                {
                    try
                    {

                        await botClient.DeleteMessageAsync(message.Chat, message.MessageId, cancellationToken: cancellationToken);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                    }
                }

                if (message.From != null && message.Text != null)
                {
                    ResponseMessage responseMessage = await _botMessageHelper.NextState(message.From.Id, message.From.Username, message.Text); //await userContext.Change(message.Text ?? "");

                    List<List<KeyboardButton>> buttons = new List<List<KeyboardButton>>();

                    if (responseMessage.UpButtons.Any())
                    {
                        buttons.Add(responseMessage.UpButtons.Select(x => new KeyboardButton(x)).ToList());
                    }
                    
                    if (responseMessage.DownButtons.Any())
                    {
                        buttons.Add(responseMessage.DownButtons.Select(x => new KeyboardButton(x)).ToList());
                    }
                    
                    if (responseMessage.Buttons.Any())
                    {
                        List<KeyboardButton> rowButtons = new List<KeyboardButton>();

                        int count = responseMessage.Buttons.Count;

                        for (int i = 0; i < count; i++)
                        {
                            if (i % 2 == 0)
                            {
                                rowButtons.Add(new KeyboardButton(responseMessage.Buttons[i]));
                            }
                            else
                            {
                                rowButtons.Add(new KeyboardButton(responseMessage.Buttons[i]));
                                buttons.Add(rowButtons);
                                rowButtons = new List<KeyboardButton>();
                            }

                            if (i == count - 1 && rowButtons.Any())
                            {
                                buttons.Add(rowButtons);
                                rowButtons = new List<KeyboardButton>();
                            }
                        }
                    }
                    
                    ReplyKeyboardMarkup replyKeyboardMarkup = new(buttons)
                    {
                        ResizeKeyboard = true,
                        OneTimeKeyboard = false,
                    };

                    if (responseMessage.ResponseMessageType == EResponseMessageType.Photo)
                    {
                        using (var fileStream = new FileStream(responseMessage.PhotoUrl, FileMode.Open, FileAccess.Read, FileShare.Read))
                        {
                            await botClient.SendPhotoAsync(message.Chat,
                                photo: new InputOnlineFile(fileStream),
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
        catch (Exception e)
        {
            Console.WriteLine(e);
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