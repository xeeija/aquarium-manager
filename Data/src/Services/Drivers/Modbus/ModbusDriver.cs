using DAL.Influx;
using DAL.MongoDB.Entities;
using DAL.MongoDB.Entities.Devices;
using Modbus.Device;
using Modbus.Utility;
using System.Net.Sockets;
using ModbusDevice = DAL.MongoDB.Entities.Devices.ModbusDevice;

namespace Services.Drivers
{
    public class ModbusDriver : Driver
    {
        TcpClient Client;
        ModbusIpMaster Master;



        private ModbusDevice Source;
        String FinalUrl;
        List<ModbusDataPoint> ModbusDataPoints = new List<ModbusDataPoint>();

        System.Timers.Timer FetchTimer = new System.Timers.Timer();


        public ModbusDriver(ModbusDevice src, List<ModbusDataPoint> datapoints) : base(src.DeviceName)
        {
            this.Source = src;
            ModbusDataPoints = datapoints;
        }


        public async override Task Connect()
        {
            try
            {
                FinalUrl = Source.Host + ":" + Source.Port;

                Client = new TcpClient(Source.Host, Source.Port);
                Master = ModbusIpMaster.CreateIp(Client);

                FetchTimer = new System.Timers.Timer(10000);
                FetchTimer.Enabled = true;
                FetchTimer.Elapsed += FetchTimer_Elapsed;

                await Read();

            }
            catch (Exception ex)
            {
                log.Fatal("Could not create Master ", ex);
            }
            IsConnected = Client.Connected;


            foreach (ModbusDataPoint dpi in ModbusDataPoints)
            {
                AddDataPoint(dpi.Name, dpi);
            }

        }

        private async void FetchTimer_Elapsed(object? sender, System.Timers.ElapsedEventArgs e)
        {
            await Read();
        }

        public async override Task Disconnect()
        {
            try
            {

                log.Information("Stopping Client");
                if (Client != null)
                {
                    Client.Dispose();
                }

                if (FetchTimer != null)
                {
                    FetchTimer.Dispose();
                    FetchTimer = null;
                }
            }
            catch (Exception e)
            {
                log.Warning("Stopping failed " + e.ToString());
            }
        }

        public async Task Read()
        {

            if (IsConnected)
            {
                foreach (ModbusDataPoint pt in ModbusDataPoints)
                {

                    if (pt.RegisterCount > 0 && pt.Register >= 0)
                    {
                        Sample mn = await GetModbusVal(pt);

                        if (mn != null)
                        {

                            if (mn.GetType() == typeof(NumericSample))
                            {
                                AddNumericMeasurement(pt.Name, mn as NumericSample);
                            }
                            else
                            {
                                AddBinaryMeasurement(pt.Name, mn as BinarySample);
                            }


                            log.Debug($"Got {mn.Value} from {pt.Name} ");
                        }
                    }
                    else
                    {
                        log.Error("Cannot Read " + pt.Name + " Register or Offset 0");
                    }
                }
            }
        }


        private NumericSample DecodeNumeric(ushort[] register, ModbusDataPoint pt)
        {
            NumericSample sample = new NumericSample();

            if (pt.DataType == DataType.Float)
            {

                if (register.Count() > 1)
                {
                    if (register.Count() == 2)
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
                else if (register.Count() == 1)
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
