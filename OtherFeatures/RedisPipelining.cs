using StackExchange.Redis;
using System.Data;

namespace OtherFeatures
{
    public class RedisPipelining
    {
        public async Task<List<string>> Pipelining()
        {
            ConnectionMultiplexer redis = await ConnectionMultiplexer.ConnectAsync("localhost:6379");

            IDatabase db = redis.GetDatabase();

            // Pipeline oluştur
            IBatch pipeline = db.CreateBatch();

            // Pipeline'e komutları ekle
            RedisKey key = "key";
            RedisKey key2 = "key2";
            await pipeline.StringSetAsync(key, "value1");
            await pipeline.StringSetAsync(key2, "value2");
            await pipeline.StringGetAsync(key);
            await pipeline.StringGetAsync(key2);

            // Pipeline'i çalıştır ve sonuçları al
            //var a = await pipeline.ExecuteAsync();
            pipeline.Execute();

            var result = await pipeline.ExecuteAsync(CommandFlags.None.ToString());
            var result1 = await pipeline.ExecuteAsync(CommandBehavior.SequentialAccess.ToString());
            List<string> strings = new List<string>();
            for (int i = 0; i < result.Length; i++)
            {
                strings.Add(result[i].ToString());
            }

            return strings;
        }
    }
}
