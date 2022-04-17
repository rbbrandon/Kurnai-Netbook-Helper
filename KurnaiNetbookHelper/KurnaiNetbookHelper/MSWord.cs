using System;
using System.IO;
using System.Text;
using System.Windows.Forms;
using System.Drawing.Printing;
using Word = Microsoft.Office.Interop.Word;

namespace KurnaiNetbookHelper
{
    public class MSWord
    {
        private Word._Application wrdApp;
        private Word._Document wrdDoc;
        private Object oMissing = System.Reflection.Missing.Value;
        private Object oFalse = false;
        private AddJobGUI _Parent;
        private Constants _Constants = new Constants();
        internal string _PrinterToUse = "";

        public MSWord(AddJobGUI parent)
        {
            _Parent = parent;
        }

        private void CreateMailMergeDataFile()
        {
            // Initialisation.
            string csvHeader = "Date,Requester,OrderNo,Model,SNID,Student,ItemCode01,ItemCode02,ItemCode03,ItemCode04,ItemCode05,ItemCode06,ItemCode07,ItemCode08,ItemCode09,ItemCode10,ItemCode11,ItemCode12,ItemCode13,ItemCode14,ItemCode15,ItemCode16,ItemCode17,ItemCode18,ItemQuantity01,ItemQuantity02,ItemQuantity03,ItemQuantity04,ItemQuantity05,ItemQuantity06,ItemQuantity07,ItemQuantity08,ItemQuantity09,ItemQuantity10,ItemQuantity11,ItemQuantity12,ItemQuantity13,ItemQuantity14,ItemQuantity15,ItemQuantity16,ItemQuantity17,ItemQuantity18,ItemDesc01,ItemDesc02,ItemDesc03,ItemDesc04,ItemDesc05,ItemDesc06,ItemDesc07,ItemDesc08,ItemDesc09,ItemDesc10,ItemDesc11,ItemDesc12,ItemDesc13,ItemDesc14,ItemDesc15,ItemDesc16,ItemDesc17,ItemDesc18,ItemPrice01,ItemPrice02,ItemPrice03,ItemPrice04,ItemPrice05,ItemPrice06,ItemPrice07,ItemPrice08,ItemPrice09,ItemPrice10,ItemPrice11,ItemPrice12,ItemPrice13,ItemPrice14,ItemPrice15,ItemPrice16,ItemPrice17,ItemPrice18,TotalPrice01,TotalPrice02,TotalPrice03,TotalPrice04,TotalPrice05,TotalPrice06,TotalPrice07,TotalPrice08,TotalPrice09,TotalPrice10,TotalPrice11,TotalPrice12,TotalPrice13,TotalPrice14,TotalPrice15,TotalPrice16,TotalPrice17,TotalPrice18,Total,Campus,IntCode,Phone,Fax,Address,GLCode,DamageType";
            string[] glSymbols = new string[4] { "", "•", "¤", "‡" };
            string glCode = "\"";
            int glSymbolIndex = 0;
            string labourReason = _Parent._LabourReason;

            string[] csvData = new string[104]
            {
                DateTime.Now.ToString("dd/MM/yyyy"), _Parent.rNameTextBox.Text, _Parent.poTextBox.Text, _Parent.modelComboBox.Text,
                "\"" + _Parent.serialTextBox.Text + "\"", "\"" + _Parent.nameTextBox.Text + " (" + _Parent.idTextBox.Text + ")\"", "", "", "", "", "", "", "", "", "",
                "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "",
                "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "",
                "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", ""
            };

            // Check for multiple GLCodes.
            string prefix = "";
            int itemCount = _Parent.objectListView1.CheckedObjects.Count;

            for (int checkedIndex = 0; checkedIndex < itemCount; checkedIndex++)
            {
                LWTItem lwtItem = (LWTItem)_Parent.objectListView1.CheckedObjects[checkedIndex];

                if (!glCode.Contains(lwtItem.GLCode))
                {
                    // Check if there are too many GL Codes than what has been provided for.
                    if (glSymbolIndex >= glSymbols.GetLength(0))
                    {
                        MessageBox.Show(_Parent, "You have selected too many products with different assigned GL Codes. This app can only handle up to " + glSymbols.GetLength(0).ToString() + " GL Codes. Please split this order.",
                            "Error: Too Many GL Codes",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error);
                        return;
                    }

                    // Add the GLCode to the list of GLCodes
                    glCode += prefix + glSymbols[glSymbolIndex] + lwtItem.GLCode;
                    prefix = ", ";
                    glSymbolIndex += 1;
                }
            }

            // Close the double quote marks for the GLCode string.
            glCode += "\"";

            // Add in checked items.
            for (int checkedIndex = 0; checkedIndex < itemCount; checkedIndex++)
            {
                LWTItem lwtItem = (LWTItem)_Parent.objectListView1.CheckedObjects[checkedIndex];
                string itemCode = lwtItem.ItemCode;
                string itemGLSymbol = glSymbols[0];

                // If more than one GL Code is in use for this order, the 2nd-4th will need symbols to define which GL code is for which items
			    // Check if more than one GL Code is mentioned
                if (glCode.Contains(","))
                {
                    string[] glCodeArray = glCode.Replace(" ", "").Replace("\"", "").Split(',');

                    for (int glIndex = 0; glIndex < glCodeArray.GetLength(0); glIndex++)
                    {
                        if (glCodeArray[glIndex].Contains(lwtItem.GLCode))
                        {
                            if (glIndex == 0)
                                itemGLSymbol = glSymbols[glIndex];
                            else
                                itemGLSymbol = glSymbols[glIndex] + " ";

                            break;
                        }
                    }
                }

                // Set the description of the current item to it's GL Code symbol, followed by the item's description
                string itemTitle = String.Format("\"{0}{1}\"", itemGLSymbol, lwtItem.Description);
                string itemPrice = lwtItem.Price;
                string itemQuantity = lwtItem.Quantity.ToString();

                // Store the item's code in the array
			    csvData[6 + checkedIndex] = itemCode;

			    // Store the item's quantity in the array
			    csvData[24 + checkedIndex] = itemQuantity;

			    // Store the item's description in the array
			    csvData[42 + checkedIndex] = itemTitle;

			    // Store the item's unit price in the array
			    csvData[60 + checkedIndex] = itemPrice;

			    // Store the item's total price in the array (formatting it in dollar.cents format)
                csvData[78 + checkedIndex] = String.Format("${0:0.00}", lwtItem.Quantity * lwtItem.PriceFloat);
            }

            // Add Labour/Delivery.
            if (_Parent.labourRadioButton.Checked)
            {
                csvData[5 + itemCount + 1] = "LEN1813";
		        csvData[23 + itemCount + 1] = "1";
		        csvData[41 + itemCount + 1] = "\"Labour Charge: " + labourReason + "\"";
		        csvData[59 + itemCount + 1] = String.Format("${0:0.00}", _Constants.COMMON_INFO["LabourCost"]);
                csvData[77 + itemCount + 1] = String.Format("${0:0.00}", _Constants.COMMON_INFO["LabourCost"]);
            }
            else if (_Parent.largeDeliveryRadioButton.Checked)
            {
                csvData[5 + itemCount + 1] = "";
                csvData[23 + itemCount + 1] = "1";
                csvData[41 + itemCount + 1] = "Delivery/Administration and Handling (Large)";
                csvData[59 + itemCount + 1] = String.Format("${0:0.00}", _Constants.COMMON_INFO["DeliveryCostLarge"]);
                csvData[77 + itemCount + 1] = String.Format("${0:0.00}", _Constants.COMMON_INFO["DeliveryCostLarge"]);
            }
            else
            {
                csvData[5 + itemCount + 1] = "";
                csvData[23 + itemCount + 1] = "1";
                csvData[41 + itemCount + 1] = "Delivery/Administration and Handling (Small)";
                csvData[59 + itemCount + 1] = String.Format("${0:0.00}", _Constants.COMMON_INFO["DeliveryCostSmall"]);
                csvData[77 + itemCount + 1] = String.Format("${0:0.00}", _Constants.COMMON_INFO["DeliveryCostSmall"]);
            }

            // Specify Damage Type.
            if (_Parent.accidentRadioButton.Checked)
                csvData[103] = "Accidental";
            else if (_Parent.deliberateRadioButton.Checked)
                csvData[103] = "Deliberate";
            else
                csvData[103] = "Unknown";

            // Specify Order Total.
            csvData[96] = _Parent.totalTextBox.Text;

            // Fill in Campus-Specific Details.
            int i = _Parent.campusComboBox.SelectedIndex;

            csvData[97] = _Constants.CAMPUS_INFO[i]["Name"];
            csvData[98] = _Constants.CAMPUS_INFO[i]["IntCode"];
            csvData[99] = _Constants.CAMPUS_INFO[i]["Phone"];
            csvData[100] = _Constants.CAMPUS_INFO[i]["Fax"];
            csvData[101] = String.Format("\"{0} Campus - {1}, {2} {3} {4}\"", _Constants.CAMPUS_INFO[i]["Name"], _Constants.CAMPUS_INFO[i]["Address"],
                _Constants.CAMPUS_INFO[i]["Suburb"], _Constants.CAMPUS_INFO[i]["State"], _Constants.CAMPUS_INFO[i]["Postcode"]);

            // Fill in GL Codes.
            csvData[102] = glCode;

            // Format data as CSV string.
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(csvHeader);

            prefix = "";
            for (int index = 0; index < csvData.GetLength(0); index++)
            {
                sb.AppendFormat("{0}{1}", prefix, csvData[index]);
                prefix = ",";
            }

            // Try writing the order data to the temp file:
            try
            {
                using (StreamWriter sw = new StreamWriter(_Constants.ORDER_TEMP))
                {
                    sw.Write(sb.ToString());
                    sw.Close();
                }
            }
            catch (IOException ioe)
            {
                MessageBox.Show(_Parent, "Error: " + ioe.Message,
                    "Unable to write temp file.",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                return;
            }
        }

        public bool GenerateOrder()
        {
            CreateMailMergeDataFile();

            string dataFile = _Constants.ORDER_TEMP;
            string template = "";

            // Select which template to use, based on the selected model
            if (_Parent.customCheckBox.Checked)
                // This is a custom PO, so use the Custom PO Template
                template = _Constants.ORDER_TEMPLATE_CUSTOM;
            else
                // This is not a custom PO, so use the standard PO Template
                template = _Constants.ORDER_TEMPLATE;

            // Check to make sure the required files exist
            if (!File.Exists(template))
            {
                // Template doesn't exist.
                // Display an error message, and then cancel PO creation.
                MessageBox.Show(_Parent, "Unable to find " + template + ".\nPlease make sure the template exists.",
                    "File Not Found",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                return false;
            }
            else if (!File.Exists(dataFile))
            {
                // CSV for the mail merge doesn't exist.
                // Display an error message, and then cancel PO creation.
                MessageBox.Show(_Parent, "Unable to find " + dataFile + ".\nPlease make sure the datafile exists and isn't open in any programs.",
                    "File Not Found",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);

                return false;
            }

            Word.WdMailMergeDestination wdSendToNewDocument = Word.WdMailMergeDestination.wdSendToNewDocument;
            Word.WdMailMergeState wdMainAndDataSource = Word.WdMailMergeState.wdMainAndDataSource;
            Word.WdSaveOptions wdDoNotSaveChanges = Word.WdSaveOptions.wdDoNotSaveChanges;
            Word.WdSaveFormat wdFormatPDF = Word.WdSaveFormat.wdFormatPDF;

            string outputFileName = _Constants.DATA_DIR + @"Generated Purchase Orders\" + _Constants.CAMPUS_INFO[_Parent.campusComboBox.SelectedIndex]["Name"] + @"\" +
                _Parent.poTextBox.Text + ".pdf";

            // Create an instance of Word  and make it invisible.
            wrdApp = new Word.Application();
            wrdApp.Visible = false;
            wrdApp.ScreenUpdating = false;

            // Open the template
            wrdDoc = wrdApp.Documents.Open(template, ref oMissing,
                ref oMissing, ref oMissing, ref oMissing, ref oMissing, ref oMissing,
                ref oMissing, ref oMissing, ref oMissing, ref oMissing, ref oMissing,
                ref oMissing, ref oMissing, ref oMissing, ref oMissing);

            // Set the data source for the mail merge
            wrdDoc.MailMerge.OpenDataSource(dataFile, ref oMissing, ref oMissing,
                ref oMissing, ref oMissing, ref oMissing, ref oMissing, ref oMissing,
                ref oMissing, ref oMissing, ref oMissing, ref oMissing, ref oMissing,
                ref oMissing, ref oMissing, ref oMissing);

            // Perform the mail merge
            Word.MailMerge wrdMailMerge = wrdDoc.MailMerge;

            if (wrdMailMerge.State == wdMainAndDataSource)
            {
                wrdMailMerge.Destination = wdSendToNewDocument;
                wrdMailMerge.Execute(ref oFalse);
            }
            else
            {
                wrdDoc.Close(wdDoNotSaveChanges, ref oMissing, ref oMissing);
                wrdDoc = null;
                wrdMailMerge = null;
                wrdApp.Quit(ref oMissing, ref oMissing, ref oMissing);
                wrdApp = null;
                File.Delete(dataFile);
                return false;
            }

            // Close the original form document.
            //wrdDoc.Saved = true;
            // Release References.
            wrdDoc.Close(wdDoNotSaveChanges, ref oMissing, ref oMissing);
            wrdDoc = null;
            wrdMailMerge = null;

            try
            {
                // Save document into PDF Format.
                wrdApp.ActiveDocument.SaveAs(outputFileName,
                    wdFormatPDF, ref oMissing, ref oMissing,
                    ref oMissing, ref oMissing, ref oMissing, ref oMissing,
                    ref oMissing, ref oMissing, ref oMissing, ref oMissing,
                    ref oMissing, ref oMissing, ref oMissing, ref oMissing);
            }
            catch (Exception)
            {
                MessageBox.Show(_Parent, "Unable to save " + outputFileName + ".\nPlease make sure the file is not open and that you have write access to that directory.",
                    "Save Failed",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);

                wrdDoc.Close(wdDoNotSaveChanges, ref oMissing, ref oMissing);
                wrdDoc = null;
                wrdMailMerge = null;
                wrdApp.Quit(wdDoNotSaveChanges, ref oMissing, ref oMissing);
                wrdApp = null;
                File.Delete(dataFile);

                return false;
            }

            PrinterSettings settings = new PrinterSettings();
            string defaultPrinter = settings.PrinterName;

            // Add print command here.
            using (PrintGUI printGUI = new PrintGUI(outputFileName, this))
                if (printGUI.ShowDialog(_Parent) == DialogResult.OK)
                {
                    wrdApp.Application.ActivePrinter = _PrinterToUse;
                    wrdApp.ActiveDocument.PrintOut();
                    wrdApp.Application.ActivePrinter = defaultPrinter;

                    MessageBox.Show(_Parent, "Purchase order has been sent to " + _PrinterToUse,
                        "Document Printed",
                        MessageBoxButtons.OK);
                }


            ((Word._Document)wrdApp.ActiveDocument).Close(wdDoNotSaveChanges, ref oMissing, ref oMissing);

            // Exit Word.
            wrdApp.Quit(ref oMissing, ref oMissing, ref oMissing);
            wrdApp = null;

            // Clean up temp file.
            File.Delete(dataFile);

            return true;
        }
    }
}
