// Learn more about F# at http://fsharp.net. See the 'F# Tutorial' project
// for more guidance on F# programming.

#r "D:\\Projects\\Elastacloud\\fluent-management\\Elastacloud.AzureManagement.Fluent\\bin\\Debug\\Elastacloud.AzureManagement.Fluent.dll"
#r "D:\\Projects\\Elastacloud\\fluent-management\\Elastacloud.AzureManagement.Fluent\\bin\\Debug\\Elastacloud.AzureManagement.Fluent.Types.dll"
#r "D:\\Projects\\Elastacloud\\fluent-management\\Elastacloud.AzureManagement.Fluent\\bin\\Debug\\Elastacloud.AzureManagement.Fluent.Utils.dll"
#r "C:\\Program Files (x86)\\Reference Assemblies\\Microsoft\\Framework\\.NETFramework\\v4.5\\System.Xml.Linq.dll"
#r @"..\packages\IPNetwork.1.3.1.0\lib\LukeSkywalker.IPNetwork.dll"
#r @"..\packages\FSharp.Data.2.0.14\lib\net40\FSharp.Data.dll"

open Elastacloud.AzureManagement.Fluent
open Elastacloud.AzureManagement.Fluent.Types
open Elastacloud.AzureManagement.Fluent.Clients
open Elastacloud.AzureManagement.Fluent.VirtualNetwork
open Elastacloud.AzureManagement.Fluent.Helpers.PublishSettings
open FSharp.Data
open System.Xml.Linq
open System.Net

open System.Security.Cryptography.X509Certificates

let subscriptionIdBizSpark = "f435e1c1-a6ae-4108-804c-c2d365c4b9f2"
let subscriptionId = "84bf11d2-7751-4ce7-b22d-ac44bf33cbe9"
let settingCert fileName subscriptionId =
    let settings = PublishSettingsExtractor fileName
    settings.AddAllPublishSettingsCertificatesToPersonalMachineStore(subscriptionId).[0] 

// Return all of the virtual networks and their subnets here

let getFromBizsparkPlus = settingCert "D:\\Projects\\BizSpark Plus-7-8-2014-credentials.publishsettings"
    
let vnClient = VirtualNetworkClient(subscriptionId, (getFromBizsparkPlus subscriptionId))
let vNets = vnClient.GetAvailableVirtualNetworks()
vNets
let vNet1 = vNets |> Seq.head 
let ipAddress1 = vNet1.AddressRanges |> Seq.head |> (fun addr -> (Seq.head addr.Subnets).FirstIp)
let availableIps = vnClient.IsIpAddressAvailable(vNet1.Name, "10.0.0.4")
// Check the networking config 
// Get the networking config
open LukeSkywalker.IPNetwork
open System
let virtualNetworkSiteName = "skynet"
let networkResource = vnClient.GetAllNetworkingConfig()
type networkProvider = XmlProvider<Resources.networkingConfig>
let vNetDocument = networkProvider.Parse(networkResource)
//  get the address space 
vNetDocument.VirtualNetworkConfiguration.VirtualNetworkSites
|> Seq.where (fun vnets -> vnets.Name = virtualNetworkSiteName)
|> Seq.map (fun addr -> addr.AddressSpace.AddressPrefixs)
|> Seq.head
|> Seq.map (fun (pfx : string) -> IPNetwork.Parse(pfx))

let getNextAvailableSubnetAddress (s : string) = 
    IPAddress.Parse(s.Trim()).GetAddressBytes() 
    |> Array.rev
    |> (fun ip -> (BitConverter.ToUInt32(ip, 0) + (uint32)1))
    |> BitConverter.GetBytes
    |> Array.rev 
    |> fun ipAddress -> IPAddress ipAddress
    |> string |> sprintf "%s/27"

let validateNextAvailableSubnetAddress ipAddress addressRange : string option = 
    let requestedNetwork = IPNetwork.Parse(ipAddress)
    let containsIp = IPNetwork.Contains(requestedNetwork, IPNetwork.Parse(addressRange))
    if not containsIp then None
    else Some(ipAddress)

let GetXmlWithAvailableSubnets xml (addressSpaceWithCidr : string) subnetName (vNet : VirtualNetworkingUtils.VirtualNetwork) = 
    let networkDocument = networkProvider.Parse(xml)
    let singleSubnet = vNet.AddressRanges 
                        |> Seq.where (fun addr -> addr.AddressPrefix = addressSpaceWithCidr)
                        |> Seq.map (fun addr -> addr.Subnets)
                        |> Seq.last
                        |> Seq.nth 1
    let validator = validateNextAvailableSubnetAddress singleSubnet.LastIp addressSpaceWithCidr
    match validator with  
    | Some(ipAddress) -> ipAddress
    | _ -> networkDocument.XElement.ToString()


getNextAvailableSubnetAddress "10.1.1.255"
GetXmlWithAvailableSubnets networkResource "10.1.0.0/16" "fred" vNet1 

type Subnet = { Cidr : string; Name : string; FirstIp : string; LastIp : string }
type AddressRange = {
    AddressPrefix : string
    Subnets : Subnet seq    
}

type VirtualNetwork = {
    Name : string
    AddressRanges : AddressRange seq
}
// TODO: Add first and last IP address
let BuildSingleAddressRange (addressSpace : IPNetwork) subnets =
    let isInIpNetwork (subnet:IPNetwork, _) = IPNetwork.Contains(addressSpace, subnet)
    let containedSubnets =
        subnets
        |> List.filter isInIpNetwork
        |> List.map(fun ((subnet : IPNetwork), name) -> { Cidr = subnet.ToString()
                                                          Name = name 
                                                          FirstIp = subnet.FirstUsable.ToString()
                                                          LastIp = subnet.LastUsable.ToString() })
    { AddressPrefix = addressSpace.ToString(); Subnets = containedSubnets }

let BuildAddressRanges addressSpaces subnets = 
    let subnets = subnets |> Seq.map (fun (sub : VirtualNetworks.Subnet) -> IPNetwork.Parse sub.CidrAddressRange, sub.Name) |> Seq.toList
    addressSpaces
    |> Seq.map (fun (adr : VirtualNetworks.AddressSpace) -> IPNetwork.Parse adr.CidrAddressRange)
    |> Seq.map (fun addressSpace -> BuildSingleAddressRange addressSpace subnets)

let toVNet (name, addressRanges) = 
    { Name  = name; AddressRanges = addressRanges }

let linuxVmClient = LinuxVirtualMachineClient(subscriptionId, (getFromBizsparkPlus subscriptionId))
let nets = linuxVmClient.GetAvailableVirtualNetworks()
nets    
|> Seq.map (fun vNet -> vNet.Name, BuildAddressRanges vNet.AddressSpaces vNet.Subnets)
|> Seq.map toVNet




// check the cidr notation 
// test to see whether the subscription is available combined.publishsettings
let bpCert = settingCert "D:\\Projects\\combined.publishsettings" subscriptionId
bpCert.Thumbprint
let bizCert = settingCert "D:\\Projects\\combined.publishsettings" subscriptionIdBizSpark
bizCert.Thumbprint

