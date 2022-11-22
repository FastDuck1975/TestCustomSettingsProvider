using System.Collections;
using System.Collections.Specialized;
using System.Configuration;
using System.Xml;

namespace TestCustomSettingsProvider
{
    internal class CustomSettingsProvider : SettingsProvider
    {
        //Root node in the Settings file, can you change that at runtime?
        const string SETTINGSROOT = "Settings";

        public override void Initialize(string name, NameValueCollection config)
        {
            base.Initialize(this.ApplicationName, config);
        }

        public override string ApplicationName
        {
            get
            {
                if (Application.ProductName.Trim().Length > 0)
                {
                    return Application.ProductName;
                }
                else
                {
                    FileInfo fi = new(Application.ExecutablePath);
                    return fi.Name[..^fi.Extension.Length];
                }
            }
            set
            {
                // Nothing to set or can we here tell witch .settings file we need so we split the settings for easier read?
            }
        }

/*        public override string Name
        {
            get
            {
                return "CustomSettingsProvider";
            }
        } */

        public static string GetAppSettingsPath()
        {
            return AppContext.BaseDirectory;
        }

        public string GetAppSettingsFilename()
        {
            return ApplicationName + ".settings";
        }

        public override void SetPropertyValues(SettingsContext settingsContext, SettingsPropertyValueCollection settingsPropertyValueCollection)
        {
            foreach (SettingsPropertyValue settingsPropertyValue in settingsPropertyValueCollection)
            {
                SetValue(settingsPropertyValue);
            }
            try
            {
                SettingsXml.Save(Path.Combine(GetAppSettingsPath(), GetAppSettingsFilename()));
            }
            catch (Exception)
            {
                // Should we handle this? Can't save
            }
        }

        public override SettingsPropertyValueCollection GetPropertyValues(SettingsContext settingsContext, SettingsPropertyCollection settingsPropertyCollection)
        {
            SettingsPropertyValueCollection settingsPropertyValueCollection = new();
            foreach (SettingsProperty settingsProperty in settingsPropertyCollection)
            {
                SettingsPropertyValue value = new(settingsProperty)
                {
                    IsDirty = false,
                    SerializedValue = GetValue(settingsProperty)
                };
                settingsPropertyValueCollection.Add(value);
            }
            return settingsPropertyValueCollection;
        }

        private XmlDocument? xmlDocument;

        private XmlDocument SettingsXml
        {
            get
            {
                if (xmlDocument == null)
                {
                    xmlDocument = new XmlDocument();
                    try
                    {
                        xmlDocument.Load(Path.Combine(GetAppSettingsPath(), GetAppSettingsFilename()));
                    }
                    catch (Exception)
                    {
                        xmlDocument.AppendChild(xmlDocument.CreateXmlDeclaration("1.0", "utf-8", string.Empty));
                        xmlDocument.AppendChild(xmlDocument.CreateNode(XmlNodeType.Element, SETTINGSROOT, ""));
                    }
                }
                return xmlDocument;
            }
        }

        private string? GetValue(SettingsProperty settingsProperty)
        {
            string? retrunValue;
            try
            {
                if (IsRoaming(settingsProperty))
                {
                    retrunValue = SettingsXml.SelectSingleNode(SETTINGSROOT + "/" + settingsProperty.Name)?.InnerText;
                }
                else
                {
                    retrunValue = SettingsXml.SelectSingleNode(SETTINGSROOT + "/" + Environment.MachineName + "/" + settingsProperty.Name)?.InnerText;
                }
            }
            catch (Exception)
            {
                if ((settingsProperty.DefaultValue != null))
                {
                    retrunValue = settingsProperty.DefaultValue.ToString();
                }
                else
                {
                    retrunValue = "";
                }
            }

            return retrunValue;
        }

        private void SetValue(SettingsPropertyValue settingsPropertyValue)
        {
            XmlElement? MachineNode;
            XmlElement? SettingNode;
            try
            {
                if (IsRoaming(settingsPropertyValue.Property))
                {
                    SettingNode = SettingsXml.SelectSingleNode(SETTINGSROOT + "/" + settingsPropertyValue.Name) as XmlElement;
                }
                else
                {
                    SettingNode = SettingsXml.SelectSingleNode(SETTINGSROOT + "/" + Environment.MachineName + "/" + settingsPropertyValue.Name) as XmlElement;
                }
            }
            catch (Exception)
            {
                SettingNode = null;
            }
            if ((SettingNode != null))
            {
                SettingNode.InnerText = settingsPropertyValue.SerializedValue.ToString()!;
            }
            else
            {
                if (IsRoaming(settingsPropertyValue.Property))
                {
                    SettingNode = SettingsXml.CreateElement(settingsPropertyValue.Name);
                    SettingNode.InnerText = settingsPropertyValue.SerializedValue.ToString()!;
                    SettingsXml.SelectSingleNode(SETTINGSROOT)?.AppendChild(SettingNode);
                }
                else
                {
                    try
                    {
                        MachineNode = SettingsXml.SelectSingleNode(SETTINGSROOT + "/" + Environment.MachineName) as XmlElement;
                    }
                    catch (Exception)
                    {
                        MachineNode = SettingsXml.CreateElement(Environment.MachineName);
                        SettingsXml.SelectSingleNode(SETTINGSROOT)?.AppendChild(MachineNode);
                    }
                    if (MachineNode == null)
                    {
                        MachineNode = SettingsXml.CreateElement(Environment.MachineName);
                        SettingsXml.SelectSingleNode(SETTINGSROOT)?.AppendChild(MachineNode);
                    }
                    SettingNode = SettingsXml.CreateElement(settingsPropertyValue.Name);
                    SettingNode.InnerText = settingsPropertyValue.SerializedValue.ToString()!;
                    MachineNode.AppendChild(SettingNode);
                }
            }
        }

        private static bool IsRoaming(SettingsProperty settingsProperty)
        {
            foreach (DictionaryEntry dictionaryEntry in settingsProperty.Attributes)
            {
                Attribute? attribute = dictionaryEntry.Value as Attribute;
                if (attribute is SettingsManageabilityAttribute)
                {
                    return true;
                }
            }
            return false;
        }
    }
}