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
        static string serviceUri = "https://learning11.crm.dynamics.com/"; 
        static string redirectUrl = "http://test.com";
        
        static void Main(string[] args)
        {
            try
            {
                string authToken = GetAuthToken();
                var crmresponseobj = RetrieveAccounts(authToken);
                Common objCommon = new Common();
                objCommon.InitializeService();
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
        public static string GetAuthToken()
        {

            // TODO Substitute your app registration values that can be obtained after you
            // register the app in Active Directory on the Microsoft Azure portal.
            string clientId = "12062478-cf37-42d9-89d1-4b4a25172aec"; // Client ID after app registration
            string userName = "powerapps@learning11.onmicrosoft.com";
            string password = "admin@123";
            UserCredential cred = new UserCredential(userName, password);

            // Authenticate the registered application with Azure Active Directory.
            AuthenticationContext authContext = new AuthenticationContext("https://login.microsoftonline.com/dfc3d03d-3d2a-4791-9271-bc11965fc2d2/oauth2/authorize", false);
            AuthenticationResult result = authContext.AcquireToken(serviceUri, clientId, cred);
            return result.AccessToken;
        }
        public static dynamic RetrieveAccounts(string authToken)
        {
            HttpClient httpClient = null;
            httpClient = new HttpClient();
            //Default Request Headers needed to be added in the HttpClient Object
            httpClient.DefaultRequestHeaders.Add("OData-MaxVersion", "4.0");
            httpClient.DefaultRequestHeaders.Add("OData-Version", "4.0");
            httpClient.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

            //Set the Authorization header with the Access Token received specifying the Credentials
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authToken);
            httpClient.BaseAddress = new Uri(serviceUri);
            // Add this line for TLS complaience
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;

            // Retrieve Contacts
            var retrieveResponse = httpClient.GetAsync("api/data/v9.1/accounts");
                var jRetrieveResponse = JObject.Parse(retrieveResponse.Result.Content.ReadAsStringAsync().Result);

                dynamic collContacts = JsonConvert.DeserializeObject(jRetrieveResponse.ToString());

                foreach (var data in collContacts.value)
                {
                    Console.WriteLine("Contact Name – " +data.name.Value);
                }
                            return null;
        }

    }
}
