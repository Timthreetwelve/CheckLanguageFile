﻿// Copyright (c) Tim Kennedy. All Rights Reserved. Licensed under the MIT License.

#region Using statements
using Microsoft.Win32;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
#endregion Using statements

namespace CheckLanguageFile;

public partial class MainWindow : Window
{
    public List<string> MissingValues { get; set; } = new();

    public MainWindow()
    {
        InitializeComponent();

        Title = $"Check Language File - {GitVersionInformation.SemVer}";
    }

    #region Check the file
    public void ReadFile(string fileToCheck)
    {
        ResourceDictionary dict1 = new();
        try
        {
            dataGrid.ItemsSource = null;
            dataGrid.Items.Clear();

            string file = Path.GetFileName(fileToCheck);
            string fullPath = Path.GetFullPath(fileToCheck);
            messages.Text = $"Validating: {fullPath}\n";

            dict1.Source = new Uri(fileToCheck, UriKind.RelativeOrAbsolute);

            messages.Text += $"There are a total of {dict1.Count} keys.\n";

            List<LanguageStrings> lStrings = new();
            int problems = 0;
            foreach (DictionaryEntry item in dict1)
            {
                LanguageStrings ls = new();
                if (string.IsNullOrEmpty(item.Key.ToString()))
                {
                    messages.Text += "\nNull or empty Key found";
                    problems++;
                }
                if (item.Value == null || item.Value.ToString()?.Length == 0)
                {
                    messages.Text += $"\nNull or empty value found on key: {item.Key}";
                    ls.StringKey = item.Key.ToString();
                    ls.StringValue = string.Empty;
                    lStrings.Add(ls);
                    problems++;
                }
                else
                {
                    ls.StringKey = item.Key.ToString();
                    ls.StringValue = item.Value.ToString();
                    lStrings.Add(ls);
                }
            }
            dataGrid.ItemsSource = lStrings.OrderBy(x => x.StringKey);
            if (problems == 0)
            {
                messages.Text += "\nNo problems were found.";
            }
        }
        catch (Exception ex)
        {
            if (ex.InnerException != null && ex.InnerException.Message != ex.Message)
            {
                messages.Text += $"{ex.Message}\n{ex.InnerException.Message}";
            }
            else
            {
                messages.Text += $"{ex.Message}\n";
            }
        }
        finally
        {
            dict1.Clear();
        }
    }
    #endregion Check the file

