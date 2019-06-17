using CRMAzureBusIntegration.Utility;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Microsoft.ServiceBus.Messaging;
using Microsoft.Xrm.Sdk;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Configuration;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;

namespace CRMAzureBusIntegration
{
    class Program
    {       
        static void Main(string[] args)
        {
            try
            {
                var client = QueueClient.CreateFromConnectionString(ConfigurationManager.AppSettings["Microsoft.ServiceBus.ConnectionString"]);
                client.OnMessage(message =>
                {
                    try
                    {
                        var context = message.GetBody<RemoteExecutionContext>();
                        Broker.Listener(context);
                        Console.WriteLine("Primary Entity :" + context.PrimaryEntityName);
                        Console.WriteLine("Message Name :" + context.MessageName);
                        Console.WriteLine("Primary Entity Id:" + context.PrimaryEntityId);
                        message.Complete();
                    }
                    catch (Exception ex)
                    {
                        message.Abandon();
                        Console.WriteLine("Inner Exception: " + ex.Message);
                    }
                });
                Console.ReadLine();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error : " + ex.Message);
            }

        }     
    }
}
