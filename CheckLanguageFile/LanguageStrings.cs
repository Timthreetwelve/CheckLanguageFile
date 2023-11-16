// Copyright (c) Tim Kennedy. All Rights Reserved. Licensed under the MIT License.

using CommunityToolkit.Mvvm.ComponentModel;

namespace CheckLanguageFile;

internal partial class LanguageStrings : ObservableObject
{
    [ObservableProperty]
    private string _stringKey;

    [ObservableProperty]
    private string _stringValue;

    [ObservableProperty]
    private string _compareValue;
}
