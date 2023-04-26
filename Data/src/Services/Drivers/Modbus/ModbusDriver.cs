using System.Net.Sockets;
using System.Timers;
using DAL.Drivers;
using DAL.Influx.Samples;
using DAL.MongoDB.Entities;
using Modbus.Device;
using Modbus.Utility;
using ModbusDevice = DAL.Drivers.ModbusDevice;
using Timer = System.Timers.Timer;

namespace Services.Drivers
{

  public class ModbusDriver : Driver
  {
    TcpClient Client;
    ModbusIpMaster Master;

    private ModbusDevice Source;
    string FinalUrl;
    List<ModbusDataPoint> ModbusDataPoints = new();

    System.Timers.Timer FetchTimer = new();


    public ModbusDriver(ModbusDevice src, List<ModbusDataPoint> datapoints) : base(src.Name)
    {
      Source = src;
      ModbusDataPoints = datapoints;
    }

    public async override Task Connect()
    {
      try
      {
        FinalUrl = Source.Host + ":" + Source.Port;

        Client = new TcpClient(Source.Host, Source.Port);
        Master = ModbusIpMaster.CreateIp(Client);

        FetchTimer = new Timer(10000) { Enabled = true };
        FetchTimer.Elapsed += FetchTimerElapsed;

        await Read();

      }
      catch (Exception ex)
      {
        log.Fatal("Could not create Master ", ex);
      }
      IsConnected = Client.Connected;

      foreach (ModbusDataPoint point in ModbusDataPoints)
      {
        AddDataPoint(point.Name, point);
      }

    }

    private async void FetchTimerElapsed(object? sender, ElapsedEventArgs e)
    {
      await Read();
    }

    public async override Task Disconnect()
    {
      try
      {

        log.Information("Stopping Client");
        Client?.Dispose();

        if (FetchTimer != null)
        {
          FetchTimer.Dispose();
          FetchTimer = null;
        }
      }
      catch (Exception ex)
      {
        log.Warning("Stopping failed " + ex.ToString());
      }
    }

    public async Task Read()
    {
      if (IsConnected)
      {
        foreach (ModbusDataPoint point in ModbusDataPoints)
        {

          if (point.RegisterCount > 0 && point.Register >= 0)
          {
            Sample sample = await GetModbusVal(point);

            if (sample != null)
            {

              if (sample.GetType() == typeof(NumericSample))
              {
                AddNumericMeasurement(point.Name, sample as NumericSample);
              }
              else
              {
                AddBinaryMeasurement(point.Name, sample as BinarySample);
              }


              log.Debug($"Got {sample.Value} from {point.Name} ");
            }
          }
          else
          {
            log.Error("Cannot Read " + point.Name + " Register or Offset 0");
          }
        }
      }
    }

    private NumericSample DecodeNumeric(ushort[] register, ModbusDataPoint pt)
    {
      NumericSample sample = new NumericSample();

      if (pt.DataType == DataType.Float)
      {

        if (register.Length > 1)
        {
          if (register.Length == 2)
          {
            ushort register0 = register[0];
            ushort register1 = register[1];

            if (pt.ReadingType == ReadingType.HighToLow)
            {
              register0 = register[1];
              register1 = register[0];
            }

            sample.Value = ModbusUtility.GetSingle(register0, register1);

            log.Verbose($"Reading {this.Name} Quantity 2 - Address {pt.Register}/{pt.RegisterCount} returned {sample.Value}");
          }
          else
          {
            sample.Value = 0;
            log.Verbose($"Reading {this.Name} Quantity 2 - Address {pt.Register}/{pt.RegisterCount} Not found");
          }
        }
        else if (register.Length == 1)
        {
          ushort register0 = register[0];
          sample.Value = Convert.ToDouble(register0);
        }
        else
        {
          sample.Value = 0;
        }

        if (pt.Offset > 0)
        {
          sample.Value = Convert.ToDouble(sample.Value) / pt.Offset;
        }
      }

      return sample;
    }

    private async Task<Sample> GetModbusVal(ModbusDataPoint pt)
    {
      Sample sample = null;

      ushort start = Convert.ToUInt16(pt.Register);
      ushort offset = Convert.ToUInt16(pt.RegisterCount);
      try
      {
        if (pt.RegisterType == RegisterType.Coil)
        {
          sample = new BinarySample();
          bool[] returnv = await Master.ReadCoilsAsync(start, 1);

          if (returnv != null && returnv.Length > 0)
          {
            sample.Value = Convert.ToBoolean(returnv[0]);
          }
          else
          {
            log.Warning("Modbus Point " + pt.Name + " could not be found: Register : " + start + " Offset: " + offset);
            sample.Value = false;
          }

        }
        else if (pt.RegisterType == RegisterType.InputStatus)
        {
          sample = new BinarySample();
          ushort[] returnv = await Master.ReadInputRegistersAsync(start, offset);

          if (returnv != null && returnv.Length > 0)
          {
            sample.Value = Convert.ToBoolean(returnv[0]);
          }
          else
          {
            log.Warning("Modbus Point " + pt.Name + " could not be found: Register : " + start + " Offset: " + offset);
            sample.Value = false;
          }

        }
        else if (pt.RegisterType == RegisterType.InputRegister)
        {
          sample = new NumericSample();

          ushort[] register = await Master.ReadInputRegistersAsync(start, offset);

          sample = DecodeNumeric(register, pt);
        }
        else if (pt.RegisterType == RegisterType.HoldingRegister)
        {
          ushort[] register = await Master.ReadHoldingRegistersAsync(start, offset);
          sample = new NumericSample();
          sample = DecodeNumeric(register, pt);
        }


      }
      catch (Exception e)
      {
        log.Error($"Could not read register {pt.Register} and quantity {start}", e);
        sample = null;
      }


      return sample;
    }

  }
}
