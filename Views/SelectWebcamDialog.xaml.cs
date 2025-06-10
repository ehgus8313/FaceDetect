using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace APR_TEST.Views
{
    /// <summary>
    /// SelectWebcamDialog.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class SelectWebcamDialog : Window
    {
        public string? SelectedDeviceName { get; private set; }

        public SelectWebcamDialog(List<string> deviceNames)
        {
            InitializeComponent();
            WebcamComboBox.ItemsSource = deviceNames;
            if (deviceNames.Count > 0)
                WebcamComboBox.SelectedIndex = 0;
        }

        private void OnSelectClicked(object sender, RoutedEventArgs e)
        {
            SelectedDeviceName = WebcamComboBox.SelectedItem?.ToString();
            DialogResult = true;
            Close();
        }
    }
}
