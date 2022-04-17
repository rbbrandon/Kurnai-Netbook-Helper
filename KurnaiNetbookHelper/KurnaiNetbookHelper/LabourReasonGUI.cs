using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace KurnaiNetbookHelper
{
    public partial class LabourReasonGUI : Form
    {
        private AddJobGUI _Parent;
        private Validate _Validate = new Validate();

        public LabourReasonGUI(AddJobGUI parent)
        {
            InitializeComponent();
            _Parent = parent;
        }

        private void LabourReasonGUI_Load(object sender, EventArgs e)
        {
            this.CenterToParent();
            labourTextBox.Text = _Parent._LabourReason;
            this.ActiveControl = labourTextBox;
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            _Parent._LabourReason = labourTextBox.Text;
            this.Close();
        }

        private void labourTextBox_TextChanged(object sender, EventArgs e)
        {
            if (_Validate.IsPhrase(labourTextBox.Text))
            {
                okButton.Enabled = true;
                toolTip.SetToolTip(labourTextBox, "");
                labourTextBox.ForeColor = Color.DarkGreen;
            }
            else
            {
                okButton.Enabled = false;
                toolTip.SetToolTip(labourTextBox, "Labour Reason may only consist of the letters a-z, the space character, single quotes, and minus characters.");
                labourTextBox.ForeColor = Color.DarkRed;
            }
        }
    }
}
