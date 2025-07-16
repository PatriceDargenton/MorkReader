using MorkReader;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _4n6MorkReader
{
    [System.Diagnostics.DebuggerStepThrough()]
    /// <summary>
	/// A Mork Document represents a Mork database and provides structured access to
	/// the whole document at once (in contrast to the Mork Parser, which is
	/// event-based)
	/// 
	/// @author mhaller
	/// </summary>
	public class MorkDocument : EventListener
    {

        /// <summary>
        /// Internal container for Dictionaries </summary>
        private IList<Dict> dicts = new List<Dict>();

        /// <summary>
        /// Internal container for Rows </summary>
        private IList<Row> rows = new List<Row>();

        /// <summary>
        /// Internal container for Tables </summary>
        private IList<Table> tables = new List<Table>();

        /// <summary>
        /// Creata a new Mork Document using the given content
        /// </summary>
        /// <param name="reader">
        ///            the Mork content </param>
        public MorkDocument(StreamReader reader) : this(reader, new DefaultExceptionHandler())
        {
        }

        public MorkDocument(StreamReader reader, ExceptionHandler exceptionHandler)
        {
            MorkParser parser = new MorkParser();
            parser.ExceptionHandler = exceptionHandler;
            parser.IgnoreTransactionFailures = true;
            parser.addEventListener(this);
            parser.parse(reader);
        }

        /// <summary>
        /// Internal
        /// </summary>
        public virtual void onEvent(Event @event)
        {
            switch (@event.eventType)
            {
                case EventType.END_DICT:
                {
                    Dict dict = new Dict("<" + @event.value + ">", dicts);
                    dicts.Add(dict);
                    break;
                }
                case EventType.ROW:
                {
                    Row row = new Row("[" + @event.value + "]", dicts);
                    rows.Add(row);
                    break;
                }
                case EventType.TABLE:
                {
                    Table table = new Table("{" + @event.value + "}", dicts);
                    tables.Add(table);
                    break;
                }
                case EventType.GROUP_COMMIT:
                {
                    MorkParser parser = new MorkParser();
                    parser.addEventListener(this);
                    parser.parse(new StreamReader(@event.value.ToMemoryStream()));
                    break;
                }
                case EventType.END_METATABLE:
                case EventType.BEGIN_TABLE:
                case EventType.BEGIN_METATABLE:
                case EventType.BEGIN_DICT:
                case EventType.BEGIN_DICT_METAINFO:
                case EventType.END_DICT_METAINFO:
                case EventType.END_OF_FILE:
                case EventType.COMMENT:
                case EventType.CELL:
                    break;
                default:
                    throw new Exception("Unimplemented event: " + @event.eventType + " for content " + @event.value);
            }
        }

        /// <summary>
        /// Returns all dictionaries found in the Mork document
        /// </summary>
        /// <returns> a list of all dictionaries </returns>
        public virtual IList<Dict> Dicts
        {
            get
            {
                return dicts;
            }
        }

        /// <summary>
        /// Returns a list of all rows which were not inherited in tables found in
        /// the Mork document
        /// </summary>
        /// <returns> a list of all rows which were not inherited in tables </returns>
        public virtual IList<Row> Rows
        {
            get
            {
                return rows;
            }
        }

        /// <summary>
        /// Returns a list of tables
        /// </summary>
        /// <returns> a list of tables found in the Mork document </returns>
        public virtual IList<Table> Tables
        {
            get
            {
                return tables;
            }
        }
    }

}
