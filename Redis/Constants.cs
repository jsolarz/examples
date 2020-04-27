using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Redis
{
  public static class Constants
  {
    public const string REDIS_CONFIG_SECTION = "RedisCacheClient";
    public const double CACHE_EXPIRATION = 2; //days of expiration

    public static class CacheKeys
    {
      public static string CONTENT => $"{Prefix}:Content:Models";

      public static string ORDER_COUNTS => $"{Prefix}:OrderCounts";
      public static string PRODUCTS => $"{Prefix}:Products";
      public static string REGIONS => $"{Prefix}:Regions";
      public static string CITIES => $"{Prefix}:Cities";
      public static string CATEGORIES => $"{Prefix}:Categories";
      public static string LANDING_PAGE => $"{Prefix}:Content:LandingPage";
      public static string LANDING_PAGE_V2 => $"{Prefix}:Content:LandingPageV2";
      public static string LANDING_PAGE_REGION => $"{Prefix}:Content:LandingPageRegion";
      public static string SUPLIER_PAGE => $"{Prefix}:Content:SupplierPage";
      public static string SITE => $"{Prefix}:Content:Site";
      public static string LANDING_PAGES => $"{Prefix}:Content:SiteContent";
      public static string VARIANT_GROUPS => $"{Prefix}:Products:VariantGroups";
      public static string CUSTOMER_LAST_ORDER => $"{Prefix}:Customers:LastOrder";

      public static string PRODUCT_REVIEWS => $"{Prefix}:Products:Reviews:{{0}}";

      // Product List Cache
      public static string PRODUCT_LIST_KEYS => $"{Prefix}:Products:Keys:Lists";
      public static string PRODUCT_LISTS => $"{Prefix}:Products:Lists:{{0}}";

      // Product Prices
      public static string PRODUCT_PRICE => $"{Prefix}:Products:Prices:{{0}}";

      public static string ORDERS => $"{Prefix}:Orders";

      public static string FAQ_SUBJECTS => $"{Prefix}:Faq:Subjects";
      public static string FAQ_QUESTIONS => $"{Prefix}:Faq:Questions";
      public static string CUSTOMER_PREMIUM => $"{Prefix}:Customers:Premium";

      public static string SUPPLIERS => $"{Prefix}:Suppliers";

      private static string Prefix => System.Configuration.ConfigurationManager.AppSettings[".Redis.KeyPrefix"];
    }
  }
}