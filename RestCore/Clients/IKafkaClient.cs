using System;
using System.Threading.Tasks;
using Confluent.Kafka;

namespace KafkaTest.Clients
{
    public interface IKafkaClient
    {
        Task<Message<string, string>> Produce(string key, string val);
    }
}
