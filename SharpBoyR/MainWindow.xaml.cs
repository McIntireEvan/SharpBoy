using System;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;

namespace SharpBoyR
{
    public partial class MainWindow : Window
    {
        GameBoy gameboy;
        //TODO: Move?

        public MainWindow()
        {
            InitializeComponent();
            //debug.Show();

            ROM rom = new ROM("tetris.gb");
            this.Title = rom.Name;

            gameboy = new GameBoy(this, rom);
            new Thread(() =>
            {
                while (true)
                {
                    gameboy.Update();
                }
            }).Start();
        }
        
        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                if (gameboy.debug.IsVisible)
                {
                    gameboy.debug.Hide();
                    gameboy.debug.logPaused = true;
                }
                else
                {
                    gameboy.debug.Show();
                    gameboy.debug.logPaused = false;
                }
            }
        }
    }
}
