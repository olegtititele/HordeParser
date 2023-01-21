using States;
using PostgreSQL;
using Bot_Keyboards;
using ConfigFile;
using Modules;
using ProjectFunctions;

using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;



namespace Handlers
{
    public static class CallHandler
    {
        private static string mainMenuPhoto = Config.menuPhoto;
        public static async Task BotOnCallbackQueryReceived(ITelegramBotClient botClient, CallbackQuery callbackQuery)
        {
            try
            {
                long chatId = callbackQuery.Message!.Chat.Id;
                int messageId = callbackQuery.Message.MessageId;
                string? firstName = callbackQuery.Message.Chat.FirstName;
                string state = DB.GetState(chatId);

                if(chatId.ToString()[0]=='-')
                {
                    return;
                }


                // if(Functions.CheckSubChannel(botClient.GetChatMemberAsync(Config.workChatId, chatId).Result.Status.ToString()))
                // {
                //     await botClient.SendTextMessageAsync(
                //         chatId: chatId,
                //         text: $"<b> Для того, чтобы пользоваться ботом подайте заявку в <a href=\"https://t.me/HORDE_SQUAD_BOT\">бота</a> и вступите в чат.</b>",
                //         parseMode: ParseMode.Html
                //     );
                //     return;
                // }

                switch(callbackQuery.Data)
                {
                    case "start_pars":
                        if(DB.GetParser(chatId) == "Start")
                        {
                            await botClient.AnswerCallbackQueryAsync(
                                callbackQueryId: callbackQuery.Id,
                                text: "У вас уже запущен парсинг.",
                                showAlert: true
                            );
                            return;
                        }

                        string platform = DB.GetPlatform(chatId);
                        DB.UpdateParser(chatId, "Start");
                        Platforms.GetParser(botClient, chatId, platform);

                        await botClient.EditMessageCaptionAsync(
                            chatId: chatId,
                            messageId: messageId,
                            caption: $"<b>{platform.ToUpperInvariant()}</b>\n\n<b>Парсинг начался. Парсер будет отслеживать самые свежие объявления начиная с текущего времнеми.</b>\n\n<b>❕ Для остановки парсера напишите </b><code>Стоп</code>",
                            parseMode: ParseMode.Html
                        );
                        return;

                    case "show_services":
                    // ПОСМОТРЕТЬ СЕРВИСЫ
                        await botClient.EditMessageCaptionAsync(
                            chatId: chatId,
                            messageId: messageId,
                            caption: "<b>‍🦺 Выберите сервис:</b>",
                            parseMode: ParseMode.Html,
                            replyMarkup: CountriesKeyboards.CountriesSitesKb
                        );
                        return;
                    case "settings":
                    // НАСТРОЙКИ
                        await botClient.EditMessageCaptionAsync(
                            chatId: chatId,
                            messageId: messageId,
                            caption: GenerateMessageText.SettingsText(),
                            parseMode: ParseMode.Html,
                            replyMarkup: Keyboards.SettingsKb()
                        );
                        return;
                    case "black_list":
                    // ЧС
                        await botClient.EditMessageCaptionAsync(
                            chatId: chatId,
                            messageId: messageId,
                            caption: GenerateMessageText.BlackListText(chatId),
                            parseMode: ParseMode.Html,
                            replyMarkup: Keyboards.BlackListKb(chatId)
                        );
                        return;
                    case "black_list_status":
                        DB.UpdateBlackList(chatId);

                        await botClient.EditMessageReplyMarkupAsync(
                            chatId: chatId,
                            messageId: messageId,
                            replyMarkup: Keyboards.BlackListKb(chatId)
                        );
                        return;
                    case "links_blacklist":
                        DB.UpdateState(chatId, "LinksBlacklist");

                        await botClient.EditMessageCaptionAsync(
                            chatId: chatId,
                            messageId: messageId,
                            caption: "<b>Введите ссылки КАТЕГОРИЙ через запятую для добавления в черный список.</b>",
                            parseMode: ParseMode.Html,
                            replyMarkup: Keyboards.blacklistLinksKb
                        );
                        return;
                    case "delete_sellers":
                        DB.UpdateState(chatId, "DeleteSellers");

                        await botClient.EditMessageCaptionAsync(
                            chatId: chatId,
                            messageId: messageId,
                            caption: "<b>Пример:</b> <code>ссылка1</code><b>|</b><code>ссылка2</code>",
                            replyMarkup: Keyboards.backToBlackList,
                            parseMode: ParseMode.Html
                        );
                        return;

                    case "configuration":
                    // КОНФИГ
                        await botClient.EditMessageCaptionAsync(
                            chatId: chatId,
                            messageId: messageId,
                            caption: GenerateMessageText.ConfigurationText(chatId),
                            parseMode: ParseMode.Html,
                            replyMarkup: Keyboards.ConfigurationKb()
                        );
                        return;
                    case "whatsapp_text":
                        DB.UpdateState(chatId, "WhatsappText");

                        string text = "<b>🖊 Введите текст для WhatsApp (<u>макс. 500 символов</u>):</b>\n\n<b>Ключевые слова для вставки:</b>\n<code>@link</code>-<i>Подставит текущую ссылку</i>\n<code>@title</code>-<i>Подставит название объявления</i>\n<code>@price</code>-<i>Подставит цену объявления</i>\n<code>@location</code>-<i>Подставит местоположение объявления</i>\n<code>@seller_name</code>-<i>Подставит имя продавца</i>\n\n✔️ <b>Вставьте ключевое слово в текст и вместо него подставится нужная информация.</b>";

                        await botClient.EditMessageCaptionAsync(
                            chatId: chatId,
                            messageId: messageId,
                            caption: text,
                            parseMode: ParseMode.Html,
                            replyMarkup: Keyboards.backToConfiguration
                        );
                        return;
                    case "timeout":
                        DB.UpdateState(chatId, "Timeout");

                        await botClient.EditMessageCaptionAsync(
                            chatId: chatId,
                            messageId: messageId,
                            caption: "<b>Установите таймаут от 10 до 30 секунд:</b>",
                            parseMode: ParseMode.Html,
                            replyMarkup: Keyboards.backToConfiguration
                        );
                        return;

                    case "seller_params":
                    // ПАРАМЕТРЫ ПРОДАВЦА
                        await botClient.EditMessageCaptionAsync(
                            chatId: chatId,
                            messageId: messageId,
                            caption: GenerateMessageText.SellerSettingsText(chatId),
                            parseMode: ParseMode.Html,
                            replyMarkup: Keyboards.SellerSettingsKb(chatId)
                        );
                        return;
                    case "account_type":
                        DB.UpdateSellerType(chatId);

                        await botClient.EditMessageReplyMarkupAsync(
                            chatId: chatId,
                            messageId: messageId,
                            replyMarkup: Keyboards.SellerSettingsKb(chatId)
                        );
                        return;
                    case "seller_announ_count":
                        int sellerAds = DB.GetSellerTotalAds(chatId);
                        
                        if(sellerAds == 1)
                        {
                            DB.UpdateSellerTotalAds(chatId, 0);

                            await botClient.AnswerCallbackQueryAsync(
                                callbackQueryId: callbackQuery.Id,
                                text: "Фильтр \"Количество объявлений продавца до 20\" отключен.",
                                showAlert:true
                            );
                        }
                        else
                        {
                            DB.UpdateSellerTotalAds(chatId, 1);

                            await botClient.AnswerCallbackQueryAsync(
                                callbackQueryId: callbackQuery.Id,
                                text: "Теперь парсер будет искать продавцов, у которых меньше 20 объявлений.",
                                showAlert:true
                            );
                        }

                        await botClient.EditMessageCaptionAsync(
                            chatId: chatId,
                            messageId: messageId,
                            caption: GenerateMessageText.SellerSettingsText(chatId),
                            parseMode: ParseMode.Html,
                            replyMarkup: Keyboards.SellerSettingsKb(chatId)
                        );
                        return;
                    case "seller_rating":
                        DB.UpdateState(chatId, "SellerRating");

                        await botClient.EditMessageCaptionAsync(
                            chatId: chatId,
                            messageId: messageId,
                            caption: "<b>⤵️ Введите рейтинг продавца.\n➖➖➖➖➖\nПример:</b> <code>3.2</code> [0 - 5]\n\n<b>Будут показаны объявления, у которых рейтинг продавца не превышает</b> <code>3.2</code><b>.</b>",
                            parseMode: ParseMode.Html,
                            replyMarkup: Keyboards.sellerRatingKb
                        );
                        return;
                    case "seller_feedback":
                        DB.UpdateState(chatId, "SellerFeedback");

                        await botClient.EditMessageCaptionAsync(
                            chatId: chatId,
                            messageId: messageId,
                            caption: "<b>⤵️ Введите количество отзывов продавца.\n➖➖➖➖➖\nПример:</b> <code>10</code>\n\n<b>Будут показаны объявления, у которых кол-во отзывов продавца не превышает</b> <code>10</code><b>.</b>",
                            parseMode: ParseMode.Html,
                            replyMarkup: Keyboards.sellerFeedbackKb
                        );
                        return;
                    case "seller_reg":
                        DB.UpdateState(chatId, "SellerRegData");
                        DateTime today = DateTime.Now;
                        
                        await botClient.EditMessageCaptionAsync(
                            chatId: chatId,
                            messageId: messageId,
                            caption: $"<b>⤵️ Укажите дату регистрации продавца.\n➖➖➖➖➖\n✔️ Пример:</b> <code>{today.AddDays(-10).ToString("dd.MM.yyyy")}</code>\n\n<b>Будут показаны объявления, у которых продавцы зарегистрировались в промежутке [</b><code>{today.AddDays(-10).ToString("dd.MM.yyyy")}</code> - <code>{today.ToString("dd.MM.yyyy")}</code><b>].</b>",
                            parseMode: ParseMode.Html,
                            replyMarkup: Keyboards.RegDateKb()
                        );
                        return;
                    
                    // КНОПКИ НАЗАД
                    case "back_to_menu":
                        await botClient.EditMessageCaptionAsync(
                            chatId: chatId,
                            messageId: messageId,
                            caption: GenerateMessageText.MenuText(chatId),
                            parseMode: ParseMode.Html,
                            replyMarkup: Keyboards.MainMenuButtons
                        );
                        return;
                    case "back_to_settings":
                        DB.UpdateState(chatId, "MainMenu");

                        await botClient.EditMessageCaptionAsync(
                            chatId: chatId,
                            messageId: messageId,
                            caption: GenerateMessageText.SettingsText(),
                            parseMode: ParseMode.Html,
                            replyMarkup: Keyboards.SettingsKb()
                        );
                        return;
                    case "back_to_blacklist":
                        DB.UpdateState(chatId, "MainMenu");

                        await botClient.EditMessageCaptionAsync(
                            chatId: chatId,
                            messageId: messageId,
                            caption: GenerateMessageText.BlackListText(chatId),
                            parseMode: ParseMode.Html,
                            replyMarkup: Keyboards.BlackListKb(chatId)
                        );
                        return;
                    case "back_to_configuration":
                        DB.UpdateState(chatId, "MainMenu");

                        await botClient.EditMessageCaptionAsync(
                            chatId: chatId,
                            messageId: messageId,
                            caption: GenerateMessageText.ConfigurationText(chatId),
                            parseMode: ParseMode.Html,
                            replyMarkup: Keyboards.ConfigurationKb()
                        );
                        return;
                    case "back_to_seller_settings":
                        DB.UpdateState(chatId, "MainMenu");

                        await botClient.EditMessageCaptionAsync(
                            chatId: chatId,
                            messageId: messageId,
                            caption: GenerateMessageText.SellerSettingsText(chatId),
                            parseMode: ParseMode.Html,
                            replyMarkup: Keyboards.SellerSettingsKb(chatId)
                        );
                        return;
                    case "back_to_services_list":
                        await botClient.EditMessageCaptionAsync(
                            chatId: chatId,
                            messageId: messageId,
                            caption: "<b>‍🦺 Выберите сервис:</b>",
                            parseMode: ParseMode.Html,
                            replyMarkup: CountriesKeyboards.CountriesSitesKb
                        );
                        return;
                }

                foreach(string country in Config.Countries)
                {
                    if(callbackQuery.Data == country)
                    {
                        if(DB.GetParser(chatId) == "Start")
                        {
                            await botClient.AnswerCallbackQueryAsync(
                                callbackQueryId: callbackQuery.Id,
                                text: "У вас уже запущен парсинг.",
                                showAlert: true
                            );
                            return;
                        }
                        
                        DB.UpdatePlatform(chatId, callbackQuery.Data);

                        string text = $"<b>{country.ToUpperInvariant()}</b>\n\n<b>⚒ Особенности площадки:</b> <code>Нет номеров телефона</code>\n\n<b>⛽ Фильтры площадки:</b>\n\t\t\t\t• Кол-во объявлений продавца\n\t\t\t\t• Дата регистрации продавца\n\t\t\t\t• Дата создания объявления\n\t\t\t\t• Рейтинг продавца\n\t\t\t\t• Кол-во отзывов";

                        await botClient.EditMessageCaptionAsync(
                            chatId: chatId,
                            messageId: messageId,
                            caption: text,
                            replyMarkup: Keyboards.startPars,
                            parseMode: ParseMode.Html
                        );
                    }
                }

                switch (state)
                {
                    case "SellerRegData":
                        SellerRegDateState.CallBackHandler(botClient, callbackQuery, chatId, messageId);
                        return;
                    case "SellerRating":
                        SellerRatingState.CallBackHandler(botClient, callbackQuery, chatId, messageId);
                        return;
                    case "SellerFeedback":
                        SellerFeedbackState.CallBackHandler(botClient, callbackQuery, chatId, messageId);
                        return;
                    case "LinksBlacklist":
                        LinksBlacklistState.CallBackHandler(botClient, callbackQuery, chatId, messageId);
                        return;
                    default:
                        return;
                }
            }
            catch
            {
                await botClient.AnswerCallbackQueryAsync(
                    callbackQueryId: callbackQuery.Id,
                    text: Config.errorMessage,
                    showAlert:false
                );
                return; 
            }
        }
    }            
}
