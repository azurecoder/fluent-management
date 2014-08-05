using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Elastacloud.AzureManagement.Fluent.Helpers;
using Elastacloud.AzureManagement.Fluent.Types.Exceptions;

namespace Elastacloud.AzureManagement.Fluent.Commands.Blobs
{
    /// <summary>
    /// Used to enable or disable storage analytics within a particular account
    /// </summary>
    internal class EnableStorageAnalyticsCommand : BlobCommand
    {
        private readonly StorageServiceType _type;
        private AnalyticsMetricsType _analytics;
        public EnableStorageAnalyticsCommand(StorageServiceType type, AnalyticsMetricsType analytics)
        {
            if(type == StorageServiceType)
                throw new FluentManagementException("unable to complete request need another storage type", "EnableStorageAnalyticsCommand");

            _type = type;
            _analytics = analytics;
            Version = "2013-08-15";
        }

        /* PUT https://myaccount.table.core.windows.net/?restype=service&comp=properties HTTP/1.1
            x-ms-version: 2013-08-15
            x-ms-date: Wed, 23 Oct 2013 04:28:19 GMT
            Authorization: SharedKey
            myaccount:Z1lTLDwtq5o1UYQluucdsXk6/iB7YxEu0m6VofAEkUE=
            Host: myaccount.table.core.windows.net*/

        /* 
         * 
         * <?xml version="1.0" encoding="utf-8"?>
<StorageServiceProperties>
    <Logging>
        <Version>1.0</Version>
              <Delete>true</Delete>
        <Read>false</Read>
        <Write>true</Write>
        <RetentionPolicy>
            <Enabled>true</Enabled>
            <Days>7</Days>
        </RetentionPolicy>
    </Logging>
    <HourMetrics>
        <Version>1.0</Version>
        <Enabled>true</Enabled>
        <IncludeAPIs>false</IncludeAPIs>
        <RetentionPolicy>
            <Enabled>true</Enabled>
            <Days>7</Days>
        </RetentionPolicy>
    </HourMetrics>
    <MinuteMetrics>
        <Version>1.0</Version>
        <Enabled>true</Enabled>
        <IncludeAPIs>false</IncludeAPIs>
        <RetentionPolicy>
            <Enabled>true</Enabled>
            <Days>7</Days>
        </RetentionPolicy>
    </MinuteMetrics>
</StorageServiceProperties>*/

        protected override sealed StorageServiceType StorageServiceType
        {
            get { return StorageServiceType.Analytics; }
        }

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
            var payload = Encoding.UTF8.GetBytes(BuildXmlPayload());
            string authHeader = CreateAuthorizationHeader(canResource, "", payload.Count());
            SendWebRequest(accessContainer, authHeader, payload, payload.Count());
        }
        /// <summary>
        /// This method builds up wht payload but currently only builds the logging and not the hourly or minute metrics
        /// </summary>
        private string BuildXmlPayload()
        {
            var doc = new XDocument(
                new XDeclaration("1.0", "utf-8", ""),
                new XElement("StorageServiceProperties",
                             new XElement("Logging", 
                                 new XElement("Version", "1.0"),
                                 new XElement("Delete", "true"),
                                 new XElement("Read", "true"),
                                 new XElement("Write", "true"),
                                 new XElement("RetentionPolicy", 
                                     new XElement("Enabled", "true"),
                                     new XElement("Days", "7")))));
            return doc.ToStringFullXmlDeclaration();
        }
    }
}
