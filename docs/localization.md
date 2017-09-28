# Aggregator Localization 

The aggregator module may be localized it two very different ways.

## Content Localization

### Using MMLINKS

MMLINKS is a free localization module which can be found
[here](http://dnn.tiendaboliviana.com/web/Modulesandresources/Modules/MMLinks/tabid/77/Locale/en-US/Default.aspx).
It is produced by Locopon and is a very good free localization
container. Please make a donation to support him if you use the module.

You simply add an MMLINKS module to your page and use it's localization
features. Then you add it into an Aggregator tab and for the tab caption
you use a special token **[MMLINKSTITLE]**. This special token will be
replaced with the MMLINKS module caption depending on what localized
content is shown.


### Using MLHTML

MLHTML is a free localization module which can be found
[here](http://www.apollo-software.nl/DotNetNuke/Modules/FreeModules/tabid/83/Default.aspx).
It is produced by Erik van Ballegoij of Apollo Software and is another
very good free localization container. Please make a donation or
purchase other modules from Apollo to support Erik if you use the
module.

You simply add an MLHTML module to your page and use it's localization
features. Then you add it into an Aggregator tab and for the tab caption
you use a special token **[MLHTMLTITLE]**. This special token will be
replaced with the MLHTML module caption depending on what localized
content is shown.


### Using tab localization

To use tab localization you need to add a tab for each locale you wish
to target. In the tab properties you can then set the locale that the
tab will be shown for. You also have the option of setting a 'fallback'
tab which will be shown only if all other tabs are not shown. For
instance you could have a fallback tab of english and two other tabs,
one for italian and one for french. If the user visiting has their
locale set to spanish, it would show only the english tab.


## Tab Localization

### Using Tab Caption localization format

This method of localizing the tab caption expects a string formatted
with locales and the accompanying text:

-   Format is: locale-1:locale-1 caption|locale-2:locale-2
    caption|...|locale-n:locale-n caption
-   Example: en-us:English|it-it:Italian|es-es:Spanish

