namespace Elastacloud.AzureManagement.Fluent.Utils

module ResourceXml = 

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

    [<Literal>]
    let Cscfg = """<ServiceConfiguration>
  <Role name="" vmName="">
    <Instances count="1"/>    
    <ConfigurationSettings>
      <Setting name="" value="" />
    </ConfigurationSettings>
    <Certificates>
      <Certificate name="" thumbprint="" thumbprintAlgorithm=""/>
    </Certificates>  
  </Role>
  <Role name="" vmName="">
    <Instances count="1"/>    
    <ConfigurationSettings>
      <Setting name="" value="" />
      <Setting name="" value="" />
    </ConfigurationSettings>
    <Certificates>
      <Certificate name="" thumbprint="" thumbprintAlgorithm=""/>
      <Certificate name="" thumbprint="" thumbprintAlgorithm=""/>
    </Certificates>  
  </Role>
  <NetworkConfiguration>
    <AccessControls>
      <AccessControl name="aclName1">
        <Rule order="" action="" remoteSubnet="" description="rule-description"/>
      </AccessControl>
    </AccessControls>
    <EndpointAcls>
      <EndpointAcl role="" endpoint="" accessControl=""/>
    </EndpointAcls>
    <Dns>
      <DnsServers>
        <DnsServer name="" IPAddress="" />
      </DnsServers>
    </Dns>
    <VirtualNetworkSite name=""/>
    <AddressAssignments>
      <InstanceAddress roleName="">
        <Subnets>
          <Subnet name=""/>
        </Subnets>
      </InstanceAddress>
      <ReservedIPs>
        <ReservedIP name=""/>
      </ReservedIPs>
    </AddressAssignments>
  </NetworkConfiguration>
</ServiceConfiguration>"""