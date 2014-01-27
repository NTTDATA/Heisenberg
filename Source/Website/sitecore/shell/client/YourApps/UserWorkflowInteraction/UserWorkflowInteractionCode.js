define(["sitecore"], function (Sitecore) {
  var UserWorkflowInteractionCode = Sitecore.Definitions.App.extend({
      initialized: function () {

          //Subscribe to the text change even on both dates
          this.StartDate.on("change:text", this.dateChanged, this);
          this.EndDate.on("change:text", this.dateChanged, this);

          //Set the error display and text color
          this.DateError.viewModel.$el.css("color", "red");
          this.DateError.viewModel.$el.css("display", "none");
      },

      convertToDate: function (date) {
          //TODO: Consolidate the regex for date conversions in JavaScript
          var matches = /^(\d{1,2})[-\/](\d{1,2})[-\/](\d{4})$/.exec(date);
          if (matches == null) return undefined;
          var d = matches[2];
          var m = matches[1] - 1;
          var y = matches[3];
          var composedDate = new Date(y, m, d);
          return composedDate;
      },

    isValidDate: function (date) {
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
        var beginDate = this.convertToDate(beginDateStr);
        var endDate = this.convertToDate(endDateStr);

        return endDate > beginDate;
    },

    dateChanged: function () {
        //Prepare the date error message and retrieve the start and end dates
          var dateMessage = '';
          var startDate = this.StartDate.get('text');
          var endDate = this.EndDate.get('text');

        //Verify that the start date is formatted correctly
          if (startDate && !this.isValidDate(startDate))
              dateMessage = "Start date must be a valid date.<br />";
        //Verify that the end date is formatted correctly
          if (endDate && !this.isValidDate(endDate))
              dateMessage += "End date must be a valid date.<br />";
        //Verify that the start date occurs befor the end date
          if (!this.beginDateBeforeEndDate(startDate, endDate))
              dateMessage += "Start date must be before End date.";

        //If there are any errors then show the message, otherwise hide it
          if (dateMessage)
              this.DateError.viewModel.$el.css("display", "block");
          else
              this.DateError.viewModel.$el.css("display", "none");
        //Set the text of the error
          this.DateError.viewModel.$el.html(dateMessage);
      }
  });

  return UserWorkflowInteractionCode;
});