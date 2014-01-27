using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sitecore.Data.Items;
using Sitecore.Data.DataProviders.Sql;
using Sitecore.Diagnostics;
using Sitecore.Pipelines;
using Sitecore.Workflows;
using Heisenberg.SCExtensions.Pipelines.GetWorkflowComment;

namespace Heisenberg.SCExtensions.Data.SqlServer
{
    /// <summary>
    /// Overrides GetHistory method of base HistoryStore to inject
    /// getWorkflowComment pipeline
    /// </summary>
    public class SqlServerHistoryStore : Sitecore.Data.SqlServer.SqlServerHistoryStore
    {
        public SqlServerHistoryStore(SqlDataApi api)
            : base(api)
        {
        }

        public SqlServerHistoryStore(string connectionString)
            : base(connectionString)
        {
        }

        /// <summary>
        /// Runs SQL query to get workflow events from WorkflowHistory table
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public override WorkflowEvent[] GetHistory(Item item)
        {
            Assert.ArgumentNotNull(item, "item");
            List<WorkflowEvent> list = new List<WorkflowEvent>();
            string sql = " SELECT {0}OldState{1}, {0}NewState{1}, {0}Text{1}, {0}User{1}, {0}Date{1} FROM {0}WorkflowHistory{1} WHERE {0}ItemID{1} = {2}itemID{3} AND {0}Language{1} = {2}language{3} AND {0}Version{1} = {2}version{3} ORDER BY {0}Sequence{1}";
            object[] parameters = new object[] { "itemID", item.ID.ToGuid(), "language", item.Language.ToString(), "version", item.Version.ToInt32() };
            using (DataProviderReader reader = this._api.CreateReader(sql, parameters))
            {
                while (reader.Read())
                {
                    string oldState = this._api.GetString(0, reader);
                    string newState = this._api.GetString(1, reader);
                    string text = this._api.GetString(2, reader);
                    string user = this._api.GetString(3, reader);
                    DateTime dateTime = this._api.GetDateTime(4, reader);

                    // Pipeline to customize workflow comment output
                    GetWorkflowCommentArgs args = new GetWorkflowCommentArgs(text);
                    CorePipeline.Run("getWorkflowComment", args);
                    text = Sitecore.StringUtil.GetString(args.Result);
                    
                    list.Add(new WorkflowEvent(oldState, newState, text, user, dateTime));
                }
            }
            return list.ToArray();
        }
    }
}
