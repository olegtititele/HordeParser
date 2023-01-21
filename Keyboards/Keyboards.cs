using Telegram.Bot.Types.ReplyMarkups;
using PostgreSQL;
using ConfigFile;

namespace Bot_Keyboards
{
    public static class Keyboards
    {

        // MENU
        public static InlineKeyboardMarkup MainMenuButtons = new(new []
        {
            new []
            {
                InlineKeyboardButton.WithCallbackData(text: "🪐 Парсинг", callbackData: "show_services"),
            },
            new []
            {
                InlineKeyboardButton.WithCallbackData(text: "Настройки", callbackData: "settings"),
            },
        });

        
        public static ReplyKeyboardMarkup MenuKb()
        {
            ReplyKeyboardMarkup Keyboard;
            Keyboard = new(new []
            {
                new KeyboardButton[] {"Меню"},
            })
            {
                ResizeKeyboard = true,
            };
            return Keyboard;
        }

        // SETTINGS
        public static InlineKeyboardMarkup SettingsKb()
        {
            InlineKeyboardMarkup kb;

            kb = new(new []
            {
                new []
                {
                    InlineKeyboardButton.WithCallbackData(text: "Черный список", callbackData: "black_list"),
                },
                new []
                {
                    InlineKeyboardButton.WithCallbackData(text: "Конфигурация", callbackData: "configuration"),
                },
                new []
                {
                    InlineKeyboardButton.WithCallbackData(text: "Параметры продавца", callbackData: "seller_params"),
                },
                new []
                {
                    InlineKeyboardButton.WithCallbackData(text: "🏚 В меню", callbackData: "back_to_menu"),
                },
            });

            return kb;
        }

        public static InlineKeyboardMarkup BlackListKb(long chatId)
        {
            string blackList = DB.GetBlackList(chatId);
            InlineKeyboardMarkup kb;
            InlineKeyboardButton blackListBtn;


            if(blackList == "Включить")
            {
                blackListBtn = InlineKeyboardButton.WithCallbackData(text: "Чёрный список: ON", callbackData: "black_list_status");
            }
            else
            {
                blackListBtn = InlineKeyboardButton.WithCallbackData(text: "Чёрный список: OFF", callbackData: "black_list_status");
            }

            kb = new(new []
            {
                new []
                {
                    blackListBtn,
                },
                new []
                {
                    InlineKeyboardButton.WithCallbackData(text: "Черный список ссылок", callbackData: "links_blacklist"),  
                },
                new []
                {
                    InlineKeyboardButton.WithCallbackData(text: "🗑 Удалить продавцов", callbackData: "delete_sellers"),
                },
                new []
                {
                    InlineKeyboardButton.WithCallbackData(text: "◀️ Назад", callbackData: "back_to_settings"),
                }
            });

            return kb;
        }

        public static InlineKeyboardMarkup ConfigurationKb()
        {
            InlineKeyboardMarkup kb;

            kb = new(new []
            {
                new []
                {
                    InlineKeyboardButton.WithCallbackData(text: "Текст WhatsApp", callbackData: "whatsapp_text"),
                    InlineKeyboardButton.WithCallbackData(text: "Тайм-аут", callbackData: "timeout"),  
                },
                new []
                {
                    InlineKeyboardButton.WithCallbackData(text: "◀️ Назад", callbackData: "back_to_settings"),
                }
            });

            return kb;
        }

        public static InlineKeyboardMarkup SellerSettingsKb(long chatId)
        {
            string sellerType = DB.GetSellerType(chatId);
            int sellerAds = DB.GetSellerTotalAds(chatId);
            InlineKeyboardMarkup kb;
            InlineKeyboardButton sellerTypeBtn;
            InlineKeyboardButton sellerAdsBtn;

            if(sellerType == "Частное лицо")
            {
                sellerTypeBtn = InlineKeyboardButton.WithCallbackData(text: "Тип продавца: Частные лица", callbackData: "account_type");
            }
            else
            {
                sellerTypeBtn = InlineKeyboardButton.WithCallbackData(text: "Тип продавца: Все типы", callbackData: "account_type");
            }

            if(sellerAds == 1)
            {
                sellerAdsBtn = InlineKeyboardButton.WithCallbackData(text: "Кол-во объявлений до 20: ON", callbackData: "seller_announ_count");
            }
            else
            {
                sellerAdsBtn = InlineKeyboardButton.WithCallbackData(text: "Кол-во объявлений до 20: OFF", callbackData: "seller_announ_count");
            }

            kb = new(new []
            {
                new []
                {
                    sellerTypeBtn,
                },
                new []
                {
                    sellerAdsBtn,
                },
                new []
                {
                    InlineKeyboardButton.WithCallbackData(text: "Рейтинг продавца", callbackData: "seller_rating"),
                },
                new []
                {
                    InlineKeyboardButton.WithCallbackData(text: "Количество отзывов", callbackData: "seller_feedback"),
                },
                new []
                {
                    InlineKeyboardButton.WithCallbackData(text: "Дата регистрации продавца", callbackData: "seller_reg"),
                },
                new []
                {
                    InlineKeyboardButton.WithCallbackData(text: "◀️ Назад", callbackData: "back_to_settings"),
                },
            });

            return kb;
        }

