var self = require("sdk/self");
var data = self.data;
var pageMod = require("sdk/page-mod");


pageMod.PageMod({
  include: ["*.youtube.com"],
  contentScriptFile: data.url("YouTube15.js"),
  contentScriptWhen: "start"
});
