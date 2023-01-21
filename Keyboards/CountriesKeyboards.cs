using Telegram.Bot.Types.ReplyMarkups;


namespace Bot_Keyboards
{
    public class CountriesKeyboards
    {
        // Клавиатура сервисов
        public static InlineKeyboardMarkup CountriesSitesKb = new(new []
        {
            new []
            {
                InlineKeyboardButton.WithCallbackData(text: "🇸🇬 carousell.sg", callbackData: "carousell.sg"),
                InlineKeyboardButton.WithCallbackData(text: "🇭🇰 carousell.com.hk", callbackData: "carousell.com.hk"),
            },
            // back btn
            new []
            {
                InlineKeyboardButton.WithCallbackData(text: "🏚 В меню", callbackData: "back_to_menu"),
            },
        });
    }
}