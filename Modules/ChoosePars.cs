using Parser;
using PostgreSQL;
using Bot_Keyboards;
using ConfigFile;

using Telegram.Bot;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.InputFiles;

namespace Modules
{
    public static class Platforms
    {
        public static void GetParser(ITelegramBotClient botClient, long chatId, string platform)
        {
            Thread load = new Thread(()=>Loading(botClient, chatId));
            switch(platform)
            {
                case "carousell.sg":
                    new Thread(()=>Carousell.StartParsing(botClient, chatId, DateTime.Now)).Start();
                    load.Start();
                    break;
                case "carousell.com.hk":
                    new Thread(()=>Carousell.StartParsing(botClient, chatId, DateTime.Now)).Start();
                    load.Start();
                    break;
            }
        }

        static async void Loading(ITelegramBotClient botClient, long chatId)
        {
            string mainMenuPhoto = Config.menuPhoto;

            while(true)
            {
                if(ProjectFunctions.Functions.CheckSubChannel(botClient.GetChatMemberAsync(Config.workChatId, chatId).Result.Status.ToString()))
                {
                    DB.UpdateParser(chatId, "Stop");

                    using (var fileStream = new FileStream(mainMenuPhoto, FileMode.Open, FileAccess.Read, FileShare.Read))
                    {
                        await botClient.SendPhotoAsync(
                            chatId: chatId,
                            photo: new InputOnlineFile(fileStream),
                            caption: "<b>⛔️ Парсинг остановлен!</b>",
                            parseMode: ParseMode.Html,
                            replyMarkup: Keyboards.backToMenu
                        );
                    }
                    return;
                }

                if(DB.GetParser(chatId) == "Stop")
                {
                    using (var fileStream = new FileStream(mainMenuPhoto, FileMode.Open, FileAccess.Read, FileShare.Read))
                    {
                        await botClient.SendPhotoAsync(
                            chatId: chatId,
                            photo: new InputOnlineFile(fileStream),
                            caption: "<b>⛔️ Парсинг остановлен!</b>",
                            parseMode: ParseMode.Html,
                            replyMarkup: Keyboards.backToMenu
                        );
                    }
                    return;
                }

                System.Threading.Thread.Sleep(10000);
            }
        }
    }
}
