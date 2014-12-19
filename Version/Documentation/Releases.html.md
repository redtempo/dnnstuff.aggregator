```
title: Release History
tags: ['intro','page']
```

```Minimum configuration DNN 6.0.3+ / DNN7+ / .NET 3.5 only```

<!-- insert-newversion -->

## 06.04.09

19/Dec/2014


* Fixes
	* Minor theme styling fixes



## 06.04.08

09/Apr/2014

* Breaking changes
	* Changes to how SelectByNum and SelectByTitle work. I changed the links to use Agg{ModuleId}\_SelectByNum={TabNumber} and Agg{ModuleId}\_SelectByTitle={TabTitle} to simplify the api. Please see new documentation page here, http://docs.dnnstuff.com/pages/aggregator/selectingtabs


## 06.04.07

28/Mar/2014

* Fixes
	* Fixed SelectByTitle tab selection

## 06.04.06

18/Feb/2014

* Fixes
	* Modified to skip rss password encryption if not password set

## 06.04.05

05/Feb/2014

* Fixes
	* Fixed issue where jQueryUI was not getting included for some skins


## 06.04.03

03/Feb/2014

* Fixes
	* Fixed issue with rotation in jQuery UI scripts

## 06.04.02

30/Jul/2013

-   Fixes
    -   Fixed bug in Blank skin
    -   Fixed jQuery Tabs/Accordion when used in DNN 7.1

## 06.04.01

14/Mar/2013

-   Updates
    -   Changed the default for the module to not load it's version of
        jQueryUI (Script Manager)

## 06.04.00

20/Feb/2013

-   Updates
    -   Added DNN7 version compiled against DNN 7.0.0

---

```Minimum configuration DNN 5.2.3+ / DNN 6+ / .NET 3.5 only```

## 06.03.00

1/Aug/2012

-   Updates
    -   Updated some edit screen styling for DNN6

## 06.02.09

16/Jul/2012

-   Updates
    -   Updated to support Azure deployment

-   Bug Fixes
    -   Fixed issue with sql install script regarding sysobjects

## 06.02.08

06/Mar/2012

-   Fixes
    -   Fixed a bug with import/export introduced in 6.2.7

## 06.02.07

03/Mar/2012

