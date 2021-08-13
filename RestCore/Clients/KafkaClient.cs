using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Confluent.Kafka;
using Confluent.Kafka.Serialization;
using Microsoft.Extensions.Configuration;

namespace KafkaTest.Clients
{
    public class KafkaClient : IKafkaClient
    {
        private Producer<string, string> producer;

        public KafkaClient(IConfiguration globalconf)
        {
            var config = new Dictionary<string, object>
            {
                { "bootstrap.servers", "172.16.94.1:9092"}

                //{ "bootstrap.servers", globalconf.GetValue<string>("Kafka:BootstrapServers") }
            };

            producer = new Producer<string, string>(config, new StringSerializer(Encoding.UTF8), new StringSerializer(Encoding.UTF8));
        }

        public async Task<Message<string, string>> Produce(string key, string val)
        {
            return await producer.ProduceAsync("test", key, val);
        }
    }
}
