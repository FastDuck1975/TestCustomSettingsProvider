using System.Collections;
using System.Collections.Specialized;
using System.Configuration;
using System.Xml;

namespace TestCustomSettingsProvider
{
    internal class CustomSettingsProvider : SettingsProvider, IApplicationSettingsProvider
    {
        private const string rootNodeName = "OIOUBL-Generator_Settings";
        private const string localSettingsNodeName = "LocalSettings";
        private const string globalSettingsNodeName = "GlobalSettings";
        private const string className = "CustomSettingsProvider";
        private XmlDocument? xmlDocument;

        private string FilePath
        {
            get
            {
                return Path.Combine(AppContext.BaseDirectory, string.Format("{0}.settings", ApplicationName));
            }
        }

        private XmlNode LocalSettingsNode
        {
            get
            {
                XmlNode settingsNode = GetSettingsNode(localSettingsNodeName);
                XmlNode? machineNode = settingsNode.SelectSingleNode(Environment.MachineName.ToLowerInvariant());
                if (machineNode == null)
                {
                    machineNode = SettingsDocument.CreateElement(Environment.MachineName.ToLowerInvariant());
                    settingsNode.AppendChild(machineNode);
                }
                return machineNode;
            }
        }

        private XmlNode GlobalSettingsNode
        {
            get
            {
                return GetSettingsNode(globalSettingsNodeName);
            }
        }

        private XmlNode RootNode
        {
            get
            {
                return SettingsDocument.SelectSingleNode(rootNodeName)!;
            }
        }

        private XmlDocument SettingsDocument
        {
            get
            {
                if (xmlDocument == null)
                {
                    try
                    {
                        xmlDocument = new();
                        xmlDocument.Load(FilePath);
                    }
                    catch (Exception)
                    {
                    }
                    if (xmlDocument?.SelectSingleNode(rootNodeName) != null)
                    {
                        return xmlDocument;
                    }
                    xmlDocument = GetBlankXmlDocument();
                }
                return xmlDocument;
            }
        }

        public override string ApplicationName
        {
            get
            {
                return Path.GetFileNameWithoutExtension(Application.ExecutablePath);
            }
            set
            {
            }
        }

        public override string Name
        {
            get
            {
                return className;
            }
        }

        public override void Initialize(string name, NameValueCollection nameValueCollection)
        {
            base.Initialize(Name, nameValueCollection);
        }

        public override void SetPropertyValues(SettingsContext aettingsContext, SettingsPropertyValueCollection settingsPropertyValueCollection)
        {
            foreach (SettingsPropertyValue settingsPropertyValue in settingsPropertyValueCollection)
            {
                SetValue(settingsPropertyValue);
            }
            try
            {
                SettingsDocument.Save(FilePath);
            }
            catch (Exception)
            {

            }
        }

        public override SettingsPropertyValueCollection GetPropertyValues(SettingsContext settingsContext, SettingsPropertyCollection settingsPropertyCollection)
        {
            SettingsPropertyValueCollection settingsPropertyValueCollection = new();
            foreach (SettingsProperty settingsProperty in settingsPropertyCollection)
            {
                settingsPropertyValueCollection.Add(new SettingsPropertyValue(settingsProperty)
                {
                    SerializedValue = GetValue(settingsProperty)
                });
            }
            return settingsPropertyValueCollection;
        }

        private void SetValue(SettingsPropertyValue settingsPropertyValue)
        {
            XmlNode targetNode = IsGlobal(settingsPropertyValue.Property) ? GlobalSettingsNode : LocalSettingsNode;
            XmlNode? settingNode = targetNode.SelectSingleNode(string.Format("Setting[@Name='{0}']", settingsPropertyValue.Name));
            if (settingNode != null)
            {
                settingNode.InnerText = settingsPropertyValue.SerializedValue.ToString()!;
            }
            else
            {
                settingNode = SettingsDocument.CreateElement("Setting");
                XmlAttribute nameAttribute = SettingsDocument.CreateAttribute("Name");
                nameAttribute.Value = settingsPropertyValue.Name;
                settingNode.Attributes!.Append(nameAttribute);
                settingNode.InnerText = settingsPropertyValue.SerializedValue.ToString()!;
                targetNode.AppendChild(settingNode);
            }
        }

        private string? GetValue(SettingsProperty settingsProperty)
        {
            XmlNode targetNode = IsGlobal(settingsProperty) ? GlobalSettingsNode : LocalSettingsNode;
            XmlNode? settingNode = targetNode.SelectSingleNode(string.Format("Setting[@Name='{0}']", settingsProperty.Name));
            if (settingNode == null)
            {
                return settingsProperty.DefaultValue != null ? settingsProperty.DefaultValue.ToString() : string.Empty;
            }
            return settingNode.InnerText;
        }

        private static bool IsGlobal(SettingsProperty settingsProperty)
        {
            foreach (DictionaryEntry dictionaryEntry in settingsProperty.Attributes)
            {
                if (dictionaryEntry.Value as Attribute is SettingsManageabilityAttribute)
                {
                    return true;
                }
            }
            return false;
        }

        private XmlNode GetSettingsNode(string name)
        {
            XmlNode? settingsNode = RootNode.SelectSingleNode(name);
            if (settingsNode == null)
            {
                settingsNode = SettingsDocument.CreateElement(name);
                RootNode.AppendChild(settingsNode);
            }
            return settingsNode;
        }

        public static XmlDocument GetBlankXmlDocument()
        {
            XmlDocument blankXmlDocument = new();
            blankXmlDocument.AppendChild(blankXmlDocument.CreateXmlDeclaration("1.0", "utf-8", string.Empty));
            blankXmlDocument.AppendChild(blankXmlDocument.CreateElement(rootNodeName));
            return blankXmlDocument;
        }

        public void Reset(SettingsContext settingsContext)
        {
            LocalSettingsNode.RemoveAll();
            GlobalSettingsNode.RemoveAll();
            xmlDocument!.Save(FilePath);
        }

        public SettingsPropertyValue GetPreviousVersion(SettingsContext settingsContext, SettingsProperty settingsProperty)
        {
            return new SettingsPropertyValue(settingsProperty);
        }

        public void Upgrade(SettingsContext settingsContext, SettingsPropertyCollection settingsPropertyCollection)
        {
        }
    }
}