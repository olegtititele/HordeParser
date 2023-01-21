using PostgreSQL;
using Bot_Keyboards;
using ConfigFile;

using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.InputFiles;

namespace States
{
    public static class SellerFeedbackState
    {
        public static async void CallBackHandler(ITelegramBotClient botClient, CallbackQuery callbackQuery, long chatId, int messageId)
        {
            try
            {
                if(callbackQuery.Data == "disable_seller_feedback")
                {
                    DB.UpdateSellerFeedback(chatId, "Отключить");

                    await botClient.AnswerCallbackQueryAsync(
                        callbackQueryId: callbackQuery.Id,
                        text: "Фильтр \"Количество отзывов продавца\" отключен.",
                        showAlert:true
                    );
                }

                DB.UpdateState(chatId, "MainMenu");

                await botClient.EditMessageCaptionAsync(
                    chatId: chatId,
                    messageId: messageId,
                    caption: GenerateMessageText.SellerSettingsText(chatId),
                    parseMode: ParseMode.Html,
                    replyMarkup: Keyboards.SellerSettingsKb(chatId)
                );
                return;
            }
            catch
            {
                await botClient.AnswerCallbackQueryAsync(
                    callbackQueryId: callbackQuery.Id,
                    text: Config.errorMessage,
                    showAlert:false
                );
            }
        }
        
        public static async void MessageHandler(ITelegramBotClient botClient, string messageText, long chatId, int messageId, string mainMenuPhoto)
        {
            try
            {
                FileStream fileStream = new FileStream(mainMenuPhoto, FileMode.Open, FileAccess.Read, FileShare.Read);

                if(int.TryParse(messageText, out int number))
                {
                    DB.UpdateSellerFeedback(chatId, messageText);
                    DB.UpdateState(chatId, "MainMenu");

                    await botClient.SendPhotoAsync(
                        chatId: chatId,
                        photo: new InputOnlineFile(fileStream),
                        caption: $"<b>Количество отзывов продавца обновлено на:</b> <code>{messageText}</code>",
                        parseMode: ParseMode.Html,
                        replyMarkup: Keyboards.backToSellerSettings
                    );
                }
                else
                {
                    await botClient.SendPhotoAsync(
                        chatId: chatId,
                        photo: new InputOnlineFile(fileStream),
                        caption: "<b>❗️ Количество отзывов продавца должно быть цифрой. Введите повторно.</b>",
                        parseMode: ParseMode.Html,
                        replyMarkup: Keyboards.sellerFeedbackKb
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