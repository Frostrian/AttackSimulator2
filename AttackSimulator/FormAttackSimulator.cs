
using System;
using System.Collections.Generic;
using System.Windows.Forms;
using MQTTnet;
using MQTTnet.Client;
using System.Text;
using System.Threading.Tasks;

namespace AttackSimulator
{
    public partial class FormAttackSimulator : Form
    {
        private HashSet<string> knownDevices = new();
        private IMqttClient mqttClient;
        public FormAttackSimulator()
        {
            InitializeComponent();
            cmbAttackType.SelectedIndex = 0;
            InitMQTTDeviceSniffer();
            this.cmbDeviceList = new System.Windows.Forms.ComboBox();
            this.cmbDeviceList.Location = new System.Drawing.Point(...);
            this.Controls.Add(this.cmbDeviceList);
        }
        private async void InitMQTTDeviceSniffer()
        {
            var factory = new MQTTnet.MqttFactory();  // doðru namespace
            var client = factory.CreateMqttClient();

            mqttClient.ApplicationMessageReceivedAsync += e =>
            {
                string topic = e.ApplicationMessage.Topic;
                string[] parts = topic.Split('/');
                if (parts.Length >= 3 && parts[0] == "sensors")
                {
                    string deviceId = parts[1];
                    if (!knownDevices.Contains(deviceId))
                    {
                        knownDevices.Add(deviceId);
                        Invoke(() => cmbDeviceList.Items.Add(deviceId));
                        Log($"Yeni cihaz tespit edildi: {deviceId}");
                    }
                }
                return Task.CompletedTask;
            };

            var options = new MqttClientOptionsBuilder().WithTcpServer("localhost", 1883).Build();

            await client.ConnectAsync(options);
            await mqttClient.SubscribeAsync("sensors/+/data");
            Log("Cihaz tarayýcý baþlatýldý.");
        }

        private async void btnStartAttack_Click(object sender, EventArgs e)
        {
            if (cmbDeviceList.SelectedItem == null)
            {
                Log("[HATA] Lütfen hedef cihaz ID’si seçin.");
                return;
            }

            string deviceId = cmbDeviceList.SelectedItem.ToString();
            int rate = (int)numRate.Value;
            string attackType = cmbAttackType.SelectedItem.ToString();

            IAttackModule attack = attackType switch
            {
                "Flood" => new FloodAttack(deviceId, rate, Log),
                _ => null
            };

            if (attack != null)
            {
                Log($"[SYSTEM] Baþlatýldý: {attackType} ({deviceId})");
                await attack.ExecuteAsync();
                Log("[SYSTEM] Saldýrý tamamlandý.");
            }
        }



        private void Log(string message)
        {
            string timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            rtbLog.AppendText($"[{timestamp}] {message}\n");
        }

        private void FormAttackSimulator_Load(object sender, EventArgs e)
        {
            cmbAttackType.Items.Add("Flood");
        }
    }
}
