namespace Modules
{
    public static class LinkGenerator
    {
        public static string GenerateWhatsAppText(string whatsapp_text, string adlink, string adname, string adprice, string adlocation, string sellername)
        {
            while(true)
            {
                if(whatsapp_text.Contains("@link"))
                {
                    whatsapp_text = whatsapp_text.Replace("@link", adlink);
                }
                else if(whatsapp_text.Contains("@title"))
                {
                    if(adname=="Не указано")
                    {
                        whatsapp_text = whatsapp_text.Replace("@title", string.Empty);
                    }
                    else
                    {
                        whatsapp_text = whatsapp_text.Replace("@title", adname);
                    }
                }
                else if(whatsapp_text.Contains("@price"))
                {
                    if(adprice=="Не указана")
                    {
                        whatsapp_text = whatsapp_text.Replace("@price", string.Empty);
                    }
                    else
                    {
                        whatsapp_text = whatsapp_text.Replace("@price", adprice);
                    }
                }
                else if(whatsapp_text.Contains("@location"))
                {
                    if(adlocation=="Не указано")
                    {
                        whatsapp_text = whatsapp_text.Replace("@location", string.Empty);
                    }
                    else
                    {
                        whatsapp_text = whatsapp_text.Replace("@location", adlocation);
                    }
                }
                else if(whatsapp_text.Contains("@seller_name"))
                {
                    if(sellername=="Не указано")
                    {
                        whatsapp_text = whatsapp_text.Replace("@seller_name", string.Empty);
                    }
                    else
                    {
                        whatsapp_text = whatsapp_text.Replace("@seller_name", sellername);
                    }
                }
                else{ return whatsapp_text; }
            }
        }
    }
}