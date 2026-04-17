using Microsoft.Win32;
using System;
using System.Globalization;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using TestTask.Services;

namespace TestTask
{
    public partial class MainWindow : Window
    {
        private readonly string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
        private readonly string employeesPath;
        private readonly string xsltPath;

        private string currentDataPath;
        private readonly IXmlProcessingService _xmlService;

        public MainWindow()
        {
            InitializeComponent();

            _xmlService = new XmlProcessingService();

            employeesPath = Path.Combine(baseDirectory, "Employees.xml");
            xsltPath = Path.Combine(baseDirectory, "Transform.xslt");

            Directory.CreateDirectory(baseDirectory);

            string defaultData1 = Path.Combine(baseDirectory, "Data1.xml");
            if (File.Exists(defaultData1))
            {
                currentDataPath = defaultData1;
                UpdateCurrentFileLabel();
                TryProcessTransformation();
            }
            else
            {
                UpdateCurrentFileLabel();
            }
        }

        private void OnLoadFileClick(object sender, RoutedEventArgs e)
        {
            try
            {
                OpenFileDialog dialog = new OpenFileDialog
                {
                    Title = "Select XML file",
                    Filter = "XML files (*.xml)|*.xml|All files (*.*)|*.*",
                    Multiselect = false,
                    CheckFileExists = true,
                    InitialDirectory = "C:\\"
                };

                if (dialog.ShowDialog() == true)
                {
                    currentDataPath = dialog.FileName;
                    UpdateCurrentFileLabel();

                    if (TryProcessTransformation())
                    {
                        CustomMessageBox.Show("XML-файл успешно загружен и обработан!", "Готово", MessageBoxImage.Information);
                    }
                }
            }
            catch (Exception ex)
            {
                CustomMessageBox.Show($"Ошибка при загрузке файла: {ex.Message}", "Ошибка", MessageBoxImage.Error);
            }
        }

        private bool TryProcessTransformation()
        {
            try
            {
                _xmlService.ProcessTransformation(currentDataPath, xsltPath, employeesPath);
                RefreshUI();
                return true;
            }
            catch (Exception ex)
            {
                CustomMessageBox.Show($"Ошибка при обработке данных: {ex.Message}", "Внимание", MessageBoxImage.Warning);
                return false;
            }
        }

        private void RefreshUI()
        {
            if (string.IsNullOrWhiteSpace(currentDataPath)) return;

            EmployeesGrid.ItemsSource = _xmlService.GetEmployeeSummaries(employeesPath);
            MonthsGrid.ItemsSource = _xmlService.GetMonthlySummaries(currentDataPath);
        }

        private void OnAddClick(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(currentDataPath) || !File.Exists(currentDataPath))
            {
                CustomMessageBox.Show("Сначала загрузите Data1.xml или Data2.xml.", "Внимание", MessageBoxImage.Warning);
                return;
            }

            string name = InputName.Text.Trim();
            string surname = InputSurname.Text.Trim();
            string amountStr = InputAmount.Text.Replace(',', '.').Trim();
            string month = (InputMonth.SelectedItem as ComboBoxItem)?.Tag?.ToString() ?? (InputMonth.SelectedItem as ComboBoxItem)?.Content?.ToString();

            if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(surname) ||
                string.IsNullOrEmpty(amountStr) || string.IsNullOrEmpty(month))
            {
                CustomMessageBox.Show("Пожалуйста, заполните все поля.", "Ошибка", MessageBoxImage.Warning);
                return;
            }

            if (!double.TryParse(amountStr, NumberStyles.Any, CultureInfo.InvariantCulture, out _))
            {
                CustomMessageBox.Show("Недопустимый формат суммы.", "Ошибка", MessageBoxImage.Warning);
                return;
            }

            try
            {
                _xmlService.AddNewPayment(currentDataPath, name, surname, amountStr, month);

                if (TryProcessTransformation())
                {
                    InputName.Clear();
                    InputSurname.Clear();
                    InputAmount.Clear();
                    InputMonth.SelectedIndex = -1;

                    CustomMessageBox.Show("Новый платеж успешно добавлен!", "Готово", MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                CustomMessageBox.Show($"Ошибка при добавлении элемента: {ex.Message}", "Ошибка", MessageBoxImage.Error);
            }
        }

        private void OnSaveFileClick(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!File.Exists(employeesPath))
                {
                    CustomMessageBox.Show("Сначала выполните обработку и сформируйте файл Employees.xml.", "Нет данных", MessageBoxImage.Warning);
                    return;
                }

                SaveFileDialog saveDialog = new SaveFileDialog
                {
                    Title = "Сохранить сформированный XML-файл",
                    Filter = "XML файл (*.xml)|*.xml|Все файлы (*.*)|*.*",
                    DefaultExt = ".xml",
                    AddExtension = true,
                    FileName = "Employees.xml",
                    OverwritePrompt = true
                };

                if (saveDialog.ShowDialog() == true)
                {
                    File.Copy(employeesPath, saveDialog.FileName, true);
                    CustomMessageBox.Show("Файл успешно сохранен.", "Готово", MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                CustomMessageBox.Show($"Ошибка при сохранении файла: {ex.Message}", "Ошибка", MessageBoxImage.Error);
            }
        }

        private void UpdateCurrentFileLabel()
        {
            CurrentFileText.Text = string.IsNullOrWhiteSpace(currentDataPath)
                ? "Файл не выбран"
                : Path.GetFileName(currentDataPath);
        }
    }
}