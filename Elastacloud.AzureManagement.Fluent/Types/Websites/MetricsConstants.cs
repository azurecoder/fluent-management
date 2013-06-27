using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elastacloud.AzureManagement.Fluent.Types.Websites
{
    public class MetricsConstants
    {
        public const string CpuTime = "CpuTime";
        public const string BytesReceived = "BytesReceived";
        public const string BytesSent = "BytesSent";
        public const string IncomingRequestBytes = "IncomingRequestBytes";
        public const string IncomingRequestResponseBytes = "IncomingRequestResponseBytes";
        public const string OutgoingRequestBytes = "OutgoingRequestBytes";
        public const string OutgoingRequestResponseBytes = "OutgoingRequestResponseBytes";
        public const string LocalReadBytes = "LocalReadBytes";
        public const string LocalWrittenBytes = "LocalWrittenBytes";
        public const string NetworkReadBytes = "NetworkReadBytes";
        public const string NetworkWrittenBytes = "NetworkWrittenBytes";
        public const string Requests = "Requests";
// ReSharper disable InconsistentNaming
        public const string Http2xx = "Http2xx";
// ReSharper restore InconsistentNaming
// ReSharper disable InconsistentNaming
        public const string Http3xx = "Http3xx";
// ReSharper restore InconsistentNaming
        public const string Http401 = "Http401";
        public const string Http403 = "Http403";
        public const string Http404 = "Http404";
        public const string Http406 = "Http406";
// ReSharper disable InconsistentNaming
        public const string Http4xx = "Http4xx";
// ReSharper restore InconsistentNaming
// ReSharper disable InconsistentNaming
        public const string Http5xx = "Http5xx";
// ReSharper restore InconsistentNaming
    }
}
