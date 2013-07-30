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
using System.Windows.Navigation;
using System.Windows.Shapes;
using Calculate.Core;

namespace Calculate
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly Calculator _calculator = new Calculator();

        public MainWindow()
        {
            InitializeComponent();
        }

        void Input_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                var r = _calculator.Calculate(Input.Text);
                var display = new TextBlock();
                display.Text = r.Expression + "=" + (r.Exception != null ? "error" : r.Value.ToString());
                History.Items.Add(display);
            }
        }
    }
}
