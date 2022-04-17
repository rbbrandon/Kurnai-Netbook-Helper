using System;
using System.IO;
using System.Drawing;
using System.Windows.Forms;

namespace KurnaiNetbookHelper
{
    public partial class EditJobGUI : Form
    {
        private Validate validate = new Validate();
        private MainGUI _Parent;
        private Job _Job;
        private string _JobTXT;
        private bool ticketValid = true;
        private bool nameValid = true;
        private bool idValid = true;

        public EditJobGUI(MainGUI parent, Job job, string jobTXT)
        {
            _Parent = parent;
            _Job = job;
            _JobTXT = jobTXT;
            InitializeComponent();
        }

        private void EditJobGUI_Load(object sender, EventArgs e)
        {
            ticketTextBox.Text = _Job.JobID;
            nameTextBox.Text = _Job.StudentName;
            idTextBox.Text = _Job.StudentID;
            this.CenterToParent();
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            string jobTXTContents = "";

            // Read existing JobTXT.
            try
            {
                using (StreamReader sr = new StreamReader(_JobTXT))
                {
                    jobTXTContents = sr.ReadToEnd();
                    sr.Close();
                }
            }
            catch (IOException ioe)
            {
                MessageBox.Show("Error: " + ioe.Message,
                    "Unable to open job file.",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                Close();
            }

            // Update JobTXT contents.
            jobTXTContents = jobTXTContents.Replace(String.Format("{0},{1},{2}", _Job.JobID, _Job.StudentID, _Job.StudentName),
                String.Format("{0},{1},{2}", ticketTextBox.Text, idTextBox.Text, nameTextBox.Text));

            // Save new JobTXT contents to file.
            try
            {
                using (StreamWriter sw = new StreamWriter(_JobTXT))
                {
                    sw.Write(jobTXTContents);
                    sw.Close();
                }
            }
            catch (IOException ioex)
            {
                MessageBox.Show("Error: " + ioex.Message,
                    "Unable to write job file.",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }

            Close();
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void ticketTextBox_TextChanged(object sender, EventArgs e)
        {
            // Change the colour of the edited textbox to indicate if the field is valid or not.
            if (validate.IsNumber(ticketTextBox.Text))
            {
                ticketTextBox.ForeColor = Color.DarkGreen;
                toolTip.SetToolTip(ticketTextBox, "");
                ticketValid = true;
            }
            else
            {
                ticketTextBox.ForeColor = Color.DarkRed;
                toolTip.SetToolTip(ticketTextBox, "Ticket number may only consist of the digits 0-9.");
                ticketValid = false;
            }

            // Enable the "OK" button if all fields are valid.
            if (ticketValid && nameValid && idValid)
                okButton.Enabled = true;
            else
                okButton.Enabled = false;
        }

        private void nameTextBox_TextChanged(object sender, EventArgs e)
        {

            // Change the colour of the edited textbox to indicate if the field is valid or not.
            if (validate.IsPhrase(nameTextBox.Text))
            {
                nameTextBox.ForeColor = Color.DarkGreen;
                toolTip.SetToolTip(nameTextBox, "");
                nameValid = true;
            }
            else
            {
                nameTextBox.ForeColor = Color.DarkRed;
                toolTip.SetToolTip(nameTextBox, "Student name may only consist of the letters a-z, the space character, single quotes, and minus characters.");
                nameValid = false;
            }

            // Enable the "OK" button if all fields are valid.
            if (ticketValid && nameValid && idValid)
                okButton.Enabled = true;
            else
                okButton.Enabled = false;
        }

        private void idTextBox_TextChanged(object sender, EventArgs e)
        {
            // Change the colour of the edited textbox to indicate if the field is valid or not.
            if (validate.IsStudentID(idTextBox.Text))
            {
                idTextBox.ForeColor = Color.DarkGreen;
                toolTip.SetToolTip(idTextBox, "");
                idValid = true;
            }
            else
            {
                idTextBox.ForeColor = Color.DarkRed;
                toolTip.SetToolTip(idTextBox, "A student ID must be 3 letters followed by 4 digits.");
                idValid = false;
            }

            // Enable the "OK" button if all fields are valid.
            if (ticketValid && nameValid && idValid)
                okButton.Enabled = true;
            else
                okButton.Enabled = false;

            // Make this textbox be uppercase only.
            int cursorPosition = idTextBox.SelectionStart;
            idTextBox.Text = idTextBox.Text.ToUpper();
            idTextBox.Select(cursorPosition, 0);
        }
    }
}
