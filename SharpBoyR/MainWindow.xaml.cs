using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;

namespace SharpBoyR
{
    public partial class MainWindow : Window
    {
        Debug debug;

        public MainWindow()
        {
            InitializeComponent();

            debug = new Debug();

            ROM rom = new ROM("tetris.gb");
            this.Title = rom.Name;
        }
        
        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                if (debug.IsVisible)
                {
                    debug.Hide();
                }
                else
                {
                    debug.Show();
                }
            }
        }
    }
}
