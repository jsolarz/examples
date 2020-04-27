using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Redis
{
  public class ConnectorSettingsFactory : IConnectorSettingsFactory
  {
    public JsonSerializerSettings Settings()
    {
      return new JsonSerializerSettings
      {
        TypeNameHandling = TypeNameHandling.All,
        Formatting = Formatting.Indented
      };
    }
  }
}
