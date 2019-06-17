# Azure Service Bus Integration

1. This project connects CRM online instance to azure service bus.
2. It can be configured and extended for any CRM entity.
3. We need to add class file under "Subscriber" folder for each entity that is configured newly.
   Subcriber class file name should be logical name for the entity.
4. Following keys need to be added:
    a. Microsoft.ServiceBus.ConnectionString - Azure Bus Connection String
    b. Namespace - CRMAzureBusIntegration.Subscribers
    c. CRM - CRM Connection String
    
Create azure service bus: 
https://docs.microsoft.com/en-us/azure/service-bus-messaging/service-bus-quickstart-portal

Configure azure service bus for CRM: 
https://community.dynamics.com/365/b/ajitpatra365crm/archive/2018/09/26/azure-service-bus-queue-integration-with-d365-part-1
    
