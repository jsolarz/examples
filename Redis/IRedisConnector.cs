using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Redis
{
  public interface IRedisConnector
  {
    bool Add<T>(string key, T value, DateTimeOffset expiresAt) where T : class;
    T Get<T>(string key) where T : class;
    bool Remove(string key);
    bool Exists(string key);
  }
}
