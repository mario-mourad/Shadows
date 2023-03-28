using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoNumberingConfig
{
    public class AutoNumber : IPlugin
    {
        public void Execute(IServiceProvider serviceProvider)
        {
            try
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

                    if (context.MessageName.ToLower() != "create" && context.Stage != 20)
                    {
                        return;
                    }
                    Entity targetEntity = context.InputParameters["Target"] as Entity;
                    Entity updateAutoNumberConfig = new Entity("cr3a2_autonumberconfiguration");
                    StringBuilder autoNumber = new StringBuilder();
                    string prefix, suffix, seperator, current, year, month, day;
                    DateTime today = DateTime.Now;
                    day = today.Day.ToString("00");
                    month = today.Month.ToString("00");
                    year = today.Year.ToString("").Substring(2);
                    

                    QueryExpression qeAutoNumberConfig = new QueryExpression()
                    {
                        EntityName = "cr3a2_autonumberconfiguration",
                        ColumnSet = new ColumnSet(true)
                    };

                    EntityCollection ecAnutoNumberConfig = service.RetrieveMultiple(qeAutoNumberConfig);
                    if (ecAnutoNumberConfig.Entities.Count == 0)
                    {
                        return;

                    }
                    foreach (Entity entity1 in ecAnutoNumberConfig.Entities)
                    {
                        if (entity1.Attributes["cr3a2_name"].ToString().ToLower() == "market")
                        {
                            prefix = entity1.GetAttributeValue<string>("cr3a2_prefix");
                            suffix = entity1.GetAttributeValue<string>("cr3a2_suffix");
                            seperator = entity1.GetAttributeValue<string>("cr3a2_seperator"); 
                             current = entity1.GetAttributeValue<string>("cr3a2_currentnumber");
                            int tempCurrent = int.Parse(current);
                            tempCurrent++;
                            current = tempCurrent.ToString("0000");
                            updateAutoNumberConfig.Id = entity1.Id;
                            updateAutoNumberConfig["cr3a2_currentnumber"] = current;
                            service.Update(updateAutoNumberConfig);
                            autoNumber.Append(prefix + seperator + year + seperator + suffix + current);
                            break;
                        }
                    }
                    targetEntity["cr3a2_marketids"] = autoNumber.ToString();
                }
            }
            catch (Exception ex)
            {
                throw new InvalidPluginExecutionException(ex.Message);
            }
        }
    }
}
