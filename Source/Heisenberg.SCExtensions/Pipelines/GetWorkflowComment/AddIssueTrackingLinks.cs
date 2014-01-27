using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Sitecore.Configuration;
using Sitecore.Diagnostics;

namespace Heisenberg.SCExtensions.Pipelines.GetWorkflowComment
{
    /// <summary>
    /// Finds Issue IDs within a workflow comment and links them to their
    /// corresponding issue tracking URL.
    /// </summary>
    public class AddIssueTrackingLinks
    {
        /// <summary>
        /// Regular expression used to locate Issue IDs within comments (case-insensitive)
        /// </summary>
        static Regex _issueIdPattern = new Regex(Settings.GetSetting("WorkflowIssueTracker.IssueIdPattern"), RegexOptions.IgnoreCase);

        /// <summary>
        /// Replaces any issue IDs in the comment with fully-formatted HTML links
        /// </summary>
        /// <param name="args"></param>
        public void Process(GetWorkflowCommentArgs args)
        {  
            Assert.ArgumentNotNull(args, "args");
            if (!string.IsNullOrEmpty(args.Result))
            {
                args.Result = _issueIdPattern.Replace(args.Result, GetIssueTrackingLink);
            }
        }

        /// <summary>
        /// Returns the fully-formatted HTML link for an issue
        /// </summary>
        /// <param name="match"></param>
        /// <returns></returns>
        protected virtual string GetIssueTrackingLink(Match match)
        {
            string issueId = match.ToString();
            string url = string.Format(Settings.GetSetting("WorkflowIssueTracker.UrlFormat"), issueId);
            return string.Format("<a href=\"{0}\" target=\"_blank\">{1}</a>", url, issueId);
        }
    }
}
