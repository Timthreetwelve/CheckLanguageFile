## Check Language File

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
- Two files are compared
- Total number of keys are listed for each file
- Missing keys are listed individually - **keys listed as "missing" in the en-US files may have been removed**
- The total number of matching keys is listed
- The grid at the bottom lists the values for each of the files. Missing values and values that are the same are color coded.

### Nothing fancy:
❌ There's no installer, just a portable version in a zip file.

❌ No help or readme file.

❌ No dark theme or settings of any kind.

❌ English UI only.

❌ No logging.

### Requires .NET 6

### Screenshots
![CheckLanguageFile_2023-11-11_15-53-50](https://github.com/Timthreetwelve/CheckLanguageFile/assets/43152358/c4761e8a-2b70-46de-b1c6-d44a62e8537e)
![CheckLanguageFile_2023-11-16_15-22-06](https://github.com/Timthreetwelve/CheckLanguageFile/assets/43152358/7e48485e-d79f-4ab8-8eac-6c5792e9e537)

