using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;

using System.Collections.Generic;
using System.Windows.Forms;

namespace ClassBuilderGenerator.Forms
{
    public partial class FrmConstructorSelector : Form
    {
        public string SelectedConstructor { get; set; }

        private readonly AsyncPackage package;
        private readonly IVsUIShell uiShell;

        public FrmConstructorSelector(AsyncPackage package, IVsUIShell uiShell, List<string> constructors)
        {
            InitializeComponent();

            this.package = package;
            this.uiShell = uiShell;

            clbConstructors.Items.AddRange(constructors.ToArray());
        }

        private void btnOk_Click(object sender, System.EventArgs e)
        {
            if(clbConstructors.CheckedItems.Count == 0 ||
                clbConstructors.CheckedItems.Count > 1)
            {
                VsShellUtilities.ShowMessageBox(
                        package,
                        "Please, select one constructor to be used!",
                        "Error",
                        OLEMSGICON.OLEMSGICON_CRITICAL,
                        OLEMSGBUTTON.OLEMSGBUTTON_OK,
                        OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_FIRST);

                return;
            }

            var result = VsShellUtilities.PromptYesNo("Are you sure you want to use the selected constructor?",
                "Please, confirm",
                OLEMSGICON.OLEMSGICON_QUERY,
                uiShell);

            if(!result)
                return;

            SelectedConstructor = clbConstructors.SelectedItem.ToString();

            Close();
        }
    }
}
