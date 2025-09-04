using System;
using System.IO;
using System.Windows.Forms;
using Project1_stock;

namespace Project1_stock
{
    internal static class Program
    {
        // Keep state and default path at class scope
        private static int _enterCount = 0;
        private static readonly string DefaultFile = Path.Combine(
            AppDomain.CurrentDomain.BaseDirectory,
            "ABBV-Day.csv"
        );

        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // Create form and prepare to catch keys
            var form = new Form_Candlesticks
            {
                KeyPreview = true,   // let form see keys before controls
                AcceptButton = null  // disable any automatic Enter-click
            };

            form.Shown += (s, e) => form.ActiveControl = null; // no initial focus

            // Set default date range for February 2021
            form.datetimepicker_Start_Date.Value = new DateTime(2021, 2, 1);
            form.datetimepicker_End_Date.Value = new DateTime(2021, 2, 28);

            // Load ABBV initially
            form.LoadStock(DefaultFile);

            // Wire up the KeyDown handler
            form.KeyDown += Form_KeyDown;

            Application.Run(form);
        }

        private static void Form_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                // Always suppress Enter key to prevent button clicks
                e.Handled = true;
                e.SuppressKeyPress = true;

                _enterCount++;
                if (_enterCount >= 2)
                {
                    // On second Enter press, reload ABBV
                    if (sender is Form_Candlesticks form)
                    {
                        form.LoadStock(DefaultFile);
                    }
                    _enterCount = 0;
                }
            }
            else
            {
                // Reset counter if any other key is pressed
                _enterCount = 0;
            }
        }
    }
}