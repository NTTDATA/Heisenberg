using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sitecore.Buckets.Pipelines.UI.DynamicFields;

namespace Heisenberg.SCExtensions.Pipelines.BucketDynamicFields
{
    public class GetItemLanguage : DynamicFieldsProcessor
    {
        /// <summary>
        ///     Add the item language to the dynamic placeholders for views
        /// </summary>
        /// <param name="args"></param>
        public override void Process(DynamicFieldsArgs args)
        {
            //Add the item language as a dynamic placeholder
            args.QuickActions.Add("Language", args.InnerItem.Language.Name);
        }
    }
}
