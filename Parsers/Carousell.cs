using HtmlAgilityPack;
using Newtonsoft.Json.Linq;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;

using PostgreSQL;
using Proxies;

namespace Parser
{
    public class Carousell
    {
        private static string userAgent = "Mozilla/5.0 (X11; Linux x86_64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/104.0.0.0 Safari/537.36";
        private static string errorImageUri = "https://upload.wikimedia.org/wikipedia/commons/3/3d/%D0%9D%D0%B5%D1%82_%D0%B8%D0%B7%D0%BE%D0%B1%D1%80%D0%B0%D0%B6%D0%B5%D0%BD%D0%B8%D1%8F.jpg";
        public static void StartParsing(ITelegramBotClient botClient, long userId, DateTime userExactTime)
        {
            HtmlWeb web = new HtmlWeb();
            DateTime exactTime = userExactTime;
            string userPlatform = DB.GetPlatform(userId);
            string userLink = DB.GetLink(userId);
            int userSellerAds = DB.GetSellerTotalAds(userId);
            string userSellerFeedback = DB.GetSellerFeedback(userId);
            string userSellerRegDate = DB.GetSellerRegDate(userId);
            decimal userSellerRating = DB.GetSellerRating(userId);
            string userSellerType = DB.GetSellerType(userId);
            string blacklist = DB.GetBlackList(userId);
            string parserCategory = DB.GetParserCategory(userId);
            List<string> blacklistCategories = DB.GetUserBlacklistLinks(userId);
            int timeout = DB.GetTimeout(userId)*1000;
            web.UserAgent = userAgent;

            try
            {
                string domen = "";
                string currency = "";

                switch(userPlatform)
                {
                    case "carousell.sg":
                        domen = "https://www.carousell.sg";
                        currency = "SGD";
                        break;
                    case "carousell.com.hk":
                        domen = "https://www.carousell.com.hk";
                        currency = "HKD";
                        break;
                }

                List<string> passedLinks = new List<string>();
  
                while(true)
                {
                    try
                    {
                        string link = GenerateLink(parserCategory, domen, userLink);

                        HtmlDocument document = web.Load(link, CarousellProxy.myProxyIP, CarousellProxy.myPort, CarousellProxy.login, CarousellProxy.password);
                        var advertisements = document.DocumentNode.SelectNodes("//div[@id=\"root\"]//a");

                        foreach (HtmlNode advertisement in advertisements)
                        {
                            if(DB.GetParser(userId) == "Stop"){ return; }
                            
                            string ad = advertisement.GetAttributeValue("href", "");
                            if(ad.Contains("https://") || !ad.Contains("/p/")){ continue; }

                            string adLink = domen + ad;

                            if(passedLinks.Contains(adLink))
                            {
                                return;
                            }
                            else
                            {
                                passedLinks.Add(adLink);

                                if(!DB.CheckAdvestisement(userId, adLink))
                                {
                                    ParsePageInfo(web, botClient, userId, adLink, exactTime, userPlatform, userLink, userSellerAds, userSellerRegDate, userSellerRating, userSellerFeedback, userSellerType, blacklist, parserCategory, blacklistCategories, domen, currency);
                                }
                            }
                        }
                    }
                    catch{ }

                    System.Threading.Thread.Sleep(timeout);
                }
            }
            catch
            {
                DB.UpdateParser(userId, "Stop");
                return;
            }
        }

