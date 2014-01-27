using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
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

namespace Heisenberg.SCExtensions.Data.Validators.ItemValidators
{
    [Serializable]
    public class DataSourceWorkflowValidator : Sitecore.Data.Validators.StandardValidator
    {
        
        public DataSourceWorkflowValidator() : base()
        {

        }

        public DataSourceWorkflowValidator(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        /// <summary>
        /// Override the Evaluate function to put the custom logic for Datasource Workflow Validator logic
        /// </summary>
        /// <returns></returns>
        protected override ValidatorResult Evaluate()
        {
            // local cache for Workflow items state
            Dictionary<ID, bool> workflowStateIsFinal = new Dictionary<ID, bool>();
            // Get the current item where validation is bieng applied
            Item item = this.GetItem();
            // get the item renderings
            RenderingReference[] renderings = item.Visualization.GetRenderings(Sitecore.Context.Device, false);

            foreach (RenderingReference rendering in renderings)
            {
                if (rendering.Settings != null)
                {
                    Item dataSourceItem = Utility.GetDataSourceItem(rendering.Settings.DataSource, item);
                    if (dataSourceItem != null)
                    {
                        // check the state of the item
                        if (dataSourceItem.Fields[Sitecore.FieldIDs.WorkflowState].HasValue)
                        {
                            // check the state of the item
                            if (! Utility.IsItemInFinalWorkflowState(dataSourceItem, workflowStateIsFinal))
                                return ValidatorResult.Error;
                        }
                    }
                }
            }
            
            return ValidatorResult.Valid;
        }

        protected override ValidatorResult GetMaxValidatorResult()
        {
            return ValidatorResult.Warning;
        }

        public override string Name
        {
            get { return "DataSourceWorkflow"; }
        }

        
    }
}
