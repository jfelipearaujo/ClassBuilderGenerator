using ClassBuilderGenerator.Core;

using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace ClassBuilderGenerator.Forms
{
    public partial class FrmMissingProperty : Form
    {
        public bool ForceCreatingOfMissingProperties { get; set; }
        public List<PropertyInformation> SelectedProperties { get; set; }

        public FrmMissingProperty(CustomConstructor customConstructor, List<PropertyInformation> missingProperties)
        {
            InitializeComponent();

            SelectedProperties = new List<PropertyInformation>();

            txtSelectedConstructor.Text = customConstructor.Constructor;

            missingProperties.ForEach((x) =>
            {
                lbMissingProperties.Items.Add($"{x.Type} {x.Name}");
            });
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            ForceCreatingOfMissingProperties = rbtnForceCreation.Checked;

            if(ForceCreatingOfMissingProperties)
            {
                foreach(var item in lbMissingProperties.SelectedItems)
                {
                    var propData = item.ToString();

                    SelectedProperties.Add(new PropertyInformation
                    {
                        Type = propData.Split(' ')[0],
                        Name = propData.Split(' ')[1]
                    });
                }
            }

            this.Close();
        }
    }
}
