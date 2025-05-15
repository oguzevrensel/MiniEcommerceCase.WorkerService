using MiniEcommerceCase.WorkerService.Interfaces;
using StackExchange.Redis;

namespace MiniEcommerceCase.WorkerService.Services
{
    public class RedisLogger : IRedisLogger
    {
        private readonly ConnectionMultiplexer _redis;

        public RedisLogger()
        {
            _redis = ConnectionMultiplexer.Connect("localhost:6379");
        }

        public async Task LogProcessedAsync(Guid orderId)
        {
            var db = _redis.GetDatabase();
            var key = $"order:{orderId}";
            var value = $"processed at {DateTime.UtcNow:O}";

            await db.StringSetAsync(key, value, TimeSpan.FromMinutes(5));
        }
    }
}
