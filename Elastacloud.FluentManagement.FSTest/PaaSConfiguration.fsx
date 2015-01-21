#r "D:\\Projects\\Elastacloud\\fluent-management\\Elastacloud.AzureManagement.Fluent\\bin\\Release\\Elastacloud.AzureManagement.Fluent.dll"
#r "D:\\Projects\\Elastacloud\\fluent-management\\Elastacloud.AzureManagement.Fluent\\bin\\Release\\Elastacloud.AzureManagement.Fluent.Types.dll"
#r "D:\\Projects\\Elastacloud\\fluent-management\\Elastacloud.AzureManagement.Fluent.Utils\\bin\\Release\\Elastacloud.AzureManagement.Fluent.Utils.dll"
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
open Elastacloud.AzureManagement.Fluent.Utils

let xml = """<?xml version="1.0" encoding="utf-8"?>
<ServiceConfiguration serviceName="WindowsAzure1" xmlns="http://schemas.microsoft.com/ServiceHosting/2008/10/ServiceConfiguration" osFamily="3" osVersion="*" schemaVersion="2012-10.1.8">
  <Role name="WebRole1">
    <Instances count="2" />
    <ConfigurationSettings>
      <Setting name="Microsoft.WindowsAzure.Plugins.Diagnostics.ConnectionString" value="UseDevelopmentStorage=true" />
      <Setting name="Microsoft.WindowsAzure.Plugins.Caching.NamedCaches" value="{&quot;caches&quot;:[{&quot;name&quot;:&quot;default&quot;,&quot;policy&quot;:{&quot;eviction&quot;:{&quot;type&quot;:0},&quot;expiration&quot;:{&quot;defaultTTL&quot;:10,&quot;isExpirable&quot;:true,&quot;type&quot;:1},&quot;serverNotification&quot;:{&quot;isEnabled&quot;:false}},&quot;secondaries&quot;:0},{&quot;name&quot;:&quot;NamedCache1&quot;,&quot;policy&quot;:{&quot;eviction&quot;:{&quot;type&quot;:-1},&quot;expiration&quot;:{&quot;defaultTTL&quot;:20,&quot;isExpirable&quot;:true,&quot;type&quot;:2},&quot;serverNotification&quot;:{&quot;isEnabled&quot;:true}},&quot;secondaries&quot;:1}]}" />
      <Setting name="Microsoft.WindowsAzure.Plugins.Caching.ClientDiagnosticLevel" value="1" />
      <Setting name="Microsoft.WindowsAzure.Plugins.Caching.DiagnosticLevel" value="1" />
      <Setting name="Microsoft.WindowsAzure.Plugins.Caching.CacheSizePercentage" value="30" />
      <Setting name="Microsoft.WindowsAzure.Plugins.Caching.ConfigStoreConnectionString" value="UseDevelopmentStorage=true" />
    </ConfigurationSettings>
  </Role>
</ServiceConfiguration>
"""

let configTracker = PaaSUtils.Cscfg(xml)
let config = configTracker.AmmendConfigForVNets("vnet", "subnet", "role")