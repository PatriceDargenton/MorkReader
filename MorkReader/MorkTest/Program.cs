
using _4n6MorkReader;
using System;
//using System.Diagnostics;
using System.IO;
using System.Text;
using static MorkTest.MorkExporter;

namespace MorkTest
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var msfPath = @"C:\Users\[MyAccount]\AppData\Roaming\Thunderbird\Profiles\xxxx.default\Mail\pop.[MyInternetServiceProvider].com\Inbox.msf";
            ReadMSF(msfPath);
        }
        
        
        public static void ReadMSF(string msfPath)
        {
            if (!File.Exists(msfPath))
            {
                Console.WriteLine($"Can't find '{msfPath}'!");
                return;
            }

            Console.WriteLine($"Reading {msfPath}...");
            DateTime dateStart = DateTime.Now;

            using (StreamReader fs = new StreamReader(msfPath))
            {
                MorkDocument document = new MorkDocument(fs);

                int tableNum = 0;
                int nbTables = document.Tables.Count;
                foreach (var table in document.Tables)
                {
                    tableNum++;
                    //if (tableNum > 10) break;
                    table.XmlName = "Messages";

                    int rowNum = 0;
                    int nbRows = table.Rows.Count;
                    foreach (var row in table.Rows)
                    {
                        rowNum++;
                        //if (rowNum > 10) break;

                        //Debug.WriteLine($"Table n°{tableNum}/{nbTables} : Line n°{rowNum}/{nbRows} RowId={row.RowId} row={row.ToString()}");

                        string sendDate = row.getValue("date");
                        string dateReceived = row.getValue("dateReceived");
                        DateTimeOffset dtoDate = ParseMailKitHexDate(sendDate);
                        DateTimeOffset dtoDateReceived = ParseMailKitHexDate(dateReceived);
                        string subjectUTF8 = row.getValue("subject");
                        string sender = row.getValue("sender");
                        string senderName = row.getValue("sender_name");
                        string msgId = row.getValue("message-id");
                        string flags = row.getValue("flags");
                        string ptFlags = row.getValue("ProtoThreadFlags");
                        string glodaDirty = row.getValue("gloda-dirty");
                        string token = row.getValue("storeToken");
                        string threadSubjectUTF8 = row.getValue("threadSubject");

                        string subject, threadSubject;
                        subject = ""; threadSubject = "";
                        if (!string.IsNullOrEmpty(subjectUTF8)) 
                        {
                            byte[] subjectBytes = Encoding.UTF8.GetBytes(subjectUTF8);
                            subject = MimeKit.Utils.Rfc2047.DecodeText(subjectBytes);
                        }
                        if (!string.IsNullOrEmpty(threadSubjectUTF8))
                        {
                            byte[] subjectBytes = Encoding.UTF8.GetBytes(threadSubjectUTF8);
                            threadSubject = MimeKit.Utils.Rfc2047.DecodeText(subjectBytes);
                        }

                        Console.WriteLine(
                            $"Date={dtoDate.LocalDateTime} DateReceived={dtoDateReceived.LocalDateTime} Subject={subjectUTF8} Sender={sender} SenderName={senderName} Msg-Id={msgId} Token={token} Flags={flags} PTFlags={ptFlags} Dirty={glodaDirty}");

                        row.XmlCells["subject"] = subject;
                        row.XmlCells["threadSubject"] = threadSubject;
                        row.XmlCells["sender"] = sender;
                        row.XmlCells["sender_name"] = senderName;
                        row.XmlCells["date"] = dtoDate.LocalDateTime.ToString();
                        row.XmlCells["dateReceived"] = dtoDateReceived.LocalDateTime.ToString();
                        row.XmlCells["message-id"] = msgId;
                        row.XmlCells["flags"] = flags;
                        row.XmlCells["ProtoThreadFlags"] = ptFlags;
                        row.XmlCells["gloda-dirty"] = glodaDirty;
                        row.XmlCells["storeToken"] = token;
                    }
                }
                // ToDo: Align this export with this one:
                //  https://github.com/KevinGoodsell/mork-converter/blob/master/src/mork
                MorkXmlExporter.ExportToXml(document, "output.xml");
            }

            DateTime dateEnd = DateTime.Now;
            TimeSpan ts = dateEnd - dateStart;
            Console.WriteLine($"File analysis time'{msfPath}' : {ts.TotalSeconds:0.00} seconds.");
        }

        private static DateTimeOffset ParseMailKitHexDate(string hexDate)
        {
            if (string.IsNullOrEmpty(hexDate))
            {
                return default(DateTimeOffset);
            }

            // Convert hexadecimal to decimal
            long timestamp = Convert.ToInt64(hexDate, 16);

            // Convert UNIX timestamp to DateTimeOffset
            DateTimeOffset dateTimeOffset = DateTimeOffset.FromUnixTimeSeconds(timestamp);

            return dateTimeOffset;
        }
    }
}
