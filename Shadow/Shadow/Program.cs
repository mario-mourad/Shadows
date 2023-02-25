using Microsoft.Crm.Sdk.Messages;
using Microsoft.PowerPlatform.Dataverse.Client;
using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shadow
{
    class Program
    {
        #region OAuth
        // TODO Enter your Dataverse environment's URL and logon info.
        static string url = "https://orga8374a37.crm4.dynamics.com";
        static string userName = "JohnYassa@Mario2294.onmicrosoft.com";
        static string password = "Hello_world";

        // This service connection string uses the info provided above.
        // The AppId and RedirectUri are provided for sample code testing.
        static string connectionString = $@"
            AuthType = OAuth;
            Url = {url};
            UserName = {userName};
            Password = {password};
           ";
        #endregion
        static void Main(string[] args)
        {
            using (ServiceClient serviceClient = new(connectionString))
            {
                if (serviceClient.IsReady)
                {
                    WhoAmIResponse response =
                        (WhoAmIResponse)serviceClient.Execute(new WhoAmIRequest());

                    Console.WriteLine("User ID is {0}.", response.UserId);
                    PerformCRUD(serviceClient);
                }
                else
                {
                    Console.WriteLine("A web service connection was not established.");
                }

            }

            // Pause the console so it does not close.
            Console.WriteLine("Press any key to exit.");

            Console.ReadLine();
        }
        static public void PerformCRUD(ServiceClient svc)
        {
            #region Create Contact
            var mycontact = new Entity("contact");
            mycontact.Attributes["lastname"] = "يسى";
            mycontact.Attributes["firstname"] = "جون";
            mycontact.Attributes["emailaddress1"] = "johnyassa1994@gmail.com";
            Guid recordid = svc.Create(mycontact);
            Console.WriteLine("contact create with id -" + recordid);
            #endregion

        }
    }
}
