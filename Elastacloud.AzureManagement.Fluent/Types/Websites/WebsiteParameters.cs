using System.Collections.Generic;

namespace Elastacloud.AzureManagement.Fluent.Types.Websites
{
    /// <summary>
    /// The website structure containing details oof the underlying website
    /// </summary>
    public class WebsiteParameters
    {
        public int CurrentNumberOfWorkers { get; set; }
        public int NumberOfWorkers { get; set; }
        public WebsiteState CurrentWorkerState { get; set; }
        public WorkerSize CurrentWorkerSize { get; set; }
        public WebsiteAvailabilityState AvailabilityState { get; set; }
    }

}