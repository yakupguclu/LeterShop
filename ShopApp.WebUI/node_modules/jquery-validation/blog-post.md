Support the jQuery Validation Plugin - Adopt a Theme
combine with release of 1.12?
combine with launch of bountysource campaign?
===

[new logo]

In [start of pledgie campaign], I started a modest campaign to raise 4000€ to finance further development of the jQuery Validation plugin. [time passed since] later, the campaign is at [current amount], around [percent of total]%. First of all, a big thank you to everyone who has donated! Here's a breakdown what I did with the funds so far:

* Buy jqueryvalidation.org and pay for hosting, setting up WordPress there and filling the site with content migrated from the old docs.jquery.com wiki.
* Pay a designer to create a logo for the library and a design for the site. You can see the logo above. You can't see the design on the site though, which is the main reason for posting this.

I've also spent a significant amount of time reviewing issues and pull requests, addressing and merging a lot of them. While its been a while since the last release, the 1.12 release will be done soon (working on that). By a very rough estimate, the time I've spent over the last year working on the site and the code is worth about 12.000€ that I could've spent on time billed to clients. And since there's still [number of open issues] open issues, there's a lot more time I could spend on this library.

Here's where you come in. In order for me to spent more time working on the library code, I'm looking for help in two areas: More funding, and someone to put the design in HTML/CSS and, eventually, a WordPress theme.

Looking for theme developer
---

You can see the new logo above, but its still not live on jqueryvalidation.org, nor is the theme. To make that happen, I'm looking for a developer to implement the design I already have into HTML/CSS, and integrate that into a WordPress theme. If you're interested in helping with this, here's everything you need:

* [This zip file](link-to.zip) contains the logo and the design, consisting of photoshop screenshots and a text file with color values [etc. pp.?].
* [This theme repo](https://github.com/jzaefferer/validation-theme) contains the existing theme, which inherits from a default WordPress theme, adding styles for the API documentation. A pull request against this repository would be perfect, but I'd be happy with any other form of contribution.
* [This content repo](https://github.com/jzaefferer/validation-content) contains the existing content of the site. Testing the new theme should be much easier when this is deployed as content to a WordPress instance. The setup is pretty reasonable, and I can help with setting it up.

What do you get if you'd build this theme? I'm open for suggestions in this area. The site already has a decent amount of traffic (about [rough traffic numbers]) and I'd be happy to list you or your company prominently as a sponsor on the site.

Why am I not doing this myself? As I've outlined above, I'm spending a significant amount of unpaid time on maintaining the plugin. That's work that is hard to outsource, since it requires good knowledge of how the implementation. On the other hand, building a WordPress theme is a relatively generic task. And while I could do this, I'm not even particular good at creating WordPress themes - I'm sure there's lots of developers who could get this done much faster than me, producing better quality code (especially the PHP portion).

New funding campaign
---

I'm launching a new funding campaign on bountysource.com! (This replaces the campaign on pledgie.org, since that site is now riddled with spam and generally unmaintained). This campaign is set at 6.000€, 50% above my previous campaign, enough to cover a years worth of a few hours per week of maintenance. This time around there are some backer benefits: BountSource will print t-shirts [and other stuff?] with the logo on them and ship them out to backers. They handle all the logistics, so it won't slow me down at all. If you aren't interested in [shirts or whatever], you can just back at one of the digital tiers. [Since BountySource will provide me with an actual list of backers, I'll be able to send updates and thank backers on the site (or whatever)].

Why is this library still relevant? Its 2014, we've got HTML5 form validation, right?
(put this on the campaign page?)
---

Here are a few reasons why this library is still relevant and will likely never be replaced by native browser features:

* Custom message display: If you've ever tried to use the native HTML5 validation, you probably noticed that each browser displays error message slightly differently. What's worse, there is no way to customize those error message. The only option you have is to implement the message display yourself - at that point, you're likely better off using this plugin.
* remote validation: This library supports remote validation, where the value of a field is sent to a server, for example in a registration form, where the value of the email field is checked against already registered users. This sounds harmless, but turns out to be fairly complicated. Consider this case: The user fills out the form, with the email address last. He then clicks on submit immediately. Now the library has to prevent the native submit event, make an ajax request to the server, wait for the response, check the response, then show an error message or, if the email was valid, trigger another form submit. For the form submit to be equivalent to the original submit, the library also has to capture what submit button was used, store that, then create a hidden input with the key/value of the submit button in the form, then trigger the submit, then remove the hidden input, but not immediately.
* [number of methods in total, in words]. That's the number of methods the library bundles, including everything under "additional_methods"(??). By now this is quite a significant collection of validation logic that browsers will never provide and that would require significant time investments to rebuild as needed. For example, validation credit card numbers is easy, using the Luhn algorithm. But that only looks at a checksum and isn't too reliable. To go further, you need to know what CC providers support what number ranges, in both length and valid digits. All this is implemented in the [additional cc method].
* Globalization! The library currently bundles translations in [number of translations] locales, from the common ones like spanish and chinese, to more uncommon ones like [most obscure locales]. Except for the english and german translations, which I wrote myself, all of these are user contributed. It is hard to estimate how much money was saved by sharing these translations between users of the library, but I'm pretty sure it is a significant amount.

That said, this library actually makes it very easy to try the HTML5 validation and then switch to using this library if you need more: The library supports most of the attributes like `required` `type="email|url|...?"`, `min="5"`, `max="15"` and even `pattern="\d+"` (via additional methods, still not recommended).

TODO: How do you specify custom messages in HTML5 validation? Can I adopt that?