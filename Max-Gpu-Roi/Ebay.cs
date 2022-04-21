using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Max_Gpu_Roi
{
    internal class Ebay
    {
        private string BaseUrl = "https://api.ebay.com/buy/browse/v1/item_summary/";
        public string AccessToken = @"v^1.1#i^1#I^3#p^1#f^0#r^0#t^H4sIAAAAAAAAAOVYbWwURRjuXq/FgpWaQiENCcfyERR2b3bve+UuHv2gV2l75UqtJQR392bbhbvd7c6cbcVgrXzoD0PEqBgkNjVNQKOEpAQVP6LFSEwMGtMY/WEAgxKNGguhxgTi7N1RrpUA0kts4v25zDvvvPM8z7zvzOyAvuKS+3fX7R4vpWbZBvpAn42iuDmgpLho1T2FtsqiApDjQA30Leuz9xdeWIPEZMIQNkBk6BqCjp5kQkNC2hikU6Ym6CJSkaCJSYgELAuxcMN6gWeBYJg61mU9QTsi1UE6ICl+RVTcbjf0QxiQiFW7FrNFJ/0ulyxC0Q2UuER8fKQfoRSMaAiLGg7SPOB5BrgZnmvhfIIHCBzPch6+nXa0QhOpukZcWECH0nCF9FgzB+vNoYoIQROTIHQoEq6NNYUj1TWNLWucObFCWR1iWMQpNLlVpceho1VMpODNp0FpbyGWkmWIEO0MZWaYHFQIXwNzB/AzUrugAjxSnAMS5GQA8yJlrW4mRXxzHJZFjTNK2lWAGlZx760UJWpIW6GMs61GEiJS7bD+mlNiQlVUaAbpmrXhR8LRKB2qF5MqrNNNpkHsWWekNuhMdEM1o7h52QXiXj/j8nAKBxVPdqJMtKzMU2aq0rW4aomGHI06XgsJajhVG3eONsSpSWsywwq2EOX6Ba5p6Pa3W4uaWcUU7tSsdYVJIoQj3bz1CkyMxthUpRSGExGmdqQlCtKiYahxempnOhez6dODgnQnxobgdHZ3d7PdLlY3O5w8AJyzrWF9TO6ESZEmvlatZ/zVWw9g1DQVmeQW8Rdwr0Gw9JBcJQC0Djrk9vhBwJfVfTKs0FTrPww5nJ2TKyJvFeLngcxzMkeKRAQ+Lh8VEsomqdPCASWxl0mK5jaIjYQoQ0YmeZZKQlONCy6Pwrv8CmTi3oDCuAOKwkieuJfhFAgBhJIkB/z/p0K53VSPQdmEOC+5nrc8j/jbulJtD0drDFhv6iga7azqNIGGYZXubY7xW6XO2p7GhxSfB3cHb7cabki+KqESZVrI/PkQwKr1/IlQpyMM49OiF5N1A0b1hCr3zqwFdpnxqGji3hhMJIhhWiTDhhHJz16dN3r/cpu4M975O6P+o/PphqyQlbIzi5U1HpEAoqGy1gnEynrSadW6LpLrh2XekkY9Ld4qubnOKNaEZIatGs9cOdk0XRY9JrMmRHrKJLdttsm6gbXo26BGzjNs6okENFu5addzMpnCopSAM62w85DgqjjDDlvO6wEer8vN8dPiJaeP0i0zbUvKx1ZsX3eH12rn5I/8UEH6x/VTx0A/ddRGUcAJlnNLwZLiwo32wrsrkYohq4oKi9QOjXy7mpDdBnsNUTVtxdSOBqF5NOdZYWAzWDjxsFBSyM3JeWUAi673FHFzF5TyPHDzHOfzAI5vB0uv99q5Cvu8+pYrm587fh8Xe7P28M/y6OVl7uM7QOmEE0UVFdj7qYLy78bm4hWtz3qVQxWh/fsHjz3aVYk63o0dMV7+4NKhA690jH/YNqi1D8lB73sXR+bVnNp+fGBpdV3X4oUd5w/+csi/vezsW5XdO3/aU/Lg/PEL5dzqr55YEC5cPPz+7EXl8MQeqbxsfPXVN4bOrYBntu+ofSc0EPx12HdgvmPTxpG9e/849s2pwU1N0fFnnrY91T18oqxi89ng6+c+Drx0unZw35KVz387su/JPz8a/HLWydO7jpz89PD3/F/z1v/eN0z9cPHMVdw857WosJL5pMc2K7rz6F0Xxj5fUTo0eln4+rcmW4U0Mno+tOrKAy/UL770xY9ju+YvD9hLu+Y2zz749upXhz6rfvxFquZed1lm+f4GIs0CmvARAAA=";
        
        /// <summary>
        /// Returns the lowest priced item based on a given search phrase. Also uses filters 
        /// to only search buy it now options in the United States with users that have 
        /// at least 97% rating and more than 100 feedbacks
        /// </summary>
        /// <param name="searchPhrase"></param>
        /// <returns></returns>
        public async Task<EbayItem> GetLowestPrice(string searchPhrase)
        {
            var ebayItem = new EbayItem();

            // Convert all spaces to %20 for browser            
            var url = BaseUrl + "search?q=" + Uri.EscapeDataString(searchPhrase);

            try
            {
                HttpWebRequest request = WebRequest.Create(url) as HttpWebRequest;
                request.Method = "GET";
                request.Headers.Add(HttpRequestHeader.Authorization, "Bearer " + AccessToken);
                request.Headers.Add(HttpRequestHeader.Accept, "application/json");
                request.Headers.Add("X-EBAY-C-MARKETPLACE-ID", "EBAY-US");
                request.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
                

                var response = (HttpWebResponse)request.GetResponse();
                var root = new Root();
                using (var streamReader = new StreamReader(response.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();
                    root = JsonSerializer.Deserialize<Root>(result);//.DeserializeAsync<Root>(result);
                }

                // get lowest price with user rating atleast 97% and over 100 items sold 
                foreach (var item in root.itemSummaries)
                {
                    // only include buy it now options from the United States
                    if (item.buyingOptions.Count > 0 && item.buyingOptions[0] == "FIXED_PRICE" && item.itemLocation.country == "US"
                        && item.seller.feedbackScore > 100 && double.Parse(item.seller.feedbackPercentage) > 97)
                    {
                        // Get item's price + shipping costs
                        var shippingPrice = 0.0;
                        if (item.shippingOptions != null && item.shippingOptions[0].shippingCostType != "CALCULATED")
                            shippingPrice = double.Parse(item.shippingOptions[0].shippingCost.value);
                        var itemPrice = double.Parse(item.price.value) + shippingPrice;

                        // If this item is lower than the previous one, it is the new lowest option
                        if (itemPrice <= ebayItem.Price || ebayItem.Price == 0)
                        {
                            var lowestEbayItem = new EbayItem();
                            lowestEbayItem.Id = item.itemId;
                            lowestEbayItem.Name = searchPhrase;
                            lowestEbayItem.Url = item.itemWebUrl;
                            lowestEbayItem.Price = double.Parse(item.price.value);
                            lowestEbayItem.LastUpdated = DateTime.Now;
                            ebayItem = lowestEbayItem;
                        }
                    }
                }
            }
            catch (System.Net.WebException ex)
            {
                System.Windows.Forms.MessageBox.Show("Error getting ebay price info! " + ex.Message);
            }
            return ebayItem;
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
