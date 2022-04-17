using System;
using System.Windows.Forms;
using System.Drawing.Printing;

namespace KurnaiNetbookHelper
{
    public partial class PrintGUI : Form
    {
        private MSWord _Word;

        public PrintGUI(string file, MSWord word)
        {
            InitializeComponent();

            _Word = word;
            fileNameLabel.Text = file;
        }

        private void PrintGUI_Load(object sender, EventArgs e)
        {
            this.CenterToParent();

            foreach (string printer in PrinterSettings.InstalledPrinters)
            {
                printerComboBox.Items.Add(printer);
            }
            
            // If no printers installed, skip this GUI.
            if (printerComboBox.Items.Count == 0)
                skipButton.PerformClick();

            PrinterSettings settings = new PrinterSettings();
            printerComboBox.SelectedItem = settings.PrinterName;
        }

        private void printButton_Click(object sender, EventArgs e)
        {
            _Word._PrinterToUse = printerComboBox.SelectedItem.ToString();
        }
    }
}
