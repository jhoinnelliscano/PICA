
using eCommerce.Commons.Objects.Config;

namespace eCommerce.PublisherSubscriber.Utilities
{
    internal static class MessagingUtilities
    {
        public enum MessageTypes
        {
            Published,
            Distributed
        }

        public static string? GetServerName() 
        {
            var server = AppConfiguration.Configuration["AppConfiguration:MQ:Server"].ToString();
            return server;
/*            string FileToRead = Path.Combine(AppContext.BaseDirectory, "MqServer.txt");
            var lines = File.ReadLines(FileToRead);
            var line = lines.FirstOrDefault();
            return line;*/
        }
    }
}
