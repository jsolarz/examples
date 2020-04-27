using Newtonsoft.Json;

namespace Redis
{
  public interface IConnectorSettingsFactory
  {
    JsonSerializerSettings Settings();
  }
}