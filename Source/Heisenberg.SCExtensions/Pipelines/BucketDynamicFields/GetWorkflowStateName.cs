using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sitecore;
using Sitecore.Buckets.Pipelines.UI.DynamicFields;
using Sitecore.Configuration;

namespace Heisenberg.SCExtensions.Pipelines.BucketDynamicFields
{
    public class GetWorkflowStateName : DynamicFieldsProcessor
    {
        /// <summary>
        ///     Add the workflow state name to the dynamic placeholders for views
        /// </summary>
        /// <param name="args"></param>
        public override void Process(DynamicFieldsArgs args)
        {
            //Add the workflow name of the current item to the Dynamic Placeholder list
            args.QuickActions.Add("WorkflowState", GetItemWorkflowStateName(args));
        }

        /// <summary>
        ///     Get the workflow state name from the current item
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        private string GetItemWorkflowStateName(DynamicFieldsArgs args)
        {
            //Grab item and master database
            var item = args.InnerItem;
            var masterDb = Factory.GetDatabase("master");

            //Initalize the workflow name return value
            var workflowStateName = "N/A";

            //If the item exist and the item has a workflow ID set then set the workflow name return value
            if (item == null) return workflowStateName;

            //Get the workflow item ID
            var workflowStateItemId = item.Fields[FieldIDs.WorkflowState].Value;
            if (string.IsNullOrEmpty(workflowStateItemId)) return workflowStateName;

            //Get the workflow item
            var workflowStateItem = masterDb.GetItem(workflowStateItemId);

            //Set the workflow name to the return value
            if (workflowStateItem != null)
                workflowStateName = workflowStateItem.Name;

            return workflowStateName;
        }
    }
}
