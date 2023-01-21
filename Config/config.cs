using System.Globalization;

namespace ConfigFile
{
    public class Config
    {
        public static string Bot_Token = "5934440175:AAHxnKKEXWOUaN0GUwdkyYmhg7UWVWPX0AQ";
        public static string menuPhoto = "background.jpg";
        public static string errorMessage = "Произошла неизвестная ошибка. Попробуйте еще раз.";
        public static long[] adminChatsId = {5468787377};
        public static long workChatId = -1001649876317;
        public static CultureInfo culture = new CultureInfo("ru-RU", false);
        public static string[] Countries = {"carousell.sg", "carousell.com.hk"};
    }
}