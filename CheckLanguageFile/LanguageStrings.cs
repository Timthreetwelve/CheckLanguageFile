// Copyright (c) Tim Kennedy. All Rights Reserved. Licensed under the MIT License.

using CommunityToolkit.Mvvm.ComponentModel;

namespace CheckLanguageFile;

internal partial class LanguageStrings : ObservableObject
{
    [ObservableProperty]
    public partial string? StringKey { get; set; }

    [ObservableProperty]
    public partial string? StringValue { get; set; }
    [ObservableProperty]
    public partial string? CompareValue { get; set; }
}
