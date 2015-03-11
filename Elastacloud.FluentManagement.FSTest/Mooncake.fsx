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
open Elastacloud.AzureManagement.Fluent.VirtualMachines.Classes
open Elastacloud.AzureManagement.Fluent.Types.VirtualMachines
open System.Collections.Generic
open System.Security.Cryptography.X509Certificates
open Elastacloud.AzureManagement.Fluent.Types.VirtualNetworks
open Elastacloud.AzureManagement.Fluent.Watchers
open Elastacloud.AzureManagement.Fluent.Helpers

let subscriptionId = "1631f3af-fc17-48e7-9dc7-b519f52534ab"
/// Start Fuctions 
let settingCert fileName subscriptionId =
    let settings = PublishSettingsExtractor fileName
    settings.AddAllPublishSettingsCertificatesToPersonalMachineStore(subscriptionId).[0] 
let getFromBizsparkPlus = settingCert "D:\\Projects\\Windows Azure 内部使用-3-10-2015-credentials.publishsettings"

// Cloud Services
let serviceClient = ServiceClient(subscriptionId, getFromBizsparkPlus subscriptionId, "briskchina", LocationConstants.ChinaNorth)
serviceClient.CreateNewCloudService(LocationConstants.ChinaNorth, "My First Fluent Service")
let mutable available = serviceClient.IsCloudServiceNameAvailable
serviceClient.DeleteCloudService()
available <- serviceClient.IsCloudServiceNameAvailable
let locations = serviceClient.AvailableLocations
// Storage Accounts
let storageClient = StorageClient(subscriptionId, getFromBizsparkPlus subscriptionId, LocationConstants.ChinaEast)
let mutable accounts = storageClient.GetStorageAccountList()
storageClient.CreateNewStorageAccount("briskchina", LocationConstants.ChinaNorth)
accounts <- storageClient.GetStorageAccountList()
storageClient.DeleteStorageAccount("briskchina")
accounts <- storageClient.GetStorageAccountList()
// Virtual Networking 
let vnClient = VirtualNetworkClient(subscriptionId, (getFromBizsparkPlus subscriptionId), LocationConstants.ChinaEast)
let vNets = vnClient.GetAvailableVirtualNetworks()
let vNetToTest = vNets |> Seq.head 
let addressRange1 = vNetToTest.AddressRanges |> Seq.head
vnClient.AddSubnetToAddressRange(vNetToTest.Name, addressRange1.AddressPrefix, "fred")
let we = vnClient.GetAvailableVirtualNetworks(LocationConstants.ChinaEast)
let ne = vnClient.GetAvailableVirtualNetworks(LocationConstants.ChinaNorth)
vnClient.RemoveSubnet(vNetToTest.Name, "fred")
vnClient.GetAllNetworkingConfig()
// Service Bus
let sbClient = ServiceBusClient(subscriptionId, (getFromBizsparkPlus subscriptionId), LocationConstants.ChinaNorth)
let bobbydavro = sbClient.CheckNamespaceExists("bobbydavro")
let fred = sbClient.CheckNamespaceExists("fred")
let fred2 = sbClient.CreateNamespace("fred")
let toolscheck = sbClient.CheckNamespaceExists("elastatools3")
let elastacloud = sbClient.CreateNamespace("elastatools3")
let clouddelete = sbClient.DeleteNamespace("elastatools3")
let sblist = sbClient.GetServiceBusNamspaceList(LocationConstants.ChinaEast)
let sblist1 = sbClient.GetServiceBusNamspaceList(LocationConstants.ChinaNorth)
let sbpolicy = sbClient.GetServiceBusConnectionString("elastatools3", "RootManageSharedAccessKey")
// Image Management
let imClient = ImageManagementClient(subscriptionId, getFromBizsparkPlus subscriptionId, LocationConstants.ChinaEast)
let imageList = imClient.ImageList
let ubuntuList = imageList
                 |> Seq.filter(fun image -> image.Name.ToLower().Contains("ubuntu"))
                 |> Seq.toList
                 |> Seq.iter(fun image -> printfn "%s" image.Name)
// Virtual Machines
let vmClient = LinuxVirtualMachineClient(subscriptionId, getFromBizsparkPlus subscriptionId, LocationConstants.ChinaEast)
let sshEndpoint = InputEndpoint(EndpointName = "ssh", LocalPort = 22, Port = Nullable(22), Protocol = Protocol.TCP)
let properties = new LinuxVirtualMachineProperties(
                                                    VmSize = VmSize.Small,
                                                    UserName = "azurecoder",
                                                    AdministratorPassword = "P@ssword761",
                                                    HostName = "briskit",
                                                    RoleName = "briskit",
                                                    CloudServiceName = "briskchina",
                                                    PublicEndpoints = List<InputEndpoint>([|sshEndpoint|]),
                                                    CustomTemplateName = "b549f4301d0b4295b8e76ceb65df47d4__Ubuntu-14_04_1-LTS-amd64-server-20150123-en-us-30GB",
                                                    DeploymentName = "briskchina",
                                                    StorageAccountName = "briskchina")
vmClient.LinuxVirtualMachineStatusEvent.Subscribe(fun vmstatus -> printfn "from %s to %s" (vmstatus.OldStatus.ToString()) (vmstatus.NewStatus.ToString()))
vmClient.CreateNewVirtualMachineDeploymentFromTemplateGallery(List<LinuxVirtualMachineProperties>([|properties|]), "briskchina")