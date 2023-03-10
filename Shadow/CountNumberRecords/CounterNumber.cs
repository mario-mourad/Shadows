using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Sdk.Workflow;
using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CountNumberRecords
{
    public class CounterNumber : CodeActivity
    {
        protected override void Execute(CodeActivityContext executionContext)
    {
        //Create the tracing service
        ITracingService tracingService = executionContext.GetExtension<ITracingService>();

        //Create the context
        IWorkflowContext context = executionContext.GetExtension<IWorkflowContext>();
        IOrganizationServiceFactory serviceFactory = executionContext.GetExtension<IOrganizationServiceFactory>();
        IOrganizationService service = serviceFactory.CreateOrganizationService(context.UserId);
            #region Fetch XMLC#
            //Fetch XMLC#

            var fieldValue = service.Retrieve(context.PrimaryEntityName , context.PrimaryEntityId , new ColumnSet(true));

            if (fieldValue.Contains("new_labelname"))
            {
                var fetchData = @"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>
                              <entity name='lead'>
                                <attribute name='fullname' />
                                <attribute name='companyname' />
                                <attribute name='telephone1' />
                                <attribute name='leadid' />
                                <order attribute='fullname' descending='false' />
                                <filter type='and'>
                                  <condition attribute='" + fieldValue["new_labelname"] + @"' operator='not-null' />
                                </filter>
                              </entity>
                            </fetch>";
                #endregion
                var leadCollection = service.RetrieveMultiple(new FetchExpression(fetchData));
                Entity leadEntity = new Entity(context.PrimaryEntityName, context.PrimaryEntityId);

                leadEntity["new_counter"] = leadCollection.Entities.Count;

                service.Update(leadEntity);
            }

     
    }
}
}


