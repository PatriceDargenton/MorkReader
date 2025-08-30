
using _4n6MorkReader;
using System.Xml;

namespace MorkTest
{
    internal class MorkExporter
    {
        public static class MorkXmlExporter
        {
            public static void ExportToXml(MorkDocument doc, string outputPath)
            {
                using (var writer = XmlWriter.Create(outputPath, 
                    new XmlWriterSettings
                    {
                        Indent = true,
                        Encoding = System.Text.Encoding.UTF8
                    }))
                {
                    writer.WriteStartDocument();
                    writer.WriteStartElement("MorkDocument");

                    foreach (var table in doc.tables) // Tables
                    {
                        writer.WriteStartElement("Table");
                        writer.WriteAttributeString("Name", table.XmlName);

                        foreach (var row in table.Rows)
                        {
                            writer.WriteStartElement("Row");
                            foreach (var cell in row.XmlCells)
                            {
                                writer.WriteStartElement("Cell");
                                writer.WriteAttributeString("Name", cell.Key);
                                writer.WriteString(cell.Value ?? "");
                                writer.WriteEndElement(); // Cell
                            }
                            writer.WriteEndElement(); // Row
                        }

                        writer.WriteEndElement(); // Table
                    }

                    writer.WriteEndElement(); // MorkDocument
                    writer.WriteEndDocument();
                }
            }
        }
    }
}
