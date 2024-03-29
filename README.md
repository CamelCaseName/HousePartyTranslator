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
|ALT + SHIFT + O|select a story folder to open all files in||
|CTRL + S|saves the file to disk|file will also be saved to the game, use setting "alsoCopyToGame" to control|
|CTRL + SHIFT + S|saves the currently selected line||
|ALT + SHIFT + S|Saves all open files||
|CTRL + F|moves the cursor to the search box|See [Search](#search)|
|CTRL + R|reloads the current file in the current tab|can be used to get rid of miscoloured lines|
|CTRL + UP|select the line above|can also be used to cycle through search queries when in the search box|
|CTRL + DOWN|select the line below|can also be used to cycle through search queries when in the search box|
|CTRL + ENTER|save line and select the one below||
|CTRL + SHIFT + ENTER|save the line, set it as approved and move one down||
|CTRL + E|opens the AutoStoryExplorer||
|CTRL + T|opens the StoryExplorer where you can select the file yourself||
|CTRL + P|opens the settings menu||
|ENTER|when performing a search, enter will cycle through the search results|use a '?' at the beginning of your search to perform it in all tabs, this will also then cycle through tabs|
|ALT + LEFT|cylcle tabs to the left||
|ALT + RIGHT|cycle tabs to the right||
|CTRL + A|Automatically translate the current sentence||

## Settings
Some explanation for the settings and what they do

|name|default value|explanation|
|-----|-----|-----|
|alsoSaveToGame|True|if true, the program will try to save the files to the game as well for easy testing|
|askForSaveDialog|True|set this to false to disable the "save changes" dialog popping up when exiting|
|autoLoadRecent|True|load the most recently opened file on start when enabled|
|autoLoadRecentIndex|True|loads the most recenlty selected line on start when enabled and possible|
|autoSave|True|enable automatic saving of the last line when a new one is selected|
|AutoSaveInterval|5 min| the interval in which the progress is automatically saved|
|autoTranslate|True|if true, the program will try to translate the selected line if it has never before been seen|
|ColoringDepth|3|The depth of the nodes that are colored in the StoryExplorer|
|dbPassword||the password for the database|
|dbUserName|user|the username for the database|
|displayVAHints|False|set to True to enable the voice acting hints in the node info text in the StoryExplorer|
|enableCustomStories|False|enables custom stories to be translated with this software|
|enableDiscordRP|True|set to False to disable the discord rich presence integration|
|enableStoryExplorerEdit|False|wether or not you can edit the extended info in the StoryExplorer|
|explorerMaxEdgeCount|100000|the max number of edges displayed in the StoryExplorer|
|exportTranslationInMissingLines|False|will add the translated string to the template when exporting missing lines|
|highlightLanguages|True|highlights the languages a story is being worked on in the language list|
|idealLength|120|the ideal length an edge should have in the StoryExplorer|
|ignoreCustomStoryWarning|False|ignores the warning that your story is detected as a custom story|
|ignoreMissingLinesWarning|False|ignores the warning that you might be missing lines which are added on saving|
|language||the two letter short form of the language you have selected|
|recent_index|-1|the index of the line to resume to, should not be edited!|
|recents_0||a path to the most recent file, should not be edited!|
|recents_1||a path to the second most recent file, should not be edited!|
|recents_2||a path to the third most recent file, should not be edited!|
|recents_3||a path to the fourth most recent file, should not be edited!|
|recents_4||a path to the fifth most recent file, should not be edited!|
|showExtendedExplorerInfo|False|toggles the extended info in the StoryExplorer|
|showSearchHighlightComments|False|toggles the highlighting of the search term in the comments|
|showSearchHighlightTranslation|False|toggles the highlighting of the search term in the translation|
|story_path||a file path to the folde rcontaining the original story character files, set when loading the first file ever into the StoryExplorer|
|template_path|C:\\Users\\|points to the folder containing the new template files for the story, in the correct folder structure. not used in normal program behaviour|
|translation_path|C..\\Users\\|path to the last opened file, used as a reference to open the folder dialog in it's parent folder|
|useFalseFolder|False|will add a "new" after the language in the game folder in order to enable development of languages already implemented in the game.|
|useRainbowEdges|False|colors the edges in the StoryExplorer in a rainbow manner|
|useRainbowNodes|False|colors the nodes in the StoryExplorer in a rainbow manner|
|version|1.2.1|the game's story verison currently present in the database|

## Search
Use a '?' at the beginning of your search to perform it in all tabs. 
Use a '!' to enable case sensitive search. You can combine them also 
like this '?!search' for a case sensitive multitab search. 
Use a '\\' in front of the ? or ! to treat it as a literal, so it will not trigger its function.
There are also search modifiers which you can apply at the start of the search term, so after any '?' or '!' modifiers.
They can be combined with the global and case sensitivity flag.
See the table below for a list. Some are exclusive to each other in the sense that they do the exact opposite. 
For a result to count the conditions of all modifiers have to be met.

|modifier|name|rules behind it|
|---|---|---|
|§id|id|query must be in the id of a line|
|§tn|translations|query must be in the translation of a line|
|§en|english/template|query must be in the template/english version of a line|
|§cm|comments|query must be in the comments of a line|
|§tx|text(no comments)|query must be in the template or translation of a line|
|§ap|approved|line must be approved|
|§un|not approved|line must not be approved|
|§td|translated|line must be translated|
|§ut|not translated|line must not be translated|
|§ma|translation matches template| query doesnt matter, translation must match template
|§rg|regex|treats the searched query as regex

### Example
- `§tx§cm§apsock` - approved lines that have sock in their translation or template and in the comments
- `?!§ap§ma` - approved lines in all tabs where the translation matches the template, case sensitive
- `§id§un§rg[0-9a-fA-F]{8}[-]?([0-9a-fA-F]{4}[-]?){3}[0-9a-fA-F]{12}$` - lines not approved and that have only a guid as their id

see [regexr](https://regexr.com) for help with regex.

# Development
If you want to build and or develop this application, 
you need the solution and all its files form this repo, 
as well as the MySql Package for Visual Studio, 
for that you need to install the MySql Development config 
from their website and then add the MySql Package to 
the solution via the NuGet package manager.
You'll probably also need some additional NuGet packages.
