using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Elastacloud.AzureManagement.Fluent.Clients;
using Elastacloud.AzureManagement.Fluent.Helpers.PublishSettings;
using Elastacloud.AzureManagement.Fluent.Types.VirtualMachines;

namespace Elastacloud.AzureManagement.Fluent.Integration.Tests
{
    class Program
    {
        static void Main(string[] args)
        {
            // should have 5 arguments though subscription_id_source, path_to_pub_settings_source,
            // destination_account_name, destination_account_key, vhd_blob_path
            // 1. test the copy of the image from source to destination account
            // ReSharper disable once InconsistentNaming
            string subscription_id_source = args[0];
            // ReSharper disable once InconsistentNaming
            var certificate_source = new PublishSettingsExtractor(args[1]).AddPublishSettingsToPersonalMachineStore();
            // ReSharper disable once InconsistentNaming
            string account_name_destination = args[2];
            // ReSharper disable once InconsistentNaming
            string account_key_destination = args[3];
            // ReSharper disable once InconsistentNaming
            string source_image_path = args[4];
            var properties = new ImageProperties()
            {
                OperatingSystem = PlatformType.Linux,
                Description = "Test from Azure Fluent Management",
                ShowInGui = false,
                IsPremium = true
            };
            var client = new ImageManagementClient(subscription_id_source, certificate_source);
            var imageList = client.ImageList;
            imageList.ForEach(image => Console.WriteLine(image.Label));
            Console.WriteLine("Image sparkius exists: {0}", client.Exists("sparkius1"));
            //client.CopyAndRegisterImageInNewSubscription(account_name_destination, account_key_destination, null,
            //    "elastaimage", "sparkius", source_image_path, properties);
        }
    }
}
