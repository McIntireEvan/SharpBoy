using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;

namespace SharpBoyR
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class Debug : Window
    {
        bool logPaused;

        public Debug()
        {
            InitializeComponent();
            textBox.VerticalScrollBarVisibility = ScrollBarVisibility.Visible;
            log_unpause.IsEnabled = false;
            logPaused = false;
        }

        public void Log(string text)
        {
            if(!logPaused)
                this.textBox.AppendText(text);
        }

        public void LogLine(string text)
        {
            if (!logPaused)
            {
                this.textBox.AppendText(text + "\u2028");
                this.textBox.ScrollToEnd();
            }
        }

        private void log_pause_Click(object sender, RoutedEventArgs e)
        {
            logPaused = true;
            log_unpause.IsEnabled = true;
            log_pause.IsEnabled = false;
        }

        private void log_unpause_Click(object sender, RoutedEventArgs e)
        {
            logPaused = false;
            log_unpause.IsEnabled = false;
            log_pause.IsEnabled = true;
        }

        private void log_clear_Click(object sender, RoutedEventArgs e)
        {
            textBox.Clear();
        }

        private void log_save_Click(object sender, RoutedEventArgs e)
        {
            using(StreamWriter writer = new StreamWriter(DateTime.Now.Ticks + "-sharpboyLog.txt"))
            {
                string text = textBox.Text;
                foreach(String s in text.Split('\u2028'))
                {
                    writer.WriteLine(s);
                }
            }
        }
    }
}
