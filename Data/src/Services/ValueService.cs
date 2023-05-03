using Context.DAL.Visuals;
using DAL.Influx;
using DAL.Influx.Samples;
using DAL.MongoDB.Entities;
using DAL.MongoDB.UnitOfWork;
using DataCollector.ReturnModels;
using DataCollector.ReturnModels.Visuals;
using MongoDB.Driver;

namespace Services
{
  public class ValueService
  {
    IUnitOfWork UnitOfWork = null;
    IInfluxUnitOfWork Influx = null;
    public ValueService(IUnitOfWork UnitOfWork, IInfluxUnitOfWork Influx)
    {
      this.UnitOfWork = UnitOfWork;
      this.Influx = Influx;
    }

    public async Task<ValueReturnModelSingle> GetLastValue(string aquarium, string datapoint)
    {
      ValueReturnModelSingle returnval = new ValueReturnModelSingle();
      Device dev = await UnitOfWork.Devices.FindOneAsync(x => x.Aquarium.Equals(aquarium));
      if (dev != null)
      {
        DataPoint dp = await UnitOfWork.DataPoints.FindOneAsync(x => x.DeviceName.Equals(dev.Name));

        if (dp != null)
        {
          Sample meas = await Influx.Repository.GetLast(dev.Aquarium, dp);
          if (meas != null)
          {

            VisualsReturnModel visu = await CreateVisuals(dp, meas);

            returnval.DataPoint = dp;
            returnval.Sample = meas;
            returnval.Visuals = visu;

            return returnval;
          }
        }
      }

      return null;
    }

    public async Task<List<ValueReturnModelSingle>> GetLastValues(string aquarium)
    {
      List<ValueReturnModelSingle> returnval = new List<ValueReturnModelSingle>();
      List<Device> dev = await UnitOfWork.Devices.FilterByAsync(x => x.Aquarium.Equals(aquarium));

      if (dev != null)
      {
        foreach (Device dev2 in dev)
        {
          List<DataPoint> dps = UnitOfWork.DataPoints.FilterBy(x => x.DeviceName.Equals(dev2.Name));

          if (dps != null)
          {
            foreach (DataPoint dp in dps)
            {
              Sample meas = await Influx.Repository.GetLast(dev2.Aquarium, dp);

              if (meas != null)
              {
                var returnmodel = new ValueReturnModelSingle
                {
                  DataPoint = dp,
                  Sample = meas,
                  Visuals = await CreateVisuals(dp, meas)
                };

                returnval.Add(returnmodel);
              }
            }
          }
        }

        return returnval;
      }

      return null;
    }

    private async Task<VisualsReturnModel> CreateVisuals(DataPoint dp, Sample ms)
    {
      DataPointVisual visuals = await UnitOfWork.Visuals.FindOneAsync(x => x.Name.Equals(dp.DataPointVisual));

      if (visuals != null)
      {

        if (dp.DataType == DataType.Boolean)
        {
          var returnval = new VisualsBinaryReturnModel
          {
            Icon = visuals.Icon
          };

          if (visuals.GetType() == typeof(BinaryDataPointVisuals))
          {
            BinaryDataPointVisuals bbp = (BinaryDataPointVisuals)visuals;
            if (bbp.ValueMapping != null && bbp.ValueMapping.Count > 0)
            {
              if (ms.GetType() == typeof(BinarySample))
              {
                var bin = (BinarySample)ms;

                var map = bbp.ValueMapping.Where(x => Convert.ToBoolean(x.Value) == bin.AsBoolean()).FirstOrDefault();

                if (map != null)
                {
                  returnval.FinalText = map.Text;
                }
              }
            }
          }

          return returnval;
        }
        else
        {
          var returnval = new VisualsNumericReturnModel
          {
            Icon = visuals.Icon
          };

          if (visuals.GetType() == typeof(NumericDataPointVisuals))
          {
            NumericDataPointVisuals bbp = (NumericDataPointVisuals)visuals;

            returnval.MinValue = bbp.MinValue;
            returnval.MaxValue = bbp.MaxValue;
            returnval.Unit = bbp.Unit;

            return returnval;
          }
        }
      }

      return null;
    }
  }
}
