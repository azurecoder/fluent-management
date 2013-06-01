/************************************************************************************************************
 * This software is distributed under a GNU Lesser License by Elastacloud Limited and it is free to         *
 * modify and distribute providing the terms of the license are followed. From the root of the source the   *
 * license can be found in /Resources/license.txt                                                           * 
 *                                                                                                          *
 * Web at: www.elastacloud.com                                                                              *
 * Email: info@elastacloud.com                                                                              *
 ************************************************************************************************************/
using System.Collections.Generic;

namespace Elastacloud.AzureManagement.Fluent.Types.Websites
{
    /// <summary>
    /// The website structure containing details oof the underlying website
    /// </summary>
    public class WebsiteParameters
    {
        /// <summary>
        /// This returns the current number of workers across the webfarm
        /// </summary>
        public int CurrentNumberOfWorkers { get; set; }
        /// <summary>
        /// This returns the number of workers used across the whole webfarm
        /// </summary>
        public int NumberOfWorkers { get; set; }
        /// <summary>
        /// The state of the current website
        /// </summary>
        public WebsiteState CurrentWorkerState { get; set; }
        /// <summary>
        /// This gets or set the size of the website workers
        /// </summary>
        public WorkerSize CurrentWorkerSize { get; set; }
        /// <summary>
        /// This sets the availability state of the workers 
        /// </summary>
        public WebsiteAvailabilityState AvailabilityState { get; set; }
    }

}