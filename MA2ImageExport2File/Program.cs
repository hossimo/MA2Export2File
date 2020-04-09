using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace MA2ImageExport2File
{
    class Program
    {

        public struct Images
        {
            public int index { get; set; }
            public string name { get; set; }
            public string value { get; set; }
        }


        static void Main(string[] args)
        {
            if (args.Length == 1)
            {
                string path = Path.GetFullPath(args[0]);
                
                if (!File.Exists(path))
                {
                    Console.WriteLine("File not found at {0}", path);
                    return;
                }
                FileStream fs = File.OpenRead(path);
                ImportXML(fs);
            }
            else
            {
                Console.WriteLine("Usage: MA2ImageExport2File <path to file>");
            }
        }

        static void ImportXML(System.IO.Stream stream)
        {
            bool captureNode = false;
            List<Images> images = new List<Images>();
            Images currentImage = new Images();

            XmlReaderSettings settings = new XmlReaderSettings();
            settings.Async = true;

            using (XmlReader reader = XmlReader.Create(stream, settings))
            {
                while (reader.Read())
                {
                    switch (reader.NodeType)
                    {
                        case XmlNodeType.Element:
                            //Console.WriteLine("Start Element {0}", reader.Name);
                            if (reader.Name == "UserImage")
                            {
                                currentImage.name = reader.GetAttribute("name");
                                currentImage.index = int.Parse(reader.GetAttribute("index"));
                                Console.WriteLine("Found: {4} > {0}.{1} ({2},{3})", currentImage.index, currentImage.name, reader.GetAttribute("width"), reader.GetAttribute("height"), images.Count + 1);
                            }
                            if (reader.Name == "Image")
                            {
                                captureNode = true;
                            }
                            if (reader.Name == "Info")
                            {
                                Console.WriteLine("Reading from Show: {0} - {1}", reader.GetAttribute("showFile"), reader.GetAttribute("datetime"));
                            }
                            break;
                        case XmlNodeType.Text:
                            //Console.WriteLine("Text Node: {0}", reader.Value);
                            if (captureNode)
                            {
                                currentImage.value = System.Text.RegularExpressions.Regex.Replace(reader.Value, @"\s+", "");
                                captureNode = false;
                                images.Add(currentImage);
                                currentImage = new Images();
                            }
                            break;
                        case XmlNodeType.EndElement:
                            //Console.WriteLine("End Element: {0}", reader.Name);
                            break;
                        default:
                            //Console.WriteLine("Other node {0} with value {1}", reader.NodeType, reader.Value);
                            break;
                    }
                }

                foreach (var item in images)
                {
                    byte[] data = System.Convert.FromBase64String(item.value);
                    string extention = ".unknown";


                    // All of this is a hack because aparently I'm too lazy to do it correctly.
                    byte[] PNG = new byte[]{ 0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A };
                    byte[] BMP = new byte[]{ 0x42, 0x4D };
                    byte[] JPEG1 = new byte[] { 0xFF, 0xD8, 0xFF, 0xDB };
                    byte[] JPEG2 = new byte[] { 0xFF, 0xD8, 0xFF, 0xE0, 0x00, 0x10, 0x4A, 0x46, 0x49, 0x46, 0x00, 0x01 };
                    byte[] JPEG3 = new byte[] { 0xFF, 0xD8, 0xFF, 0xEE };
                    byte[] JPEG4x1 = new byte[] { 0xFF, 0xD8, 0xFF, 0xE1 }; //then skip 2
                    byte[] JPEG4x2 = new byte[] { 0x45, 0x78, 0x69, 0x66, 0x00, 0x00 };


                    // PNG
                    int x = 0;
                    foreach (var i in PNG)
                    {
                        if (i != data[x])
                            break;
                        x++;
                        if (x == PNG.Length)
                        {
                            extention = ".png";
                            goto Finish; // I did say this was a hack!
                        }
                    }

                    // BMP
                    x = 0;
                    foreach (var i in BMP)
                    {
                        if (i != data[x])
                            break;
                        x++;
                        if (x == BMP.Length)
                        {
                            extention = ".bmp";
                            goto Finish;
                        }
                    }

                    // JPEG1
                    x = 0;
                    foreach (var i in JPEG1)
                    {
                        if (i != data[x])
                            break;
                        x++;
                        if (x == JPEG1.Length)
                        {
                            extention = ".jpg";
                            goto Finish;
                        }
                    }

                    // JPEG2
                    x = 0;
                    foreach (var i in JPEG2)
                    {
                        if (i != data[x])
                            break;
                        x++;
                        if (x == JPEG2.Length)
                        {
                            extention = ".jpg";
                            goto Finish;
                        }
                    }

                    // JPEG3
                    x = 0;
                    foreach (var i in JPEG3)
                    {
                        if (i != data[x])
                            break;
                        x++;
                        if (x == JPEG3.Length)
                        {
                            extention = ".jpg";
                            goto Finish;
                        }
                    }

                    // JPEG4x1
                    x = 0;
                    foreach (var i in JPEG4x1)
                    {
                        if (i != data[x])
                            break;
                        x++;
                        if (x == JPEG4x1.Length)
                        {
                            // should loop at the seond part but this is already a hack
                            extention = ".jpg";
                            goto Finish;
                        }
                    }



                Finish:
                    File.WriteAllBytes(item.index + "." + item.name + extention, data);
                    Console.WriteLine("Wrote >" + Path.GetFullPath(item.index + "." + item.name + extention));
                }
                
            }
        }
    }
}
