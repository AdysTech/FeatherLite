using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace AdysTech.FeatherLite.Settings
{
    #region AppManifest
    //http://dotnetbyexample.blogspot.com/2011/03/easy-access-to-wmappmanifestxml-app.html
    public static class AppManifestInfo
    {
        private static Mutex threadSync = new Mutex (false, System.Guid.NewGuid ().ToString ());
        static Dictionary<string, string> properties = ParseManifest ();

        public static Dictionary<string, string> Properties
        {
            get
            {
                return new Dictionary<string, string> (properties);
            }
        }

        public static string GetPropertyValue(string property)
        {
            if ( !properties.ContainsKey (property) )
                return "";
            return properties[property];
        }

        private static Dictionary<string, string> ParseManifest()
        {

            if ( !threadSync.WaitOne (1000) )
                throw new TimeoutException ("Could not get hold on AppManifest");

            var properties = new Dictionary<string, string> ();
            var appManifestXml = XDocument.Load ("WMAppManifest.xml");
            string nodeName = "", nodeValue = "";
            using ( var reader = appManifestXml.CreateReader (ReaderOptions.OmitDuplicateNamespaces) )
            {
                reader.MoveToContent ();
                // Parse the file starting with the second book node.
                do
                {
                    switch ( reader.NodeType )
                    {
                        case XmlNodeType.Element:
                            nodeName = reader.Name;
                            while ( reader.MoveToNextAttribute () )
                            {
                                if ( reader.Name != "" && reader.Value != "" )
                                {
                                    var attrib = nodeName + " " + reader.Name;
                                    if ( properties.ContainsKey (attrib) )
                                        properties[attrib] += ", " + reader.Value;
                                    else
                                        properties.Add (attrib, reader.Value);
                                }
                            }
                            break;
                        case XmlNodeType.Text:
                            nodeValue = reader.Value;
                            break;
                        case XmlNodeType.EndElement:
                            if ( nodeName != "" && nodeValue != "" )
                                properties.Add (nodeName, nodeValue);
                            nodeName = "";
                            nodeValue = "";
                            break;
                    }
                } while ( reader.Read () );
                reader.Close ();
                threadSync.ReleaseMutex ();
            }

            return properties;
        }


    }
    #endregion
}
