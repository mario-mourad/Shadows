using Microsoft.Crm.Sdk.Messages;
using Microsoft.PowerPlatform.Dataverse.Client;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Newtonsoft.Json.Linq;
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
                    //PerformCRUD(serviceClient);
                    //CreateCase(serviceClient);
                    //IncidentResolution(serviceClient);
                    CheckParentCase(serviceClient);
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
            mycontact.Attributes["lastname"] = "Yassa";
            mycontact.Attributes["firstname"] = "Jack";
            mycontact.Attributes["emailaddress1"] = "JackYassa@gmail.com";
            Guid recordid = svc.Create(mycontact);
            Console.WriteLine("contact create with id -" + recordid);
            #endregion

        }
        static public void CreateCase(ServiceClient svc)
        {
            #region Create Case
            var myCase = new Entity("incident");
            myCase.Attributes["title"] = "New Create Case";
            myCase.Attributes["description"] = "New DESCRIPTION For Details";
            myCase.Attributes["customerid"] = new EntityReference("account", new Guid("83883308-7AD5-EA11-A813-000D3A33F3B4")); //Account-Customer
            myCase.Attributes["ownerid"] = new EntityReference("systemuser", new Guid("d7b6cbe9-71a4-ed11-aad1-000d3a2afebd")); //Owner
            myCase.Attributes["caseorigincode"] = new OptionSetValue(1); //This Option Set (1=Phone)
            Guid recordid = svc.Create(myCase);
            Console.WriteLine("Case create with id - " + recordid);
            #endregion

          
            
        }
        static public void IncidentResolution(ServiceClient svc)
        {
            #region IncidentResolution
            Entity IncidentResolution = new Entity("incidentresolution");
            IncidentResolution.Attributes["subject"] = "Subject Closed";
            IncidentResolution.Attributes["incidentid"] = new EntityReference("incident", new Guid("9d0e79ae-27b5-ed11-83ff-000d3a4bbea4"));
            // Create the request to close the incident, and set its resolution to the
            // resolution created above
            CloseIncidentRequest closeRequest = new CloseIncidentRequest();
            closeRequest.IncidentResolution = IncidentResolution;
            // Set the requested new status for the closed Incident
            closeRequest.Status = new OptionSetValue(5); //Problem Solved
            // Execute the close request
            CloseIncidentResponse closeResponse = (CloseIncidentResponse)svc.Execute(closeRequest);
            Console.WriteLine("Resolved Case Done");
            #endregion
        }
        static public void CheckParentCase(ServiceClient svc)
        {
            // Define Condition Values
            //var query_parentcaseid = "39a0159a-1717-4821-8053-d28fec01ba94"; //Child Case
            var query_parentcaseid = "ea904c59-e1b5-ed11-83ff-000d3a4bbea4"; //Not Child Case
            // Instantiate QueryExpression query
            var query = new QueryExpression("incident");
            // Add columns to query.ColumnSet
            query.ColumnSet.AddColumns("incidentid", "title");
            // Define filter query.Criteria
            query.Criteria.AddCondition("parentcaseid", ConditionOperator.Equal, query_parentcaseid);
            var caseCollection = svc.RetrieveMultiple(query);
            if (caseCollection.Entities.Count > 0) 
            {
                foreach (var crmCase in caseCollection.Entities)
                {

                    Console.WriteLine("###############################");
                    Console.WriteLine("Child Case Title: " + crmCase["title"]);


                }
                Console.WriteLine("YES");
            }
            else
            {
                Console.WriteLine("NO");
            }
           
        }
    }
}
