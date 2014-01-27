using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using Sitecore.Configuration;
using Sitecore.ContentSearch.SearchTypes;
using Sitecore.ContentSearch.Security;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using Sitecore.Web;
using Sitecore.ContentSearch.Linq;
using Sitecore.Workflows;

namespace Heisenberg.sitecore.shell.client.YourApps.WorkflowDataSource
{
    public class WorkflowDatasourceController : Controller
    {
        /// <summary>
        ///     Accepts "BeginDate" and "EndDate" and retrieves all workflow records within that time span, one per item
        ///     Returns a JSON formatted string of:
        ///     {
        ///         errorMessage: string,
        ///         success: bool,
        ///         data: [UserName, IssuesUpdated]
        ///     }
        /// </summary>
        /// <returns></returns>
        public string GetWorkflowData()
        {
            //Initalize possible error message to be returned
            string errorMessage = string.Empty;

            //Grab begin and end date from the form data (Passed in as data using ajax)
            var beginDate = WebUtil.GetFormValue("BeginDate");
            var endDate = WebUtil.GetFormValue("EndDate");

            //Initalize return collection
            var data = new List<WorkflowUserRecords>();

            //Initalize Date Types
            DateTime begin;
            DateTime end;

            //Server side validation of date range
            if (DateTime.TryParse(beginDate, out begin) && DateTime.TryParse(endDate, out end) && begin < end)
            {
                //Add a day to end date to ensure we're getting changes made late into the selected day
                end = end.AddDays(1);

                //Grab the master indexer
                var index = Sitecore.ContentSearch.ContentSearchManager.GetIndex("sitecore_master_index");
                //Grab the master database
                var masterDb = Factory.GetDatabase("master");

                //Using the search context for the master indexer
                using (var searchIndex = index.CreateSearchContext(SearchSecurityOptions.DisableSecurityCheck))
                {
                    //Retrieve all items that have had their Updated date within the date range provided
                    var results =
                        searchIndex.GetQueryable<SearchResultItem>()
                            .Where(i => i.Updated > begin && i.Updated < end);

                    //Grab the SearchResultItems as actual sitecore items
                    var items = results.Select(i => i.GetItem());

                    //Get the workflow provider from the master database
                    var workflowProvider = masterDb.WorkflowProvider;

                    //Loop over the filtered items list
                    foreach (var item in items)
                    {
                        WorkflowEvent lastHistoryItem;

                        //Gets the latest workflow history entry for the current item. Returns false if any issues are found while getting entry
                        if (GetLatestWorkflowHistoryEntry(workflowProvider, item, begin, end, out lastHistoryItem))
                            continue;

                        //Update the data for the user that made the last history entry
                        UpdateDataForUser(data, lastHistoryItem);
                    }
                }
            }
            else
            {
                errorMessage = "Entries given are not in the correct date format.";
            }

            var returnValues = new
            {
                data,
                errorMessage,
                success = string.IsNullOrEmpty(errorMessage)
            };

            return new JavaScriptSerializer().Serialize(returnValues);
        }

        /// <summary>
        ///     Ensures a record exists for a user before adding to the list of issues they worked
        /// </summary>
        /// <param name="data"></param>
        /// <param name="lastHistoryItem"></param>
        private static void UpdateDataForUser(List<WorkflowUserRecords> data, WorkflowEvent lastHistoryItem)
        {
            //Retrieve the user that made the update from our return data, or null if no entry exist.
            //This acts similarly to a dictionary but gives us control of more information to be added later
            var person = data.FirstOrDefault(dataItem => dataItem.UserName == lastHistoryItem.User);

            //If the user that made the change to the current item has no previous entry make one
            if (person == null)
            {
                //Create entry for the user and add them to the return data
                person = new WorkflowUserRecords
                {
                    UserName = lastHistoryItem.User,
                    IssuesUpdated = 0
                };
                data.Add(person);
            }
            //Update the number of issues updated for the returned user
            person.IssuesUpdated++;
        }

        /// <summary>
        ///     Retrieves the latest history entry and determines if its latest modify date is within the date range
        /// </summary>
        /// <param name="workflowProvider"></param>
        /// <param name="item"></param>
        /// <param name="begin"></param>
        /// <param name="end"></param>
        /// <param name="lastHistoryItem"></param>
        /// <returns></returns>
        private static bool GetLatestWorkflowHistoryEntry(IWorkflowProvider workflowProvider, Item item, DateTime begin,
            DateTime end, out WorkflowEvent lastHistoryItem)
        {
            lastHistoryItem = null;

            //Get the workflow for the current item
            var itemWorkflow = workflowProvider.GetWorkflow(item);

            //If the workflow for the item is null then move to the next item
            if (itemWorkflow == null) return true;
            //Grab the history of the items workflow
            var itemHistory = itemWorkflow.GetHistory(item);

            //If the history of the item is null or has no entries then move to the next item
            if (itemHistory == null || itemHistory.Length <= 0) return true;

            //Grab the latest history update to an item
            lastHistoryItem = itemHistory[itemHistory.Length -1];

            //Ensure that the latest history update exist in the specified date range
            return lastHistoryItem.Date <= begin || lastHistoryItem.Date >= end;
        }

        /// <summary>
        ///     Simple Data Object for simple JSON seralizing
        /// </summary>
        private class WorkflowUserRecords
        {
            public string UserName { get; set; }
            public int IssuesUpdated { get; set; }
        }
    }

}