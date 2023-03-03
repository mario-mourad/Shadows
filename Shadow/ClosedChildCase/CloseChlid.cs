using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace ClosedChildCase
{
    public class CloseChlid : IPlugin
    {
        public void Execute(IServiceProvider serviceProvider)
        {
            #region Contact Plugin
            // Obtain the tracing service (debugger)
            ITracingService tracingService =
            (ITracingService)serviceProvider.GetService(typeof(ITracingService));

            // Obtain the execution context from the service provider.(information Entity Data)
            IPluginExecutionContext context = (IPluginExecutionContext)
                serviceProvider.GetService(typeof(IPluginExecutionContext));

            // The InputParameters collection contains all the data passed in the message request.(Entity name)
            if (context.InputParameters.Contains("Target") &&
                context.InputParameters["Target"] is Entity)
            
            {
                // Obtain the target entity from the input parameters.(Entity Work)
                Entity entity = (Entity)context.InputParameters["Target"];

                // Obtain the organization service reference which you will need for  
                // web service calls.  
                IOrganizationServiceFactory serviceFactory =
                    (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));
                //organization service (service.(create + update +delete +re +re M+........))
                IOrganizationService service = serviceFactory.CreateOrganizationService(context.UserId);
            #endregion

                try
                {
                    #region RetrieveMultiple (query)
                    // Define Condition Values
                    var query_parentcaseid = context.PrimaryEntityId;
                    // Instantiate QueryExpression query
                    var query = new QueryExpression("incident");
                    // Add columns to query.ColumnSet
                    query.ColumnSet.AddColumns("incidentid", "title", "statuscode");
                    // Define filter query.Criteria
                    query.Criteria.AddCondition("parentcaseid", ConditionOperator.Equal, query_parentcaseid);

                    var retrievedCase = new Entity("incident", query_parentcaseid);
                    //Use Entity class with entity logical name
                    var CaseCollection = service.RetrieveMultiple(query);
                    #endregion
                    #region retrieve a record using Id (Case)
                    // Retrieve a record using Id and show record in consolpe app.
                    //Retrieve (name Entity ,GUID,ColumSet)
                    Entity retrievedCase1 = service.Retrieve("incident", context.PrimaryEntityId, new ColumnSet(true));
                    #endregion

                    #region parentCaseOptionSet
                    OptionSetValue parentCaseOptionSet = new OptionSetValue();
                    parentCaseOptionSet = (OptionSetValue)retrievedCase1.Attributes["statuscode"];
                    var optionValue1 = parentCaseOptionSet.Value;
                    #endregion
                    if (optionValue1 == 5)
                    {

                        foreach (var crmCase in CaseCollection.Entities)

                        {
                            #region childCaseOptionSet
                            OptionSetValue childCaseOptionSet = new OptionSetValue();
                            childCaseOptionSet = (OptionSetValue)crmCase.Attributes["statuscode"];
                            var optionValue2 = childCaseOptionSet.Value;
                            #endregion
                            if (optionValue2 != 5)
                            {
                                #region IncidentResolution
                                Entity IncidentResolution = new Entity("incidentresolution");
                                //string
                                IncidentResolution.Attributes["subject"] = "Subject Closed";
                                //lookup (entity name ,GUID)
                                IncidentResolution.Attributes["incidentid"] = new EntityReference("incident", crmCase.Id);
                                // Create the request to close the incident, and set its resolution to the
                                // resolution created above
                                CloseIncidentRequest closeRequest = new CloseIncidentRequest();
                                closeRequest.IncidentResolution = IncidentResolution;
                                // Set the requested new status for the closed Incident
                                closeRequest.Status = new OptionSetValue(5); //Problem Solved
                                                                             // Execute the close request
                                CloseIncidentResponse closeResponse = (CloseIncidentResponse)service.Execute(closeRequest);
                                #endregion
                            }
                            else
                            {

                            }
                        }
                    }
                    }
                catch (FaultException<OrganizationServiceFault> ex)
                {
                    throw new InvalidPluginExecutionException("An error occurred in FollowUpPlugin NEW Plugin Jonn", ex);
                }

                catch (Exception ex)
                {
                    tracingService.Trace("FollowUpPlugin John: {0}", ex.ToString());
                    throw;
                }
            }
        }
    }
}
