using AttackSimulator;
using MQTTnet;
using MQTTnet.Client;
using System.Text;
using System.Threading.Tasks;

public class FloodAttack : IAttackModule
{
    private readonly string deviceId;
    private readonly int rate;
    private readonly Action<string> log;

    public FloodAttack(string id, int msgRate, Action<string> logAction)
    {
        deviceId = id;
        rate = msgRate;
        log = logAction;
    }

    public async Task ExecuteAsync()
    {
        var factory = new MQTTnet.MqttFactory();
        var client = factory.CreateMqttClient();

        var options = new MqttClientOptionsBuilder()
            .WithTcpServer("localhost", 1883)
            .Build();

        await client.ConnectAsync(options);

        for (int i = 0; i < 20; i++) // 20 paketlik mini flood
        {
            var payload = $"{{\"deviceId\":\"{deviceId}\",\"temp\":999}}";
            var message = new MqttApplicationMessageBuilder()
                .WithTopic($"sensors/{deviceId}/data")
                .WithPayload(Encoding.UTF8.GetBytes(payload))
                .Build();

            await client.PublishAsync(message);
            log($"Flood mesajı gönderildi -> {deviceId}");
            await Task.Delay(1000 / rate);
        }

        await client.DisconnectAsync();
    }
}