        static void ParsePageInfo(HtmlWeb web, ITelegramBotClient botClient, long userId, string adLink, DateTime exactTime, string userPlatform, string userLink, int userSellerTotalAds, string userSellerRegDate, decimal userSellerRating, string userSellerFeedback, string userSellerType, string blacklist, string parserCategory, List<string> blacklistCategories, string domen, string currency)
        {
            string adCategory = "";
            string adPrice = "";
            string adTitle = "";
            string adImage = "";
            string sellerType = "Частное лицо";
            string sellerLink = "";
            string sellerName = "";
            string adDescription = "";
            string adLocation = "";
            decimal sellerRating = 0.0M;
            int sellerFeedback = 0;
            int sellerTotalAds = 1;
            DateTime adRegDate = DateTime.Today;
            DateTime sellerRegDate = DateTime.Today;

            
            HtmlDocument adDocument = web.Load(adLink, CarousellProxy.myProxyIP, CarousellProxy.myPort, CarousellProxy.login, CarousellProxy.password);

            var categories = adDocument.DocumentNode.SelectNodes("//a[@typeof=\"WebPage\"]");
            
            foreach(var category in categories)
            {
                try
                {
                    adCategory = domen + category.GetAttributeValue("href", "");

                    if(blacklistCategories.Contains(adCategory))
                    {
                        return;
                    }
                }
                catch
                {
                    continue;
                }
            }
            
            string adId = adLink.Split("/p/")[1].Split('/')[0].Split('-')[^1].Trim();

            JObject json = GetAdInfoJson(adDocument);
            if (json is  null) return;

            try
            {
                sellerName = json["Listing"]!["listingsMap"]![adId]!["seller"]!["username"]!.ToString();
                sellerLink = $"{domen}/u/{sellerName}";

                if(!Functions.CheckBlacklisSellerLink(userId, sellerLink, blacklist)){ return; }
            }
            catch
            {
                return;
            }


            try
            {
                adRegDate = Convert.ToDateTime(json["Listing"]!["listingsMap"]![adId]!["last_modified"]!.ToString()).AddHours(3).AddMinutes(1);
            }
            catch{ }

            if(!Functions.CheckAdRegDate(exactTime, adRegDate)){ return; }

            try
            {
                sellerRegDate = Convert.ToDateTime(json["Listing"]!["listingsMap"]![adId]!["seller"]!["date_joined"]!.ToString());
            }
            catch{ }

            if(!Functions.CheckSellerRegDate(userSellerRegDate, sellerRegDate)){ return; }

            if(userSellerTotalAds == 1)
            {
                try
                {
                    HtmlDocument sellerDocument = web.Load(sellerLink, CarousellProxy.myProxyIP, CarousellProxy.myPort, CarousellProxy.login, CarousellProxy.password);

                    JObject sellerJson = GetAdInfoJson(sellerDocument);

                    var userAdsBlock = sellerJson["Listing"]!["listingsMap"];
                    int ads = 0;

                    foreach(var ad in userAdsBlock!){
                        ads++;
                    }

                    sellerTotalAds = ads;
                }
                catch{ }

                if(!Functions.CheckSellerTotalAds(sellerTotalAds)){ return; }
            }

            try
            {
                sellerFeedback = Int32.Parse(json["Listing"]!["listingsMap"]![adId]!["seller"]!["feedback_count"]!.ToString());
            }
            catch{ }

            if(!Functions.CheckSellerFeedback(userSellerFeedback, sellerFeedback)){ return; }
            
            try
            {
                bool isBusiness = Convert.ToBoolean(json["Listing"]!["listingsMap"]![adId]!["seller"]!["profile"]!["is_official_partner"]!.ToString());

                if(isBusiness){ sellerType = "Бизнесс аккаунт"; }
            }
            catch{ }

            if(!Functions.CheckSellerType(userSellerType, sellerType)){ return; }

            try
            {
                sellerRating = decimal.Parse(json["Listing"]!["listingsMap"]![adId]!["seller"]!["feedback_score"]!.ToString());
            }
            catch{ }

            if(!Functions.CheckSellerRating(userSellerRating, sellerRating)){ return; }

            try
            {
                adTitle = adDocument.DocumentNode.SelectSingleNode("//p[@data-testid=\"new-listing-details-page-desktop-text-title\"]").InnerText;
            }
            catch
            {
                adTitle = "Не указано";
            }

            try
            {
                string adPriceBlock = adDocument.DocumentNode.SelectSingleNode("//div[@id=\"FieldSetField-Container-field_price\"]").InnerText;
                adPrice = Functions.ConvertPrice(adPriceBlock, currency);
            }
            catch
            {
                adPrice = "Не указана";
            }

            try
            {
                adLocation = json["Listing"]!["listingsMap"]![adId]!["location_name"]!.ToString();
                if(String.IsNullOrEmpty(adLocation)){ adLocation = "Не указано"; }
            }
            catch
            {
                adLocation = "Не указано";
            }

            try
            {
                adImage = json["Listing"]!["listingsMap"]![adId]!["photos"]![0]!["image_url"]!.ToString();
            }
            catch
            {
                adImage = errorImageUri;
            }

            Functions.AddToBlacklist(userId, userPlatform!, adLink, sellerLink, "null");

            SendLogToTg(botClient, userId, adLink, adTitle, adDescription, adPrice, adLocation, adImage, adRegDate, sellerName, sellerLink, sellerTotalAds, sellerRating, sellerFeedback, sellerRegDate, sellerType);

            return;
        }


