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
        A5,

        /// <summary>
        /// A6 (4 x 1.6GHz CPU, 28GB RAM, 1,000GB Storage)
        /// </summary>
        A6,

        /// <summary>
        /// A7 (8 x 1.6GHz CPU, 56GB RAM, 2,040GB Storage)
        /// </summary>
        A7,

        /// <summary>
        /// A8 (4 x 1.6GHz CPU, 56GB RAM, 1,000GB Storage)
        /// </summary>
        A8,

        /// <summary>
        /// A7 (8 x 1.6GHz CPU, 112GB RAM, 2,040GB Storage)
        /// </summary>
        A9,
        // this is the basic tariff which doesn't include the load balancing so is cheaper 
// ReSharper disable InconsistentNaming
        Basic_A0,
// ReSharper restore InconsistentNaming
// ReSharper disable once InconsistentNaming
        Basic_A1,
// ReSharper disable once InconsistentNaming
        Basic_A2,
// ReSharper disable once InconsistentNaming
        Basic_A3,
// ReSharper disable once InconsistentNaming
        Basic_A4,
        // <summary>1 core, 3.5G, 127 OS, 50G SSD</summary>
        // ReSharper disable once InconsistentNaming
        STANDARD_D1,
        // <summary>2 core, 7G, 127 OS, 100G SSD</summary>
        // ReSharper disable once InconsistentNaming
        STANDARD_D2,
        // <summary>4 core, 14G, 127 OS, 200G SSD</summary>
        // ReSharper disable once InconsistentNaming
        STANDARD_D3,
        // <summary>8 core, 28G, 127 OS, 400G SSD</summary>
        // ReSharper disable once InconsistentNaming
        STANDARD_D4,
        // <summary>2 core, 14G, 127 OS, 100G SSD</summary>
        // ReSharper disable once InconsistentNaming
        STANDARD_D11,
        // <summary>4 core, 28G, 127 OS, 200G SSD</summary>
        // ReSharper disable once InconsistentNaming
        STANDARD_D12,
        // <summary>8 core, 56G, 127 OS, 400G SSD</summary>
        // ReSharper disable once InconsistentNaming
        STANDARD_D13,
        // <summary>1 core, 112G, 127 OS, 800G SSD</summary>
        // ReSharper disable once InconsistentNaming
        STANDARD_D14,
        // <summary>1 core, 3.5G, 127 OS, 7G SSD, 3200/32MBsec</summary>
        // ReSharper disable once InconsistentNaming
        STANDARD_DS1,
        // <summary>2 core, 7G, 127 OS, 14G SSD, 6400/64MBsec</summary>
        // ReSharper disable once InconsistentNaming
        STANDARD_DS2,
        // <summary>4 core, 14G, 127 OS, 28G SSD, 12800/128MBsec</summary>
        // ReSharper disable once InconsistentNaming
        STANDARD_DS3,
        // <summary>8 core, 28G, 127 OS, 56G SSD, 25600/256MBsec</summary>
        // ReSharper disable once InconsistentNaming
        STANDARD_DS4,
        // <summary>1 core, 28G, 127 OS, 28G SSD, 6400/64MBsec</summary>
        // ReSharper disable once InconsistentNaming
        STANDARD_DS11,
        // <summary>2 core, 56G, 127 OS, 56G SSD, 12800/128MBsec</summary>
        // ReSharper disable once InconsistentNaming
        STANDARD_DS12,
        // <summary>4 core, 112G, 127 OS, 112G SSD, 25600/256MBsec</summary>
        // ReSharper disable once InconsistentNaming
        STANDARD_DS13,
        // <summary>8 core, 224G, 127 OS, 224G SSD, 56200/512MBsec</summary>
        // ReSharper disable once InconsistentNaming
        STANDARD_DS14,
        // <summary>2 core, 28G, 127 OS, 384G SSD</summary>
        // ReSharper disable once InconsistentNaming
        STANDARD_G1,
        // <summary>4 core, 56G, 127 OS, 768G SSD</summary>
        // ReSharper disable once InconsistentNaming
        STANDARD_G2,
        // <summary>8 core, 112G, 127 OS, 1536G SSD</summary>
        // ReSharper disable once InconsistentNaming
        STANDARD_G3,
        // <summary>16 core, 224G, 127 OS, 3072G SSD</summary>
        // ReSharper disable once InconsistentNaming
        STANDARD_G4,
        // <summary>32 core, 448G, 127 OS, 6144G SSD</summary>
        // ReSharper disable once InconsistentNaming
        STANDARD_G5
    }
}