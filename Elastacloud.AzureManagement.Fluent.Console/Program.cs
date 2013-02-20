using System;
using Elastacloud.AzureManagement.Fluent.Console.Properties;
using System.Linq;

// e.g. vm create subscription_id publish_settings_file cloud_service_name role_name password
namespace Elastacloud.AzureManagement.Fluent.Console
{
    class Program
    {
        static void Main(string[] args)
        {
            var executor = ParseTokens(args);
            executor.Execute();
            System.Console.WriteLine();
            System.Console.WriteLine(Resources.Program_Main_Press_any_key_to_exit);
            System.Console.Read();
        }

        public static IExecute ParseTokens(string[] args)
        {
            if(args.Count() != 8)
                throw new ApplicationException("unable to complete request ensure that the requisite number of parameters are used");
            var factory = new ApplicationFactory(args[0], args[1], args[2], args[3], args[4], args[5], args[6], args[7]);
            return factory.GetExecutor();
        }
    }
}
