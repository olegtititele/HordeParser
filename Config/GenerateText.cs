using PostgreSQL;

namespace ConfigFile
{
    public static class GenerateMessageText
    {
        public static string MenuText(long chatId)
        {
            string text = $"<b>HORDE LITE PARSER</b>\n\n🆔 <b>ID:</b> <code>{chatId}</code>\n\n<b>🔭 Данный бот предназначен для поиска самых свежих объявлений на площадках CAROUSELL.</b>\n\n<b>По всем вопросам обращайтесь к @azshara_queen.</b>";
            return text;
        }

        public static string SettingsText()
        {
            string text = $"<b><u>Настройки</u></b>\n\n<b>• Здесь вы можете изменить настройки ЧС.</b>\n<b>• Здесь вы можете изменить настройки парсера.</b>\n<b>• Здесь вы можете изменить конфигурацию продавца, необходимую для парсинга.</b>";

            return text;
        }

        public static string BlackListText(long chatId)
        {
            string text = $"<b>🕴 Продавцов в ЧС:</b> <code>{DB.BlacklistLength(chatId)}</code>\n\n<b>🔗 Ссылок в ЧС:</b> <code>{DB.GetUserBlacklistLinks(chatId).Count()}</code>";

            return text;
        }

        public static string ConfigurationText(long chatId)
        {
            string text = $"<b>🍃 Текст WhatsApp:</b> <code>{DB.GetWhatsappText(chatId)}</code>\n\n<b>🕐 Тайм-аут после запроса:</b> <code>{DB.GetTimeout(chatId)} cек.</code>";

            return text;
        }

        public static string SellerSettingsText(long chatId)
        {
            string sellerFeedback = DB.GetSellerFeedback(chatId);
            string sellerRegDate = DB.GetSellerRegDate(chatId);
            DateTime dt;

            if(sellerFeedback == "Отключить")
            {
                sellerFeedback = "Отключено";
            }

            if(DateTime.TryParse(sellerRegDate, out dt))
            {
                sellerRegDate = dt.ToString("dd.MM.yyyy");
            }
            else
            {
                sellerRegDate = "Отключено";
            }

            string text = $"<b>🧷 Рейтинг продавца — </b><code>{DB.GetSellerRating(chatId)}</code>\n\n<b>🧷 Количество отзывов продавца — </b><code>{sellerFeedback}</code>\n\n<b>🧷 Дата регистрации продавца — </b><code>{sellerRegDate}</code>";

            return text;
        }
    }
}