        public static InlineKeyboardMarkup RegDateKb()
        {
            DateTime today = DateTime.Today;
            InlineKeyboardMarkup regDateKb;

            regDateKb = new(new []
            {
                new []
                {
                    InlineKeyboardButton.WithCallbackData(text: $"Сегодня - {today.ToString("dd.MM.yyyy")}", callbackData: "today_date"),
                },
                new []
                {
                    InlineKeyboardButton.WithCallbackData(text: $"Отключить", callbackData: "disable_reg_date"),
                },
                new []
                {
                    InlineKeyboardButton.WithCallbackData(text: "◀️ Назад", callbackData: "back_to_seller_settings"),
                }
            });

            return regDateKb;
        }

        public static InlineKeyboardMarkup sellerRatingKb = new(new []
        {
            new []
            {
                InlineKeyboardButton.WithCallbackData(text: $"0", callbackData: "0"),
                InlineKeyboardButton.WithCallbackData(text: $"1", callbackData: "1"),
                InlineKeyboardButton.WithCallbackData(text: $"2", callbackData: "2"),
                InlineKeyboardButton.WithCallbackData(text: $"3", callbackData: "3"),
                InlineKeyboardButton.WithCallbackData(text: $"4", callbackData: "4"),
                InlineKeyboardButton.WithCallbackData(text: $"5", callbackData: "5"),
            },
            new []
            {
                InlineKeyboardButton.WithCallbackData(text: "◀️ Назад", callbackData: "back_to_seller_settings"),
            }
        });

        public static InlineKeyboardMarkup sellerFeedbackKb = new(new []
        {
            new []
            {
                InlineKeyboardButton.WithCallbackData(text: $"Отключить", callbackData: "disable_seller_feedback"),
            },
            new []
            {
                InlineKeyboardButton.WithCallbackData(text: "◀️ Назад", callbackData: "back_to_seller_settings"),
            }
        });

        public static InlineKeyboardMarkup blacklistLinksKb = new(new []
        {
            new []
            {
                InlineKeyboardButton.WithCallbackData(text: "🗑 Удалить все ссылки", callbackData: "delete_all_links"),
            },
            new []
            {
                InlineKeyboardButton.WithCallbackData(text: "◀️ Назад", callbackData: "back_to_blacklist"),
            },
        });


        // PARSER
        public static InlineKeyboardMarkup startPars = new(new []
        {
            new []
            {
                InlineKeyboardButton.WithCallbackData(text: "🎭 Начать парсинг", callbackData: "start_pars"),
            },
            new []
            {
                InlineKeyboardButton.WithCallbackData(text: "◀️ Назад", callbackData: "back_to_services_list"),
            },
        });

        // BACK KEYBOARDS
        public static InlineKeyboardMarkup backToMenu = new(new []
        {
            new []
            {
                InlineKeyboardButton.WithCallbackData(text: "🏚 В меню", callbackData: "back_to_menu"),
            },
        });
        public static InlineKeyboardMarkup backToBlackList = new(new []
        {
            new []
            {
                InlineKeyboardButton.WithCallbackData(text: "◀️ Назад", callbackData: "back_to_blacklist"),
            },
        });

        public static InlineKeyboardMarkup backToConfiguration = new(new []
        {
            new []
            {
                InlineKeyboardButton.WithCallbackData(text: "◀️ Назад", callbackData: "back_to_configuration"),
            },
        });
        public static InlineKeyboardMarkup backToSellerSettings = new(new []
        {
            new []
            {
                InlineKeyboardButton.WithCallbackData(text: "◀️ Назад", callbackData: "back_to_seller_settings"),
            },
        });

        public static InlineKeyboardMarkup backToCountries = new(new []
        {
            new []
            {
                InlineKeyboardButton.WithCallbackData(text: "◀️ Назад", callbackData: "back_to_countries"),
            },
        });
    }
}