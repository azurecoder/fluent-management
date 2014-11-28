namespace Elastacloud.AzureManagement.Fluent.VirtualNetwork
open LukeSkywalker.IPNetwork
open Elastacloud.AzureManagement.Fluent.Types

module VirtualNetworkingUtils = 
    ///Types
    type Subnet = { Cidr : string; Name : string; FirstIp : string; LastIp : string }
    type AddressRange = {
        AddressPrefix : string
        Subnets : Subnet seq    
    }
    type VirtualNetwork ={
        Name : string
        AddressRanges : AddressRange seq
    }
    ///Functions
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
