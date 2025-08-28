
using _4n6MorkReader;
using System;
//using System.Diagnostics;
using System.IO;

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
                        string subject = row.getValue("subject");
                        string sender = row.getValue("sender");
                        string senderName = row.getValue("sender_name");
                        string msgId = row.getValue("message-id");
                        string flags = row.getValue("flags");
                        string ptFlags = row.getValue("ProtoThreadFlags");
                        string glodaDirty = row.getValue("gloda-dirty");
                        string token = row.getValue("storeToken");
                        string threadSubject = row.getValue("threadSubject");

                        if (string.IsNullOrEmpty(subject)) subject = threadSubject;
                        
                        Console.WriteLine(
                            $"Date={dtoDate.LocalDateTime} DateReceived={dtoDateReceived.LocalDateTime} Subject={subject} Sender={sender} SenderName={senderName} Msg-Id={msgId} Token={token} Flags={flags} PTFlags={ptFlags} Dirty={glodaDirty}");
                    }
                }
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
