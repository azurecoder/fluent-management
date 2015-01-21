namespace Elastacloud.AzureManagement.Fluent.Utils

open FSharp.Data
open System.Xml.Linq
open Elastacloud.AzureManagement.Fluent
open Elastacloud.AzureManagement.Fluent.VirtualNetwork

module PaaSUtils = 
    // get the definition and use in the functions
    type CscfgDefinition = XmlProvider<ResourceXml.Cscfg>
    // encapsulate the cscfg class 
    type Cscfg(configuration : string) =

        member this.AmmendConfigForVNets(vnetName : string, subnetName : string, roleName : string) = 
            let (!!) : string -> XName = XName.op_Implicit

            let provider = CscfgDefinition.Parse configuration  
            provider.NetworkConfiguration.VirtualNetworkSite.XElement.Add(vnetName)
            (provider.NetworkConfiguration.AddressAssignments.InstanceAddress.RoleName = roleName) |> ignore
            provider.NetworkConfiguration.AddressAssignments.InstanceAddress.Subnets.XElement.Add(XElement(!!"Subnet", subnetName))
            provider.XElement.ToString() 
            