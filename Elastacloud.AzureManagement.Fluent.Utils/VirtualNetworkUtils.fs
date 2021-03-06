﻿namespace Elastacloud.AzureManagement.Fluent.VirtualNetwork

open LukeSkywalker.IPNetwork
open Elastacloud.AzureManagement.Fluent
open Elastacloud.AzureManagement.Fluent.Utils
open Elastacloud.AzureManagement.Fluent.Types
open FSharp.Data
open System
open System.Net
open System.Xml

module VirtualNetworkingUtils = 
    /// Types for the subnet and address space creation
    type Subnet = { Cidr : string; Name : string; FirstIp : string; LastIp : string }
    type AddressRange = {
        AddressPrefix : string
        Subnets : Subnet seq    
    }
    type VirtualNetwork ={
        Name : string
        AddressRanges : AddressRange seq
    }
    /// Functions for the returning of subnets and address spaces
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

    let private BuildAddressRanges addressSpaces subnets = 
        let subnets = subnets |> Seq.map (fun (sub : VirtualNetworks.Subnet) -> IPNetwork.Parse sub.CidrAddressRange, sub.Name) |> Seq.toList
        addressSpaces
        |> Seq.map (fun (adr : VirtualNetworks.AddressSpace) -> IPNetwork.Parse adr.CidrAddressRange)
        |> Seq.map (fun addressSpace -> BuildSingleAddressRange addressSpace subnets)

    let private toVNet (name, addressRanges) = 
        { Name  = name; AddressRanges = addressRanges }

    let ConvertVNetToHierarchicalModel (vNets : VirtualNetworks.VirtualNetworkSite seq) = 
        vNets 
        |> Seq.map (fun vNet -> vNet.Name, BuildAddressRanges vNet.AddressSpaces vNet.Subnets)
        |> Seq.map toVNet
    /// Functions for the addition of subnets into an existing vnet
    type vNetDocument = XmlProvider<ResourceXml.networkingConfig>

    let private getMinimumNetworkSubnetAddressWithNetmask (s : string) = 
        IPAddress.Parse(s.Trim()).GetAddressBytes() 
        |> Array.rev
        |> (fun ip -> (BitConverter.ToUInt32(ip, 0) + (uint32)32))
        |> BitConverter.GetBytes
        |> Array.rev 
        |> fun ipAddress -> IPAddress ipAddress
        |> string |> sprintf "%s/27"
    let private validateNextAvailableSubnetAddress ipAddress addressRange : string option = 
        let requestedNetwork = IPNetwork.Parse(getMinimumNetworkSubnetAddressWithNetmask ipAddress)
        let containsIp = IPNetwork.Contains(IPNetwork.Parse(addressRange), requestedNetwork)
        if not containsIp then None
        else Some(requestedNetwork.ToString())
    let NextAvailableSubnet addressSpaceWithCidr (vNet : VirtualNetwork) = 
        let subnetCollection = vNet.AddressRanges 
                                    |> Seq.where (fun addr -> addr.AddressPrefix = addressSpaceWithCidr)
                                    |> Seq.collect (fun addr -> addr.Subnets)
        let singleSubnet = match subnetCollection |> Seq.isEmpty with
                           | true -> IPNetwork.Parse(addressSpaceWithCidr).FirstUsable.ToString()
                           | false -> (subnetCollection |> Seq.last).LastIp                                     
        let validator = validateNextAvailableSubnetAddress singleSubnet addressSpaceWithCidr
        match validator with  
        | Some(ipAddress) -> ipAddress
        | _ -> null