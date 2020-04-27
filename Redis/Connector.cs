using Newtonsoft.Json;
using StackExchange.Redis;
using StackExchange.Redis.Extensions.Core.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Redis
{
  /// <summary>
  /// Redis database connector
  /// </summary>
  public class Connector : IRedisConnector, IDisposable
  {
    private static readonly Lazy<ConnectionMultiplexer> LazyConnection = new Lazy<ConnectionMultiplexer>(() =>
    {
      // Get the configuration section from web.config
      RedisCachingSectionHandler config = (RedisCachingSectionHandler)System.Configuration.ConfigurationManager.GetSection(Constants.REDIS_CONFIG_SECTION);
      var configOptions = new ConfigurationOptions
      {
        AllowAdmin = config.AllowAdmin,
        ClientName = "",
        AbortOnConnectFail = false,
        //ConnectRetry = 5,
        ConnectTimeout = config.ConnectTimeout,
        DefaultDatabase = config.Database,
        //ResponseTimeout = config.ConnectTimeout,
        Ssl = config.Ssl,
        EndPoints =
            {
                {
                    config.RedisHosts[0].Host,
                    config.RedisHosts[0].CachePort
                }
            }
      };

      return ConnectionMultiplexer.Connect(configOptions);
    });
    private static ConnectionMultiplexer ConnectionMultiplexer => LazyConnection.Value;

    private readonly JsonSerializerSettings _settings;

    public Connector(IConnectorSettingsFactory settingsFactory)
    {
      _settings = settingsFactory.Settings();
      _settings.Formatting = Formatting.Indented;
      _settings.TypeNameHandling = TypeNameHandling.All;
    }

    public bool Add<T>(string key, T value, DateTimeOffset expiresAt) where T : class
    {
      var serializedObject = JsonConvert.SerializeObject(value, _settings);
      var expiration = expiresAt.Subtract(DateTimeOffset.Now);
      if (expiration.TotalMilliseconds < 0)
        return true;
      return ConnectionMultiplexer.GetDatabase().StringSet(key, serializedObject, expiration);
    }

    public T Get<T>(string key) where T : class
    {
      RedisValue serializedObject = ConnectionMultiplexer.GetDatabase().StringGet(key);

      try
      {
        return JsonConvert.DeserializeObject<T>(serializedObject, _settings);
      }
      catch (Exception ex)
      {
        return null;
      }
    }

    public bool Remove(string key)
    {
      return ConnectionMultiplexer.GetDatabase().KeyDelete(key);
    }

    public bool Exists(string key)
    {
      return ConnectionMultiplexer.GetDatabase().KeyExists(key);
    }

    public void Dispose()
    {
      //_connectionMultiplexer?.Dispose();
    }
  }


}
