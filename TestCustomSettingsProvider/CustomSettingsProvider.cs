using System.Collections;
using System.Collections.Specialized;
using System.Configuration;
using System.Xml;

namespace TestCustomSettingsProvider
{
    internal class CustomSettingsProvider : SettingsProvider, IApplicationSettingsProvider
    {
        const string settingsRoot = "Settings";
        const string localSetttingsNodeName = "localSettings";
        const string globalSettingsNodeName = "globalSettings";
        const string className = "CustomSettingsProvider";
        XmlDocument xmlDocument;

        private string FilePath
        {
            get
            {
                return Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), string.Format("{0}.settings", ApplicationName));
            }
        }

        private XmlNode LocalSettingsNode
        {
            get
            {
                XmlNode settingsNode = GetSettingsNode(localSetttingsNodeName);
                XmlNode machineNode = settingsNode.SelectSingleNode(Environment.MachineName.ToLowerInvariant());
                if(machineNode == null)
                {
                    machineNode = XmlSettings.CreateElement(Environment.MachineName.ToLowerInvariant());
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
                return XmlSettings.SelectSingleNode(settingsRoot);
            }
        }

        private XmlDocument XmlSettings
        {
            get
            {
                if(xmlDocument == null)
                {
                    try
                    {
                        xmlDocument = new();
                        xmlDocument.Load(FilePath);
                    }
                    catch (Exception)
                    {

                    }
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

        public override void Initialize(string name, NameValueCollection config)
        {
            base.Initialize(Name, config);
        }

        public override void SetPropertyValues(SettingsContext settingsContext, SettingsPropertyValueCollection settingsPropertyValueCollection)
        {
            foreach(SettingsPropertyValue settingsPropertyValue in settingsPropertyValueCollection)
            {
                SetValue(settingsPropertyValue);
            }
            try
            {
                XmlSettings.Save(FilePath);
            }
            catch (Exception)
            {
                //Do nothing
            }
        }

        public override SettingsPropertyValueCollection GetPropertyValues(SettingsContext settingsContext, SettingsPropertyCollection settingsPropertyCollection)
        {
            SettingsPropertyValueCollection settingsPropertyValueCollection = new();
            foreach(SettingsProperty settingsProperty in settingsPropertyCollection)
            {
                settingsPropertyValueCollection.Add(new(settingsProperty)
                {
                    SerializedValue = GetValue(settingsProperty)
                });
            }
            return settingsPropertyValueCollection;
        }

        private void SetValue(SettingsPropertyValue settingsPropertyValue)
        {
            XmlNode targetNode = IsGlobal(settingsPropertyValue.Property) ? GlobalSettingsNode : LocalSettingsNode;
            XmlNode settingsNode = targetNode.SelectSingleNode(string.Format("setting[@name='{0}]", settingsPropertyValue.Name));
            if (settingsNode != null)
            {
                settingsNode.InnerText = settingsPropertyValue.SerializedValue.ToString();
            }
            else
            {
                settingsNode = XmlSettings.CreateElement("setting");
                XmlAttribute xmlAttribute = XmlSettings.CreateAttribute("name");
                settingsNode.Attributes.Append(xmlAttribute);
                settingsNode.InnerText = settingsPropertyValue.SerializedValue.ToString();
                targetNode.AppendChild(settingsNode);
            }
        }

        private string GetValue(SettingsProperty settingsProperty)
        {
            XmlNode targetNode = IsGlobal(settingsProperty) ? GlobalSettingsNode : LocalSettingsNode;
            XmlNode settingsNode = targetNode.SelectSingleNode(string.Format("setting[@name='{0}']", settingsProperty.Name));
            if (settingsNode != null)
            {
                return settingsProperty.DefaultValue != null ? settingsProperty.DefaultValue.ToString() : string.Empty;
            }
            return settingsNode.InnerText;
        }

        private bool IsGlobal(SettingsProperty settingsProperty)
        {
            foreach(DictionaryEntry dictionaryEntry in settingsProperty.Attributes)
            {
                if(dictionaryEntry.Value as Attribute is SettingsManageabilityAttribute)
                {
                    return true;
                }
            }
            return false;
        }

        private XmlNode GetSettingsNode(string name)
        {
            XmlNode settingsNode = RootNode.SelectSingleNode(name);
            if (settingsNode == null)
            {
                settingsNode = XmlSettings.CreateElement(name);
                RootNode.AppendChild(settingsNode);
            }
            return settingsNode;
        }

        public SettingsPropertyValue GetPreviousVersion(SettingsContext settingsContext, SettingsProperty settingsProperty)
        {
            return new(settingsProperty);
        }

        public void Reset(SettingsContext settingsContext)
        {
            LocalSettingsNode.RemoveAll();
            GlobalSettingsNode.RemoveAll();
            XmlSettings.Save(FilePath);
        }

        public void Upgrade(SettingsContext settingsContext, SettingsPropertyCollection settingsPropertyCollection)
        {
        }
    }
}