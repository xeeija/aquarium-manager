using Microsoft.AspNetCore.Mvc.ModelBinding;
using Utils;

namespace Services.Utils
{
  public class ModelStateWrapper : IModelStateWrapper
  {
    private ModelStateDictionary modelState;

    protected Serilog.ILogger log = Logger.ContextLog<ModelStateWrapper>();

    public ModelStateWrapper(ModelStateDictionary modelState)
    {
      this.modelState = modelState;
      Clear();
    }

    public void AddError(string key, string errorMessage)
    {
      log.Debug("Model has an Error: {Error} - Message: {Message}", key, errorMessage);
      modelState.AddModelError(key, errorMessage);
    }

    public bool IsValid => modelState.IsValid;

    public void Clear()
    {
      modelState.Clear();
    }

    public Dictionary<string, string> Errors
    {
      get
      {
        var errors = new Dictionary<string, string>();

        foreach (KeyValuePair<string, ModelStateEntry> err in modelState)
        {
          ModelStateEntry modelStateEntry = err.Value;

          var errormessage = "";

          ModelErrorCollection coll = modelStateEntry.Errors;

          foreach (ModelError error in coll)
          {
            errormessage += error.ErrorMessage;
          }

          errors.Add(err.Key, errormessage);
        }

        return errors;
      }
    }
  }
}
