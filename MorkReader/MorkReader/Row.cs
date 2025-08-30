using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace _4n6MorkReader
{
    /// <summary>
    /// A Row can be located either in the root of Mork files or in tables. A Row
    /// contains multiple values and has a row identifier associated. A row can also
    /// be scoped.
    /// 
    /// @author mhaller
    /// </summary>
    public class Row
    {
        public Dictionary<string, string> XmlCells { get; set; } = new Dictionary<string, string>();

        /// <summary>
        /// The identifier of the row </summary>
        private string rowId;

        /// <summary>
        /// An optional scope of the row </summary>
        private string scopeName;

        /// <summary>
        /// The parsed values of the cells in the Row </summary>
        private Aliases aliases;

        /// <summary>
        /// Parse a Row in the given content (must include the brackets) and resolves
        /// references using the given dictionaries.
        /// 
        /// The Row can optionally have a scope.
        /// </summary>
        /// <param name="content">
        ///            the Mork content of a Row including opening and closing
        ///            brackets. </param>
        /// <param name="dicts">
        ///            a list of dictionaries </param>
        public Row(string content, IList<Dict> dicts)
        {
            content = StringUtils.removeCommentLines(content);
            content = StringUtils.removeNewlines(content);
            //Pattern pattern = Pattern.compile("\\s*\\[\\s*(\\w*):(\\^?\\w*)\\s*(.*)\\s*\\]");
            Match matcher = Regex.Match(content, "\\s*\\[\\s*(\\w*):(\\^?\\w*)\\s*(.*)\\s*\\]");
            if (!matcher.Success)
            {
                //Pattern pattern3 = Pattern.compile("\\[\\-([0-9A-F]*)\\]");
                Match matcher3 = Regex.Match(content, "\\[\\-([0-9A-F]*)\\]");
                if (matcher3.Success)
                {
                    // Row without cells (row within a transaction, e.g. for removal)
                    rowId = matcher3.Groups[1].Value;
                    aliases = new Aliases();
                }
                else
                {
                    // Try to match simple row without scope name
                    //Pattern pattern2 = Pattern.compile("\\s*\\[\\s*(-?\\w*)\\s*(.*)\\s*\\]");
                    Match matcher2 = Regex.Match(content, "\\s*\\[\\s*(-?\\w*)\\s*(.*)\\s*\\]");
                    if (!matcher2.Success)
                    {
                        throw new Exception("Row does not match RegEx: " + content);
                    }
                    rowId = matcher2.Groups[1].Value;
                    string cells = matcher2.Groups[2].Value;
                    aliases = new Aliases(cells, dicts);
                }
            }
            else
            {
                rowId = matcher.Groups[1].Value;

                string scopeValue = matcher.Groups[2].Value;
                if (scopeValue.StartsWith("^", StringComparison.Ordinal))
                {
                    scopeName = Dict.dereference(scopeValue, dicts, ScopeTypes.COLUMN_SCOPE);
                }
                else
                {
                    scopeName = scopeValue;
                }

                string cells = matcher.Groups[3].Value;
                aliases = new Aliases(cells, dicts);
            }
        }

        /// <summary>
        /// Parse a Row in the given content (must include the brackets). Does not
        /// resolve references as no dictionaries are given. The Row can optionally
        /// have a scope.
        /// </summary>
        /// <param name="content">
        ///            the Mork content of a Row including opening and closing
        ///            brackets. </param>
        public Row(string content) : this(content, Dict.EMPTY_LIST)
        {
        }

        /// <summary>
        /// Returns the identifier of the row, usually a numeric value
        /// </summary>
        /// <returns> the identifier of the row </returns>
        public virtual string RowId
        {
            get
            {
                return this.rowId;
            }
        }

        /// <summary>
        /// Returns an optional scope of the row, might be <code>null</code>.
        /// </summary>
        /// <returns> the scope of the row if defined, or <code>null</code> </returns>
        public virtual string ScopeName
        {
            get
            {
                return this.scopeName;
            }
        }

        /// <summary>
        /// Returns the value of a cell with the given id. The id must already be
        /// dereferenced.
        /// </summary>
        /// <param name="id">
        ///            the id of the cell </param>
        /// <returns> the dereferenced literal value of the Cell with the given id </returns>
        public virtual string getValue(string id)
        {
            return aliases.getValue(id);
        }

        /// <summary>
        /// Returns a Map of all values found in the Row
        /// </summary>
        /// <returns> a Map of all values found in the Row. The column header names
        ///         (ids) and the values are already dereferenced. </returns>
        public virtual IDictionary<string, Alias> Aliases
        {
            get
            {
                return aliases.getAliases();
            }
        }

        /// <summary>
        /// access to this row's keySet
        /// @return
        /// </summary>
        public virtual IList<string> KeySet
        {
            get
            {
                return aliases.KeySet;
            }
        }

        /// <summary>
        /// Formats the content of this row showing all values.
        /// </summary>
        /// <returns> the content of this row showing all values. </returns>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("[\r\n");
            foreach (KeyValuePair<string, Alias> e in aliases.getAliases())
            {
                if (!"".Equals(e.Value.Value))
                {
                    sb.Append("	{");
                    sb.Append(e.Key);
                    sb.Append('=');
                    sb.Append(e.Value.Value);
                    sb.Append("},\r\n");
                }
            }

            sb.Append("]\r\n");
            return sb.ToString();
        }
    }

}
