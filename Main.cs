using System.Diagnostics.Eventing.Reader;
using System.Xml;
using static ProCheck.ValidateFileSafety;

namespace ProCheck
{
    public partial class Main : Form
    {

        string filePath = string.Empty;
        bool hasAttemptedToLoadOnce = false;
        bool bypassDefender = false;


        private void InitUiState()
        {
            fileLoadProgressBar.Visible = false;
            statusLabel.Visible = true;
            statusLabel.Text = "Select a file to continue.";
            hasAttemptedToLoadOnce = false;


            GenericDataReload();
        }

        private void DisplayErrorAndReload(string message)
        {
            MessageBox.Show(message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            InitUiState();
        }

        public Main()
        {
            InitializeComponent();
            InitUiState();
            GenericDataReload();
        }

        private void GenericDataReload()
        {
            if (hasAttemptedToLoadOnce)
            {
                analyzeNowToolStripMenuItem.Enabled = true;
            }
            else
            {
                analyzeNowToolStripMenuItem.Enabled = false;
            }
        }



        private void processFilePathBeeingSet(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(filePath))
            {
                MessageBox.Show("Please select an executable file first.");
                return;
            }

            fileLoadProgressBar.Visible = true;
            statusLabel.Text = "Verifing file integrity...";

            int fileStatus = ValidateFileSafety.isFileSafe(filePath, bypassDefender);
            bool hasCancelled = true;
            if (fileStatus == 1) DisplayErrorAndReload("The file does not exist.");
            else if (fileStatus == 2) DisplayErrorAndReload("The file is potentially unsafe.");
            else if (fileStatus == 3) DisplayErrorAndReload("The file is not a valid executable.");
            else if (fileStatus == 4) DisplayErrorAndReload("The file is corrupted. (Not a valid PE file)");
            else if (fileStatus == 5) DisplayErrorAndReload("An unknown error occurred.");
            else if (fileStatus == 6) DisplayErrorAndReload("Defender scan failed.");
            else hasCancelled = false;

            if (hasCancelled) return;

            statusLabel.Text = "One second!";
            fileLoadProgressBar.Value = 10;
            fileLoadProgressBar.Maximum = 100;

        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void loadNewToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (selectExecutableDialogue.ShowDialog() == DialogResult.OK)
            {
                filePath = selectExecutableDialogue.FileName;
            }

            // for the sake of it ig
            hasAttemptedToLoadOnce = true;
            GenericDataReload();

            // I am nice, and the user might be dumb, so here we go
            processFilePathBeeingSet(sender, e);
        }

        private void forceAnalysisToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //deprecated, removing this will cause everything to break, so I will leave it here for now
        }

        private void bypassDefenderUNSAFEToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!bypassDefender)
            {
                if (MessageBox.Show("IMPORTANT! \n-------------------\nBy continuing, you agree that you are completely aware of the risks involved. \nProCheck is NOT SANDBOXED, MALWARE CAN AND WILL EXECUTE! \nDisabling this exposes you to even more risks than usual, so it is NOT RECOMMENDED to proceed.", "Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
                {
                    bypassDefender = !bypassDefender;
                }
            }
            else
            {
                bypassDefender = !bypassDefender;
            }
            bypassDefenderUNSAFEToolStripMenuItem.Checked = bypassDefender;
        }

        private void analyzeNowToolStripMenuItem_Click(object sender, EventArgs e)
        {
            processFilePathBeeingSet(sender, e);
        }
    }
}
