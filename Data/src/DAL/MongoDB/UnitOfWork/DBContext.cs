using MongoDB.Driver;
using Serilog;
using Utils;

namespace DAL.MongoDB.UnitOfWork
{
    public class DBContext
    {
        ILogger log = Logger.ContextLog<DBContext>();

        public IMongoDatabase DataBase { get; set; }
        MongoClient Client;
        public bool IsConnected
        {
            get
            {
                return DataBase != null;
            }
        }

        public DBContext()
        {

            log.Debug("Connecting to Database");


            Task tks = Connect();
            tks.Wait();

        }

        public async Task Connect()
        {

            SettingsReader reader = new SettingsReader();
            DBSettings settings = reader.GetSettings<DBSettings>("MongoDbSettings");


            MongoClientSettings clientsettings = new MongoClientSettings();
            clientsettings.Server = new MongoServerAddress(settings.Server, settings.Port);

            if (!string.IsNullOrEmpty(settings.Username) && !string.IsNullOrEmpty(settings.Password))
            {
                clientsettings.Credential = MongoCredential.CreateCredential("admin", settings.Username, settings.Password);
            }


            Client = new MongoClient(clientsettings);
            DataBase = Client.GetDatabase(settings.DatabaseName);



            if (DataBase != null)
            {
                log.Information("Successfully connected to Mongo DB " + settings.Server + ":" + settings.Port);
            }
            else
            {

                log.Fatal("Could not connect to Mongo DB " + settings.Server + ":" + settings.Port);
            }
        }

        public async Task Disconnect()
        {

        }


    }
}
