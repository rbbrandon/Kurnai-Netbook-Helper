using System;
using System.Windows.Forms;

namespace KurnaiNetbookHelper
{
    public partial class MultipleAccountsGUI : Form
    {
        private AddJobGUI _Parent;
        private string[,] _List;
        private int _SearchType;

        /// <summary>
        ///     Creates a GUI for selected an account out of many.
        /// </summary>
        /// <param name="parent">The parent GUI (this).</param>
        /// <param name="list">A 2D array containing [friendly name, actual needed data]</param>
        /// <param name="searchType">0 = Name Search, 1 = ID Search, 2 = SNID Search.</param>
        public MultipleAccountsGUI(AddJobGUI parent, string[,] list, int searchType)
        {
            _Parent = parent;
            _List = list;
            _SearchType = searchType;
            InitializeComponent();
        }

        private void MultipleAccountsGUI_Load(object sender, EventArgs e)
        {
            this.CenterToParent();
            for (int row = 0; row < _List.GetLength(0); row++)
            {
                matchComboBox.Items.Add(new ComboBoxItem(_List[row, 0], _List[row, 1]));
            }
            matchComboBox.SelectedIndex = 0;
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            ComboBoxItem selectedItem = matchComboBox.SelectedItem as ComboBoxItem;

            switch (_SearchType)
            {
                case 0:
                    //Change name
                    _Parent.idTextBox.Text = selectedItem.ID;
                    break;
                case 1:
                    //_Parent.idTextBox.Text = selectedItem.ID;
                    _Parent.serialTextBox.Text = selectedItem.ID;
                    break;
            }
            
            Close();
        }
    }
}
