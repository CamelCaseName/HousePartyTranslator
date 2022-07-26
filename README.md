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
|CTRL + ALT + S|Saves all open files||
|CTRL + F|moves the cursor to the search box|use a '?' at the beginning of your search to perform it in all tabs. use a '!' to enable case sensitive search. you can combine them also like so '?!search' for a case sensitive multitab search. use a '\\' in front of the ? or ! to treat it as a literal. somewhat combineable.|
|CTRL + R|reloads the current file in the current tab|can be used to get rid of miscoloured lines|
|CTRL + UP|select the line above||
|CTRL + DOWN|select the line below||
|CTRL + ENTER|save line and select the one below||
|CTRL + SHIFT + ENTER|save the line, set it as approved and move one down||
|CTRL + E|opens the AutoStoryExplorer||
|CTRL + T|opens the StoryExplorer where you can select the file yourself||
|CTRL + P|opens the settings menu||
|ENTER|when performing a search, enter will cycle through the search results|use a '?' at the beginning of your search to perform it in all tabs, this will also then cycle through tabs|
|ALT + LEFT|cylcle tabs to the left||
|ALT + RIGHT|cycle tabs to the right||

## Settings
Some explanation for the settings and what they do

|name|default value|explanation|
|-----|-----|-----|
|alsoSaveToGame|True|if true, the program will try to save the files to the game as well for easy testing|
|askForSaveDialog|True|set this to false to disable the "save changes" dialog popping up when exiting|
|autoLoadRecent|True|load the most recently opened file on start when enabled|
|autoLoadRecentIndex|True|loads the most recenlty selected line on start when enabled and possible|
|autoSave|True|enable automatic saving of the last line when a new one is selected|
|autoTranslate|True|if true, the program will try to translate the selected line if it has never before been seen|
|dbPassword||the password for the database|
|displayVAHints|False|set to True to enable the voice acting hints in the node info text in the StoryExplorer|
|enableDiscordRP|True|set to False to disable the discord rich presence integration|
|language||the two letter short form of the language you have selected|
|recent_index|-1|the index of the line to resume to, should not be edited!|
|recents_0||a path to the most recent file, should not be edited!|
|recents_1||a path to the second most recent file, should not be edited!|
|recents_2||a path to the third most recent file, should not be edited!|
|recents_3||a path to the fourth most recent file, should not be edited!|
|recents_4||a path to the fifth most recent file, should not be edited!|
|story_path||a file path to the folde rcontaining the original story character files, set when loading the first file ever into the StoryExplorer|
|template_path|C:\\Users\\|points to the folder containing the new template files for the story, in the correct folder structure. not used in normal program behaviour|
|translation_path|path to the last opened file, used as a reference to open the folder dialog in it's parent folder|
|useFalseFolder|False|will add a "new" after the language in the game folder in order to enable development of languages already implemented in the game.|
|version|0.22|the game's story verison currently present in the database|


# Development
If you want to build and or develop this application, 
you need the solution and all its files form this repo, 
as well as the MySql Package for Visual Studio, 
for that you need to install the MySql Development config 
from their website and then add the MySql Package to 
the solution via the NuGet package manager.
You'll probably also need some additional NuGet packages.