        static async void SendLogToTg(ITelegramBotClient botClient, long userId, string adLink, string adTitle, string adDescription, string adPrice, string adLocation, string adImage, DateTime adRegDate, string sellerName, string sellerLink, int sellerTotalAds, decimal sellerRating, int sellerFeedback, DateTime sellerRegDate, string sellerType)
        {
            adDescription = adDescription.Replace('<', '`').Replace('>', '`').Replace('"', '\"');
            adTitle = adTitle.Replace('<', '`').Replace('>', '`').Replace('"', '\"');

            string adInfo = $"<b>📦 Название: </b><a href=\"{adLink}\">{adTitle}</a>\n<b>💲 Цена: </b>{adPrice}\n<b>🧔🏻 Продавец: </b><a href=\"{sellerLink}\">{sellerName}</a>\n<b>💠 Тип продавца: </b><code>{sellerType}</code>\n\n<b>📅 Добавлено: </b><b>{adRegDate.ToString().Split(' ')[0]}</b> {adRegDate.ToString().Split(' ')[1]}\n<b>📝 Количество объявлений: </b><code>{sellerTotalAds}</code>\n<b>📆 Дата регистрации: </b><b>{sellerRegDate.ToString("dd.MM.yyyy")}</b>\n\n<b>🌠 Рейтинг: </b><code>{sellerRating}</code>\n<b>🔢 Количество отзывов: </b><code>{sellerFeedback}</code>\n\n<b>📷 Фото: </b><code>{adImage}</code>\n\n<b>📍 Местоположение: </b>{adLocation}";

            try
            {
                try
                {
                    await botClient.SendPhotoAsync(
                        chatId: userId,
                        photo: adImage,
                        caption: adInfo,
                        parseMode: ParseMode.Html
                    );
                }
                catch
                {
                    await botClient.SendPhotoAsync(
                        chatId: userId,
                        photo: errorImageUri,
                        caption: adInfo,
                        parseMode: ParseMode.Html
                    );
                }
            }
            catch
            {
                await botClient.SendTextMessageAsync(
                    chatId: userId,
                    text: $"<b>📦 Название: </b><a href=\"{adLink}\">{adTitle}</a>\n<b>💲 Цена: </b>{adPrice}\n<b>🧔🏻 Продавец: </b><a href=\"{sellerLink}\">{sellerName}</a>\n<b>💠 Тип продавца: </b><code>{sellerType}</code>\n\n<b>📅 Добавлено: </b><b>{adRegDate.ToString().Split(' ')[0]}</b> {adRegDate.ToString().Split(' ')[1]}\n<b>📝 Количество объявлений: </b><code>{sellerTotalAds}</code>\n<b>📆 Дата регистрации: </b><b>{sellerRegDate.ToString("dd.MM.yyyy")}</b>\n\n<b>🌠 Рейтинг: </b><code>{sellerRating}</code>\n<b>🔢 Количество отзывов: </b><code>{sellerFeedback}</code>\n\n<b>📷 Фото: </b>{adImage}\n\n<b>📍 Местоположение: </b>{adLocation}",
                    parseMode: ParseMode.Html,
                    disableWebPagePreview: true
                );
            }

            return;
        }

        static JObject GetAdInfoJson(HtmlDocument document)
        {
            var scripts = document.DocumentNode.SelectNodes("//script");

            foreach(var script in scripts)
            {
                if(script.InnerText.Contains("window.initialState="))
                {
                    string json = script.InnerText.Split("window.initialState=")[1];
                    JObject jObject = JObject.Parse(json);

                    return jObject;
                }
            }

            return null!;
        }

        static string GenerateLink(string parserCategory, string domen, string userLink)
        {
            string newLink = $"{domen}/search/?addRecent=false&canChangeKeyword=false&includeSuggestions=false&sc=0a0208301a0408bbe1722a140a0b636f6c6c656374696f6e7312030a013078013204080378013a02180742060801100118004a0620012801400150005a020801&searchId=9_f3-V&searchType=all&sort_by=3";
            
            return newLink;           
        }
    }
}
