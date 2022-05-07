# HousePartyTranslator
## Download
You can get the latest release [here](https://github.com/CamelCaseName/HousePartyTranslator/releases/latest).

## Usage
Just start the .exe file. On your first launch it will ask you for a password, 
you can get that from @CamelCaseName(Lenny on the discord). 
It will also ask you to select a language from the dropdown list at the top. 
Refer to [this video](https://www.youtube.com/watch?v=Si3EU0niGX4) 
for more infos on this application and how to use it.

# Hotkeys and other hot stuff
List of all hotkeys so far

|keys|action|remarks|
|-----|-----|-----|
|CTRL + O|select a file to load into the current tab||
|CTRL + SHIFT + O|select a file to be opened in a new tab||
|CTRL + S|saves the file to disk|file will also be saved to the game, use setting "alsoCopyToGame" to control|
|CTRL + SHIFT + S|saves the currently selected line||
|CTRL + F|moves the cursor to the search box|use a '?' at the beginning of your search to perform it in all tabs|
|CTRL + R|reloads the current file in the current tab|can be used to get rid of miscoloured lines|
|CTRL + UP|select the line above||
|CTRL + DOWN|select the line below||
|CTRL + LEFT|cylcle tabs to the left||
|CTRL + RIGHT|cycle tabs to the right||
|CTRL + ENTER|save line and select the one below||
|CTRL + SHIFT + ENTER|save the line, set it as approved and move one down||
|CTRL + E|opens the AutoStoryExplorer||
|CTRL + X|opens the StoryExplorer where you can select the file yourself||
|CTRL + P|opens the settings menu||
|ENTER|when performing a search, enter will cycle through the search results|use a '?' at the beginning of your search to perform it in all tabs, this will also then cycle through tabs|

## Settings


# Development
If you want to build and or develop this application, 
you need the solution and all its files form this repo, 
as well as the MySql Package for Visual Studio, 
for that you need to install the MySql Development config 
from their website and then add the MySql Package to 
the solution via the NuGet package manager.
