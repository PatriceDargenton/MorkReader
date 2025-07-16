using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace _4n6MorkReader
{
    /// <summary>
	/// Parses a Mork table, which has a table identifier and contains multiple rows.
	/// 
	/// @author mhaller
	/// </summary>
	public class Table
    {

        /// <summary>
        /// The (probably numeric) table identifier </summary>
        private string tableId;

        /// <summary>
        /// An optional scope for the table </summary>
        private string scopeName;

        /// <summary>
        /// Internal container for rows found within the Table definition </summary>
        private IList<Row> rows = new List<Row>();

        /// <summary>
        /// Parses a new Mork Table.
        /// 
        /// As no dictionaries are given, the Mork Table must not use any references.
        /// </summary>
        /// <param name="content"> </param>
        public Table(string content) : this(content, Dict.EMPTY_LIST)
        {
        }

        /// <summary>
        /// Parses a new Mork Table and resolves any references to literal values
        /// using the given list of dictionaries.
        /// </summary>
        /// <param name="content">
        ///            the Mork content to parse </param>
        /// <param name="dicts">
        ///            a list of Dictionaries to resolve literal values. </param>
        public Table(string content, IList<Dict> dicts)
        {
            // "{ 1:cards [ 1 (name=Jack) ] [ 2 (name=John)] }"
            // content = StringUtils.removeCommentLines(content.trim());
            // content = StringUtils.removeNewlines(content);

            // "{1:foobar { [-43F] }"

            // Match table without scope

            //Pattern pattern = Pattern.compile("\\{\\s*([-\\w]*):(\\w*)\\s*(\\[.*\\]\\s*)*\\}");
            Match matcher = Regex.Match(content, "\\{\\s*([-\\w]*):(\\w*)\\s*(\\[.*\\]\\s*)*\\}");
            string rowsContent = null;
            if (!matcher.Success)
            {
                // Try to match with referenced scope
                //Pattern pattern1 = Pattern.compile("\\{\\s*([-\\w]*):\\^([0-9A-Z]*)\\s*(\\[.*\\]\\s*)*\\}");
                Match matcher1 = Regex.Match(content, "\\{\\s*([-\\w]*):\\^([0-9A-Z]*)\\s*(\\[.*\\]\\s*)*\\}");
                if (!matcher1.Success)
                {
                    //Pattern pattern0 = Pattern.compile("\\{\\s*([-\\w]*)\\s*(\\[.*\\]\\s*)*\\}");
                    Match matcher0 = Regex.Match(content, "\\{\\s*([-\\w]*)\\s*(\\[.*\\]\\s*)*\\}");
                    if (!matcher0.Success)
                    {
                        //Pattern pattern2 = Pattern.compile("\\{\\s*([-\\w]*):\\^([0-9A-Z]*)(.*)");
                        Match matcher2 = Regex.Match(StringUtils.removeNewlines(content), "\\{\\s*([-\\w]*):\\^([0-9A-Z]*)(.*)");
                        if (!matcher2.Success)
                        {
                            throw new System.ArgumentException("Table does not match any of the known formats: " + content);
                        }
                        else
                        {
                            tableId = matcher2.Groups[1].Value;
                            scopeName = Dict.dereference("^" + matcher2.Groups[2].Value, dicts, ScopeTypes.COLUMN_SCOPE);
                            rowsContent = matcher2.Groups[3].Value;
                        }
                    }
                    else
                    {
                        tableId = matcher0.Groups[1].Value;
                        rowsContent = matcher0.Groups[2].Value;
                    }
                }
                else
                {
                    tableId = matcher1.Groups[1].Value;
                    scopeName = Dict.dereference("^" + matcher1.Groups[2].Value, dicts, ScopeTypes.COLUMN_SCOPE);
                    rowsContent = matcher1.Groups[3].Value;
                }
            }
            else
            {
                tableId = matcher.Groups[1].Value;
                scopeName = matcher.Groups[2].Value;
                rowsContent = matcher.Groups[3].Value;
            }

            //Pattern rowsPattern = Pattern.compile("\\[[^\\]]*\\]");
            Match rowsMatcher = Regex.Match(rowsContent, "\\[[^\\]]*\\]");
            while (rowsMatcher.Success)
            {
                Row row = new Row(rowsMatcher.Groups[0].Value, dicts);
                rows.Add(row);

                rowsMatcher = rowsMatcher.NextMatch();
            }
        }

        /// <summary>
        /// Returns the (probably numeric) table identifier
        /// </summary>
        /// <returns> the table identifier </returns>
        public virtual string TableId
        {
            get
            {
                return tableId;
            }
        }

        /// <summary>
        /// Returns the optional scope of the table, or <code>null</code>
        /// </summary>
        /// <returns> the scope of the table, if found in the table definition, or
        ///         <code>null</code> </returns>
        public virtual string ScopeName
        {
            get
            {
                return scopeName;
            }
        }

        /// <summary>
        /// Returns an unmodifiable list of Mork Rows
        /// </summary>
        /// <returns> an unmodifiable list of Mork Rows, might be empty but never
        ///         <code>null</code> </returns>
        public virtual IList<Row> Rows
        {
            get
            {
                return rows;
            }
        }
    }

}
