using DAL.Influx;
using DAL.MongoDB.Entities;
using DAL.MongoDB.Entities.Devices;
using DAL.MongoDB.UnitOfWork;
using Serilog;
using Services.Drivers;
using System.Collections.Concurrent;
using Utils;

namespace Services
{
    public class ServiceStart
    {
        protected ILogger log = Logger.ContextLog<ServiceStart>();
        IUnitOfWork UnitOfWork = null;
        IInfluxUnitOfWork InfluxUnitOfWork = null;
        List<Driver> Drivers = new List<Driver>();
        System.Timers.Timer timer = null;
        public ServiceStart(IUnitOfWork unitOfWork, IInfluxUnitOfWork InfluxUow)
        {
            UnitOfWork = unitOfWork;
            InfluxUnitOfWork = InfluxUow;

        }

        public async Task Start()
        {
            log.Information("Starting");
            timer = new System.Timers.Timer(10000);
            timer.Enabled = true;
            timer.Elapsed += Timer_Elapsed;
            List<MQTTDevice> mqtdevices = UnitOfWork.Devices.GetMQTTDevices();

            foreach (MQTTDevice device in mqtdevices)
            {
                List<DataPoint> dps = UnitOfWork.DataPoints.GetDataPointsForDevice(device.DeviceName);

                List<MQTTDataPoint> datapoints = dps.Cast<MQTTDataPoint>().ToList();

                MQTTDriver mqtt = new MQTTDriver(device, datapoints);

                Drivers.Add(mqtt);

                Task.Run(() => mqtt.Connect());
            }

            List<ModbusDevice> moddevices = UnitOfWork.Devices.GetModbusDevices();


            foreach (ModbusDevice device in moddevices)
            {
                List<DataPoint> dps = UnitOfWork.DataPoints.GetDataPointsForDevice(device.DeviceName);

                List<ModbusDataPoint> datapoints = dps.Cast<ModbusDataPoint>().ToList();

                ModbusDriver modbus = new ModbusDriver(device, datapoints);

                Drivers.Add(modbus);

                Task.Run(() => modbus.Connect());
            }

            await Save();
        }

        private async void Timer_Elapsed(object? sender, System.Timers.ElapsedEventArgs e)
        {
            await Save();
        }

        private async Task Save()
        {
            ConcurrentBag<Sample> samples = new ConcurrentBag<Sample>();
            foreach (Driver dr in Drivers)
            {
                foreach (KeyValuePair<String, ConcurrentBag<Sample>> smp in dr.Measurements)
                {
                    samples.AddRange(smp.Value);
                }
            }

            await InfluxUnitOfWork.Influx.InsertManyAsync(samples);

            foreach (Driver dr in Drivers)
            {
                await dr.Clear();
            }
        }



        public async Task Stop()
        {
            foreach (Driver dr in Drivers)
            {
                await dr.Disconnect();
            }

        }
    }
}
