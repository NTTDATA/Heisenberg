using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sitecore;
using Sitecore.Buckets.Pipelines.UI.DynamicFields;
using Sitecore.Configuration;
using Sitecore.Data;
using Sitecore.Data.Fields;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using Sitecore.Shell.Framework.CommandBuilders;
using Sitecore.Workflows;

namespace Heisenberg.SCExtensions.Pipelines.BucketDynamicFields
{
    public class GetWorkflowQuickActionHistory : DynamicFieldsProcessor
    {
        /// <summary>
        ///     Add the workflow history to the dynamic placeholders for views
        /// </summary>
        /// <param name="args"></param>
        public override void Process(DynamicFieldsArgs args)
        {
            //Set the Workflow History Actions dynamic field to a history link with a hidden table of history information
            args.QuickActions.Add("WorkflowHistoryActions", GetWorkflowHistory(args));
        }

        /// <summary>
        ///     Generates a button that can be clicked to show the history table
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        private string GetWorkflowHistory(DynamicFieldsArgs args)
        {
            //Get the item
            var item = args.InnerItem;

            //Create the history button
            var stringBuilder = new StringBuilder("<span class='history-button' style=\"font-weight:bold;");
            stringBuilder.Append("background: url(\'/temp/IconCache/Applications/16x16/history.png') no-repeat 6px 3px;");
            stringBuilder.Append("padding-left:25px;\">");
            stringBuilder.Append("<a href='' onclick=\"return false;\">History</a>");

            //Create the history table
            stringBuilder.Append(BuildHistoryView(item));

            //Close the loop
            stringBuilder.Append("</span>");

            //Return the information
            return stringBuilder.ToString();
        }

        /// <summary>
        ///     Creates a div/class based structure for styling that contains the history of the items workflow transitions
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        private string BuildHistoryView(Item item)
        {
            //Get the master database and the workflow provider
            var masterDb = Factory.GetDatabase("master");
            var workflowProvider = masterDb.WorkflowProvider;

            //Get the workflow
            var workflow = workflowProvider.GetWorkflow(item);

            //If we don't have a workflow, return
            if (workflow == null) return string.Empty;

            //Get history
            var history = workflow.GetHistory(item);

            //Create the main container for the history information
            var container = new StringBuilder("<div class='history-container' style='display:none;'>");

            //Loop through the history
            foreach (var historyEntry in history)
            {
                //Get the new and old state items
                var oldStateItem = masterDb.GetItem(historyEntry.OldState);
                var newStateItem = masterDb.GetItem(historyEntry.NewState);

                var newStateIconImg = newStateItem[FieldIDs.Icon];
                var newStateIcon = GetIconImageSource(newStateIconImg);

                container.Append("<div class='history-item'>");
                container.AppendFormat("<img src='{0}' height='16px' width='16px' />", newStateIcon);
                
                container.Append("<div class='history-header'>");
                container.AppendFormat("<div class='history-header-user'>{0}</div>", historyEntry.User);
                container.AppendFormat("<div class='history-header-date'>{0}</div>", historyEntry.Date.ToString("D"));
                container.Append("</div>");
                
                container.Append("<div class='history-seperator'></div>");
                
                container.Append("<div class='history-content'>");
                container.AppendFormat("<div class='history-content-transition'>Changed from <strong>{0}</strong> to <strong>{1}</strong></div>",
                    oldStateItem == null ? "No state" : oldStateItem.Name, newStateItem.Name);
                container.AppendFormat("<div class='history-content-comment'>{0}</div>", historyEntry.Text);
                container.Append("</div>");
                
                container.Append("</div>");
            }

            container.Append("</div>");

            return container.ToString();
        }

        /// <summary>
        ///  Returns the themed icon source path for the given command
        /// </summary>
        /// <param name="icon"></param>
        /// <returns></returns>
        private string GetIconImageSource(string icon)
        {
            if (icon.Length == 0)
            {
                icon = "/sitecore/images/blank.gif";
            }
            else
            {
                var idDefault = Sitecore.Web.UI.ImageDimension.idDefault;
                idDefault = Sitecore.Web.UI.ImageDimension.id16x16;
                icon = Sitecore.Resources.Images.GetThemedImageSource(icon, idDefault);
            }
            return icon;
        }
    }
}
