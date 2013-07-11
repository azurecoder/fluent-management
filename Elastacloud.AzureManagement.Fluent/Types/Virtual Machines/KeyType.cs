/************************************************************************************************************
 * This software is distributed under a GNU Lesser License by Elastacloud Limited and it is free to         *
 * modify and distribute providing the terms of the license are followed. From the root of the source the   *
 * license can be found in /Resources/license.txt                                                           * 
 *                                                                                                          *
 * Web at: www.elastacloud.com                                                                              *
 * Email: info@elastacloud.com                                                                              *
 ************************************************************************************************************/
namespace Elastacloud.AzureManagement.Fluent.Types.VirtualMachines
{
    /// <summary>
    /// The type of keys used to deploy a linux vm
    /// </summary>
    public enum KeyType
    {
        /// <summary>
        /// The public key used to deploy a linux vm 
        /// </summary>
        PublicKey,
        /// <summary>
        /// The key pair used to deploy the vm
        /// </summary>
        KeyPair
    }
}