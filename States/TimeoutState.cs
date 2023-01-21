using PostgreSQL;
using Bot_Keyboards;
using ConfigFile;

using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.InputFiles;

namespace States
{
    public static class TimeoutState
    {   
        public static async void MessageHandler(ITelegramBotClient botClient, string messageText, long chatId, int messageId, string mainMenuPhoto)
        {
            try
            {
                FileStream fileStream = new FileStream(mainMenuPhoto, FileMode.Open, FileAccess.Read, FileShare.Read);

                if(int.TryParse(messageText, out int number))
                {
                    if(number >= 10 && number <= 30)
                    {
                        DB.UpdateTimeout(chatId, number);
                        await botClient.SendPhotoAsync(
                            chatId: chatId,
                            photo: new InputOnlineFile(fileStream),
                            caption: $"<b>Тайм-аут изменен на:</b> <code>{DB.GetTimeout(chatId)}</code>",
                            parseMode: ParseMode.Html,
                            replyMarkup: Keyboards.backToConfiguration
                        );

                        string state = "MainMenu";
                        DB.UpdateState(chatId, state);
                    }
                    else
                    {
                        await botClient.SendPhotoAsync(
                            chatId: chatId,
                            photo: new InputOnlineFile(fileStream),
                            caption: "<b>❗️ Тайм-аут не должен быть меньше 10 секунд и превышать 30 секунд. Введите повторно.</b>",
                            parseMode: ParseMode.Html,
                            replyMarkup: Keyboards.backToConfiguration
                        ); 
                    }
                }
                else
                {
                    await botClient.SendPhotoAsync(
                        chatId: chatId,
                        photo: new InputOnlineFile(fileStream),
                        caption: "<b>❗️ Тайм-аут должен быть цифрой. Введите повторно.</b>",
                        parseMode: ParseMode.Html,
                        replyMarkup: Keyboards.backToConfiguration
                    );
                }
            }
            catch
            {
                return;
            }
        }
    }
}