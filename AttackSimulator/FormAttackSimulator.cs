using System;
using System.Collections.Generic;
using System.Windows.Forms;
using MQTTnet;
using MQTTnet.Client;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using MQTTnet.Server;

namespace AttackSimulator
{
    public partial class FormAttackSimulator : Form
    {
        private HashSet<string> knownDevices = new();
        private IMqttClient mqttClient;
        private ComboBox cmbDeviceList;
        private ComboBox cmbAttackType;
        private NumericUpDown numRate;
        private RichTextBox rtbLog;
        private RichTextBox rtbAttackLog;
        private Button btnStartAttack;
        private System.Timers.Timer discoveryTimer;
        private bool discoveryEnded = false;

        public FormAttackSimulator()
        {
            InitializeComponent();

            cmbDeviceList = new ComboBox { Location = new System.Drawing.Point(20, 20), Width = 200 };
            cmbAttackType = new ComboBox { Location = new System.Drawing.Point(20, 60), Width = 200 };
            cmbAttackType.Items.Add("Flood");
            cmbAttackType.Items.Add("Spoof");
            cmbAttackType.SelectedIndex = 0;
            numRate = new NumericUpDown { Location = new System.Drawing.Point(20, 100), Minimum = 1, Maximum = 100, Value = 10, Width = 200 };
            btnStartAttack = new Button { Text = "Saldýrýyý Baþlat", Location = new System.Drawing.Point(20, 140), Width = 200 };
            btnStartAttack.Click += btnStartAttack_Click;
            rtbLog = new RichTextBox { Location = new System.Drawing.Point(250, 20), Width = 400, Height = 150 };
            rtbAttackLog = new RichTextBox { Location = new System.Drawing.Point(250, 190), Width = 400, Height = 130 };

            Controls.Add(cmbDeviceList);
            Controls.Add(cmbAttackType);
            Controls.Add(numRate);
            Controls.Add(btnStartAttack);
            Controls.Add(rtbLog);
            Controls.Add(rtbAttackLog);

            InitMQTTDeviceSniffer();
        }

        private async void InitMQTTDeviceSniffer()
        {
            var factory = new MQTTnet.MqttFactory();
            mqttClient = factory.CreateMqttClient();

            mqttClient.ApplicationMessageReceivedAsync += e =>
            {
                string topic = e.ApplicationMessage.Topic;
                string payload = Encoding.UTF8.GetString(e.ApplicationMessage.Payload);
                Log($"MQTT Mesajý: {topic} -> {payload}");

                string[] parts = topic.Split('/');
                if (parts.Length >= 3)
                {
                    string deviceType = parts[0];  // sensor / camera / etc
                    string deviceId = parts[1];    // SENSOR_001 etc
                    AddDeviceToCombo(deviceType, deviceId);
                }

                return Task.CompletedTask;
            };

            var options = new MqttClientOptionsBuilder().WithTcpServer("localhost", 1883).Build();

            await mqttClient.ConnectAsync(options);
            await mqttClient.SubscribeAsync("#");
            Log("Cihaz tarayýcý baþlatýldý. Tüm topic'ler dinleniyor (#)");

            discoveryTimer = new System.Timers.Timer(5 * 60 * 1000); // 5 dakika
            discoveryTimer.Elapsed += (s, e) =>
            {
                discoveryEnded = true;
                Log("[SÝSTEM] Cihaz keþif süresi sona erdi. Dinleme sona erdi.");
                discoveryTimer.Stop();
            };
            discoveryTimer.AutoReset = false;
            discoveryTimer.Start();
        }

        private void AddDeviceToCombo(string deviceType, string deviceId)
        {
            if (!knownDevices.Contains(deviceId))
            {
                knownDevices.Add(deviceId);
                string displayName = $"{deviceId} [{deviceType}]";

                if (cmbDeviceList.InvokeRequired)
                {
                    cmbDeviceList.Invoke(new Action(() => cmbDeviceList.Items.Add(displayName)));
                }
                else
                {
                    cmbDeviceList.Items.Add(displayName);
                }

                Log($"Yeni cihaz tespit edildi: {displayName}");
            }
        }

        private async void btnStartAttack_Click(object sender, EventArgs e)
        {
            if (cmbDeviceList.SelectedItem == null)
            {
                Log("[HATA] Lütfen hedef cihaz ID’si seçin.");
                return;
            }

            string selectedItem = cmbDeviceList.SelectedItem.ToString();
            string deviceId = selectedItem.Split(' ')[0]; // "SENSOR_001" kýsmýný al
            int rate = (int)numRate.Value;
            string attackType = cmbAttackType.SelectedItem.ToString();

            IAttackModule attack = attackType switch
            {
                "Flood" => new FloodAttack(deviceId, rate, Log),
                "Spoof" => new DeviceIdSpoofAttack(deviceId, rate, Log),
                _ => null
            };

            if (attack != null)
            {
                LogAttack($"[SÝSTEM] Saldýrý baþlatýldý: {attackType} ({deviceId})");
                await attack.ExecuteAsync();
                LogAttack("[SÝSTEM] Saldýrý tamamlandý.");
            }
        }

        private void Log(string message)
        {
            string timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            string fullMessage = $"[{timestamp}] {message}\n";

            if (rtbLog.InvokeRequired)
                rtbLog.Invoke(new Action(() => rtbLog.AppendText(fullMessage)));
            else
                rtbLog.AppendText(fullMessage);
        }

        private void LogAttack(string message)
        {
            string timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            string fullMessage = $"[{timestamp}] {message}\n";

            if (rtbAttackLog.InvokeRequired)
                rtbAttackLog.Invoke(new Action(() => rtbAttackLog.AppendText(fullMessage)));
            else
                rtbAttackLog.AppendText(fullMessage);
        }
    }
}




