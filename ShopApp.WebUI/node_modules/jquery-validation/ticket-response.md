Get help ticket
---

Thanks for the report, but this doesn't indicate an issue with the validation plugin. To get help, I recommend to put together a test page using [jsfiddle](http://jsfiddle.net) or [jsbin](http://jsbin.com). Also a post on [the forum](http://forum.jquery.com/using-jquery-plugins) or [StackOverflow](http://stackoverflow.com) is going to be more helpful.

Email valid/invalid, should be invalid/valid!
---

Thanks for the report. Unfortunately what you consider a valid email address can be invalid elsewhere, and the other way round. This plugin is using the same regular expression that the [HTML5 specification suggests for browsers to use](http://www.whatwg.org/specs/web-apps/current-work/multipage/states-of-the-type-attribute.html#e-mail-state-%28type=email%29). We will follow their lead and use the same check. If you think the specification is wrong, please report the issue to them.
