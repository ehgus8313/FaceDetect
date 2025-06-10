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
        public int SelectedIndex { get; private set; } = -1;

        public SelectWebcamDialog(List<string> devices)
        {
            InitializeComponent();
            WebcamComboBox.ItemsSource = devices;
            if (devices.Count > 0)
                WebcamComboBox.SelectedIndex = 0;
        }

        private void OnSelectClicked(object sender, RoutedEventArgs e)
        {
            SelectedIndex = WebcamComboBox.SelectedIndex;
            DialogResult = true;
            Close();
        }
    }
}
