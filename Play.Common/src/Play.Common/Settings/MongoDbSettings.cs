namespace Play.Common.Settings
{
    public class MongoDbSetting
    {
        public string Host { get; set; }
        public string Port { get; set; }
        public string ConnectionString { get { return $"mongodb://{Host}:{Port}"; } }
    }

}
