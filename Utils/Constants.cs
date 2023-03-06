using System.Reflection;

namespace Utils;

public static class Constants
{
  public static String CurrentFolder
  {
    get
    {
      string codeBase = Assembly.GetExecutingAssembly().Location; // Codebase
      UriBuilder uri = new UriBuilder(codeBase);
      string path = Uri.UnescapeDataString(uri.Path);
      return Path.GetDirectoryName(path);
    }
  }
}
