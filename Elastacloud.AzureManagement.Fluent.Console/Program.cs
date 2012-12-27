using System.Security.Cryptography.X509Certificates;
using Elastacloud.AzureManagement.Fluent.Clients;
using Elastacloud.AzureManagement.Fluent.Helpers.PublishSettings;

namespace Elastacloud.AzureManagement.Fluent.Console
{
    class Program
    {
        private static X509Certificate2 _certificate;
        static void Main(string[] args)
        {
            System.Console.WriteLine("Mobile service");
            System.Console.WriteLine("==============");

            var publishsettings = @"C:\Users\Richard\Desktop\Engagements\AllAccounts.publishsettings";
            var settings = new PublishSettingsExtractor(publishsettings);
            
            _certificate = settings.AddPublishSettingsToPersonalMachineStore();
            var executor = ParseTokens(args);
            executor.Execute();
            System.Console.WriteLine();
            System.Console.WriteLine("Press any key to exit");
            System.Console.Read();
        }

        public static IExecute ParseTokens(string[] args)
        {
            // turn this into an enum as this app gets more comprehensive
            bool mobile = false, create = false;
            switch (args[0])
            {
                case "mobile":
                    mobile = true;
                    break;
            }
            switch (args[1])
            {
                case "create":
                    create = true;
                    break;
                case "read":
                    create = false;
                    break;
            }
            if (mobile && create)
            {
                return new MobileCreate(args[2], _certificate, args[3]);
            }
            return new MobileGet(args[2], _certificate, args[3]);
        }
    }
}
