using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Elastacloud.AzureManagement.Fluent.Types.Exceptions;

namespace Elastacloud.AzureManagement.Fluent.Commands.Blobs
{
    internal class GetStorageAnalyticsEnabledCommand : BlobCommand
    {
        private readonly StorageServiceType _type;
        private AnalyticsMetricsType _analytics;
        public GetStorageAnalyticsEnabledCommand(StorageServiceType type, AnalyticsMetricsType analytics)
        {
            if(type == StorageServiceType)
                throw new FluentManagementException("unable to complete request need another storage type", "EnableStorageAnalyticsCommand");

            _type = type;
            _analytics = analytics;
            Version = "2013-08-15";
            HttpVerb = HttpVerbGet;
        }
        protected override sealed StorageServiceType StorageServiceType
        {
            get { return StorageServiceType.Analytics; }
        }

        public bool StorageAnalyticsEnabled { private set; get; }

        /// <summary>
        /// Abstract method used to execute the any command againt blob storage
        /// </summary>
        public override void Execute()
        {
            string storageType;
            switch (_type)
            {
                case StorageServiceType.Blob:
                    storageType = BlobService;
                    break;
                case StorageServiceType.Queue:
                    storageType = QueueService;
                    break;
                default:
                    storageType = TableService;
                    break;
            }
            string accessContainer = String.Format("http://{0}.{1}.core.windows.net/?restype=service&comp=properties",
                AccountName, storageType);
            string canResource = String.Format("/{0}/\ncomp:properties\nrestype:service", AccountName);
            string authHeader = CreateAuthorizationHeader(canResource);
            PayloadAnalyser = requestBody =>
            {
                if (requestBody.Length == 0)
                    return false;
                var document = XDocument.Parse(requestBody);

                var loggingElement = document.Element("StorageServiceProperties").Element("Logging");
                if (loggingElement == null)
                    return false;
                bool readValue = bool.Parse(loggingElement.Element("Read").Value);
                bool writeValue = bool.Parse(loggingElement.Element("Write").Value);

                return (StorageAnalyticsEnabled = readValue && writeValue);
            };
            SendWebRequest(accessContainer, authHeader);
        }
    }
}
