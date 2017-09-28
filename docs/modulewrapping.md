
# Aggregator Module Wrapping 



What is module wrapping
-----------------------

In previous releases of Aggregator you only had a single way of
including modules within tabs. You merely added the module to the tab
and each module was shown in order underneath the optional html/text
area of the tab.

As of version 5.6.8 you now have the option of overriding this behaviour
with the module wrapping feature. To wrap a module you simply add the
modules token into the html/text area of the tab itself. The token for
each module is unique and is made up of the prefix MOD and the module id
of the module, for instance **[MOD478]** for the module with a module id
of 478. To make it easier to figure out, the module token is presented
in the listing after you add it to a tab.

![Showing module tokens](images\ModuleWrapping_Tokens.jpg)

Steps to wrap a module
----------------------

1.  Add a module to a tab using the 'Add an existing module to a tab'
    option
2.  Locate the proper token next to the module you just added. In this
    example the token is **[MOD478]**
3.  Edit the tab itself by clicking on the pencil icon next to the tab
4.  In the html/text area, add the module token placing it anywhere
    within your html markup and save.

The example below shows an example of wrapping the module inside a table
structure.

![Wrapping inside a table](images\ModuleWrapping_Html.jpg)

And here is the resulting tab after we have saved it.

![Wrapping result](images\ModuleWrapping_Result.jpg)
