using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _4n6MorkReader
{
    /// <summary>
	/// writes a MorkDocument to output according to https://wiki.mozilla.org/Mork
	/// http://www-archive.mozilla.org/mailnews/arch/mork/grammar.txt
	/// 
	/// @author wf
	/// 
	/// </summary>
	public class MorkWriter
    {
        internal MorkDocument currentDocument;

        internal StreamWriter writer;

        internal OutputStyle outputStyle = OutputStyle.formatted;

        internal int indentation = 0;

        internal int tabSize = 2;

        /// <returns> the tabSize </returns>
        public virtual int TabSize
        {
            get
            {
                return tabSize;
            }
            set
            {
                this.tabSize = value;
            }
        }


        /// <returns> the outputStyle </returns>
        public virtual OutputStyle getOutputStyle()
        {
            return outputStyle;
        }

        /// <param name="outputStyle">
        ///          the outputStyle to set </param>
        public virtual void setOutputStyle(OutputStyle outputStyle)
        {
            this.outputStyle = outputStyle;
        }

        /// <summary>
        /// indent the output
        /// </summary>
        protected internal virtual void indent()
        {
            indentation += tabSize;
        }

        /// <summary>
        /// unindent the output
        /// </summary>
        protected internal virtual void unindent()
        {
            indentation -= tabSize;
        }

        // the magic string
        public const string zm_Magic = "//<!-- <mdb:mork:z v=\"1.4\"/> -->";

        /// <summary>
        /// zm:LineEnd ::= #xA #xD | #xD #xA | #xA | #xD / 1 each if possible /
        /// </summary>
        public const string zm_LineEnd = "\n";

        /// <summary>
        /// zm:S ::= (#x20 | #x9 | #xA | #xD | zm:Continue | zm:Comment)+ / space /
        /// </summary>
        public const string zm_Space = " ";

        /// <returns> the writer </returns>
        public virtual StreamWriter Writer
        {
            get
            {
                return writer;
            }
            set
            {
                this.writer = value;
            }
        }


        /// <returns> the currentDocument </returns>
        public virtual MorkDocument CurrentDocument
        {
            get
            {
                return currentDocument;
            }
            set
            {
                this.currentDocument = value;
            }
        }


        /// <summary>
        /// create a MorkWriter for a given MorkDocument
        /// </summary>
        /// <param name="document"> </param>
        public MorkWriter(MorkDocument document)
        {
            this.currentDocument = document;
        }

        /// <summary>
        /// write the Indentation depending on the outputStyle
        /// </summary>
        /// <exception cref="IOException"> </exception>
        //JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
        //ORIGINAL LINE: public void writeIndent() throws java.io.IOException
        public virtual void writeIndent()
        {
            writer.Write(zm_LineEnd);
            switch (outputStyle)
            {
                case OutputStyle.formatted:
                    for (int i = 0; i < indentation; i++)
                    {
                        writer.Write(zm_Space);
                    }
                    break;
                case OutputStyle.humandReadable:
                case OutputStyle.terse:
                default:
                    break;
            }
        }

        /// <summary>
        /// write to the given file
        /// </summary>
        /// <param name="file">
        ///          a file </param>
        public virtual void write(string path)
        {
            try
            {
                write(new StreamWriter(new FileStream(path, FileMode.OpenOrCreate)));
            }
            catch (FileNotFoundException ex)
            {
                throw ex;
            }
            catch (IOException ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// write to the given output stream
        /// </summary>
        /// <param name="outputStream">
        ///          an output stream </param>
        /// <exception cref="IOException"> </exception>
        //JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
        //ORIGINAL LINE: public void write(java.io.OutputStream outputStream) throws java.io.IOException
        public virtual void write(Stream outputStream)
        {
            write(new StreamWriter(outputStream));
        }

        /// <summary>
        /// write to the given OutputStreamWriter
        /// </summary>
        /// <param name="outputStreamWriter"> </param>
        /// <exception cref="IOException"> </exception>
        //JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
        //ORIGINAL LINE: public void write(java.io.OutputStreamWriter outputStreamWriter) throws java.io.IOException
        public virtual void write(StreamWriter outputStreamWriter)
        {
            this.writer = outputStreamWriter;
            writeDocument();
        }

        /// <summary>
        /// write a Mork document
        /// </summary>
        /// <exception cref="IOException"> </exception>
        //JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
        //ORIGINAL LINE: public void writeDocument() throws java.io.IOException
        public virtual void writeDocument()
        {
            if (currentDocument != null && writer != null)
            {
                writeHeader();
                writeContents();
                writer.Close();
            }
        }

        /// <summary>
        /// write Contents - that is the multiple zm:Content | zm:Group zm:Start ::=
        /// zm:Magic zm:LineEnd zm:Header (zm:Content | zm:Group)
        /// </summary>
        /// <exception cref="IOException">
        ///  </exception>
        //JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
        //ORIGINAL LINE: protected void writeContents() throws java.io.IOException
        protected internal virtual void writeContents()
        {
            // TODO allow for multiple contents according to grammar
            writeContent();
            writeGroup();
        }

        /// <summary>
        /// write Content zm:Content ::= (zm:Dict | zm:Table | zm:Update)
        /// </summary>
        /// <exception cref="IOException"> </exception>
        //JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
        //ORIGINAL LINE: protected void writeContent() throws java.io.IOException
        protected internal virtual void writeContent()
        {
            IList<Dict> dicts = currentDocument.Dicts;
            IList<Table> tables = currentDocument.Tables;
            for (int i = 0; i < Math.Max(dicts.Count, tables.Count); i++)
            {
                if (i < dicts.Count)
                {
                    writeDict(dicts[i]);
                }
                if (i < tables.Count)
                {
                    writeTable(tables[i]);
                }
            }
        }

        /// <summary>
        /// write the given table 
        /// zm:Table ::= zm:S? '{' zm:S? zm:Id zm:TableItem zm:S? '}'
        /// </summary>
        /// <param name="table"> </param>
        /// <exception cref="IOException"> </exception>
        //JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
        //ORIGINAL LINE: protected void writeTable(Table table) throws java.io.IOException
        protected internal virtual void writeTable(Table table)
        {
            indent();
            writeIndent();
            writer.Write('{');
            writeId(table.TableId);
            writer.Write(':');
            indent();
            writeIndent();
            IList<Row> rows = table.Rows;
            foreach (Row row in rows)
            {
                writeRow(row);
            }
            unindent();
            writeIndent();
            writer.Write('}');
            unindent();
        }

        /// <summary>
        /// write the given Row zm:Row ::= zm:S? '[' zm:S? zm:Id zm:RowItem* zm:S? ']'
        /// </summary>
        /// <param name="row"> </param>
        /// <exception cref="IOException"> </exception>
        //JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
        //ORIGINAL LINE: protected void writeRow(Row row) throws java.io.IOException
        protected internal virtual void writeRow(Row row)
        {
            indent();
            writeIndent();
            writer.Write('[');
            writeId(row.RowId);
            IDictionary<string, Alias> aliases = row.Aliases;
            foreach (string id in aliases.Keys)
            {
                writeRowItem(id, aliases[id]);
            }
            //writeIndent();
            writer.Write(']');
            unindent();
        }

        /// <summary>
        /// write the given row item
        /// </summary>
        /// <param name="name"> </param>
        /// <param name="alias">
        ///          zm:RowItem ::= zm:MetaRow | zm:Cell 
        ///          zm:MetaRow ::= zm:S? '[' zm:S?  zm:Cell* zm:S? ']' / meta attributes /
        ///          zm:Cell    ::= zm:S? '(' zm:Column zm:S? zm:Slot? ')' 
        ///          zm:Column  ::= zm:S? (zm:Name | zm:ValueRef) 
        ///          zm:Slot    ::= zm:Value | zm:AnyRef   zm:S?
        ///          zm:Value   ::= '=' ([^)] | '\' zm:NonCRLF | zm:Continue |
        ///          zm:Dollar)* / content ')', '\', and '$' must be quoted with '\'
        ///          inside zm:Value /
        ///          zm:ValueRef  ::= zm:S? '^' zm:Id / use '^' to avoid zm:Name ambiguity / </param>
        /// <exception cref="IOException"> </exception>
        //JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
        //ORIGINAL LINE: protected void writeRowItem(String name, Alias alias) throws java.io.IOException
        protected internal virtual void writeRowItem(string name, Alias alias)
        {
            //indent();
            //writeIndent();
            writer.Write('(');
            if (alias.RefId.StartsWith("^"))
            {
                writer.Write(alias.RefId);
            }
            else
            {
                writeName(name);
            }
            if (alias.ValueRef != null)
            {
                writer.Write(alias.ValueRef);
            }
            else
            {
                writeValue(alias.Value);
            }
            writer.Write(')');
            //unindent();
        }

        /// <summary>
        /// write the given Dictionary
        /// zm:Dict      ::= zm:S? '&lt;' zm:DictItem* zm:S? '&gt;'
        /// zm:DictItem  ::= zm:MetaDict | zm:Alias
        /// zm:MetaDict  ::= zm:S? '&lt;' zm:S? zm:Cell* zm:S? '&gt;' / meta attributes /
        /// zm:Alias     ::= zm:S? '(' zm:Id zm:S? zm:Value ')' </summary>
        /// <param name="dict"> </param>
        /// <exception cref="IOException">  </exception>
        //JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
        //ORIGINAL LINE: protected void writeDict(Dict dict) throws java.io.IOException
        protected internal virtual void writeDict(Dict dict)
        {
            indent();
            writeIndent();
            writer.Write('<');
            indent();
            Aliases aliases = dict.Aliases;
            IList<string> keys = aliases.KeySet;
            foreach (string key in keys)
            {
                writeAlias(key, aliases.getAlias(key));
            }
            unindent();
            writeIndent();
            writer.Write('>');
            unindent();
        }

        /// <summary>
        /// write the given alias (key / value pairs) 
        /// zm:Alias ::= zm:S? '(' zm:Id zm:S? zm:Value ')'
        /// </summary>
        /// <param name="key">
        ///          - the id of the alias </param>
        /// <param name="alias">
        ///          - the value of alias </param>
        /// <exception cref="IOException"> </exception>
        //JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
        //ORIGINAL LINE: protected void writeAlias(String key, Alias alias) throws java.io.IOException
        protected internal virtual void writeAlias(string key, Alias alias)
        {
            writeIndent();
            writer.Write('(');
            writeId(key);
            writeValue(alias.Value);
            writer.Write(')');
        }

        /// <summary>
        /// write a value zm:Value ::= '=' ([^)] | '\' zm:NonCRLF | zm:Continue |
        /// zm:Dollar)*
        /// </summary>
        /// <param name="value"> </param>
        /// <exception cref="IOException"> </exception>
        //JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
        //ORIGINAL LINE: private void writeValue(String value) throws java.io.IOException
        private void writeValue(string value)
        {
            writer.Write('=');
            writer.Write(value);
        }

        /// <summary>
        /// write the given name
        /// zm:Name      ::= [a-zA-Z:_] zm:MoreName*
        /// zm:MoreName  ::= [a-zA-Z:_+-?!]
        /// / names only need to avoid space and '^', so this is more limiting / </summary>
        /// <param name="name"> </param>
        /// <exception cref="IOException">  </exception>
        //JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
        //ORIGINAL LINE: protected void writeName(String name) throws java.io.IOException
        protected internal virtual void writeName(string name)
        {
            writer.Write(name);
        }

        /// <summary>
        /// zm:Id ::= zm:Hex+ / a row, table, or value id is naked hex /
        /// </summary>
        /// <param name="key"> </param>
        /// <exception cref="IOException"> </exception>
        //JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
        //ORIGINAL LINE: private void writeId(String key) throws java.io.IOException
        private void writeId(string key)
        {
            writer.Write(key);
        }

        /// <summary>
        /// write a group
        /// </summary>
        protected internal virtual void writeGroup()
        {
            // TODO implement

        }

        /// <summary>
        /// write a Mork 1.4 header zm:Magic zm:LineEnd zm:Header
        /// </summary>
        /// <exception cref="IOException"> </exception>
        //JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
        //ORIGINAL LINE: protected void writeHeader() throws java.io.IOException
        protected internal virtual void writeHeader()
        {
            writer.Write(zm_Magic);
            writer.Write(zm_LineEnd);
            // TODO write header
        }

    }
}
