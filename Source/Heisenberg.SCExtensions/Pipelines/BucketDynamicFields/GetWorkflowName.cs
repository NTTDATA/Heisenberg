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
    public class GetWorkflowName : DynamicFieldsProcessor
    {
        /// <summary>
        ///     Adds the workflow name to the dynamic placeholders for views
        /// </summary>
        /// <param name="args"></param>
        public override void Process(DynamicFieldsArgs args)
        {
            //Add the workflow name to the dynamic placeholders
            args.QuickActions.Add("Workflow", GetItemWorkflowName(args));
        }

        /// <summary>
        ///     Get the workflow name out of the current item
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        private string GetItemWorkflowName(DynamicFieldsArgs args)
        {
            //Grab item and master database
            var item = args.InnerItem;
            var masterDb = Factory.GetDatabase("master");

            //Initalize the workflow name return value
            var workflowName = "N/A";

            //If the item exist and the item has a workflow ID set then set the workflow name return value
            if (item == null) return workflowName;

            //Get the workflow item ID
            var workflowItemId = item.Fields[FieldIDs.Workflow].Value;
            if (string.IsNullOrEmpty(workflowItemId)) return workflowName;

            //Get the workflow item
            var workflowItem = masterDb.GetItem(workflowItemId);

            //Set the workflow name to the return value
            if (workflowItem != null)
                workflowName = workflowItem.Name;

            return workflowName;
        }
    }
}
