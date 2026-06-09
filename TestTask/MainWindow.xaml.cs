using Microsoft.Win32;
using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using TestTask.Configuration;
using TestTask.Models;
using TestTask.Services;

namespace TestTask
{
    public partial class MainWindow : Window
    {
        private readonly AppPaths _paths;
        private readonly IXmlProcessingService _xmlService;
        private readonly IPaymentValidator _paymentValidator;

        private string? _currentDataPath;

        public MainWindow()
        {
            InitializeComponent();

            _paths = App.Services.Paths;
            _xmlService = App.Services.XmlService;
            _paymentValidator = App.Services.PaymentValidator;

            Directory.CreateDirectory(_paths.BaseDirectory);

            if (File.Exists(_paths.DefaultDataFile))
            {
                _currentDataPath = _paths.DefaultDataFile;
                TryProcessTransformation();
            }

            UpdateCurrentFileLabel();
        }

        private void OnLoadFileClick(object sender, RoutedEventArgs e)
        {
            try
            {
                OpenFileDialog dialog = new()
                {
                    Title = "Select XML file",
                    Filter = "XML files (*.xml)|*.xml|All files (*.*)|*.*",
                    Multiselect = false,
                    CheckFileExists = true,
                    InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)
                };

                if (dialog.ShowDialog() != true)
                    return;

                _currentDataPath = dialog.FileName;
                UpdateCurrentFileLabel();

                if (TryProcessTransformation())
                {
                    CustomMessageBox.Show("XML-файл успешно загружен и обработан!", "Готово", MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                CustomMessageBox.Show($"Ошибка при загрузке файла: {ex.Message}", "Ошибка", MessageBoxImage.Error);
            }
        }

        private bool TryProcessTransformation()
        {
            if (string.IsNullOrWhiteSpace(_currentDataPath))
                return false;

            try
            {
                _xmlService.ProcessTransformation(_currentDataPath, _paths.XsltFile, _paths.EmployeesFile);
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
            if (string.IsNullOrWhiteSpace(_currentDataPath))
                return;

            EmployeesGrid.ItemsSource = _xmlService.GetEmployeeSummaries(_paths.EmployeesFile);
            MonthsGrid.ItemsSource = _xmlService.GetMonthlySummaries(_currentDataPath);
        }

        private void OnAddClick(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(_currentDataPath) || !File.Exists(_currentDataPath))
            {
                CustomMessageBox.Show("Сначала загрузите XML-файл с данными.", "Внимание", MessageBoxImage.Warning);
                return;
            }

            PaymentInput payment = new()
            {
                Name = InputName.Text.Trim(),
                Surname = InputSurname.Text.Trim(),
                Amount = InputAmount.Text.Replace(',', '.').Trim(),
                Month = GetSelectedMonth()
            };

            ValidationResult validation = _paymentValidator.Validate(payment);
            if (!validation.IsValid)
            {
                CustomMessageBox.Show(validation.ErrorMessage!, "Ошибка", MessageBoxImage.Warning);
                return;
            }

            try
            {
                _xmlService.AddNewPayment(_currentDataPath, payment);

                if (TryProcessTransformation())
                {
                    ClearPaymentForm();
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
                if (!File.Exists(_paths.EmployeesFile))
                {
                    CustomMessageBox.Show("Сначала выполните обработку и сформируйте файл Employees.xml.", "Нет данных", MessageBoxImage.Warning);
                    return;
                }

                SaveFileDialog saveDialog = new()
                {
                    Title = "Сохранить сформированный XML-файл",
                    Filter = "XML файл (*.xml)|*.xml|Все файлы (*.*)|*.*",
                    DefaultExt = ".xml",
                    AddExtension = true,
                    FileName = "Employees.xml",
                    OverwritePrompt = true
                };

                if (saveDialog.ShowDialog() != true)
                    return;

                File.Copy(_paths.EmployeesFile, saveDialog.FileName, true);
                CustomMessageBox.Show("Файл успешно сохранен.", "Готово", MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                CustomMessageBox.Show($"Ошибка при сохранении файла: {ex.Message}", "Ошибка", MessageBoxImage.Error);
            }
        }

        private string GetSelectedMonth()
        {
            if (InputMonth.SelectedItem is ComboBoxItem item)
                return item.Tag?.ToString() ?? item.Content?.ToString() ?? string.Empty;

            return string.Empty;
        }

        private void ClearPaymentForm()
        {
            InputName.Clear();
            InputSurname.Clear();
            InputAmount.Clear();
            InputMonth.SelectedIndex = -1;
        }

        private void UpdateCurrentFileLabel()
        {
            CurrentFileText.Text = string.IsNullOrWhiteSpace(_currentDataPath)
                ? "Файл не выбран"
                : Path.GetFileName(_currentDataPath);
        }
    }
}
