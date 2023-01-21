using PostgreSQL;
using Bot_Keyboards;
using ConfigFile;

using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.InputFiles;

namespace States
{
    public static class DeleteSellersState
    {
        public static async void MessageHandler(ITelegramBotClient botClient, string messageText, long chatId, int messageId, string mainMenuPhoto)
        {
            try
            {
                FileStream fileStream = new FileStream(mainMenuPhoto, FileMode.Open, FileAccess.Read, FileShare.Read);
                bool first_seller = false;
                bool last_seller = false;
                int deleteSellers = 0;
                string firstSellerLink = messageText.Split("|")[0];
                string lastSellerLink = messageText.Split("|")[1];

                foreach(var seller in DB.GetAllBlSellers(chatId))
                {
                    
                    if(seller == firstSellerLink)
                    {
                        first_seller = true;
                    }

                    if(seller == lastSellerLink)
                    {
                        last_seller = true;
                    }

                    if(last_seller)
                    {   
                        deleteSellers += 1;
                        DB.DeleteSeller(chatId, seller);
                        break;
                    }

                    if(!first_seller)
                    {
                        continue;
                    }
                    else
                    {
                        deleteSellers += 1;
                        DB.DeleteSeller(chatId, seller);
                    }
                    
                }

                await botClient.SendPhotoAsync(
                    chatId: chatId,
                    photo: new InputOnlineFile(fileStream),
                    caption: $"<b>Удалено ссылок:</b> <code>{deleteSellers}</code>",
                    parseMode: ParseMode.Html,
                    replyMarkup: Keyboards.backToBlackList
                );


                string state = "MainMenu";
                DB.UpdateState(chatId, state);
            }
            catch
            {
                return;
            }
        }
    }
}