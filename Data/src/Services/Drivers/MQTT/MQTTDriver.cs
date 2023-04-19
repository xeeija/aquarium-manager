using DAL.Influx;
using DAL.MongoDB.Entities;
using DAL.MongoDB.Entities.Devices;
using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Extensions.ManagedClient;
using MQTTnet.Packets;
using Newtonsoft.Json;

namespace Services.Drivers
{
    public class MQTTDriver : Driver
    {
        IManagedMqttClient Client;

        private MQTTDevice Source;
        String FinalUrl;
        List<MQTTDataPoint> MQTTDataPoints = new List<MQTTDataPoint>();
        public MQTTDriver(MQTTDevice src, List<MQTTDataPoint> datapoints) : base(src.DeviceName)
        {
            this.Source = src;
            this.MQTTDataPoints = datapoints;
        }

        public async override Task Connect()
        {
            log.Information("Created Client - trying to connect to " + FinalUrl);


            MqttFactory mqttFactory = new MqttFactory();
            Client = mqttFactory.CreateManagedMqttClient();

            Client.ApplicationMessageReceivedAsync += Client_ApplicationMessageReceivedAsync;
            Client.ConnectedAsync += Client_ConnectedAsync;
            Client.DisconnectedAsync += Client_DisconnectedAsync;

            var mqttClientOptions = new MqttClientOptionsBuilder()
               .WithTcpServer(Source.Host, Source.Port)
               .WithClientId(Source.DeviceName)
               .Build();


            List<MqttTopicFilter> topics = new List<MqttTopicFilter>();
            foreach (MQTTDataPoint dpi in this.MQTTDataPoints)
            {
                if (dpi.GetType() == typeof(MQTTDataPoint))
                {
                    MQTTDataPoint dp = (MQTTDataPoint)dpi;
                    MqttTopicFilter topicFilterAnalog = new MqttTopicFilter { Topic = dp.TopicName, QualityOfServiceLevel = MQTTnet.Protocol.MqttQualityOfServiceLevel.AtLeastOnce };

                    log.Debug("Adding Topic " + dp.TopicName + " to subscription");
                    topics.Add(topicFilterAnalog);

                    AddDataPoint(dp.TopicName, dp);
                }
            }


            await this.Client.SubscribeAsync(topics.ToArray());

            var managedMqttClientOptions = new ManagedMqttClientOptionsBuilder()
        .WithClientOptions(mqttClientOptions)
        .Build();

            await Client.StartAsync(managedMqttClientOptions);



        }

        private Task Client_DisconnectedAsync(MqttClientDisconnectedEventArgs obj)
        {
            if (obj != null && obj.ConnectResult != null)
            {
                log.Information("Disconnected from " + FinalUrl + ": " + obj.ConnectResult.ReasonString);
            }
            IsConnected = false;

            return Task.CompletedTask;
        }

        private Task Client_ConnectedAsync(MqttClientConnectedEventArgs arg)
        {
            log.Information("Connected to " + FinalUrl);
            // obj.ConnectResult
            IsConnected = true;
            return Task.CompletedTask;
        }

        private Task Client_ApplicationMessageReceivedAsync(MqttApplicationMessageReceivedEventArgs obj)
        {
            String message = obj.ApplicationMessage.ConvertPayloadToString();

            log.Verbose("Topic " + obj.ApplicationMessage.Topic + " received " + message);

            try
            {
                MQTTItem converted = JsonConvert.DeserializeObject<MQTTItem>(message);

                if (converted != null)
                {

                    MQTTDataPoint dp = (MQTTDataPoint)GetDataPoint(obj.ApplicationMessage.Topic);

                    if (dp != null)
                    {
                        Sample sample = null;
                        if (dp.DataType == DataType.Float)
                        {
                            sample = new NumericSample();
                            float val = (float)(Convert.ToDouble(converted.Value) / dp.Offset);
                            sample.Value = dp;
                        }
                        else
                        {
                            sample = new BinarySample();
                            sample.Value = converted.Value;
                        }

                        //https://www.epochconverter.com/

                        sample.TimeStamp = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddSeconds(converted.TimeStamp);

                        sample.Tag = dp.Name;

                        if (sample.GetType() == typeof(NumericSample))
                        {
                            AddNumericMeasurement(dp.Name, sample as NumericSample);
                        }
                        else
                        {
                            AddBinaryMeasurement(dp.Name, sample as BinarySample);
                        }

                    }
                }

            }
            catch (Exception e)
            {
                log.Fatal("Could not convert Payload to JSON Object");
            }

            return Task.CompletedTask;
        }





        public async override Task Disconnect()
        {
            try
            {

                log.Information("Stopping Client");
                if (Client != null)
                {
                    Client.StopAsync();
                    Client.Dispose();
                }
            }
            catch (Exception e)
            {
                log.Warning("Stopping failed " + e.ToString());
            }
        }


    }
}
