namespace Elastacloud.AzureManagement.Fluent.VirtualNetwork

module Resources = 

    [<Literal>]
    let networkingConfig = """
    <NetworkConfiguration xmlns="http://schemas.microsoft.com/ServiceHosting/2011/07/NetworkConfiguration">
        <VirtualNetworkConfiguration>
        <Dns>
            <DnsServers>
            <DnsServer name="" IPAddress=""/>
            </DnsServers>
        </Dns>
        <LocalNetworkSites>
            <LocalNetworkSite name="">
            <VPNGatewayAddress>gateway-address</VPNGatewayAddress>
            <AddressSpace>
                <AddressPrefix>address-prefix</AddressPrefix>
            </AddressSpace>
            </LocalNetworkSite>
        </LocalNetworkSites>
        <VirtualNetworkSites>
            <VirtualNetworkSite name="" AffinityGroup="" Location="">
            <Gateway profile="">
                <VPNClientAddressPool>
                <AddressPrefix>address-prefix</AddressPrefix>
                </VPNClientAddressPool>
                <ConnectionsToLocalNetwork>
                <LocalNetworkSiteRef name="">
                    <Connection type=""/>
                </LocalNetworkSiteRef>
                </ConnectionsToLocalNetwork>
            </Gateway>
            <DnsServersRef>
                <DnsServerRef name=""/>
            </DnsServersRef>
            <Subnets>
                <Subnet name="">
                    <AddressPrefix>address-prefix</AddressPrefix>
                </Subnet>
                <Subnet name="">
                    <AddressPrefix>address-prefix</AddressPrefix>
                </Subnet>
            </Subnets>
            <AddressSpace>
                <AddressPrefix>address-prefix</AddressPrefix>
                <AddressPrefix>address-prefix</AddressPrefix>
            </AddressSpace>
            </VirtualNetworkSite>
            <VirtualNetworkSite name="" AffinityGroup="" Location="">
            <Gateway profile="">
                <VPNClientAddressPool>
                <AddressPrefix>address-prefix</AddressPrefix>
                </VPNClientAddressPool>
                <ConnectionsToLocalNetwork>
                <LocalNetworkSiteRef name="">
                    <Connection type=""/>
                </LocalNetworkSiteRef>
                </ConnectionsToLocalNetwork>
            </Gateway>
            <DnsServersRef>
                <DnsServerRef name=""/>
            </DnsServersRef>
            <Subnets>
                <Subnet name="">
                    <AddressPrefix>address-prefix</AddressPrefix>
                </Subnet>
                <Subnet name="">
                    <AddressPrefix>address-prefix</AddressPrefix>
                </Subnet>
            </Subnets>
            <AddressSpace>
                <AddressPrefix>address-prefix</AddressPrefix>
                <AddressPrefix>address-prefix</AddressPrefix>
            </AddressSpace>
            </VirtualNetworkSite>
        </VirtualNetworkSites>
        </VirtualNetworkConfiguration>
    </NetworkConfiguration>"""