# Aggregator Upload Skins 

## Using the Upload Skins menu

To use this option you must be logged in as host. Located any Aggregator
module and in the context menu for the module you will see an Upload
Skin option. 

![Upload Skin Menu](images\UploadSkin.png)

Select this option and then in the next screen, browse for your skin
file and then click on Upload Skin.

## Using ftp

If you are comfortable using ftp, this is a good option as well. Just
connect to your site using an ftp application, then browse to the
/DesktopModules/DNNStuff-Aggregator/Skins folder.

If you have a zip file that contains a skin, extract that first before
uploading and simply upload the skin to the /Skins folder.

### Skin File Structure

A skin file is simply a zip file that has been created by zipping up a
skin folder. The skin file is unzipped into the
/DesktopModules/DNNStuff-Aggregator/skins folder so your skin file
should include the proper folder structure to mimic the layout of the
skin.

*Example:*

If the skin you wish to create an upload for is named Outlook and has a
Top template inside it, the zip file would contain a folder in it's base
named Outlook, Outlook would have a Top folder inside it, and then the
Top folder would have it's individual skin files with in.

![Aggregator Skin Structure of Zip File](images\SkinStructure.png)
