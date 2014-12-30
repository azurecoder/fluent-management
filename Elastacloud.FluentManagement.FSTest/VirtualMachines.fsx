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

let subscriptionId = "84bf11d2-7751-4ce7-b22d-ac44bf33cbe9"
/// Start Fuctions 
let settingCert fileName subscriptionId =
    let settings = PublishSettingsExtractor fileName
    settings.AddAllPublishSettingsCertificatesToPersonalMachineStore(subscriptionId).[0] 
let getFromBizsparkPlus = settingCert "D:\\Projects\\BizSpark Plus-7-8-2014-credentials.publishsettings"

let storageClient = StorageClient(subscriptionId, getFromBizsparkPlus subscriptionId)
let accounts = storageClient.GetStorageAccountList()
let account = accounts |> Seq.filter (fun account -> account.Name = "azurecoder11")

let vmClient = LinuxVirtualMachineClient(subscriptionId, getFromBizsparkPlus subscriptionId)
let sshEndpoint = InputEndpoint(EndpointName = "ssh",
                                LocalPort = 22,
                                Port = Nullable(22),
                                Protocol = Protocol.TCP)
let properties = new LinuxVirtualMachineProperties(
                                                    VmSize = VmSize.Small,
                                                    UserName = "azurecoder",
                                                    AdministratorPassword = "P@ssword761",
                                                    HostName = "briskit",
                                                    RoleName = "briskit",
                                                    CloudServiceName = "briskit1000",
                                                    PublicEndpoints = List<InputEndpoint>([|sshEndpoint|]),
                                                    CustomTemplateName = "b39f27a8b8c64d52b05eac6a62ebad85__Ubuntu_DAILY_BUILD-trusty-14_04_1-LTS-amd64-server-20141202.1-en-us-30GB",
                                                    DeploymentName = "briskit1000",
                                                    StorageAccountName = "clustered",
                                                    AvailabilitySet = "clustered",
                                                    VirtualNetwork = VirtualNetworkDescriptor(
                                                                                              VirtualNetworkName = "fsnet",
                                                                                              SubnetName = "fred"))

vmClient.CreateNewVirtualMachineDeploymentFromTemplateGallery(
                                                              List<LinuxVirtualMachineProperties>([|properties|]),
                                                              "briskit1000")
// test 1: Ensure that above contains no subnets when it's created and returns the address range + 1 ip
// test 2: Receive events on state changes and ensure readyrole
vmClient.LinuxVirtualMachineStatusEvent.Subscribe(fun vmstatus -> printfn "from %s to %s" (vmstatus.OldStatus.ToString()) (vmstatus.NewStatus.ToString()))
// test 3: When created delete the subnet from the vnet - should generate a subnet busy exception of some sort
let vnClient = VirtualNetworkClient(subscriptionId, (getFromBizsparkPlus subscriptionId))
vnClient.AddSubnetToAddressRange("bigbadbeetleborgs", "10.0.0.0/20", "max-1")
vnClient.RemoveSubnet("bigbadbeetleborgs", "max-1")
let images = vmClient.GetCurrentUbuntuImage()
