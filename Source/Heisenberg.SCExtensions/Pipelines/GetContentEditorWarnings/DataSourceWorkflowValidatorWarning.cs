using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sitecore.Data.Items;
using Sitecore.Globalization;
using Sitecore.Pipelines;
using Sitecore.Pipelines.GetContentEditorWarnings;
using Sitecore.Web.UI.HtmlControls;
using Sitecore.Workflows;
using Sitecore.Data;
using Sitecore.Data.Fields;
using Sitecore.Data.Validators;
using Sitecore.Layouts;

namespace Heisenberg.SCExtensions.Pipelines.GetContentEditorWarnings
{
    public class DataSourceWorkflowValidatorWarning
    {
        protected GetContentEditorWarningsArgs.ContentEditorWarning _contentEditorWarning = null;
        protected Dictionary<ID, bool> workflowStateIsFinal = new Dictionary<ID, bool>();

        public void Process(GetContentEditorWarningsArgs args)
        {
            Item item = args.Item;

            // check if the current item is in final workflow state and then check for validation errors 
            if (! Utility.IsItemInFinalWorkflowState(item, workflowStateIsFinal))
                return;
            // set the warning Title
            _contentEditorWarning = args.Add();
            _contentEditorWarning.Title = Sitecore.Globalization.Translate.Text("If you publish now, the current item may not render properly because the following dependencies have not been moved to a final workflow state. Click the links below to review these items:");
            
            SetValidatorWarningMessages(item, args);

        }
        /// <summary>
        /// Set the Validator warning messages if the Datasource Item is not in final work flow set at renderings on the Item
        /// </summary>
        /// <param name="item"></param>
        /// <param name="args"></param>
        protected void SetValidatorWarningMessages(Item item, GetContentEditorWarningsArgs args)
        {
            // get all the rendering at the item
            RenderingReference[] renderings = item.Visualization.GetRenderings(Sitecore.Context.Device, false);

            foreach (RenderingReference rendering in renderings)
            {
                if (rendering.Settings != null)
                {
                    Item dataSourceItem = Utility.GetDataSourceItem(rendering.Settings.DataSource, item);
                    if (dataSourceItem != null)
                    {
                        // check the state of the item
                        if (!Utility.IsItemInFinalWorkflowState(dataSourceItem, workflowStateIsFinal))
                          AddWarningOptions(dataSourceItem);
                    }
                }
            }
        }
        
        /// <summary>
        /// Add the warning options
        /// </summary>
        /// <param name="dataSourceItem">Datasource Item set at the rendering</param>
        private void AddWarningOptions(Item dataSourceItem)
        {
            _contentEditorWarning.AddOption(dataSourceItem.Name, string.Format("item:load(id={0},language={1},version={2})", dataSourceItem.ID, dataSourceItem.Language, dataSourceItem.Version.Number));
        }
    }
}
