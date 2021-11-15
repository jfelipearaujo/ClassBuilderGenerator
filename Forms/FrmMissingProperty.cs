using ClassBuilderGenerator.Core;

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
                lbMissingProperties.Items.Add($"{x.Type} {x.Name}");
            });
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            ForceCreatingOfMissingProperties = rbtnForceCreation.Checked;

            if(ForceCreatingOfMissingProperties)
            {
                foreach(var item in lbMissingProperties.Items)
                {
                    var propData = item.ToString();

                    var propertyInfo = new PropertyInformation
                    {
                        Type = propData.Split(' ')[0],
                        Name = propData.Split(' ')[1]
                    };

                    if(propertyInfo.Type.Contains("System.Collections.Generic"))
                    {
                        propertyInfo.Type = propertyInfo.Type
                            .Replace("System.Collections.Generic.", string.Empty);

                        var start = propertyInfo.Type.IndexOf("<") + 1;
                        var end = propertyInfo.Type.LastIndexOf(">");
                        var subPropType = propertyInfo.Type.Substring(start, end - start);

                        if(subPropType.Contains("."))
                        {
                            var subListObject = subPropType.Substring(subPropType.LastIndexOf(".") + 1);

                            var collectionType = propertyInfo.Type
                                .Substring(0, propertyInfo.Type.IndexOf("<"));

                            propertyInfo.Type = $"{collectionType}<{subListObject}>";
                        }
                    }
                    else if(propertyInfo.Type.Contains("."))
                    {
                        propertyInfo.Type = propertyInfo.Type
                            .Substring(propertyInfo.Type.LastIndexOf(".") + 1);
                    }

                    MissingProperties.Add(propertyInfo);
                }
            }

            this.Close();
        }
    }
}
