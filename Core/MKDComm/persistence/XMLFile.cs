using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.IO;
using System.Reflection;

namespace mkdinfo.persistence
{
    public abstract class XMLFile
    {
        public static void save(Object obj, string file){
            if (obj != null)
            {
                XmlSerializer serializer = new XmlSerializer(obj.GetType());
                FileStream fs = new FileStream(file, FileMode.Create);
                try
                {
                    serializer.Serialize(fs, obj);
                }
                finally { fs.Close(); }
            }
        }

        public static Object load(Type type, string file)
        {
            Object r = null;
            int counter = 0;
            while (counter < 3 && r == null)
            {
                if (file != null && File.Exists(file))
                {
                    XmlSerializer serializer = new XmlSerializer(type);
                    FileStream fs = new FileStream(file, FileMode.Open);
                    try
                    {
                        r = serializer.Deserialize(fs);
                    }
                    catch (Exception e)
                    {
                        if (e == null) { }
                    }
                    finally
                    {
                        fs.Close();
                    }

                }
            }
            return r;
        }


        public static string getApplicationData(string fileName = null)
        {
            string customAppFolder = System.IO.Path.GetFileName(Assembly.GetEntryAssembly().GetName().Name);
            string settingsDirectory = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            if (!Directory.Exists(settingsDirectory))
                Directory.CreateDirectory(settingsDirectory);
            settingsDirectory = Path.Combine(settingsDirectory, customAppFolder.Trim());
            if (!Directory.Exists(settingsDirectory))
                Directory.CreateDirectory(settingsDirectory);
            if (fileName != null)
                return Path.Combine(settingsDirectory, fileName);
            return settingsDirectory;
        }


    }
}
