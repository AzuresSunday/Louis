using Panuon.UI.Silver;
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
using GalaSoft.MvvmLight.Messaging;

namespace Louis
{
    /// <summary>
    /// MainWindow1.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : WindowX
    {
        private bool Requesting = false;

        public MainWindow()
        {
            InitializeComponent();
            Messenger.Default.Register<bool>(this, "FocusOnSearch", f =>
            {
                SearchTextbox.SelectionStart = SearchTextbox.Text.Length;
                SearchTextbox.Focus();
            });

            Messenger.Default.Register<bool>(this, "Requesting", f =>
            {
                Requesting = f;
            });
        }

        private void ScrollViewer_OnScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            var offsetValue = (e.OriginalSource as ScrollViewer).VerticalOffset + 356 * 2;
            var scrollableHeight = (e.OriginalSource as ScrollViewer).ScrollableHeight;

            if (offsetValue >= scrollableHeight)
            {
                Messenger.Default.Send(true, "RequestData");
            }
        }
    }
}