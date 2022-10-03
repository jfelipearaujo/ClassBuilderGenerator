using System.Collections.Generic;
using System.Windows.Forms;

namespace ClassBuilderGenerator.Forms
{
    public partial class FrmConstructorSelector : Form
    {
        public string SelectedConstructor { get; set; }

        public FrmConstructorSelector(List<string> constructors)
        {
            InitializeComponent();

            clbConstructors.Items.AddRange(constructors.ToArray());
        }

        private void btnOk_Click(object sender, System.EventArgs e)
        {
            if (clbConstructors.CheckedItems.Count == 0 ||
                clbConstructors.CheckedItems.Count > 1)
            {
                MessageBox.Show("Please, select one constructor to be used!",
                        "Error",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);

                return;
            }

            var result = MessageBox.Show("Are you sure you want to use the selected constructor?",
                "Please, confirm",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (result == DialogResult.No)
                return;

            SelectedConstructor = clbConstructors.SelectedItem.ToString();

            Close();
        }
    }
}
