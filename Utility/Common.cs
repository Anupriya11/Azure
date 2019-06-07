using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Tooling.Connector;
using System;
using System.Configuration;
using System.Net;

namespace CRMAzureBusIntegration.Utility
{
    public class Common
    {
        public IOrganizationService InitializeService()
        {
            IOrganizationService service = null;
            try
            {
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                CrmServiceClient serviceClient = new CrmServiceClient(ConfigurationManager.ConnectionStrings["CRM"].ConnectionString);
                service = serviceClient.OrganizationWebProxyClient ?? (IOrganizationService)serviceClient.OrganizationServiceProxy;
                Entity a = service.Retrieve("account", new Guid("DDA36E51-9944-E911-A825-000D3A1670F6"), new Microsoft.Xrm.Sdk.Query.ColumnSet(true));
            }
            catch (Exception ex)
            {
                Console.WriteLine("Inside InitializeService - Exception - " + ex.Message);
            }
            return service;
        }
    }
}
