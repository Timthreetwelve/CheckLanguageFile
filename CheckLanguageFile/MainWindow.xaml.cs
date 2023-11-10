using Microsoft.Win32;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows;
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
                ResourceDictionary dict1 = new()
                {
                    Source = new Uri(fileToCheck, UriKind.RelativeOrAbsolute)
                };
                messages.Text = $"Checking: {file}\n";
                messages.Text += $"The input file has no duplicate keys.\nThere are a total of {dict1.Count} keys.";

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
                    messages.Text += "\nNo problems found.";
                }
            }
            catch (Exception ex)
            {
                if (ex.InnerException != null)
                {
                    messages.Text = $"{ex.Message}\n{ex.InnerException.Message}";
                }
                else
                {
                    messages.Text = $"{ex.Message}\n";
                }
            }
        }
        #endregion Check the file

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
            }
        }

        private void Button_Clear_Click(object sender, RoutedEventArgs e)
        {
            dataGrid.ItemsSource = null;
            dataGrid.Items.Clear();
            textBox.Text = string.Empty;
            messages.Text = string.Empty;
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
