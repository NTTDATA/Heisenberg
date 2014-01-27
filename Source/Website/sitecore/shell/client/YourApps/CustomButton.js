define(["sitecore"], function (Sitecore) {
  Sitecore.Factories.createBaseComponent({
    name: "CustomButton",
    base: "ControlBase",
    selector: ".sc-CustomButton",
    attributes: [
    { name: "myProperty", value: "$el.text" }
    ]
  });
});
