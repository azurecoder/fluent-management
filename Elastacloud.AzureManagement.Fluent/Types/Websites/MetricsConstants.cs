using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elastacloud.AzureManagement.Fluent.Types.Websites
{
    public class MetricsConstants
    {
        public string CpuTime = "CpuTime";
        public string BytesReceived = "BytesReceived";
        public string BytesSent = "BytesSent";
        public string IncomingRequestBytes = "IncomingRequestBytes";
        public string IncomingRequestResponseBytes = "IncomingRequestResponseBytes";
        public string OutgoingRequestBytes = "OutgoingRequestBytes";
        public string OutgoingRequestResponseBytes = "OutgoingRequestResponseBytes";
        public string LocalReadBytes = "LocalReadBytes";
        public string LocalWrittenBytes = "LocalWrittenBytes";
        public string NetworkReadBytes = "NetworkReadBytes";
        public string NetworkWrittenBytes = "NetworkWrittenBytes";
        public string Requests = "Requests";
// ReSharper disable InconsistentNaming
        public string Http2xx = "Http2xx";
// ReSharper restore InconsistentNaming
// ReSharper disable InconsistentNaming
        public string Http3xx = "Http3xx";
// ReSharper restore InconsistentNaming
        public string Http401 = "Http401";
        public string Http403 = "Http403";
        public string Http404 = "Http404";
        public string Http406 = "Http406";
// ReSharper disable InconsistentNaming
        public string Http4xx = "Http4xx";
// ReSharper restore InconsistentNaming
// ReSharper disable InconsistentNaming
        public string Http5xx = "Http5xx";
// ReSharper restore InconsistentNaming
    }
}