    #region Compare dictionaries
    public void CompareLanguageDictionaries()
    {
        btnExport.IsEnabled = false;

        if (!File.Exists(tbxFile1.Text))
        {
            _ = MessageBox.Show("File 1 not found",
                "ERROR",
                MessageBoxButton.OK,
                MessageBoxImage.Error);
            return;
        }

        if (!File.Exists(tbxFile2.Text))
        {
            _ = MessageBox.Show("File 2 not found",
                "ERROR",
                MessageBoxButton.OK,
                MessageBoxImage.Error);
            return;
        }

        ResourceDictionary dict1 = new();
        ResourceDictionary dict2 = new();

        try
        {
            dict1.Source = new Uri(tbxFile1.Text, UriKind.RelativeOrAbsolute);
            dict2.Source = new Uri(tbxFile2.Text, UriKind.RelativeOrAbsolute);
        }
        catch (Exception ex)
        {
            if (ex.InnerException != null)
            {
                _ = MessageBox.Show("{ex.Message}\n{ex.InnerException.Message}",
                    "Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
            else
            {
                _ = MessageBox.Show("{ex.Message}",
                    "Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
            return;
        }

        string dict1FileName = Path.GetFileName(dict1.Source.ToString());
        string dict2FileName = Path.GetFileName(dict2.Source.ToString());

        CompareGrid.Columns[1].Header = dict1FileName;
        CompareGrid.Columns[2].Header = dict2FileName;

        List<LanguageStrings> l1Strings = new();
        List<LanguageStrings> l2Strings = new();

        foreach (DictionaryEntry kvp in dict1)
        {
            LanguageStrings ls = new()
            {
                StringKey = kvp.Key.ToString(),
                StringValue = kvp.Value.ToString(),
            };
            l1Strings.Add(ls);
        }
        foreach (DictionaryEntry kvp in dict2)
        {
            LanguageStrings ls = new()
            {
                StringKey = kvp.Key.ToString(),
                StringValue = kvp.Value.ToString(),
            };
            l2Strings.Add(ls);
        }

        l1Strings = l1Strings.OrderBy(l => l.StringKey).ToList();
        l2Strings = l2Strings.OrderBy(l => l.StringKey).ToList();
        List<LanguageStrings> combined = new();

        txtKeys1.Text = $"{l1Strings.Count} keys";
        txtKeys2.Text = $"{l2Strings.Count} keys";

        StringComparer comparer = StringComparer.Ordinal;

        IEnumerable<string> result2 = l2Strings.Select(x => x.StringKey).Except(l1Strings.Select(x => x.StringKey), comparer);
        if (result2.Any())
        {
            foreach (var item in result2)
            {
                Debug.WriteLine($"{dict1FileName} does not have key {item}");
            }
        }

        IEnumerable<string> result1 = l1Strings.Select(x => x.StringKey).Except(l2Strings.Select(x => x.StringKey), comparer);
        if (result1.Any())
        {
            foreach (var item in result1)
            {
                Debug.WriteLine($"{dict2FileName} does not have key {item}");
                MissingValues.Add(item);
            }
            btnExport.IsEnabled = true;
        }

        IEnumerable<string> result3 = l2Strings.Select(x => x.StringKey).Intersect(l1Strings.Select(x => x.StringKey), comparer);
        if (result3.Any())
        {
            Debug.WriteLine($"{result3.Count()} keys are the same");
        }

        foreach (var item in l1Strings)
        {
            LanguageStrings ls = new()
            {
                StringKey = item.StringKey,
                StringValue = item.StringValue,
                CompareValue = GetDictValue(l2Strings, item.StringKey)
            };
            combined.Add(ls);
        }
        CompareGrid.ItemsSource = combined;
    }
    #endregion Compare dictionaries

    #region Get a value from a dictionary
    private static string GetDictValue(List<LanguageStrings> list, string key)
    {
        string x = list.Find(x => x.StringKey == key)?.StringValue;
        if (string.IsNullOrEmpty(x))
        {
            return string.Empty;
        }
        return list.Find(x => x.StringKey == key).StringValue;
    }
    #endregion Get a value from a dictionary

    #region Button click events
    private void Button_Check_Click(object sender, RoutedEventArgs e)
    {
        messages.Text = string.Empty;

        if (File.Exists(textBox.Text.Replace("\"", "")))
        {
            string fileName = Path.GetFullPath(textBox.Text.Replace("\"", ""));
            ReadFile(fileName);
        }
        else
        {
            _ = MessageBox.Show("File not found",
                                "ERROR",
                                MessageBoxButton.OK,
                                MessageBoxImage.Error);
        }
    }

    private void Button_FileOpen_Click(object sender, RoutedEventArgs e)
    {
        OpenFileDialog dlgOpen = new()
        {
            Title = "Enter File Name",
            CheckFileExists = true,
            CheckPathExists = true,
            Filter = "Xaml files (*.xaml)|*.xaml"
        };
        if (dlgOpen.ShowDialog() == true)
        {
            ReadFile(dlgOpen.FileName);
            textBox.Text = dlgOpen.FileName;
            textBox.Focus();
            textBox.CaretIndex = dlgOpen.FileName.Length;
            textBox.ScrollToEnd();
        }
    }

    private void Button_File1Open_Click(object sender, RoutedEventArgs e)
    {
        OpenFileDialog dlgOpen = new()
        {
            Title = "Browse for File 1",
            CheckFileExists = true,
            CheckPathExists = true,
            Multiselect = true,
            Filter = "Xaml files (*.xaml)|*.xaml"
        };
        if (dlgOpen.ShowDialog() == true)
        {
            if (dlgOpen.FileNames.Length == 1)
            {
                tbxFile1.Text = dlgOpen.FileName;
                tbxFile1.Focus();
                tbxFile1.CaretIndex = dlgOpen.FileName.Length;
                tbxFile1.ScrollToEnd();
            }
            else if (dlgOpen.FileNames.Length == 2)
            {
                tbxFile1.Text = dlgOpen.FileNames[0];
                tbxFile1.Focus();
                tbxFile1.CaretIndex = dlgOpen.FileNames[0].Length;
                tbxFile1.ScrollToEnd();

                tbxFile2.Text = dlgOpen.FileNames[1];
                tbxFile2.Focus();
                tbxFile2.CaretIndex = dlgOpen.FileNames[1].Length;
                tbxFile2.ScrollToEnd();

                CompareLanguageDictionaries();
            }

        }
    }

    private void Button_File2Open_Click(object sender, RoutedEventArgs e)
    {
        OpenFileDialog dlgOpen = new()
        {
            Title = "Browse for File 2",
            CheckFileExists = true,
            CheckPathExists = true,
            Multiselect = true,
            Filter = "Xaml files (*.xaml)|*.xaml"
        };
        if (dlgOpen.ShowDialog() == true)
        {
            if (dlgOpen.FileNames.Length == 1)
            {
                tbxFile2.Text = dlgOpen.FileName;
                tbxFile2.Focus();
                tbxFile2.CaretIndex = dlgOpen.FileName.Length;
                tbxFile2.ScrollToEnd();
            }
            else if (dlgOpen.FileNames.Length == 2)
            {
                tbxFile1.Text = dlgOpen.FileNames[0];
                tbxFile1.Focus();
                tbxFile1.CaretIndex = dlgOpen.FileNames[0].Length;
                tbxFile1.ScrollToEnd();

                tbxFile2.Text = dlgOpen.FileNames[1];
                tbxFile2.Focus();
                tbxFile2.CaretIndex = dlgOpen.FileNames[1].Length;
                tbxFile2.ScrollToEnd();

                CompareLanguageDictionaries();
            }

        }
    }

    private void Button_Clear_Click(object sender, RoutedEventArgs e)
    {
        dataGrid.ItemsSource = null;
        dataGrid.Items.Clear();
        messages.Text = string.Empty;
    }

    private void Compare_Button_Click(object sender, RoutedEventArgs e)
    {
        CompareLanguageDictionaries();
        ShowAllRadio.IsChecked = true;
    }

    private void Compare_Clear_Button_Click(object sender, RoutedEventArgs e)
    {
        CompareGrid.ItemsSource = null;
        CompareGrid.Items.Clear();
        txtKeys1.Text = string.Empty;
        txtKeys2.Text = string.Empty;
    }

    private void Button_Swap_Click(object sender, RoutedEventArgs e)
    {
        SwapFiles();
        RemoveFilter();
    }

    private void Export_Button_Click(object sender, RoutedEventArgs e)
    {
        ExportMissing();
    }
    #endregion Button click events

    #region Radio buttons
    private void Radio_NoValue_Checked(object sender, RoutedEventArgs e)
    {
        FilterNoValue();
    }

    private void Radio_SameValue_Checked(object sender, RoutedEventArgs e)
    {
        FilterSameValue();
    }

    private void Radio_ShowAll(object sender, RoutedEventArgs e)
    {
        RemoveFilter();
    }
    #endregion Radio buttons

    #region Filter changed
    private void TbxFilter_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
    {
        FilterTheGrid();
    }
    #endregion Filter changed

    #region Drag and drop events
    private void TextBox_PreviewDrop(object sender, DragEventArgs e)
    {
        if (e.Data?.GetDataPresent(DataFormats.FileDrop) == true)
        {
            textBox.Text = ((DataObject)e.Data).GetFileDropList().Cast<string>().ToList().FirstOrDefault();
            if (textBox.Text is not null)
            {
                ReadFile(textBox.Text);
            }
        }
    }

    private void TextBoxFile1_PreviewDrop(object sender, DragEventArgs e)
    {
        if (e.Data?.GetDataPresent(DataFormats.FileDrop) == true)
        {
            tbxFile1.Text = ((DataObject)e.Data).GetFileDropList().Cast<string>().ToList().FirstOrDefault();
        }
    }

    private void TextBoxFile2_PreviewDrop(object sender, DragEventArgs e)
    {
        if (e.Data?.GetDataPresent(DataFormats.FileDrop) == true)
        {
            tbxFile2.Text = ((DataObject)e.Data).GetFileDropList().Cast<string>().ToList().FirstOrDefault();
        }
    }

    private void TextBox_Drop(object sender, DragEventArgs e)
    {
        TextBox box = sender as TextBox;
        box.Focus();
        box.CaretIndex = box.Text.Length;
        box.ScrollToEnd();
    }

    private void TextBox_PreviewDragOver(object sender, DragEventArgs e)
    {
        if (e.Data.GetDataPresent(DataFormats.FileDrop))
        {
            FileInfo fileInfo = new(((DataObject)e.Data).GetFileDropList().Cast<string>().ToList().FirstOrDefault());
            e.Effects = (((DataObject)e.Data).GetFileDropList()
                .Cast<string>()
                .ToList()?.
                Count == 1 && fileInfo.Extension == ".xaml")
                ? DragDropEffects.Copy
                : DragDropEffects.None;
        }
        else
        {
            e.Effects = DragDropEffects.None;
        }
        e.Handled = true;
    }
    #endregion Drag and drop events

    #region Filter the datagrid
    public void FilterTheGrid(bool exit = false)
    {
        string filter = tbxFilter.Text;

        ICollectionView cv = CollectionViewSource.GetDefaultView(CompareGrid.ItemsSource);
        if (filter?.Length == 0)
        {
            cv.Filter = null;
            if (exit)
            {
                return;
            }
        }
        else
        {
            cv.Filter = o =>
            {
                LanguageStrings ls = o as LanguageStrings;
                return ls.StringKey.Contains(filter, StringComparison.OrdinalIgnoreCase) ||
                       ls.StringKey.Contains(filter, StringComparison.OrdinalIgnoreCase);
            };
        }
    }
    #endregion Filter the datagrid

    #region Filter CompareGrid
    private void FilterNoValue()
    {
        if (CompareGrid.ItemsSource != null)
        {
            ICollectionView cv = CollectionViewSource.GetDefaultView(CompareGrid.ItemsSource);
            cv.Filter = o =>
            {
                LanguageStrings ls = o as LanguageStrings;
                if (ls.StringValue?.Length == 0)
                {
                    return true;
                }
                else if (ls.CompareValue?.Length == 0)
                {
                    return true;
                }
                return false;
            };
        }
    }

    private void FilterSameValue()
    {
        if (CompareGrid.ItemsSource != null)
        {

            ICollectionView cv = CollectionViewSource.GetDefaultView(CompareGrid.ItemsSource);
            cv.Filter = o =>
            {
                LanguageStrings ls = o as LanguageStrings;
                return ls.CompareValue == ls.StringValue;
            };
        }
    }

    private void RemoveFilter()
    {
        if (CompareGrid.ItemsSource != null)
        {
            ICollectionView cv = CollectionViewSource.GetDefaultView(CompareGrid.ItemsSource);
            cv.Filter = null;
            ShowAllRadio.IsChecked = true;
        }
    }
    #endregion Filter CompareGrid

    #region Swap compare files
    public void SwapFiles()
    {
        (tbxFile2.Text, tbxFile1.Text) = (tbxFile1.Text, tbxFile2.Text);
        CompareLanguageDictionaries();
    }
    #endregion Swap compare files

    #region Escape Key
    private void TabItem_KeyUp(object sender, System.Windows.Input.KeyEventArgs e)
    {
        if (e.Key == System.Windows.Input.Key.Escape)
        {
            CompareGrid.SelectedItem = null;
        }
    }
    #endregion Escape Key

    #region Format and export keys with missing values
    public void ExportMissing()
    {
        if (MissingValues.Count > 0)
        {
            StringBuilder sb = new();
            foreach (var item in MissingValues)
            {
                _ = sb.Append("<sys:String x:Key=\"").Append(item).AppendLine("\"></sys:String>");
            }
            string missing = sb.ToString();

            SaveFileDialog save = new()
            {
                Title = "Save",
                Filter = "Txt File|*.txt",
                FileName = "ExportedStrings.txt",
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
            };
            bool? result = save.ShowDialog();
            if (result == true)
            {
                File.WriteAllText(save.FileName, missing);
            }

        }
    }
    #endregion Format and export keys with missing values
}
