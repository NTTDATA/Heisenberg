using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sitecore.Shell.Framework.Commands;
using Sitecore.Web.UI.Sheer;

namespace Heisenberg.SCExtensions.Commands
{
    /// <summary>
    ///     Helper for opening a SPEAK component from the start menu
    /// </summary>
    public class OpenWorkflowPerformanceTool : Command
    {
        public override void Execute(CommandContext context)
        {
            //Command required to open a SPEAK component from the start menu. There seems to be a bug when trying to open SPEAK using a normal Application Shortcut
            SheerResponse.ShowModalDialog("/sitecore/client/your%20Apps/UserWorkflowInteraction");
        }
    }
}
