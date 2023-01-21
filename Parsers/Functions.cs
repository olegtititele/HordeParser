using PostgreSQL;
using System.Globalization;


namespace Parser
{
    static class Functions
    {
        public static string ConvertPhone(string phoneBlock)
        {
            string convertedPhone = "";
            for (int i = 0; i < phoneBlock.Length; i++)
            {
                if (Char.IsDigit(phoneBlock[i]))
                {
                    convertedPhone += phoneBlock[i];
                }
                else if(phoneBlock[i] == '+')
                {
                    convertedPhone += phoneBlock[i];
                }
                
                continue;
            }
            return convertedPhone;
        }
        
        public static string ConvertPrice(string priceBlock, string currency)
        {// Форматировать цену
            string price = "";
            for (int i = 0; i < priceBlock.Length; i++)
            {
                if (Char.IsDigit(priceBlock[i]))
                {
                    price += priceBlock[i];
                }
                else if(priceBlock[i] == ',' || priceBlock[i] == '.' || priceBlock[i] == '\'')
                {
                    price += priceBlock[i];
                }
                else
                {
                    continue;
                }
            }
            return $"{price} {currency}";
        }

        public static int LeaveOnlyNumbers(string line)
        {// Оставить только цифры
            string newLine = "";
            for (int i = 0; i < line.Length; i++)
            {
                if (Char.IsDigit(line[i]))
                {
                    newLine += line[i];
                }
                else
                {
                    continue;
                }
            }
            return Int32.Parse(newLine);
        }

        
        public static bool CheckBlacklistAds(long userId, string phoneNumber, string blacklist)
        {// Проверка номера телефона
            if(blacklist == "Отключить")
            {
                return true;
            }
            else
            {
                if(DB.CheckPhoneNumber(userId, phoneNumber))
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
        }

        public static bool CheckBlacklisSellerLink(long userId, string sellerLink, string blacklist)
        {// Проверка номера телефона
            if(blacklist == "Отключить")
            {
                return true;
            }
            else
            {
                if(DB.CheckSellerlink(userId, sellerLink))
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
        }

        public static bool CheckSellerType(string userSellerType, string sellerType)
        {// Проверка типа аккаунта
            if(userSellerType == "Частное лицо")
            {
                if(sellerType == "Частное лицо")
                {
                    return true;
                }
                return false;
            }
            return true;
        }

        public static bool CheckSellerTotalAds(int sellerTotalAds)
        {// Проверка количества объявлений продавца
            if(20 > sellerTotalAds)
            {
                return true;
            }
            return false;
        }
        
        public static bool CheckAdRegDate(DateTime exactTime, DateTime adRegDate)
        {// Проверка даты регистрации объявления
            if(exactTime <= adRegDate)
            {
                return true;
            }
            return false;
        }
        
        public static bool CheckSellerRegDate(string userSellerRegDate, DateTime sellerRegDate)
        {// Проверка даты регистрации продавца
            if(userSellerRegDate == "Отключить")
            {
                return true;
            }
            else
            {
                if(Convert.ToDateTime(userSellerRegDate) <= sellerRegDate)
                {
                    return true;
                }
                return false;
            }
        }

        public static bool CheckSellerRating(decimal userSellerRating, decimal sellerRating)
        {// Проверка рейтинга продавца
            if(userSellerRating <= sellerRating)
            {
                return true;
            }
            return false;
        }

        public static bool CheckSellerFeedback(string userSellerFeedback, int sellerFeedback)
        {// Проверка количества отзывов продавца
            if(userSellerFeedback == "Отключить")
            {
                return true;
            }  
            else
            {
                if(Int32.Parse(userSellerFeedback) >= sellerFeedback)
                {
                    return true;
                }
                return false;
            }
        }

        
        public static void AddToBlacklist(long userId, string platform, string adLink, string sellerLink, string phoneNumber)
        {// Добавить объявление в бд
            try
            {
                DB.AddAdvertisementToBlackList(userId, platform, adLink, sellerLink, phoneNumber);
            }
            catch(Exception e)
            {
                Console.WriteLine(e);
                return; 
            }
        }

        public static DateTime UnixTimeToDateTime(long unixtime)
        {
            System.DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
            dtDateTime = dtDateTime.AddMilliseconds(unixtime).ToLocalTime();
            return dtDateTime;
        }
    }
}