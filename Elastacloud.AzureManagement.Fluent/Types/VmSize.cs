/************************************************************************************************************
 * This software is distributed under a GNU Lesser License by Elastacloud Limited and it is free to         *
 * modify and distribute providing the terms of the license are followed. From the root of the source the   *
 * license can be found in /Resources/license.txt                                                           * 
 *                                                                                                          *
 * Web at: www.elastacloud.com                                                                              *
 * Email: info@elastacloud.com                                                                              *
 ************************************************************************************************************/

namespace Elastacloud.AzureManagement.Fluent.Types
{
    /// <summary>
    /// The VM size of the role instance
    /// </summary>
    public enum VmSize
    {
        /// <summary>
        /// Extra small VM (1GHz CPU, 768MB RAM, 20GB Storage)
        /// </summary>
        ExtraSmall,

        /// <summary>
        /// Small VM (1.6GHz CPU, 1.75GB RAM, 225GB Storage)
        /// </summary>
        Small,

        /// <summary>
        /// Medium VM (2 x 1.6GHz CPU, 3.5GB RAM, 490GB Storage)
        /// </summary>
        Medium,

        /// <summary>
        /// Large VM (4 x 1.6GHz CPU, 7GB RAM, 1,000GB Storage)
        /// </summary>
        Large,

        /// <summary>
        /// Extra large VM (8 x 1.6GHz CPU, 14GB RAM, 2,040GB Storage)
        /// </summary>
        ExtraLarge,

        /// <summary>
        /// A6 (4 x 1.6GHz CPU, 28GB RAM, 1,000GB Storage)
        /// </summary>
        A6,

        /// <summary>
        /// A7 (8 x 1.6GHz CPU, 56GB RAM, 2,040GB Storage)
        /// </summary>
        A7
    }
}