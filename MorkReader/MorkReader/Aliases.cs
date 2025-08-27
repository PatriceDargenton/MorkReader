using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _4n6MorkReader
{
    public class Aliases
    {

        private IDictionary<string, Alias> dict = new Dictionary<string, Alias>();
        public Aliases()
        {

        }
        public Aliases(string aliases) : this(aliases, Dict.EMPTY_LIST)
        {
        }
        public Aliases(string aliases, IList<Dict> dicts)
        {
            //JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
            //ORIGINAL LINE: final java.io.StringReader reader = new java.io.StringReader(aliases);
            TextReader reader = new StringReader(aliases);
            //JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
            //ORIGINAL LINE: final StringBuffer alias = new StringBuffer();
            StringBuilder alias = new StringBuilder();
            bool inParentheses = false;
            try
            {
                int c = reader.Read();
                while (c != -1)
                {
                    switch (c)
                    {
                        case '\\':
                            int escapedCharacter = reader.Read();
                            alias.Append((char)escapedCharacter);
                            break;
                        case '(':
                            inParentheses = true;
                            alias.Append((char)c);
                            break;
                        case ')':
                            if (inParentheses)
                            {
                                alias.Append((char)c);
                                parseSingleAlias(dicts, alias.ToString().Trim());
                                alias.Length = 0;
                                inParentheses = false;
                            }
                            break;
                        default:
                            if (inParentheses)
                            {
                                alias.Append((char)c);
                            }
                            break;
                    }
                    c = reader.Read();
                }
            }
            catch (IOException)
            {
                throw new Exception("Format of alias not supported: " + aliases);
            }
        }
        //JAVA TO C# CONVERTER WARNING: 'final' parameters are ignored unless the option to convert to C# 7.2 'in' parameters is selected:
        //ORIGINAL LINE: private void parseSingleAlias(final java.util.List<Dict> dicts, final String aliasStr)
        private void parseSingleAlias(IList<Dict> dicts, string aliasStr)
        {
            if (aliasStr.Length < 3)
            {
                throw new Exception("Alias must be at least 3 characters: " + aliasStr);
            }
            //JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
            //ORIGINAL LINE: final String withoutParentheses = aliasStr.substring(1, (aliasStr.length() - 1) - 1);
            string withoutParentheses = aliasStr.Substring(1, (aliasStr.Length - 1) - 1);
            bool isLiteral = withoutParentheses.IndexOf('=') != -1;
            if (isLiteral)
            {
                string refid = aliasStr.Substring(1, aliasStr.IndexOf('=') - 1);
                string id = refid;
                if (id.StartsWith("^", StringComparison.Ordinal))
                {
                    id = Dict.dereference(id, dicts, ScopeTypes.COLUMN_SCOPE);
                }
                string value = aliasStr.SubstringSpecial(aliasStr.IndexOf('=') + 1, aliasStr.Length - 1);
                Alias alias = new Alias(refid, id, value, null);
                dict[id.Trim()] = alias;
            }
            else
            {
                //string refid = aliasStr.Substring(1, aliasStr.IndexOf('^', 2) - 1);
                int len = aliasStr.IndexOf('^', 2) - 1;
                string refid;
                refid = aliasStr;
                if (len >= 0) refid = aliasStr.Substring(1, len);
                else Debug.WriteLine("Warning: " + aliasStr + ": len=" + len);

                string id = refid;
                string valueref = aliasStr.SubstringSpecial(aliasStr.IndexOf('^', 2), aliasStr.Length - 1);
                if (id.StartsWith("^", StringComparison.Ordinal))
                {
                    id = Dict.dereference(id, dicts, ScopeTypes.COLUMN_SCOPE);
                }
                string value = valueref;
                if (valueref.StartsWith("^", StringComparison.Ordinal))
                {
                    value = Dict.dereference(value, dicts, ScopeTypes.ATOM_SCOPE);
                }
                Alias alias = new Alias(refid, id, value, valueref);
                dict[id.Trim()] = alias;
            }
        }
        public virtual int count()
        {
            return dict.Count;
        }
        public virtual IDictionary<string, Alias> getAliases()
        {
            return dict;
        }

        //public virtual void printAliases(PrintStream @out)
        //{
        //    @out.println(dict);
        //}
        public virtual IList<string> KeySet
        {
            get
            {
                return dict.Keys?.ToList();
            }
        }
        public virtual string getValue(string id)
        {
            //try
            //{
                //Alias alias = dict[id.Trim()];
                var idt = id.Trim();
                if (!dict.ContainsKey(idt)) return null;
                var alias = dict[idt];
                if (alias != null) return alias.Value;
            //}
            //catch (Exception ex) { }
            return null;
        }
        public virtual Alias getAlias(string id)
        {
            return dict[id.Trim()];
        }
    }

}
