// Copyright (c) Tim Kennedy. All Rights Reserved. Licensed under the MIT License.

using Microsoft.Win32;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace CheckLanguageFile
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        #region Check the file
        public void ReadFile(string fileToCheck)
        {
            try
            {
                dataGrid.ItemsSource = null;
                dataGrid.Items.Clear();

                string file = Path.GetFileName(fileToCheck);
                messages.Text = $"Validating: {file}\n";

                ResourceDictionary dict1 = new()
                {
                    Source = new Uri(fileToCheck, UriKind.RelativeOrAbsolute)
                };
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
        }
        #endregion Check the file

        #region Compare dictionaries
        public void CompareLanguageDictionaries()
        {
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
                    comparemessages.Text = $"{ex.Message}\n{ex.InnerException.Message}\n";
                    comparemessages.Text += "\nCheck each file before comparing.";
                }
                else
                {
                    comparemessages.Text = $"{ex.Message}\n";
                    comparemessages.Text += "\nCheck each file before comparing.";
                }
                return;
            }

            string dict1FileName = Path.GetFileName(dict1.Source.ToString());
            string dict2FileName = Path.GetFileName(dict2.Source.ToString());

            List<LanguageStrings> l1Strings = new();
            List<LanguageStrings> l2Strings = new();

            foreach (DictionaryEntry kvp in dict1)
            {
                LanguageStrings ls = new()
                {
                    StringKey = kvp.Key.ToString(),
                    StringValue = kvp.Value.ToString()
                };
                l1Strings.Add(ls);
            }
            foreach (DictionaryEntry kvp in dict2)
            {
                LanguageStrings ls = new()
                {
                    StringKey = kvp.Key.ToString(),
                    StringValue = kvp.Value.ToString()
                };
                l2Strings.Add(ls);
            }

            l1Strings = l1Strings.OrderBy(l => l.StringKey).ToList();
            l2Strings = l2Strings.OrderBy(l => l.StringKey).ToList();

            comparemessages.Text += $"[File 1] {dict1FileName} has {l1Strings.Count} keys\n";
            comparemessages.Text += $"[File 2] {dict2FileName} has {l2Strings.Count} keys\n";

            StringComparer comparer = StringComparer.Ordinal;

            IEnumerable<string> result2 = l2Strings.Select(x => x.StringKey).Except(l1Strings.Select(x => x.StringKey), comparer);
            if (result2.Any())
            {
                foreach (var item in result2)
                {
                    comparemessages.Text += $"[File 1] ({dict1FileName}) is missing key {item} \n";
                }
            }

            IEnumerable<string> result1 = l1Strings.Select(x => x.StringKey).Except(l2Strings.Select(x => x.StringKey), comparer);
            if (result1.Any())
            {
                foreach (var item in result1)
                {
                    comparemessages.Text += $"[File 2] {dict2FileName} is missing key {item} \n";
                }
            }


            IEnumerable<string> result3 = l2Strings.Select(x => x.StringKey).Intersect(l1Strings.Select(x => x.StringKey), comparer);
            if (result3.Any())
            {
                comparemessages.Text += $"{result3.Count()} keys are the same\n";
            }

        }
        #endregion Compare dictionaries

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
                Filter = "Xaml files (*.xaml)|*.xaml"
            };
            if (dlgOpen.ShowDialog() == true)
            {
                tbxFile1.Text = dlgOpen.FileName;
                tbxFile1.Focus();
                tbxFile1.CaretIndex = dlgOpen.FileName.Length;
                tbxFile1.ScrollToEnd();
            }
        }

        private void Button_File2Open_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dlgOpen = new()
            {
                Title = "Browse for File 2",
                CheckFileExists = true,
                CheckPathExists = true,
                Filter = "Xaml files (*.xaml)|*.xaml"
            };
            if (dlgOpen.ShowDialog() == true)
            {
                tbxFile2.Text = dlgOpen.FileName;
                tbxFile2.Focus();
                tbxFile2.CaretIndex = dlgOpen.FileName.Length;
                tbxFile2.ScrollToEnd();
            }
        }

        private void Button_Clear_Click(object sender, RoutedEventArgs e)
        {
            dataGrid.ItemsSource = null;
            dataGrid.Items.Clear();
            textBox.Text = string.Empty;
            messages.Text = string.Empty;
        }

        private void Compare_Button_Click(object sender, RoutedEventArgs e)
        {
            comparemessages.Text = string.Empty;
            CompareLanguageDictionaries();
        }

        private void Compare_Clear_Button_Click(object sender, RoutedEventArgs e)
        {
            comparemessages.Text = string.Empty;
        }
        #endregion Button click events

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

            ICollectionView cv = CollectionViewSource.GetDefaultView(dataGrid.ItemsSource);
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
    }
}
