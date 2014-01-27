using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sitecore.ContentSearch;
using Sitecore.ContentSearch.ComputedFields;
using Sitecore.Data.Items;

namespace Heisenberg.SCExtensions.ContentSearch.ComputedFields
{
    /// <summary>
    /// Computed field to index an item's workflow state
    /// </summary>
    public class WorkflowState : IComputedIndexField
    {
        /// <summary>
        /// Gets or sets the indexed field name
        /// </summary>
        public string FieldName { get; set; }

        /// <summary>
        /// Gets or sets the indexed return type
        /// </summary>
        public string ReturnType { get; set; }

        /// <summary>Computes the display name of an item's workflow state</summary>
        /// <param name="indexable">the indexable item</param>
        /// <returns>the workflow state display name</returns>
        public object ComputeFieldValue(IIndexable indexable)
        {
            Item item = indexable as SitecoreIndexableItem;

            // Check item is not null
            if (item == null)
            {
                return null;
            }

            // Check for database
            var database = item.Database;
            if (database == null)
            {
                return null;
            }

            // Check for workflow provider on database
            if (database.WorkflowProvider == null)
            {
                return null;
            }

            // Check if item is in workflow
            var wf = database.WorkflowProvider.GetWorkflow(item);
            if (wf != null)
            {
                // Return display name of workflow state
                var state = wf.GetState(item);
                if (state != null)
                {
                    return state.DisplayName;
                }
            }

            return null;
        }
    }
}
