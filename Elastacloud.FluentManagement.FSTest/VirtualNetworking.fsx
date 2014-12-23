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
open LukeSkywalker.IPNetwork
open System

open System.Security.Cryptography.X509Certificates

let subscriptionId = "84bf11d2-7751-4ce7-b22d-ac44bf33cbe9"
/// Start Fuctions 
let settingCert fileName subscriptionId =
    let settings = PublishSettingsExtractor fileName
    settings.AddAllPublishSettingsCertificatesToPersonalMachineStore(subscriptionId).[0] 
let getFromBizsparkPlus = settingCert "D:\\Projects\\BizSpark Plus-7-8-2014-credentials.publishsettings"
/// remove a subnet from a virtual network
let vnetClient = VirtualNetworkClient(subscriptionId, getFromBizsparkPlus subscriptionId)
vnetClient.RemoveSubnet("fsnet", "Subnet-2")

let linuxClient = LinuxVirtualMachineClient(subscriptionId, getFromBizsparkPlus subscriptionId)
let subnets = linuxClient.GetVirtualMachineSubnetCollection("biscuitbriskit")

let getMinimumNetworkSubnetAddressWithNetmask (s : string) = 
    IPAddress.Parse(s.Trim()).GetAddressBytes() 
    |> Array.rev
    |> (fun ip -> (BitConverter.ToUInt32(ip, 0) + (uint32)32))
    |> BitConverter.GetBytes
    |> Array.rev 
    |> fun ipAddress -> IPAddress ipAddress
    |> string |> sprintf "%s/27"
let validateNextAvailableSubnetAddress ipAddress addressRange : string option = 
    let requestedNetwork = IPNetwork.Parse(ipAddress)
    let containsIp = IPNetwork.Contains(IPNetwork.Parse(addressRange), requestedNetwork)
    if not containsIp then None
    else Some(requestedNetwork.ToString())
let NextAvailableSubnet addressSpaceWithCidr (vNet : VirtualNetworkingUtils.VirtualNetwork) = 
    let singleSubnet = vNet.AddressRanges 
                        |> Seq.where (fun addr -> addr.AddressPrefix = addressSpaceWithCidr)
                        |> Seq.map (fun addr -> addr.Subnets)
                        |> Seq.last |> Seq.last
    let nextAvailableSubnetHost = getMinimumNetworkSubnetAddressWithNetmask singleSubnet.LastIp
    let validator = validateNextAvailableSubnetAddress nextAvailableSubnetHost addressSpaceWithCidr
    match validator with  
    | Some(nextAvailableSubnetHost) -> nextAvailableSubnetHost
    | _ -> null
/// End Functions
// test functions start to end 
let vnClient = VirtualNetworkClient(subscriptionId, (getFromBizsparkPlus subscriptionId))
let vNets = vnClient.GetAvailableVirtualNetworks()
let vNetToTest = vNets |> Seq.head 
let addressRange1 = vNetToTest.AddressRanges |> Seq.head
vnClient.AddSubnetToAddressRange(vNetToTest.Name, addressRange1.AddressPrefix, "fred");


validateNextAvailableSubnetAddress "10.1.0.255" "10.1.0.0/16"
NextAvailableSubnet addressRange1.AddressPrefix vNetToTest  
// end test functions
let singleSubnet = vNetToTest.AddressRanges 
                    |> Seq.where (fun addr -> addr.AddressPrefix = addressRange1.AddressPrefix)
                    |> Seq.map (fun addr -> addr.Subnets)
                    |> Seq.last |> Seq.last
let nextAvailableSubnetHost = getMinimumNetworkSubnetAddressWithNetmask singleSubnet.LastIp
let requestedNetwork = IPNetwork.Parse(nextAvailableSubnetHost)
let containsIp = IPNetwork.Contains(IPNetwork.Parse(addressRange1.AddressPrefix), requestedNetwork)
let validator = validateNextAvailableSubnetAddress nextAvailableSubnetHost addressRange1.AddressPrefix
getMinimumNetworkSubnetAddressWithNetmask singleSubnet.LastIp
validateNextAvailableSubnetAddress "10.1.0.255" "10.1.0.0/16"


// Return all of the virtual networks and their subnets here
let availableIps = vnClient.IsIpAddressAvailable(vNetToTest.Name, "10.0.0.4")
// Check the networking config 
// Get the networking config

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