using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Max_Gpu_Roi
{
    internal class Ebay
    {
        private string BaseUrl = "https://api.ebay.com/buy/browse/v1/item_summary/";
        public string SecretId = "PRD-42c30d68b1b3-cbfb-4ffc-afe4-308b";
        public string ClientId = "JamieHor-MaxGpuRo-PRD-f42c30d68-351f1ef5";
        public string tempAccessToken = @"v^1.1#i^1#f^0#I^3#r^0#p^1#t^H4sIAAAAAAAAAOVYa2wUVRTu9EUQWgxBUISwDpqguLN3Znd2Zwd2dWkLXehj261saTQ4M3unO+3szDD3Dm2N1U0JqOADQ0yKCVIUDRpJfUTFmDQqP8RXEIUfpv7wHRPA+ENChBidmS1lWwkg3cQmbrLZzLnnnvt93znn3rsDcpUz79hWv+1sFTGjdCgHcqUEQc8CMysrlleXlS6sKAEFDsRQ7tZc+UDZLyuRkFUNvhUiQ9cQ9PRmVQ3xrjFCWqbG6wJSEK8JWYh4LPHJWGMDz1CAN0wd65Kukp54bYQM+WlG5GBYZkMC5PzAtmoXYrbpETItsjAkMyGaAywjsgF7HCELxjWEBQ1HSAYwjBcEvAzTBoJ8IMizHMX6Qx2kZz00kaJrtgsFyKgLl3fnmgVYLw9VQAia2A5CRuOx1cnmWLy2rqltpa8gVnRMhyQWsIUmPtXoaehZL6gWvPwyyPXmk5YkQYRIXzS/wsSgfOwCmGuA70othLgw4CQuEE4DCEWxKFKu1s2sgC+Pw7Eoaa/suvJQwwruu5KithpiF5Tw2FOTHSJe63F+WixBVWQFmhGyblVsQyyRIKNrhawC63XT2yj0rjGsVt2baK31ygFG8oN0kPP6WVqmocyOLZSPNibzpJVqdC2tOKIhT5OOV0EbNZysjb9AG9upWWs2YzJ2EBX6ceMa0h1OUvNZtHBGc/IKs7YQHvfxyhkYn42xqYgWhuMRJg+4Etm5NgwlTU4edGtxrHx6UYTMYGzwPl9PTw/V46d0s9PHAED72hsbklIGZgXS9nV6Pe+vXHmCV3GpSNCeiRQe9xk2ll67Vm0AWicZDbAcCIfGdJ8IKzrZ+g9DAWffxI4oVoeE/UxIZNiACASWY9P+YnRIdKxIfQ4OKAp93qxgdkNsqIIEvZJdZ1YWmkqa97My4+dk6E0Hw7I3EJZlr8img15ahtDtVynM/Z8a5WpLPQklE+Ki1HrR6jzOtW+y2lOJOgOuNXWUSGRqMibQMKzRgy1JpkvMrO5tWieHWNwTudpuuCT5GlWxlWmz1y+GAE6vF0+Eeh1hmJ4SvaSkGzChq4rUN70S7DfTCcHEfUmoqrZhSiRjhhEvzl5dNHr/cpu4Nt7FO6P+o/PpkqyQU7LTi5UzH9kBBEOhnBOIkvSsz+l1XbCvH455o4t6SrwV++Y6rVjbJPNslXT+ykm5dCm0WaJMiHTLtG/bVLNzA2vTu6Fmn2fY1FUVmuvpKfdzNmthQVThdGvsIhS4Ikyzw5YOsiBI21O4KfGS3KN043TbkoqxFZevucZrtW/in/xoifuhB4i3wADxeilBAB+4jV4Kbqksu6e8bPZCpGBIKYJMIaVTs/+7mpDqhn2GoJillcRDjXzLiYLXCkP3gRvHXyzMLKNnFbxlAIsujlTQcxZUMQwI2N9gIMhyHWDpxdFyen75POLZA+/PodaJe46kXiGOeeCWuocHQdW4E0FUlJQPECV1N+wb+WHvgqO1Ku5f8mt107k3NfHuR1sOpfojxz+991xV9YbUsu1Pzd/oSR18L+dv2Gpt2rrb98fIvt9zX546PGj9LNeuO/VIX8PmRbUfP7Dj+/2p37I7D884P/ztT7tbu3OfbG7PZAfOGSM5taty/+j9736+rOaA/vX2Fw/tQn8OH7ZOLx9V+JbBgy93Ht1zfuhAzaOfrex46ekPhhe/c5w48vauN07ObfSj0eihE+LIky0PLgxRzSvunLdk74qyM3P3g9nD8W0/3tX12PLTo4tvSu0YvO6rZ6qP7/ymvf25LXNfRY9XLf7iiZ3lt3f+dWbDCydnfzdry7ITHx57vi4U3PHR9a/Jwtn+/puX1gcr8+n7Gw/RGoPwEQAA";
        public string RuName = "Jamie_Horton-JamieHor-MaxGpu-kayglub";
        public string AccessToken = "";
        public DateTime TokenExpirationDate;
        public string AppAccessToken = "";


        private bool GetAppAccessToken()
        {
            // Token isn't expired
            if (TokenExpirationDate.Year == DateTime.Now.Year && TokenExpirationDate.CompareTo(DateTime.Now) < 0)
                return true;
            
            // Toke expired, get a new one
            var url = "https://api.ebay.com/identity/v1/oauth2/token";

            var values = new Dictionary<string, string>
              {
                  { "grant_type", "client_credentials" },
                  { "scope", "https://api.ebay.com/oauth/api_scope"}
              };
            var payload = new FormUrlEncodedContent(values);

            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "application/x-www-form-urlencoded");
            var base64Creds = Convert.ToBase64String(Encoding.UTF8.GetBytes(ClientId + ":" + SecretId));
            client.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", "Basic " + base64Creds);   
            var response = client.PostAsync(url, payload).Result;
            var responseString = response.Content.ReadAsStringAsync().Result;
            AppAccessToken = responseString[0].ToString();

            dynamic data = JsonSerializer.Deserialize<ExpandoObject>(responseString);
            AppAccessToken = data.access_token.ToString();
            TokenExpirationDate = DateTime.Now.AddSeconds(data.expires_in.GetInt32());

            return true;
        }

        private async void RefreshAccessToken()
        {
            var url = "https://api.ebay.com/identity/v1/oauth2/token";

            var values = new Dictionary<string, string>
              {
                  { "grant_type", "refresh_token" },
                  { "refresh_token", AppAccessToken },
                  { "scope", "https://api.ebay.com/oauth/api_scope"}
              };
            var content = new FormUrlEncodedContent(values);

            HttpClient client = new HttpClient();
            var response = await client.PostAsync(url, content);

            client.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "application/x-www-form-urlencoded");
            client.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", "Basic " + Convert.ToBase64String(Encoding.UTF8.GetBytes(ClientId + ":" + SecretId)));

            var responseString = await response.Content.ReadAsStringAsync();
            AccessToken = responseString[0].ToString();

        }

       

        /// <summary>
        /// Returns the lowest priced item based on a given search phrase. Also uses filters 
        /// to only search buy it now options in the United States with users that have 
        /// at least 97% rating and more than 100 feedbacks
        /// </summary>
        /// <param name="searchPhrase"></param>
        /// <returns></returns>
        public List<EbayItem> GetLowestPrice(Gpu gpu)
        {
            var searchPhrase = gpu.Name;

            if(!GetAppAccessToken())
                return new List<EbayItem>();

            var ebayItems = new List<EbayItem>();

            /*
            if(gpu.ModelNumber == "3060" && gpu.VersionSuffix == "ti")
            {
                var come = "get me for debugging";
            }
            */

            // If nvidia and a possible lhr/non lhr confusion specify non lhr unless gpu.lhr is true
            if (gpu.Manufacturer.ToLower() == "nvidia" && int.TryParse(gpu.ModelNumber, out var parsedModelNum) 
                && parsedModelNum > 3000
                && parsedModelNum != 3050 )
            {
                if (gpu.Lhr)
                    searchPhrase += " lhr"; // Putting lhr in the search phrase 2x seems to return more accurate results
                else
                    searchPhrase += " non-lhr";
            }

            // If amd add vram size to search
            var version = gpu.VersionPrefix == null ? "" : gpu.VersionPrefix.ToLower();
            var model = gpu.ModelNumber.ToLower();
            var versionSuffix = gpu.VersionSuffix == null ? "" : gpu.VersionSuffix.ToLower();

            if (version == "r9" || model == "radeon" || model == "vega")
                { }
            else if (gpu.Manufacturer.ToLower() == "amd" && gpu.VramSize > 0)
                searchPhrase += " " + gpu.VramSize + "gb";

            // Convert all spaces to %20 for browser            
            var url = BaseUrl + "search?q=" + Uri.EscapeDataString(searchPhrase);

                try
                {
                var root = new Root();
                
                HttpWebRequest request = WebRequest.Create(url) as HttpWebRequest;
                request.Method = "GET";                
                request.Headers.Add(HttpRequestHeader.Authorization, "Bearer " + AppAccessToken);                
                request.Headers.Add(HttpRequestHeader.Accept, "application/json");
                request.Headers.Add("X-EBAY-C-MARKETPLACE-ID", "EBAY-US");
                request.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;


                var response = (HttpWebResponse)request.GetResponse();

                using (var streamReader = new StreamReader(response.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();
                    root = JsonSerializer.Deserialize<Root>(result);//.DeserializeAsync<Root>(result);
                }
                
                if (root.itemSummaries == null)
                    return ebayItems;

                var gpuModelNumber = searchPhrase.ToLower();

                // get lowest price with user rating atleast 97% and over 100 items sold 
                foreach (var item in root.itemSummaries)
                {
                    // only include buy it now options from the United States for $100 or more
                    if (item.buyingOptions.Count > 0 && item.buyingOptions[0] == "FIXED_PRICE" && item.itemLocation.country == "US"
                        && item.seller.feedbackScore > 100 && double.Parse(item.seller.feedbackPercentage) > 97
                        && double.Parse(item.price.value) > 100 && item.condition != "For parts or not working"
                        && !item.title.ToLower().Contains("cooler only") && !item.title.ToLower().Contains("fan only") && !item.title.ToLower().Contains("fans only")
                        && !item.title.ToLower().Contains("no fan") && !item.title.ToLower().Contains("missing fan")
                        && !item.title.ToLower().Contains("read description") && !item.title.ToLower().Contains("fans broken") && !item.title.ToLower().Contains("fan broken")
                        && !item.title.ToLower().Contains("artifact") && item.title.ToLower().Contains(gpu.ModelNumber)
                        && item.title.ToLower().Contains(versionSuffix))
                    {

                        // Don't get non-lhr options if looking for lhr cards
                        if (searchPhrase.ToLower().Contains("non lhr") || searchPhrase.ToLower().Contains("non-lhr")
                            || searchPhrase.ToLower().Contains("fhr") || !searchPhrase.ToLower().Contains("lhr"))
                        {
                            // non lhr card search 
                            if (item.title.ToLower().Contains("fhr") || item.title.ToLower().Contains("non-lhr")
                            || item.title.ToLower().Contains("non lhr") || item.title.ToLower().Contains("no lhr")
                            || item.title.ToLower().Contains("founders edition") || item.title.ToLower().Contains(gpuModelNumber + "fe"))
                            {
                                // this is specifically a non lhr card 
                            }
                            else if (item.title.ToLower().Contains("lhr"))
                                continue; // skip this lhr card we looking for non lhr
                        }
                        else if (searchPhrase.ToLower().Contains("lhr") && item.title.ToLower().Contains("fhr") || item.title.ToLower().Contains("non-lhr")
                            || item.title.ToLower().Contains("non lhr") || item.title.ToLower().Contains("no lhr"))
                        {
                            // searching for lhr card but this is a non lhr card skip this one                            
                            continue;
                        }

                        // If we not looking for ti and this is a ti card skit it 
                        if (!searchPhrase.ToLower().Contains("ti") && (item.title.ToLower().Contains(gpuModelNumber + " ti")
                            || item.title.ToLower().Contains(gpuModelNumber + "ti")))
                            continue;
                        // Or if this is a ti card and we looking for ti card
                        else if (searchPhrase.ToLower().Contains("ti") && (item.title.ToLower().Contains(gpuModelNumber + "ti")
                            || item.title.ToLower().Contains(gpuModelNumber + " ti")))
                        { }



                        // Get item's price + shipping costs
                        var shippingPrice = 0.0;
                        if (item.shippingOptions != null && item.shippingOptions[0].shippingCostType != "CALCULATED")
                            shippingPrice = double.Parse(item.shippingOptions[0].shippingCost.value);
                        var itemPrice = double.Parse(item.price.value) + shippingPrice;


                        var lowestEbayItem = new EbayItem();
                        var rnd = new Random();                        
                        lowestEbayItem.Id = rnd.Next(99).ToString();
                        searchPhrase.Replace("lhr lhr", "lhr"); // Remove double lhr 
                        lowestEbayItem.Name = searchPhrase;
                        lowestEbayItem.Url = item.itemWebUrl;
                        lowestEbayItem.Price = itemPrice;
                        lowestEbayItem.LastUpdated = DateTime.Now;
                        ebayItems.Add(lowestEbayItem);

                    }
                }
            }
            catch (System.Net.WebException ex)
            {
                System.Windows.Forms.MessageBox.Show("Error getting ebay price info! " + ex.Message);
            }
            ebayItems.Sort((y, x) => y.Price.CompareTo(x.Price));
            return ebayItems;
        }
        
    }


    public class Image
    {
        public string imageUrl { get; set; }
    }

    public class Price
    {
        public string value { get; set; }
        public string currency { get; set; }
    }

    public class Seller
    {
        public string username { get; set; }
        public string feedbackPercentage { get; set; }
        public int feedbackScore { get; set; }
    }

    public class ThumbnailImage
    {
        public string imageUrl { get; set; }
    }

    public class ShippingCost
    {
        public string value { get; set; }
        public string currency { get; set; }
    }

    public class ShippingOption
    {
        public string shippingCostType { get; set; }
        public ShippingCost shippingCost { get; set; }
    }

    public class ItemLocation
    {
        public string country { get; set; }
        public string postalCode { get; set; }
    }

    public class Category
    {
        public string categoryId { get; set; }
    }

    public class AdditionalImage
    {
        public string imageUrl { get; set; }
    }

    public class OriginalPrice
    {
        public string value { get; set; }
        public string currency { get; set; }
    }

    public class DiscountAmount
    {
        public string value { get; set; }
        public string currency { get; set; }
    }

    public class MarketingPrice
    {
        public OriginalPrice originalPrice { get; set; }
        public string discountPercentage { get; set; }
        public DiscountAmount discountAmount { get; set; }
        public string priceTreatment { get; set; }
    }

    public class ItemSummary
    {
        public string itemId { get; set; }
        public string title { get; set; }
        public Image image { get; set; }
        public Price price { get; set; }
        public string itemHref { get; set; }
        public Seller seller { get; set; }
        public string condition { get; set; }
        public string conditionId { get; set; }
        public List<ThumbnailImage> thumbnailImages { get; set; }
        public List<ShippingOption> shippingOptions { get; set; }
        public List<string> buyingOptions { get; set; }
        public string epid { get; set; }
        public string itemAffiliateWebUrl { get; set; }
        public string itemWebUrl { get; set; }
        public ItemLocation itemLocation { get; set; }
        public List<Category> categories { get; set; }
        public List<AdditionalImage> additionalImages { get; set; }
        public bool adultOnly { get; set; }
        public string legacyItemId { get; set; }
        public bool availableCoupons { get; set; }
        public DateTime itemCreationDate { get; set; }
        public bool topRatedBuyingExperience { get; set; }
        public bool priorityListing { get; set; }
        public string listingMarketplaceId { get; set; }
        public MarketingPrice marketingPrice { get; set; }
    }

    public class Root
    {
        public string href { get; set; }
        public int total { get; set; }
        public string next { get; set; }
        public int limit { get; set; }
        public int offset { get; set; }
        public List<ItemSummary> itemSummaries { get; set; }
    }

    public class EbayItem
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public double Price { get; set; }
        public string Url { get; set; }
        public DateTime LastUpdated { get; set; }
    }
}
