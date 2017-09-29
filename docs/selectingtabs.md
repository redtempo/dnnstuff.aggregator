In the default configuration, the first tab is always the initial tab to
be shown. Sometimes it is desirable to have another tab show instead.
There are a couple different ways to accomplish this.

To select a tab using a url you will need to add a querystring parameter
to your url in a specific format. You can select the tab in one of three
ways.

## By Tab Number - SelectByNum

Add the following querystring parameter to your url,
Agg{ModuleId}\_SelectByNum={TabNumber} where {ModuleId} is the
ModuleId of the Aggregator module and {TabNumber} is the tab number you
wish to select.

### Example

The original page url is ```<http://localhost/Test/tabid/54/Default.aspx>```

The module id of the Aggregator module is 370 and you wish to select tab
number 3.

The url to select this is, ```<http://localhost/Test/tabid/54/Default.aspx?Agg370_SelectByNum=3>```

## By Tab Title - SelectByTitle

Add the following querystring parameter to your url,
Agg{ModuleId}\_SelectByTitle={TabTitle} where {ModuleId} is the
ModuleId of the Aggregator module and {TabTitle} is the title of the tab
you wish to select.

### Example

The original page url is ```<http://localhost/Test/tabid/54/Default.aspx>```

The module id of the Aggregator module is 370 and you wish to select the
tab with a title of 'Page 5'

The url to select this is, ```<http://localhost/Test/tabid/54/Default.aspx?Agg370_SelectByTitle=Page 5>```


## Selecting a tab using Javascript

To select a tab using javascript, you simply have to call the proper
javascript function for the specific Aggregator module and tab. The
syntax for the function is Agg**ModuleId**\_SelectTab(**tabnumber**,**ModuleId**);

### Example

The module id of the Aggregator module is 370 and you wish to select tab
3

``` js
<a onclick="javascript:Agg370_SelectTab(3,370);" href="javascript:void(0)">Link To Tab 3</a>
```