-   New Features
    -   Added localization support for the free Nuntio Content module
        (http://nuntiocontent.codeplex.com/)
        -   To localize a tab, place a Nuntio module into a tab and then
            use the token [NUNTIOTITLE] in the tab title

-   Enhancements
    -   Update default jQuery UI script loading for DNN6
    -   Update install for DNN5/DNN6
    -   Update styling for DNN6 edit screens

-   Fixes
    -   Fixed an issue with the jShowoff skin

## 06.02.06

14/Dec/2011

-   Added Kwicks skin template
    (http://www.dnnstuff.com/Modules/AggregatorTabbedModules/AggregatorDemos/kwicks/tabid/424/Default.aspx)

## 06.02.05

18/Nov/2011

-   Added module ordering within tabs

## 06.02.04

11/Oct/2011

-   Fixed IE7 positioning bug when using fixed height/width

## 06.02.03

25/Aug/2011

-   Fixed an error with Import/Export (IPortable) - LoadEvent property
    was causing an ModuleLoadException
-   Fixed an issue caused by modulesettings.cs in DNN 6.0

## 06.02.01

15/Aug/2011

-   Fixed a problem with Copy Aggregator - modules inside Aggregator
    weren't showing up in the list of modules

## 06.02.00

8/Aug/2011

-   Added [LOCALE] token
-   Fixed DNN6 compatiblity issues
-   Minimum DNN 5.1 or DNN 6.0

## 06.01.09

15/Dec/2010

-   NOTE: Last DNN4 version
-   Fixed Turkish 'i' problem with token parsing

## 06.01.08

23/Nov/2010

-   Added tab caption localization using formatted string
    -   Format is: locale-1:locale-1 caption|locale-2:locale-2
        caption|...|locale-n:locale-n caption
    -   Example: en-us:English|it-it:Italian|es-es:Spanish

## 06.01.07

11/Nov/2010

-   Added [LASTTAB] token - if value is True then tab is the last tab
-   Added LoadEvent property to module edit - this determines when
    modules are rendered during the page lifecycle - the default setting
    is fine for 99% of modules

## 06.01.06

19/Oct/2010

-   Fixed bug in uninstall
-   Fixed missing {objectQualifier} in stored proc
    DNNStuff\_Aggregator\_UpdateAggregatorModule
-   Fixed bug in module wrapping - [MODxxxx] wasn't being removed if
    user didn't have access to view the module

## 06.01.05

07/Oct/2010

-   Fixed deadlock issue with ListAggregator stored procedure

## 06.01.04

27/Jul/2010

-   Updated cookie handling so only a single cookie needed across all
    Aggregator instances
    -   This should resolve the problem of exceeding the browser domain
        cookie limit on sites that use many Aggregators
    -   If you have created your own templates that include a custom
        script.txt file you will need to edit the file to take advantage
        of this. See wiki for details on [SAVEACTIVETAB] script token

-   Added jShowOff template (see available demo)
-   Added collapsible and start collapsed settings to jQueryUI Accordion
-   Removed styles from module.css and added them to
    /Resources/Support/edit.css to reduce some css burden
-   Added two new custom property types, Directory and Files, to the
    settings.xml specification

## 06.01.02

13/Jul/2010

-   Updated compatibility with DNN 5.5.0 beta

## 06.01.01

04/Jun/2010

-   Fixed bug where skin specific tab properties weren't properly being
    saved for new tabs

## 06.01.00

05/May/2010

-   Minimum DNN version is now 4.6.2! Please don't upgrade to this
    version if you are lower than 4.6.2.
-   Fixed problem with unhandled error in ModuleCommunication section
-   Fixed problem with SmallImageUrl property in jQueryUI/ContentSlider
    skin

## 06.00.14

01/Apr/2010

-   Added module.css back to the build after I went missing
    -   Anyone who upgraded to 06.00.12 through 06.00.13 can skip this
        release.
    -   If you installed 06.00.12 through 06.00.13 as a new module
        install then please upgrade to this version
    -   to enable tabs within the management screens

## 06.00.13

29/Mar/2010

-   Updated jQueryUI/Accordion to support Active Hover

## 06.00.12

29/Mar/2010

-   Updated jQueryUI/Accordion to support Remember last tab, default tab

## 06.00.11

18/Mar/2010

-   Added CodaSlider skin - requires jQuery and easings
-   Added [MODULEFOLDER] tag - points to base Aggregator folder i.e.
    \\DesktopModules\\DNNStuff - Aggregator\\
-   Added jquery.easing.1.3.js to Resources and is now included for any
    skins requiring jQuery
-   Updated TabPage.html in all skins - added display:none to style and
    removed [TABPAGEACTION]
-   Deprecated [TABPAGEACTION]
-   Updated script.txt files - added //[CDATASTART] and //[CDATAEND] for
    XHTML compatibility
-   Removed tables from 'No Container.ascx' for those who want to do
    tableless design

## 06.00.10

12/Mar/2010

-   Fixed a bug in token generation regarding QS\_ querystring tokens
    resulting in a null object reference

## 06.00.09

01/Mar/2010

-   Added href="\#[MODULEID]" to Resources\\Paging.html
    -   this will allow intelligent tab navigation keeping the current
        module in view

-   Added jQuery tabs skin with custom settings
-   Added jQuery accordion skin with custom settings
-   Added jQuery featured content slider with custom settings
-   Added jQuery support
    -   if script.txt includes token [REQUIRESJQUERY] the module will
        attempt to reference the jQuery library if the option is set in
        the Script Manager

-   Added jQueryUI support
    -   if script.txt includes token [REQUIRESJQUERYUI] the module will
        attempt to reference the jQuery and jQueryUI library if the
        option is set in the Script Manager

-   Added Script Manager screen
    -   allows you to manage portal wide script settings for jQuery and
        jQueryUI library inclusion

-   Added Skin Manager screen
    -   allows you to copy skins from one name to another
    -   allows you to edit skin files

-   Added querystring values to available tokens
    -   If querystring key and value is Test1=value1 then the token
        would be QS\_Test1

-   Added module settings option to allow embedded module settings to be
    edited from within Aggregator management screen
-   Added blank template
    -   Useful when using as a target of another aggregator and when
        hiding tabs

-   Changed all tabright.gif files to be wider (300px) to support wider
    tab captions
-   Changed all settings screens to tabbed based interface (using light
    weight Yetii tabs, very nice, <http://www.kminek.pl/lab/yetii/>)
-   Added head.txt script file for injection of script into the head of
    a page
-   Fixed a bug when max rss items was greater than the number of rss
    items available
-   Added height/width settings
    -   Added [HEIGHT], [WIDTH], [HEIGHT\_STYLE], [WIDTH\_STYLE] tokens
    -   Modified all shipped skins to use [HEIGHT\_STYLE],
        [WIDTH\_STYLE] tokens
    -   If height/width are integers then px units are assumed,
        otherwise text is used as is
    -   [HEIGHT\_STYLE] will contain width:100px; if 100 entered,
        width:30em; if 30em entered or empty string if nothing entered
        (same for [WIDTH\_STYLE]) - makes templates easier to create
        without adding additional template logic tokens

-   Fixed a problem with selecting the default tab
-   Upgraded markup, javascript to be XHTML compliant
-   Added IPortable support
-   Added custom template properties for Aggregator and for tabs
    -   This allows you to further customize your own custom skins with
        properties that are selectable within the Edit tabs screen
    -   Properties are supplied in a properties.xml file within the skin
        folder and can be set by the module editor

-   Added Copy Aggregator
    -   This new feature allows you to copy an exising Aggregator to
        another page, included the embedded modules either by reference
        or duplicates

## 05.06.08

14/Jul/2009

-   Added [Aggregator\_ModuleWrapping|module wrapping] functionality
    -   Up until now, modules added to a tab had to appear in order
        below the html/text content of the tab. Now you can simply add a
        special token into the tab content area that injects the module
        exactly where you want it
    -   Ex. If the module you are injecting into the tab has a module id
        of 345, simple enter the token [MOD345] into the html/text of
        the tab content area. The token needed for each module is shown
        beside the module name in the list of the tabs modules.

## 05.06.07

16/Jun/2009

-   Added Agg[ModuleId]\_HideTabs querystring syntax to hide tabs
    -   Ex. Agg384\_HideTabs=3,5 will hide tabs 3,5 for an Aggregator
        with moduleid of 384

-   Added [Aggregator\_Localization|MLHTML] (Apollo Module Localization)
    support using [MLHTMLTITLE] token for tab title

## 05.06.06

31/Mar/2009

-   Fixed a problem reported with postback tab urls not including
    previous querystring values

## 05.06.05

31/Mar/2009

-   Updated javascript injection to support skin specific javascript
    overrides
    -   Under normal operation, the module uses script.txt from the
        /Resources/Scripts folder to control tab functions. If you wish
        to make modifications to this behaviour, simply copy the
        script.txt file into your skin folder and it will be used
        instead.

-   Updated module rendering to remove table if only a single module
    rendered inside a tab
    -   When modules are rendered inside a tab page, a table is used to
        more precisely position the modules. Now, if only a single
        module is rendered inside a tab (which is the most often case),
        the table isn't used at all

-   Updated token replacement for templates
    -   Mainly internal code changes for token replacement inside
        templates
    -   Added more tokens

-   Added an option to add tabs quickly by specifying tab names in bulk
    (Quick Settings -\> Quick Tabs)
    -   This option allows you to add a bunch of tabs quickly by
        specifying multiple tab names at the same time, 1 per line

-   Added an option to add all remaining modules on the current page as
    tabs (Add All Page Modules)
    -   As an example where this is handy, say you have a page with 4
        modules on it and you want to have each module show up in it's
        own tab. Simply add an Aggregator to the page, go into 'Edit
        Tabs' and click on the 'Add All Page Modules' button and it will
        create four tabs, and place each module into it's own tab

-   Fixed issue where modules on other pages were losing their ordering
    - [Discussion on
    Forum](http://www.dnnstuff.com/Support/Forums/tabid/189/forumid/4/postid/1451/view/topic/language/en-US/Default.aspx|Issue)
    -   A user (robg) noticed that if he brought modules from other
        pages into Aggregator, they would sometime lose their panel
        placement and ordering in the other page.

-   Fixed issue where copied modules were being hidden if they were in
    Aggregator on one page but weren't on another
-   Added postback option for tabs - if enabled, the tab click will
    cause a postback and modules won't be rendered on postback tabs if
    they are not the selected tab
    -   If you have created your own skin and want to enable this option
        you should look at how it's done in the tab.html and
        tabpage.html of any shipped skins

-   Added default tab number - the initial tab opened can be set to a
    tab other than tab 1
-   Added an option to select whether or not the last opened tab is
    remembered across page views
    -   This option is only active when the page is in View mode, when
        in Edit mode or on a page postback it should remember the tab
        you were on

## 05.05.01

09/Feb/2009

-   Fixed a bug causing repeated calls to the
    Delisoft\_MMLinksTitleLocales

## 05.05.00

01/Jan/2009

-   Fixed issue where [UNIQUE] token wasn't getting rendered for final
    layout
-   Tweaked default skin - removed tabnumber from caption, thinned
    tabpage border, fixed bottom tab
-   Added RSS functionality
    -   Tabs and their content can be driven by an rss feed. Each item
        in the rss feed becomes a tab. You can limit the number of items
        retrieved and the content can be templated using the
        RSSContent.html template file

-   Changed token replacement engine, now accepts [IFtoken] and
    [IFNOTtoken] syntax
-   Fixed search indexing - indexed text will now open the correct tab,
    not just the last opened tab or first
    -   At this point this only works for text entered through the
        intrinsic text/html facility of the tab and doesn't include
        modules contained within the tabs

## 05.00.03

22/Sept/2008

-   Added MultiColored style for the Simple skin
-   Fixed EditTabs - Container.DataItem issues - Medium Trust
    -   User using Medium Trust at godaddy hosting found a problem with
        Medium Trust

## 05.00.02

29/July/2008

-   Added ability to specify custom javascript within the skin templates
-   Fixed IE bug when using back button (ActivateTab, ActivateTabPage
    now hide all tabs before showing the current one)

## 05.00.01

22/Apr/2008

-   Fixed ExplodingBoy, TabMenuB
-   Fixed querystring select tab linking
-   Added help to EditTab for creating tab link url and javascript tab
    select
-   Added support for MMLinks from Locopon. Use [MMLINKSTITLE] token in
    tab caption to grab localized caption for first module in tab

## 05.00.00

09/Feb/2008

-   Added template support
-   Updated caption token replacement to built in DNN TokenReplace
    function
-   Fixed problem when user deleted module using DNN delete function
    before deleting from the Aggregator
-   Added link column to Tabs section in Manage Aggregator - link
    provides syntax needed to select a given tab from a link on another
    page or same page
-   Added customizable delay for Active Hover - default is 0
    milliseconds (no delay)
-   Client side tab links, [TABMODULEID]\_SelectTab(n);
-   Added hide all tabs
-   Added targeting other Aggregators
-   Templates now reside inside of skins

## 04.05.02

12/Sep/2007

-   Changed loading so that modules that support partial rendering are
    rendered in page init

`   while all other modules are rendered in page load as before `

## 04.05.01

19/Jul/2007

-   Added objectQualifier to constraints, problem when doing multiple
    host installs in a single db
-   Changed loading to intialize to reflect changes to ajax partial
    rendering

## 04.05.00

07/Apr/2007

-   Fixed DNN 4.5.0 compatibility issue

## 04.04.01

13/Mar/2007

-   Fixed issue with datareader that wasn't being closed properly

## 04.04.00

15/Jan/2007

-   Reversioned to 4.4.x to reflect DNN 4.4.0+ status
-   Fixed issue that caused only a single fallback tab to remain if more
    than one fallback was present

## 04.00.03

29/Dec/2006 - DNN 4.4.0+ only

-   Fixed ClearTabCache problem introduced with DNN 4.4.0

## 04.00.02

13/Sep/2006

-   Fixed Simple Inline style
-   Added XPLunaVar skin - a variable width caption skin
-   Fixed problem determining connection string with DNN 3.2.0

## 04.00.01

31/Jul/2006

-   Removed condition that tab should be visible for modules to be added
    from it

## 04.00.00

18/Jul/2006

-   Added Prev/Next links
-   Added PrevNext style to themeing
-   Added Multiple Modules per tab
-   Added Tab and Module localization

## 03.01.04

30/Jan/2006

-   Fixed issue that caused certain modules with grids etc. not to page
    or bind properly
-   Fixed issue that caused nested modules to sometimes display both
    inside and outside the Aggregator

## 03.01.01

18/Dec/2005

-   Changed cookie information path to root instead of page level

## 03.01.00

05/Aug/2005

-   added alternate caption which can include html to display for tab
    caption
-   tokens can be used to substitute common portal variables within the
    caption text

## 03.00.02

09/May/2005

-   Fixed issue that caused the querystring tab selection to fail
-   Tabs may be selected by using the syntax
    Module[AggregatorModuleId]\_SelectById=[ModuleId], for example ...
    default.aspx?Module123\_SelectById=345, will select the tab
    containing module345 within the Aggregator module 123

## 03.00.01

20/Mar/2005

-   Fixed issue that caused 'All Tabs' modules to appear multiple times
    in the module selector
-   Added a custom 'No Container.ascx' file that is used to render
    modules without titles

## 03.00.00

06/Feb/2005

-   Port to DotNetNuke 3.0.12

