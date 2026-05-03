<h1 align="center">
 Check Language File
</h1>

<div align="center">
  
[![GitHub](https://img.shields.io/github/license/Timthreetwelve/CheckLanguageFile?style=plastic&color=seagreen)](https://github.com/Timthreetwelve/CheckLanguageFile/blob/main/LICENSE)
[![NET6win](https://img.shields.io/badge/.NET-10.0--Windows-blueviolet?style=plastic)](https://dotnet.microsoft.com/en-us/download) 
[![GitHub release (latest by date)](https://img.shields.io/github/v/release/Timthreetwelve/CheckLanguageFile?style=plastic)](https://github.com/Timthreetwelve/CheckLanguageFile/releases/latest) 
[![GitHub Release Date](https://img.shields.io/github/release-date/timthreetwelve/CheckLanguageFile?style=plastic&color=orange)](https://github.com/Timthreetwelve/CheckLanguageFile/releases/latest) 
[![GitHub commits since latest release (by date)](https://img.shields.io/github/commits-since/timthreetwelve/CheckLanguageFile/latest?style=plastic)](https://github.com/Timthreetwelve/CheckLanguageFile/commits/main)
[![GitHub last commit](https://img.shields.io/github/last-commit/timthreetwelve/CheckLanguageFile?style=plastic)](https://github.com/Timthreetwelve/CheckLanguageFile/commits/main)
[![GitHub commits](https://img.shields.io/github/commit-activity/m/timthreetwelve/CheckLanguageFile?style=plastic)](https://github.com/Timthreetwelve/CheckLanguageFile/commits/main)
[![GitHub Stars](https://img.shields.io/github/stars/timthreetwelve/CheckLanguageFile?style=plastic&color=goldenrod&logo=github)](https://docs.github.com/en/get-started/exploring-projects-on-github/saving-repositories-with-stars)
[![GitHub all releases](https://img.shields.io/github/downloads/Timthreetwelve/CheckLanguageFile/total?style=plastic&label=total%20downloads&color=teal)](https://github.com/Timthreetwelve/CheckLanguageFile/releases) 
[![GitHub release (by tag)](https://img.shields.io/github/downloads/timthreetwelve/CheckLanguageFile/latest/total?style=plastic&color=2196F3&label=downloads%20latest%20version)](https://github.com/Timthreetwelve/CheckLanguageFile/releases/latest)
[![GitHub Issues](https://img.shields.io/github/issues/timthreetwelve/CheckLanguageFile?style=plastic&color=orangered)](https://github.com/Timthreetwelve/CheckLanguageFile/issues)
[![GitHub Issues](https://img.shields.io/github/issues-closed/timthreetwelve/CheckLanguageFile?style=plastic&color=slateblue)](https://github.com/Timthreetwelve/CheckLanguageFile/issues)

</div>

This is a utility program intended to be used to validate the resource dictionary based language files used in the **Timthreetwelve** applications. The language files may also be compared to other language files to check for missing or deleted keys.

Files to be checked are expected to be xaml resource dictionaries with a `.xaml` file extension.

Files to be checked can be selected:
- By using the file open dialog (three dot button next to the boxes)
- By drag & drop into the appropriate textbox
- By copying and pasting

### In the File Validation tab, files are checked for:
- Syntax errors
- Duplicate keys (technically only the first duplicate key, since a duplicate key will throw an exception)
- Null or empty keys
- Null or empty values
  
Additionally, all keys and the associated string values are listed in a grid. The grid can be filtered by key or string.

### In the Compare Files tab:
- Two files are compared.
- Total number of keys are listed for each file.
- The grid at the bottom lists the values for each of the files. Missing values and values that are the same are color coded.
- The grid can be filtered to show only keys with no values or keys where the value is the same for both files.
- Both files can be added at the same time when using the file open dialog. Simply `Ctrl+Click` on two files.
- File names are displayed in the grid column headers.
- Export the keys with missing values. Lines are formatted for the xaml file. Just add the translated strings between the `><` and paste into the appropriate place in the language file.

### Nothing fancy:

❌ No help or readme file.

❌ No dark theme or settings of any kind.

❌ English UI only.

❌ No logging.

### ⚠️ Requires .NET 10 ⚠️

### Screenshots
![CheckLanguageFile_2023-11-11_15-53-50](https://github.com/Timthreetwelve/CheckLanguageFile/assets/43152358/c4761e8a-2b70-46de-b1c6-d44a62e8537e)

![CheckLanguageFile_2023-12-12_21-24-27](https://github.com/Timthreetwelve/CheckLanguageFile/assets/43152358/0a26d0ef-714e-4a5d-9109-bae6d49c485f)
