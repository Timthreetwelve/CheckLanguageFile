// Copyright (c) Tim Kennedy. All Rights Reserved. Licensed under the MIT License.

using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace CheckLanguageFile;

class CellConverter : IMultiValueConverter
{
    public object Convert(object[] value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value[1] is string v1 && value[0] is string v0)
        {
            if (v1?.Length == 0)
            {
                return Brushes.MistyRose;
            }
            return v1 == v0 ? Brushes.LemonChiffon : Brushes.Transparent;

        }
        return Brushes.Transparent;
    }

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
    {
        return (object[])Binding.DoNothing;
    }
}
