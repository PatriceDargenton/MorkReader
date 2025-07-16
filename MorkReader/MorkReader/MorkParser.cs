using MorkReader;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _4n6MorkReader
{
    /// <summary>
    /// Parses Mork content using event-based parsing.
    /// 
    /// @author mhaller
    /// </summary>
    public class MorkParser
    {
        private bool InstanceFieldsInitialized = false;

        public MorkParser()
        {
            if (!InstanceFieldsInitialized)
            {
                InitializeInstanceFields();
                InstanceFieldsInitialized = true;
            }
        }

        private void InitializeInstanceFields()
        {
            log = new Log(this);
        }


        /// <summary>
        /// Simple logging </summary>
        private Log log;

        /// <summary>
        /// Internal container of event listeners </summary>
        private ICollection<EventListener> eventListeners = new LinkedList<EventListener>();

        /// <summary>
        /// Event reused for all events fired to all event listeners </summary>
        private Event @event = new Event();

        /// <summary>
        /// Ignore exception when they happen within the parsing of mork groups,
        /// which is something like a transaction.
        /// </summary>
        private bool ignoreTransactionFailures = false;

        /// <summary>
        /// Adds the listener to the list of listeners notified of Mork events while
        /// parsing
        /// </summary>
        /// <param name="listener">
        ///            the listener to add to the list of listeners </param>
        public virtual void addEventListener(EventListener listener)
        {
            eventListeners.Add(listener);
        }

        /// <summary>
        /// Removes the given listener
        /// </summary>
        /// <param name="listener">
        ///            the listener to remove from the list of listeners being
        ///            notified of Mork events while parsing </param>
        public virtual void removeEventListener(EventListener listener)
        {
            eventListeners.Remove(listener);
        }

        /// <summary>
        /// Fires the given event to all registered event listeners. The event object
        /// is reused.
        /// </summary>
        /// <param name="eventType">
        ///            one of <seealso cref="EventType"/>, must not be null </param>
        /// <param name="value">
        ///            an optional String value, might be null </param>
        private void fireEvent(EventType eventType, string value)
        {
            @event.eventType = eventType;
            @event.value = value;
            foreach (EventListener eventListener in eventListeners)
            {
                eventListener.onEvent(@event);
            }
        }

        /// <summary>
        /// Convenience method to fire events which have no String content
        /// associated.
        /// </summary>
        /// <param name="eventType">
        ///            one of <seealso cref="EventType"/> </param>
        private void fireEvent(EventType eventType)
        {
            fireEvent(eventType, null);
        }

        /// <summary>
        /// Parse the given String content
        /// </summary>
        /// <param name="morkContent">
        ///            the content to parse </param>
        public virtual void parse(string morkContent)
        {
            parse(new StreamReader(morkContent.ToMemoryStream()));
        }

        /// <summary>
        /// Parse the given input stream
        /// </summary>
        /// <param name="inputStream">
        ///            an input stream </param>
        public virtual void parse(Stream inputStream)
        {
            parse(new StreamReader(inputStream));
        }

        ///// <summary>
        ///// Parse the given file
        ///// </summary>
        ///// <param name="file">
        /////            a file </param>
        //public virtual void parse(string path)
        //{
        //    try
        //    {
        //        parse(new StreamReader(path));
        //    }
        //    catch (FileNotFoundException e)
        //    {
        //        throw e;
        //    }
        //}

        /// <summary>
        /// Parse the Mork content
        /// </summary>
        /// <param name="reader">
        ///            a reader </param>
        public virtual void parse(StreamReader reader)
        {
            try
            {
                //StreamReader pis = new StreamReader(reader, 8);
                parseMain(reader);
            }
            catch (IOException e)
            {
                throw e;
            }
        }

        // < - open angle - begins a dict (inside a dict, begins metainfo row)
        // > - close angle - ends a dict
        // [ - open bracket - begins a row (inside a row, begins metainfo row)
        // ] - close bracket - ends a row
        // { - open brace - begins a table (inside a table, begins metainfo row)
        // } - close brace - ends a table
        // ( - open paren - begins a cell
        // ) - close paren - ends a cell

        /// <summary>
        /// Parses the root elements in a Mork content
        /// </summary>
        //JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
        //ORIGINAL LINE: private void parseMain(java.io.StreamReader pis) throws java.io.IOException
        private void parseMain(StreamReader pis)
        {
            do
            {
                int c = pis.Read();
                switch (c)
                {
                    case '<':
                        parseDict(pis);
                        break;
                    case '[':
                        parseRow(pis);
                        break;
                    case '(':
                        parseCell(pis);
                        break;
                    case '{':
                        parseTable(pis);
                        break;
                    case '/':
                    {
                        int d = pis.Read();
                        if (d == '/')
                        {
                            parseComment(pis);
                            break;
                        }
                        throw new Exception("Unexpected character at current position: " + (char)d);
                    }
                    case '@':
                    {
                        int d = pis.Peek();
                        if (d == '$')
                        {
                            pis.Read();
                            int e = pis.Peek();
                            if (e == '$')
                            {
                                pis.Read();
                                int f = pis.Peek();
                                if (f == '{')
                                {
                                    pis.Read();
                                    // Read ID until "{@" appears
                                    if (ignoreTransactionFailures)
                                    {
                                        try
                                        {
                                            parseGroup(pis);
                                        }
                                        catch (Exception exception)
                                        {
                                            log.warn("Ignoring parsing error within group", exception);
                                        }
                                    }
                                    else
                                    {
                                        parseGroup(pis);
                                    }
                                    break;
                                }
                                //pis.unread(f);
                            }
                            //pis.unread(e);
                        }
                        //pis.unread(d);
                        break;
                    }
                    case -1:
                    {
                        fireEvent(EventType.END_OF_FILE);
                        return;
                    }
                }
            } while (true);
        }

        /// <summary>
        /// Parses a group, extracts its full content and fires an event.
        /// 
        /// Note that the id of the Group is lost currently and there is no way of
        /// retrieving the id of a Group this way.
        /// </summary>
        /// <param name="pis"> </param>
        /// <exception cref="IOException"> </exception>
        //JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
        //ORIGINAL LINE: private void parseGroup(java.io.StreamReader pis) throws java.io.IOException
        private void parseGroup(StreamReader pis)
        {
            string id = parseUntil(pis, "{@".ToCharArray());
            string content = parseUntil(pis, "@$$}".ToCharArray());
            string abort = parseUntil(pis, (id + "}@").ToCharArray());
            if ("~abort~".Equals(abort))
            {
                fireEvent(EventType.GROUP_ABORT, id);
            }
            else
            {
                fireEvent(EventType.GROUP_COMMIT, content);
            }
        }

        /// <summary>
        /// Parse the input as long as the given string is not yet found. Stop if the
        /// string is found and return all content read so far.
        /// </summary>
        /// <param name="pis">
        ///            the input stream, which is positioned after the
        ///            <code>string</code> </param>
        /// <param name="string">
        ///            the content to look for to stop reading </param>
        /// <returns> the content parsed, without the content of the
        ///         <code>string</code> parameter which is silently removed </returns>
        /// <exception cref="IOException"> </exception>
        //JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
        //ORIGINAL LINE: private String parseUntil(java.io.StreamReader pis, char[] string) throws java.io.IOException
        private string parseUntil(StreamReader pis, char[] @string)
        {
            //JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
            //ORIGINAL LINE: final StringBuffer buf = new StringBuffer();
            StringBuilder buf = new StringBuilder();
            char[] temp = new char[@string.Length];
            temp[0] = @string[0];
            while (true)
            {
                int c = pis.Read();
                if (c == -1)
                {
                    continue;
                }
                else if ((char)c == @string[0])
                {
                    int d = pis.Read(temp, 1, temp.Length - 1);
                    if (d == -1)
                    {
                        continue;
                    }
                    if (Enumerable.SequenceEqual(@string, temp))
                    {
                        break;
                    }
                    else
                    {
                        buf.Append(temp, 0, temp.Length);
                        continue;
                    }
                }
                buf.Append((char)c);
            }
            return buf.ToString();
        }

        /// <summary>
        /// Parses a Mork Table which can also contain metatables. All other content
        /// is given back in the event fired.
        /// </summary>
        /// <param name="pis"> </param>
        /// <exception cref="IOException"> </exception>
        //JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
        //ORIGINAL LINE: private void parseTable(java.io.StreamReader pis) throws java.io.IOException
        private void parseTable(StreamReader pis)
        {
            fireEvent(EventType.BEGIN_TABLE);
            //JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
            //ORIGINAL LINE: final StringBuffer buffer = new StringBuffer();
            StringBuilder buffer = new StringBuilder();
            //JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
            //ORIGINAL LINE: final StringBuffer metaTableBuffer = new StringBuffer();
            StringBuilder metaTableBuffer = new StringBuilder();
            bool inMetaTable = false;
            do
            {
                int c = pis.Read();
                switch (c)
                {
                    case '{':
                    {
                        inMetaTable = true;
                        fireEvent(EventType.BEGIN_METATABLE);
                        break;
                    }
                    case '}':
                    {
                        if (inMetaTable)
                        {
                            fireEvent(EventType.END_METATABLE, metaTableBuffer.ToString());
                            inMetaTable = false;
                            break;
                        }
                        fireEvent(EventType.TABLE, buffer.ToString());
                        return;
                    }
                    default:
                    {
                        if (c == '\r' || c == '\n' || c == -1)
                        {
                            break;
                        }
                        if (inMetaTable)
                        {
                            metaTableBuffer.Append((char)c);
                        }
                        else
                        {
                            buffer.Append((char)c);
                        }
                        break;
                    }
                }
            } while (true);
        }

        /// <summary>
        /// Parses a Mork Cell until its closed and fires an event with the content
        /// of the cell. The event does not contain the opening and closing brackets,
        /// which are silently ignored.
        /// </summary>
        /// <param name="pis"> </param>
        /// <exception cref="IOException"> </exception>
        //JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
        //ORIGINAL LINE: private void parseCell(java.io.StreamReader pis) throws java.io.IOException
        private void parseCell(StreamReader pis)
        {
            //JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
            //ORIGINAL LINE: final StringBuffer buffer = new StringBuffer();
            StringBuilder buffer = new StringBuilder();
            do
            {
                int c = pis.Read();
                switch (c)
                {
                    case '\\':
                        int escapedCharacter = pis.Read();
                        if (escapedCharacter == -1)
                        {
                            throw new IOException("Escape character must not be last character in file");
                        }
                        buffer.Append((char)escapedCharacter);
                        break;
                    case ')':
                        fireEvent(EventType.CELL, buffer.ToString());
                        return;
                }
                if (c == '\r' || c == '\n' || c == -1)
                {
                    break;
                }
                buffer.Append((char)c);
            } while (true);
        }

        /// <summary>
        /// Parses a row and fires an event when the row is finished. The event does
        /// not include the open and closing brackets.
        /// </summary>
        /// <param name="pis"> </param>
        /// <exception cref="IOException"> </exception>
        //JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
        //ORIGINAL LINE: private void parseRow(java.io.StreamReader pis) throws java.io.IOException
        private void parseRow(StreamReader pis)
        {
            //JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
            //ORIGINAL LINE: final StringBuffer buffer = new StringBuffer();
            StringBuilder buffer = new StringBuilder();
            do
            {
                int c = pis.Read();
                switch (c)
                {
                    case ']':
                        fireEvent(EventType.ROW, buffer.ToString());
                        return;
                }
                if (c == -1)
                {
                    break;
                }
                buffer.Append((char)c);
            } while (true);
        }

        /// <summary>
        /// Read until first encounter of newline character. Consume all newline
        /// characters.
        /// </summary>
        /// <param name="pis"> </param>
        /// <exception cref="IOException"> </exception>
        //JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
        //ORIGINAL LINE: private void parseComment(java.io.StreamReader pis) throws java.io.IOException
        private void parseComment(StreamReader pis)
        {
            //JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
            //ORIGINAL LINE: final StringBuffer buffer = new StringBuffer();
            StringBuilder buffer = new StringBuilder();
            do
            {
                int c = pis.Read();
                if (c == '\r' || c == '\n' || c == -1)
                {
                    break;
                }
                buffer.Append((char)c);
            } while (true);
            // Now consume all newlines
            do
            {
                int c = pis.Peek();
                if (c == -1)
                {
                    fireEvent(EventType.COMMENT, buffer.ToString());
                    return;
                }
                if (c != '\r' && c != '\n')
                {
                    //pis.unread(c);
                    fireEvent(EventType.COMMENT, buffer.ToString());
                    return;
                }
                pis.Read();
            } while (true);
        }

        /// <summary>
        /// Parse a whole dictionary.
        /// 
        /// The <code>escaped</code> flag denotes if a special character is ignored
        /// as command and thus read as a value object, because it has been escaped
        /// using a backslash previsouly.
        /// </summary>
        /// <param name="pis"> </param>
        /// <exception cref="IOException"> </exception>
        //JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
        //ORIGINAL LINE: private void parseDict(final java.io.StreamReader pis) throws java.io.IOException
        //JAVA TO C# CONVERTER WARNING: 'final' parameters are ignored unless the option to convert to C# 7.2 'in' parameters is selected:
        private void parseDict(StreamReader pis)
        {
            fireEvent(EventType.BEGIN_DICT);
            //JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
            //ORIGINAL LINE: final StringBuffer buffer = new StringBuffer();
            StringBuilder buffer = new StringBuilder();
            bool inMetaDict = false;
            bool isInCell = false;
            do
            {
                int c = pis.Read();
                if (isInCell)
                {
                    switch (c)
                    {
                        case -1:
                        {
                            fireEvent(EventType.END_DICT, buffer.ToString());
                            return;
                        }
                        case ')':
                            if (buffer[buffer.Length - 1] != '\\')
                            {
                                isInCell = false;
                            }
                            buffer.Append((char)c);
                            break;
                        case '$':
                            if (buffer[buffer.Length - 1] != '\\')
                            {
                                parseEncodedCharacter(pis, buffer);
                            }
                            else
                            {
                                buffer.Append((char)c);
                            }
                            break;
                        default:
                            if (c == '\r' || c == '\n')
                            {
                                break;
                            }
                            buffer.Append((char)c);
                            break;
                    }
                }
                else
                {
                    switch (c)
                    {
                        case -1:
                        {
                            fireEvent(EventType.END_DICT, buffer.ToString());
                            return;
                        }
                        case '<':
                            fireEvent(EventType.BEGIN_DICT_METAINFO);
                            inMetaDict = true;
                            buffer.Append((char)c);
                            break;
                        case '$':
                            parseEncodedCharacter(pis, buffer);
                            break;

                        case '>':
                            if (inMetaDict)
                            {
                                fireEvent(EventType.END_DICT_METAINFO);
                                inMetaDict = false;
                                buffer.Append((char)c);
                                break;
                            }
                            fireEvent(EventType.END_DICT, buffer.ToString());
                            return;
                        case '(':
                            if (!isInCell)
                            {
                                isInCell = true;
                            }
                            buffer.Append((char)c);
                            break;
                        case ')':
                            if (isInCell)
                            {
                                isInCell = false;
                            }
                            buffer.Append((char)c);
                            break;
                        case '/':
                            if (isInCell)
                            {
                                buffer.Append((char)c);
                                break;
                            }
                            int d = pis.Read();
                            if (d == '/')
                            {
                                parseComment(pis);
                                break;
                            }
                            buffer.Append((char)c);
                            break;
                        default:
                            if (c == '\r' || c == '\n')
                            {
                                break;
                            }
                            buffer.Append((char)c);
                            break;
                    }
                }
            } while (true);
        }

        /// <summary>
        /// Parses encoded characters into their UTF-8 representation.
        /// 
        /// Example: "$C3$B6$C3$A4$C3$BC$C3$9F" is "äöüß"
        /// </summary>
        /// <param name="pis"> </param>
        /// <param name="buffer"> </param>
        /// <exception cref="IOException"> </exception>
        //JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
        //ORIGINAL LINE: private void parseEncodedCharacter(final java.io.StreamReader pis, final StringBuffer buffer) throws java.io.IOException
        //JAVA TO C# CONVERTER WARNING: 'final' parameters are ignored unless the option to convert to C# 7.2 'in' parameters is selected:
        private void parseEncodedCharacter(StreamReader pis, StringBuilder buffer)
        {
            int c1 = pis.Read();
            int c2 = pis.Read();
            int i1 = Convert.ToInt32(new string(new char[] { (char)c1, (char)c2 }), 16);
            int dollar = pis.Read(); // $
            if (dollar == '$')
            {
                int c3 = pis.Read();
                int c4 = pis.Read();
                int i2 = Convert.ToInt32(new string(new char[] { (char)c3, (char)c4 }), 16);
                buffer.Append((new sbyte[] { (sbyte)i1, (sbyte)i2 }).ToUTF8String());
            }
            else
            {
                buffer.Append((char)i1);
                //pis.unread(dollar); //TODO
            }
        }

        /// <summary>
        /// If set to true, the mork parser ignores exceptions when they happen
        /// within the parsing of groups, as they are often non-essential for reading
        /// address book information.
        /// </summary>
        /// <param name="ignoreTransactionFailures"> </param>
        public virtual bool IgnoreTransactionFailures
        {
            set
            {
                this.ignoreTransactionFailures = value;
            }
        }

        public virtual ExceptionHandler ExceptionHandler
        {
            set
            {
                ExceptionManager.setExceptionHandler(value);
            }
        }

    }
}
