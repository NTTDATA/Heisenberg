define(["sitecore"], function (Sitecore) {
    var model = Sitecore.Definitions.Models.ControlModel.extend({
        initialize: function (options) {
            this._super();

            this.set("BeginDate", "");
            this.set("EndDate", "");
            this.set("Output", "");
            this.set("errorText", "");

            //Subscribe to Begin and End date changes
            this.on("change:BeginDate", this.updateItems, this);
            this.on("change:EndDate", this.updateItems, this);
        },

        convertToDate: function (date) {
            /// <summary>
            ///     Converts the given string date into a Date object
            /// </summary>
            var matches = /^(\d{1,2})[-\/](\d{1,2})[-\/](\d{4})$/.exec(date);
            if (matches == null) return undefined;
            var d = matches[2];
            var m = matches[1] - 1;
            var y = matches[3];
            var composedDate = new Date(y, m, d);
            return composedDate;
        },

        isValidDate: function (date) {
            /// <summary>
            ///     Ensures the date string provided is an actual date
            /// </summary>
            var matches = /^(\d{1,2})[-\/](\d{1,2})[-\/](\d{4})$/.exec(date);
            if (matches == null) return false;
            var d = matches[2];
            var m = matches[1] - 1;
            var y = matches[3];
            var composedDate = new Date(y, m, d);
            return composedDate.getDate() == d &&
                    composedDate.getMonth() == m &&
                    composedDate.getFullYear() == y;
        },

        beginDateBeforeEndDate: function (beginDateStr, endDateStr) {
            /// <summary>
            ///     Ensures the begin date occurs befor the end date
            /// </summary>
            var beginDate = this.convertToDate(beginDateStr);
            var endDate = this.convertToDate(endDateStr);

            return endDate > beginDate;
        },

        updateItems: function () {
            /// <summary>
            ///     Updates the output using the given input using ajax
            /// </summary>

            //Grab the begin and end date
            var beginDate = this.get("BeginDate");
            var endDate = this.get("EndDate");

            //Perform validations on the dates to ensure that they are properly formatted and aligned
            if (this.isValidDate(beginDate) && this.isValidDate(endDate) && this.beginDateBeforeEndDate(beginDate, endDate)) {

                //Do an ajax call to the workflow data source item to retrieve a list of information regarding workflow between the two dates
                $.ajax({
                    url: "/api/sitecore/WorkflowDatasource/GetWorkflowData",
                    type: "POST",
                    data: {
                        beginDate: beginDate,
                        endDate: endDate
                    },
                    context: this,
                    success: function (data) {
                        var jsonData = JSON.parse(data);
                        if (jsonData.success) {
                            this.set("errorText", "");
                            this.set("Output", jsonData.data);
                        }
                        else {
                            this.set("errorText", jsonData.errorText);
                        }
                    }
                });
            }
        },

    });

    var view = Sitecore.Definitions.Views.ControlView.extend({
        initialize: function (options) {
            this._super();

        }
    });

    Sitecore.Factories.createComponent("WorkflowDatasource", model, view, ".sc-WorkflowDatasource");
});
