using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sitecore.Data;
using Sitecore.Data.Fields;
using Sitecore.Data.Validators;
using Sitecore.Data.Items;
using Sitecore.Layouts;
using Sitecore.Pipelines;
using Sitecore.Web.UI.HtmlControls;
using Sitecore.Workflows;

namespace Heisenberg.SCExtensions
{
    /// <summary>
    /// Utility calss for helper functions
    /// </summary>
    public class Utility
    {
        protected static Database _master = Sitecore.Configuration.Factory.GetDatabase("master");
        /// <summary>
        /// Get Datasource Item from rendering Datasource
        /// </summary>
        /// <param name="dataSource">Rending Datasource</param>
        /// <param name="item">Item</param>
        /// <returns></returns>
        public static  Item GetDataSourceItem(string dataSource, Item item)
        {
            Item dataSourceItem = null;
            if (!string.IsNullOrEmpty(dataSource))
            {
                try
                {
                    dataSourceItem = _master.GetItem(dataSource);
                }
                catch (Exception ex)
                {
                    Sitecore.Diagnostics.Log.Warn("Error Getting Source Item :: DataSourceWorkflowValidator", ex, item );
                }
            }
            return dataSourceItem;
        }

         /// <summary>
         /// Checks the final work state of the items and return true
         /// </summary>
         /// <param name="item">Datasource Item</param>
         /// <param name="workflowStateIsFinal">local cache</param>
         /// <returns></returns>
        public static  bool IsItemInFinalWorkflowState(Item item, Dictionary<ID, bool> workflowStateIsFinal)
        {
            bool finalState = false;
            if (item.Fields[Sitecore.FieldIDs.WorkflowState].HasValue)
            {
                Item workFlowState = null;
                try
                {
                    ID itemId = new ID(item.Fields[Sitecore.FieldIDs.WorkflowState].Value);
                    // check if the work flow state item exists in local cache
                    if (workflowStateIsFinal.ContainsKey(itemId))
                    {
                        finalState = (bool) workflowStateIsFinal[itemId];
                    }
                    else
                    {
                        workFlowState = _master.GetItem(new ID(item.Fields[Sitecore.FieldIDs.WorkflowState].Value));
                        if (workFlowState != null)
                        {
                            CheckboxField finalcheckBox = workFlowState.Fields[Sitecore.WorkflowFieldIDs.FinalState];
                            if (finalcheckBox != null && finalcheckBox.Checked)
                                finalState = true;
                        }
                        // add the work flow item in local cache
                        workflowStateIsFinal.Add(itemId,finalState);
                    }

                }
                catch (Exception ex)
                {
                    Sitecore.Diagnostics.Log.Warn("Error Getting Wrokflow State Item :: DataSourceWorkflowValidator", ex, item);
                }
                
            }
            return finalState;
        }
    }
}
