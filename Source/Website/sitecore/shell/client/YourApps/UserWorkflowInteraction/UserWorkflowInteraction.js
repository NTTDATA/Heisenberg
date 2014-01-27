define(["sitecore",
    "/Scripts/jqPlot/plugins/jqplot.donutRenderer.min.js",
    "/Scripts/jqPlot/plugins/jqplot.pieRenderer.min.js"],
    function (Sitecore,
        jqplotPie,
        jqplotDoughnut) {

        //Use require to load the donut and pie renderers

  var model = Sitecore.Definitions.Models.ControlModel.extend({
    initialize: function (options) {
        this._super();
        $('#doughnutGraph').text("Select a date range to display data.");

        //Setup items property
        this.set("items", "");

        //Subscribed to the items changed
        this.on("change:items", this.updateWorkflowData, this);
    },

    updateWorkflowData: function () {
        //Get items
        var items = this.get("items");

        //Prepare data array
        var data = [];

        var issueCount = 0;
        //Loop through items (Object array) and add them to data in jQueryPlot format
        $(items).each(function (index, item) {
            data[index] = [item.UserName, item.IssuesUpdated];
            issueCount += item.IssuesUpdated;
        });

        //Empty the graph
        $('#doughnutGraph').empty();

        //If the data is empty then print a message that no items were found
        if (data.length === 0) {
            $('#doughnutGraph').text("No items found in provided date range");
        }
        else {
            var title = "User Workflow Activity <br /> Items moved through workflow <br />" + issueCount + " total issues";

            //Set up the graph using jQuery Plot
            var plot1 = jQuery.jqplot('doughnutGraph', [data],
                {
                    seriesDefaults: {
                        // Make this a pie chart.
                        renderer: jQuery.jqplot.PieRenderer,
                        rendererOptions: {
                            // Put data labels on the pie slices.
                            // By default, labels show the percentage of the slice.
                            showDataLabels: true,
                            dataLabels: 'value'
                        }
                    },
                    legend: { show: true, location: 'e' },
                    title: { text: title }
                }
            );
        }
    }
  });

  var view = Sitecore.Definitions.Views.ControlView.extend({
    initialize: function (options) {
        this._super();

    }
  });

  Sitecore.Factories.createComponent("UserWorkflowInteraction", model, view, ".sc-UserWorkflowInteraction");
});