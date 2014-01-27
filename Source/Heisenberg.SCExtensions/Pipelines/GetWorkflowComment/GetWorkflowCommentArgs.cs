using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Sitecore.Diagnostics;
using Sitecore.Pipelines;

namespace Heisenberg.SCExtensions.Pipelines.GetWorkflowComment
{
    /// <summary>
    /// Class to encapsulate arguments for the getWorkflowComment pipeline
    /// </summary>
    [Serializable]
    public class GetWorkflowCommentArgs : PipelineArgs
    {
        private string _result;
        private string _text;

        /// <summary>
        /// The resulting workflow comment text
        /// </summary>
        public string Result
        {
            get
            {
                return this._result;
            }
            set
            {
                this._result = value;
            }
        }

        /// <summary>
        /// The raw comment text from the WorkflowHistory table
        /// </summary>
        public string Text
        {
            get
            {
                return this._text;
            }
            set
            {
                Assert.ArgumentNotNull(value, "value");
                this._text = value;
            }
        }

        /// <summary>
        /// Constructor to initialize private member values
        /// </summary>
        /// <param name="text"></param>
        public GetWorkflowCommentArgs(string text)
        {
            Error.AssertObject(text, "text");
            this._text = text;
            this._result = text;
        }

        /// <summary>
        /// Deserializes arguments to corresponding private members
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        protected GetWorkflowCommentArgs(SerializationInfo info, StreamingContext context)
        {
            Assert.ArgumentNotNull(info, "info");
            this._text = (string)info.GetValue("text", typeof(string));
            this._result = (string)info.GetValue("result", typeof(string));
        }

        /// <summary>
        /// Serializes value of result
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            Assert.ArgumentNotNull(info, "info");
            base.GetObjectData(info, context);
            info.AddValue("result", this._result);
        }
    }
}
