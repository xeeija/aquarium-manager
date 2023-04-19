using DAL.MongoDB.Entities;
using DAL.MongoDB.Entities.Devices;
using DAL.MongoDB.UnitOfWork;
using Services.Drivers;

namespace Tests
{
    public class ModbusDeviceTest
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void Test1()
        {
            ModbusDevice device = new ModbusDevice();

        }


        [Test]
        public async Task CreateDevice()
        {
            ModbusDevice device = new ModbusDevice();

            List<DataPoint> pts = new List<DataPoint>
            {
                CreateFloatModbusDP("Pump Current", "PumpCurrent", 1, 1),
                CreateFloatModbusDP("Pump Voltage", "PumpVoltage", 3, 1),
                CreateBooleanModbusDP("Pump Status", "PumpStatus", 1),
                CreateWriteBooleanModbusDP("Turn Pump OnOff", "PumpOnOff", 1)
            };

            UnitOfWork uow = new UnitOfWork();
            await uow.DataPoints.InsertManyAsync(pts);


        }


        [Test]
        public async Task ReadModbus()
        {
            ModbusDevice device = new ModbusDevice();
            device.Active = true;
            device.DeviceName = "Pump";
            device.Aquarium = "SchiScho";
            device.SlaveID = 1;
            device.Host = "127.0.0.1";
            device.Port = 502;

            UnitOfWork uow = new UnitOfWork();
            List<DataPoint> datapoints = uow.DataPoints.GetDataPointsForDevice("Pump");

            List<ModbusDataPoint> listOfModbusDataPoints = datapoints.Cast<ModbusDataPoint>().ToList();

            ModbusDriver driver = new ModbusDriver(device, listOfModbusDataPoints);

            await driver.Connect();
            Assert.IsTrue(driver.IsConnected);
            await driver.Read();
            Assert.Greater(driver.Measurements.Count, 0);

        }



        private ModbusDataPoint CreateFloatModbusDP(String name, String dbname, int register, int offset = 1)
        {
            ModbusDataPoint pt = new ModbusDataPoint();
            pt.Device = "Pump";
            pt.Name = name;
            pt.RegisterCount = 2;
            pt.ReadingType = ReadingType.HighToLow;
            pt.Register = register;
            pt.RegisterType = RegisterType.HoldingRegister;
            pt.Offset = offset;
            pt.DataType = DataType.Float;

            return pt;

        }

        private ModbusDataPoint CreateBooleanModbusDP(String name, String dbname, int register)
        {
            ModbusDataPoint pt = new ModbusDataPoint();
            pt.Name = name;
            pt.Device = "Pump";
            pt.RegisterCount = 1;
            pt.Register = register;
            pt.RegisterType = RegisterType.Coil;
            pt.DataType = DataType.Boolean;

            return pt;

        }

        private ModbusDataPoint CreateWriteBooleanModbusDP(String name, String dbname, int register)
        {
            ModbusDataPoint pt = new ModbusDataPoint();
            pt.Name = name;
            pt.Device = "Pump";
            pt.RegisterCount = 1;
            pt.Register = register;
            pt.RegisterType = RegisterType.WriteSingleCoil;
            pt.DataType = DataType.Boolean;

            return pt;

        }

    }
}