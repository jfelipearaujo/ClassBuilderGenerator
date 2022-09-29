using ClassBuilderGenerator.Helpers;
using ClassBuilderGenerator.Models;

using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace ClassBuilderGenerator.Forms
{
    public partial class FrmMissingProperty : Form
    {
        public bool ForceCreatingOfMissingProperties { get; set; }
        public List<PropertyInformation> MissingProperties { get; set; }

        public FrmMissingProperty(CustomConstructor customConstructor, List<PropertyInformation> missingProperties)
        {
            InitializeComponent();

            MissingProperties = new List<PropertyInformation>();

            txtSelectedConstructor.Text = customConstructor.Constructor;

            missingProperties.ForEach((x) =>
            {
                lbMissingProperties.Items.Add($"{x.Type} {x.OriginalName}");
            });
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            ForceCreatingOfMissingProperties = rbtnForceCreation.Checked;

            if (ForceCreatingOfMissingProperties)
            {
                foreach (var item in lbMissingProperties.Items)
                {
                    var propData = item.ToString();

                    MissingProperties.Add(new PropertyInformation
                    {
                        Type = propData.Split(' ')[0].RemoveNamespace(),
                        OriginalName = propData.Split(' ')[1]
                    });
                }
            }

            this.Close();
        }
    }
}
