# Aggregator Templates 

The layout of each Aggregator skin is ultimately defined by a template
of some sort. In the Edit Tabs screen you will see two dropdown boxes,
one is named **Tab Skin** and the other is named **Tab Template**. By
definition, the Tab Skin is merely a way to group a number of Tab
Templates together that have the same color and style while exhibiting
different layouts etc. The reason it's set up this way is because in
earlier versions of Aggregator we had skins and layouts where layouts
could be one of a number of different looks such as top, bottom, left,
right and inline. Somewhere around version 4 of Aggregator I decided
that I'd just template each layout separately and that is where the
template concept came from.

Let's recap:

-   A skin corresponds to a sub folder of the
    /DotNetNukeModules/DNNStuff - Aggregator/Skins folder and groups a
    number of subfolders.

-   A template corresponds to a sub folder of the
    /DotNetNukeModules/DNNStuff - Aggregator/Skins/*SkinName* folder
    where *SkinName* is the name of the skin.

Each template is then made up of a number of html template files and
optionally a script.txt file (new in 5.6.5). There are currently 7
different html files that are normally used to make up a complete
template although a few of them are optional if various features within
the Aggregator module are not enabled. For instance if you don't turn on
the paging option in Aggregator then the Paging and PagingItem template
files are unnecessary. In fact if you leave them out of your template
and do decide to enable the option later on, the Aggregator module will
use the default templates located in the /DotNetNukeModules/DNNStuff -
Aggregator/Resources/Templates folder. The same is true for the optional
scripts.txt file that is used to drive the behaviour of the tabs. This
file is located in the /DotNetNukeModules/DNNStuff -
Aggregator/Resources/Scripts folder and is the base script to drive all
of the tab functionality. If you wish to override this functionality for
a specific skin template you can copy it into the template directory and
modify it and it will be picked up there each times it's needed for the
skin you are using.

The following documentation is an exhaustive list of templates and
tokens used in Aggregator. The best way to learn is to take a look at a
set of functional template files and scripts and lookup what each token
does.

![Skin, Template and Resource Folders](images\SkinFolder.jpg)

## HTML Template Files

Template files are standard text files that contain html elements as
well as other Aggregator specific tokens and define how each piece of
the Aggregator will look when rendered on the page. Although the
template files are html they are not formal html files in that they do
not contain the html, body, and title tags etc.

When the Aggregator is rendering itself, the first thing it looks for is
the Layout.html file and everything it does from then on depends on what
tokens it comes across in this file.

### Layout

**Layout.html** - this defines the overall layout of the module and
should contain the [TABSTRIP] and [TABPAGES] tokens. This file will
determine the overall layout of the skin such as whether tabs are on the
top ,bottom ,left ,right ,inline or some other layout you come up with.
An example of a different type of layout you could produce would be an
accordion layout or a scrolling news layout using the RSS feed
capability of Aggregator.

### TabStrip

**TabStrip.html** - this defines the layout of the section that contains
the tabs should contain a [TABS] token. This file will be used when the
[TABSTRIP] token is encountered.

### Tab

**Tab.html** - this defines how each tab will be rendered and must
contain [TABID], [TABACTION] and [TABCAPTION] tokens. This file will be
used when the [TAB] token is encountered for each tab shown for the
module.

### TabPage

**TabPage.html** - this defines how each content pane will be rendered
and must contain [TABPAGEID], [TABPAGEACTION] and [TABPAGECONTENT]
tokens. This file will be used when the [TABPAGE] token is encountered
for each tab shown for the module.

### Paging

**Paging.html** - this defines how the paging controls will be rendered
and should contain some combination of [PAGEFIRSTACTION],
[PAGELASTACTION], [PAGEPREVACTION], [PAGENEXTACTION] and
[PAGINGITEMLIST] tokens. This file will be used when the [PAGING] token
is encountered OR if you have paging enabled and haven't included the
[PAGING] token then this will be included at the end of the layout
automatically.

### PagingItem

**PagingItem.html** - this defines how the individual paging page number
controls will be rendered and must contain a [PAGEACTION] token. This
file will be used when the [PAGINGITEMLIST] token is encountered. It is
repeated for each tab shown for the module.

### RSSContent

**RSSContent.html** - this defines how RSS items will be rendered and
should contain a number of the RSS tokens defined below. This file will
be used for each RSS item in the RSS feed and will be rendered inside
the [TABPAGECONTENT] token.

![Anatomy of a Skin Template](images\TabAnatomy.gif)

## Script Files

Most of the shipped skins rely on a single script file named script.txt
located in /DotNetNukeModules/DNNStuff - Aggregator/Resources/Scripts.
In the absence of a script.txt file within the template folder of your
skin, this is the file that will be used to run the tabs. In some of the
sample tokens located below you will see javascript command such as
Agg465\_SelectTab(1) etc. These token values are generated to match the
function names within the default script.txt file. The script.txt file
is not linked to like a typical javascript file might be. It is read in
and parsed for tags and then included within the rendered content of the
page for each Aggregator on the page.

### Initialize function

When the module is rendered, a special function is called once when the
page loads so that the Aggregator may initialize itself. The function
called is named [UNIQUE]Initialize(). As mentioned below, the [UNIQUE]
token will evaluate to a unique string for the instance of the
Aggregator. For example the initialize function for an Aggregator with a
moduleid of 465 will evaluate to Agg465\_Initialize(). If you wish to
create a custom template with a custom script.txt file you must include
this function even if it doesn't do anything. The best way to start
customizing the script.txt file is to copy the default one from the
/DotNetNukeModules/DNNStuff - Aggregator/Resources/Scripts folder into
your own template folder and then start to customize.

## Tokens

Tokens are used inside template html files and are replaced at runtime
with content specific to the token name.

### Layout Tokens

**[TABSTRIP]**

Replaced with contents of the tabstrip.html template

**[TABS]**

Replaced with the content created by multiple applications of the
tab.html template, once for each tab.

**[TABPAGES]**

Replaced with the content created by multiple applications of the
tabpage.html template, once for each tab.

**[TABPAGECONTENT]**

Replaced by the content contained on the tab page, i.e. the modules you
are including on the tab

**[PAGING]**

Replaced with the content created by the paging.html template. If the
paging option is not turned on in the module settings, this will be
replaced with an empty string. If the prev/next option is turned on and
you have not defined a [PAGING] token in your template layout, one will
be automatically added to the bottom of your layout.

**[PAGINGITEMLIST]**

Replaced with the content created by multiple applications of the
pagingitem.html template, once for each tab. This is normally used to
create numeric pager links, one for each tab such as 1,2,3,4,5.


### Tab Tokens

The following tokens are specific to the tab and are normally used in
the tab specific templates such as Tab.html and TabPage.html. The values
of these tokens changes depending on which tab is being iterated over at
the time.

**[TABID]**

Replaced by a unique id for the tab. Ex. Agg465\_Tab3

**[TABNUMBER]**

Replaced by the numeric tab number of the tab. Ex. 3

**[TABCAPTION]**

Replaced by the tab caption. Ex. Tab 3

**[TABPAGEID]**

Replaced by a unique id for the tab page.

**[TABACTION]**

Replaced by the click and hover actions required by a tab. Ex.
> onclick=javascript:Agg465\_SelectTab(3,465);
> onmouseover=javascript:Agg465\_MouseOverTab(this);
> onmouseout=javascript:Agg465\_MouseOutTab(this);

**[PAGEITEMACTION]**

Replaced by the click actions required to select this tab. Ex.
> onclick=javascript:Agg465\_SelectTab(3,465);

**[CURRENTTAB]**

Replaced by True if this tab is the currently selected tab or False if
otherwise. Ex. True

**[LASTTAB]**

Replaced by True if this tab is the last tab or False if otherwise. Ex.
True

**[NEXTTABCAPTION]**

Replaced by the next tab caption. Ex. Tab 4

**[NEXTTABNUMBER]**

Replaced by the next tab number or 1 if this the last tab. Ex. 4

**[POSTBACK]**

Replaced by True if this tab is configured as a postback tab or False if
otherwise. Ex. False

**[POSTBACKSELECTTAB]**

Replaced by the url that will cause the postback tab to be shown. Ex.

> http://localhost/Home/tabid/37/Agg465\_SelectTab/3/Default.aspx

**[MODULETITLE]**

Replaced by the title of the 1st module within the tab or empty string
if there isn't a module embedded. Ex. Contacts

**[MMLINKSTITLE]**

Replaced by the title of the 1st MMLinks module within the tab or empty
string if there isn't a module embedded. This token is used when you
wish the localized MMLinks module title to appear somewhere in the tab
template and is normally used in the Tab title field.

### Aggregator Tokens

The following tokens are specific to the Aggregator module and can be
used in any of the templates. The values of these tokens does not change
after rendering starts. Many of these tokens are used only within the
script.txt file.

**[UNIQUE]**

Replaced by a unique string for the Aggregator instance. Currently it is
replaced with the combination of the string 'Agg' then the moduleid and
then an underscore but could change in future versions. This is used in
various script and template files to create unique element names so two
or more Aggregators on a single page don't have naming conflicts. Ex.
Agg465\_

**[PARENTID]**

Replaced by the Aggregator parent client id Ex.
> dnn\_ctr465\_ModuleContent

**[MODULEID]**

Replaced by the ModuleId of the Aggregator Ex. 465

**[TABMODULEID]**

Replaced by the TabModuleId of the Aggregator Ex. 23


**[SKIN]**

Replaced by the combination of skin name and template name. This is
generally used to create a full class name for an element. Ex.
Default\_Top

**[SKINFOLDER]**

Replaced by the path to the skin folder. This is generally used to
provide a path to images in templates. For instance, if your skin is
named 'Default' and your template is 'Top', this will point to the
'\\DesktopModules\\DNNStuff - Aggregator\\Skins\\Default\\Top' folder

**[SKINBASEFOLDER]**

Replaced by the path to the skin base folder. This is generally used to
provide a path to images in templates that are used for the whole skin
series. For instance, if your skin is named 'Default', this will point
to the '\\DesktopModules\\DNNStuff - Aggregator\\Skins\\Default' folder

**[SELECTEDTABNUMBER]**

Replaced by the number of the currently selected tab. Ex. 3

**[TABCOUNT]**

Replaced by number of tabs shown. Ex. 10

**[TABPAGEACTION]**

Replaced by the actions required for the tab page.

**[IMAGEURL]**

Replaced by the url that points to the /images folder in the DNN root
directory.

> http://localhost/images

**[SELECTTARGET]**

Replaced by the code required for the tab targets to be selected. This
is used only in the script.txt file.

**[PAGEFIRSTACTION]**

Replaced by the click actions required to select the first tab.

> onclick=javascript:Agg465_SelectTab(1,465);

**[PAGEPREVACTION]**

Replaced by the click actions required to select the prev tab.

> onclick=javascript:Agg465_SelectPrevTab();

**[PAGENEXTACTION]**

Replaced by the click actions required to select the next tab.

> onclick=javascript:Agg465_SelectNextTab();

**[PAGELASTACTION]**

Replaced by the click actions required to select the last tab.

> onclick=javascript:Agg465_SelectTab(10,465);

**[FIRSTCAPTION]**

Replaced by the caption of the first tab. Ex. Tab 1

**[PREVCAPTION]**

Replaced by the caption of the previous tab. Ex. Tab 2

**[NEXTCAPTION]**

Replaced by the caption of the next tab. Ex. Tab 4

**[LASTCAPTION]**

Replaced by the caption of the last tab. Ex. Tab 10


### Aggregator Option Tokens

The following tokens are specific to the Aggregator module and can be
used in any of the templates. The values of these tokens does not change
after rendering starts. They are equivalent to the options you will find
in the Aggregator settings screen. These tokens are primarily used in
the script.txt file to control template flow by using the [IFTOKEN] and
[IFNOTTOKEN] logic tests.

**[HIDETABS]**

Aggregator option to hide tabs Ex. True/False

**[ACTIVEHOVER]**

Aggregator option for active hover Ex. True/False

**[ACTIVEHOVERDELAY]**

Aggregator option for active hover delay Ex. 1000

**[HIDESINGLETAB]**

Aggregator option to hide single tabs Ex. True/False

**[HIDETITLES]**

Aggregator option to hide titles Ex. True/False

**[SHOWPAGER]**

Aggregator option to show pager Ex. True/False

**[DEFAULTTABNUMBER]**

Aggregator option for the default tab number Ex. 4

**[REMEMBERLASTOPENTAB]**

Aggregator option to remember last open tabs across page views Ex.
True/False


### RSS Specific Tokens

These tokens should only be used in an RSSContent.html template.
(version 5.5+)

**[RSSTITLE]**

Replaced by the RSS item title.

**[RSSAUTHOR]**

Replaced by the RSS item author.

**[RSSDESCRIPTION]**

Replaced by the RSS item description.

**[RSSLINK]**

Replaced by the RSS item link url.

**[RSSPUBDATE]**

Replaced by the RSS item publish date.

**[RSSENCLOSUREURL]**

Replaced by the RSS item enclosure url (if there is one).

**[RSSENCLOSURETYPE]**

Replaced by the RSS item enclosure type (if there is one) ex.
image/jpeg.

## Token Logic

Tokens, especially the ones that evaluate to either a True or a False,
can be used to perform some rudimentary logic within the script or
template files.

See [Tokens\_Logic|Logic Tokens]

As an example, here is a snippet that I did for a client that uses the
logic syntax of tokens to render different content based on various
Aggregator options. 

```xml

[IFPOSTBACK value="true"]

[IFCURRENTTAB value="true"]

<li>
<span><span id="[TABID]" [TABACTION] class="[SKIN]_TabUnselected">[TABCAPTION]</span></span>

</li>
[/IFCURRENTTAB] [IFCURRENTTAB value="false"] [IFACTIVEHOVER value="true"]

<li>
<a href="[POSTBACKSELECTTAB]"><span><span id="[TABID]" onmouseover="javascript:[UNIQUE]MouseOverTab(this);
parent.location='[POSTBACKSELECTTAB]';" class="[SKIN]_TabUnselected">[TABCAPTION]</span></span></a>

</li>
[/IFACTIVEHOVER] [IFACTIVEHOVER value="false"]

<li>
<a href="[POSTBACKSELECTTAB]"><span><span id="[TABID]" onmouseover="javascript:[UNIQUE]MouseOverTab(this);"
onmouseout="javascript:[UNIQUE]MouseOutTab(this);" class="[SKIN]_TabUnselected">[TABCAPTION]</span></span></a>

</li>
[/IFACTIVEHOVER] [/IFCURRENTTAB] [/IFPOSTBACK] [IFPOSTBACK value="false"]

<li>
<span><span id="[TABID]" [TABACTION] class="[SKIN]_TabUnselected">[TABCAPTION]</span></span>

</li>
[/IFPOSTBACK]

```

As you can see in this example, the token logic syntax can be nested
within other tokens to combine various criteria.
