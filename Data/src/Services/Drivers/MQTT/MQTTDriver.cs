using DAL.Drivers;
using DAL.Influx.Samples;
using DAL.MongoDB.Entities;
using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Extensions.ManagedClient;
using MQTTnet.Packets;
using Newtonsoft.Json;

namespace Services.Drivers;

public class MQTTDriver : Driver
{
  IManagedMqttClient Client;

  string FinalUrl;
  List<MQTTDataPoint> MQTTDataPoints = new();
  private MQTTDevice Source;

  public MQTTDriver(MQTTDevice src, List<MQTTDataPoint> datapoints) : base(src.Name)
  {
    Source = src;
    MQTTDataPoints = datapoints;
  }

  public async override Task Connect()
  {
    log.Information("Created Client - trying to connect to " + FinalUrl);

    var mqttFactory = new MqttFactory();
    Client = mqttFactory.CreateManagedMqttClient();

    Client.ApplicationMessageReceivedAsync += Client_ApplicationMessageReceivedAsync;
    Client.ConnectedAsync += Client_ConnectedAsync;
    Client.DisconnectedAsync += Client_DisconnectedAsync;

    var mqttClientOptions = new MqttClientOptionsBuilder()
      .WithTcpServer(Source.Host, Source.Port)
      .WithClientId(Source.Name)
      .Build();

    List<MqttTopicFilter> topics = new();
    foreach (MQTTDataPoint point in MQTTDataPoints)
    {
      if (point.GetType() == typeof(MQTTDataPoint))
      {
        // MQTTDataPoint mqttPoint = (MQTTDataPoint)point;
        var topicFilterAnalog = new MqttTopicFilter()
        {
          Topic = point.Topic,
          QualityOfServiceLevel = MQTTnet.Protocol.MqttQualityOfServiceLevel.AtLeastOnce
        };

        log.Debug("Adding Topic " + point.Topic + " to subscription");
        topics.Add(topicFilterAnalog);

        AddDataPoint(point.Topic, point);
      }
    }

    await Client.SubscribeAsync(topics.ToArray());

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
    string message = obj.ApplicationMessage.ConvertPayloadToString();

    log.Verbose("Topic " + obj.ApplicationMessage.Topic + " received " + message);

    try
    {
      var converted = JsonConvert.DeserializeObject<MQTTItem>(message);

      if (converted != null)
      {
        MQTTDataPoint point = (MQTTDataPoint)GetDataPoint(obj.ApplicationMessage.Topic);

        if (point != null)
        {
          Sample sample = null;
          if (point.DataType == DataType.Float)
          {
            // sample = new NumericSample();
            // float val = (float)(Convert.ToDouble(converted.Value) / point.Offset);
            // sample.Value = point;
            sample = new NumericSample()
            {
              Value = Convert.ToSingle(Convert.ToDouble(converted.Value) / point.Offset),
            };
          }
          else
          {
            sample = new BinarySample()
            {
              Value = converted.Value
            };
          }

          //https://www.epochconverter.com/
          sample.Timestamp = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddSeconds(converted.Timestamp);
          sample.Tag = point.Name;

          if (sample.GetType() == typeof(NumericSample))
          {
            AddNumericMeasurement(point.Name, sample as NumericSample);
          }
          else
          {
            AddBinaryMeasurement(point.Name, sample as BinarySample);
          }

        }
      }

    }
    catch (Exception ex)
    {
      log.Fatal("Could not convert Payload to JSON Object " + ex.Message);
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
        await Client.StopAsync();
        Client.Dispose();
      }
    }
    catch (Exception ex)
    {
      log.Warning("Stopping failed " + ex.Message);
    }
  }

}
