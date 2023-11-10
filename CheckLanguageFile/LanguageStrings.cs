using CommunityToolkit.Mvvm.ComponentModel;

namespace CheckLanguageFile;

internal partial class LanguageStrings : ObservableObject
{
    [ObservableProperty]
    private string _stringKey;

    [ObservableProperty]
    private string _stringValue;
}
