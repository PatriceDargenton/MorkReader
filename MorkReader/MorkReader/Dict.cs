using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace _4n6MorkReader
{
    /// <summary>
    /// A dictionary contains key/value pairs. The keys are used in data cells (e.g.
    /// Aliases Definitions) to compress data and reference the values stored in
    /// dictionaries.
    /// 
    /// Each dictionary can optionally be categorized into a scope. The default scope
    /// is the Atom Scope, which is used for literal values of actual content (in
    /// contrast to the Column Scope, which us used for header data only)
    /// 
    /// @author mhaller
    /// </summary>
    public class Dict
    {
        /// <summary>
        /// A typed empty list of dictionaries </summary>
        public static readonly IList<Dict> EMPTY_LIST = new List<Dict>(0);

        /// <summary>
        /// The name of the scope, e.g. 'a' or 'atomScope' </summary>
        private string scopeName;

        /// <summary>
        /// The value of the scope, usually 'c' </summary>
        private string scopeValue;

        /// <summary>
        /// Internal reference to aliases which were included in the Dictionary
        /// definition
        /// </summary>
        private Aliases aliases;

        /// <returns> the aliases </returns>
        public virtual Aliases Aliases
        {
            get
            {
                return aliases;
            }
        }


        /// <summary>
        /// Parse a Dictionary using the given String content. The simplest
        /// dictionary possible is <code>&gt;&lt;</code>.
        /// </summary>
        /// <param name="dictString">
        ///            a valid dictionary definition </param>
        public Dict(string dictString) : this(dictString, Dict.EMPTY_LIST)
        {
        }

        /// <summary>
        /// Parse a Dictionary using the given String content. The simplest
        /// dictionary possible is <code>&gt;&lt;</code>.
        /// </summary>
        /// <param name="dictString">
        ///            a valid dictionary definition </param>
        /// <param name="dicts">
        ///            a preexisting list of dictionaries </param>
        public Dict(string dictString, IList<Dict> dicts)
        {
            // dictString = StringUtils.removeCommentLines(dictString);
            // dictString = StringUtils.removeNewlines(dictString);
            //Pattern pattern = Pattern.compile("\\s*<\\s*(<\\(?.*\\)?>)?[\\s\\n\\r]*(.*)>[\\s\\r\\n]*", Pattern.MULTILINE | Pattern.DOTALL);
            Match matcher = Regex.Match(dictString, "\\s*<\\s*(<\\(?.*\\)?>)?[\\s\\n\\r]*(.*)>[\\s\\r\\n]*", RegexOptions.Multiline | RegexOptions.Singleline);
            if (!matcher.Success)
            {
                throw new Exception("RegEx does not match: " + dictString);
            }
            string scopeDef = matcher.Groups[1].Value;
            string aliasesDef = matcher.Groups[2].Value;

            // Scope
            if (!string.ReferenceEquals(scopeDef, null))
            {
                //Pattern scopePattern = Pattern.compile("<\\(?(.*)=([^\\)])\\)?>");
                Match scopeMatcher = Regex.Match(scopeDef, "<\\(?(.*)=([^\\)])\\)?>");
                if (scopeMatcher.Success)
                {
                    scopeName = scopeMatcher.Groups[1].Value;
                    scopeValue = scopeMatcher.Groups[2].Value;
                }
            }

            // Aliases
            aliases = new Aliases(aliasesDef, dicts);
        }

        /// <summary>
        /// Returns the default scope of the parsed dictionary. This is not
        /// necessarily the same as the "global default scope", which is the Atom
        /// Scope.
        /// 
        /// Since Aliases itself could be scoped, a dictionary has a its own default
        /// scope for contained non-scoped aliases.
        /// </summary>
        /// <returns> the default scope of the Dictionary, one of <seealso cref="ScopeTypes"/> </returns>
        public virtual ScopeTypes DefaultScope
        {
            get
            {
                if (!string.ReferenceEquals(scopeValue, null) && scopeValue.ToLower().StartsWith("c", StringComparison.Ordinal))
                {
                    return ScopeTypes.COLUMN_SCOPE;
                }
                return ScopeTypes.ATOM_SCOPE;
            }
        }

        /// <summary>
        /// Returns the name of the scope,if the Dictionary included a scope
        /// definition.
        /// </summary>
        /// <returns> the name of the scope of the Dictionary, if there was any, or
        ///         <code>null</code> </returns>
        public virtual string ScopeName
        {
            get
            {
                return scopeName;
            }
        }

        /// <summary>
        /// Returns the value of the scope, if the Dictionary has any.
        /// </summary>
        /// <returns> the value of the scope, or <code>null</code> if there was no
        ///         explicit scope definition. </returns>
        public virtual string ScopeValue
        {
            get
            {
                return scopeValue;
            }
        }

        /// <summary>
        /// Returns the value of a parsed alias, if the Dictionary declared it.
        /// </summary>
        /// <param name="id">
        ///            the Alias Key to dereference </param>
        /// <returns> the value of the alias with the key id </returns>
        public virtual string getValue(string id)
        {
            return aliases.getValue(id);
        }

        /// <summary>
        /// Returns the number of aliases available in this Dictionary
        /// </summary>
        /// <returns> the count number of aliases available </returns>
        public virtual int AliasCount
        {
            get
            {
                return aliases.count();
            }
        }

        /// <summary>
        /// Dereferences a pointer to a value using this dictionary.
        /// </summary>
        /// <param name="id">
        ///            the id with the "^"-Prefix </param>
        public virtual string dereference(string id)
        {
            if (!id.StartsWith("^", StringComparison.Ordinal))
            {
                throw new Exception("dereference() must be called with a reference id including the prefix '^'");
            }
            string oid = id.Substring(1);
            string value = getValue(oid);
            return value;
        }

        /// <summary>
        /// Dereferences a pointer to a value using the given list of dictionaries to
        /// resolve it in the given scope.
        /// </summary>
        /// <param name="id">
        ///            the pointer id </param>
        /// <param name="dicts">
        ///            a list of dictionaries </param>
        /// <param name="scope">
        ///            the scope to look in </param>
        /// <returns> the value if could be dereferenced </returns>
        /// <exception cref="RuntimeException">
        ///             if the dictionaries are empty or the value could not be found </exception>
        public static string dereference(string id, IList<Dict> dicts, ScopeTypes scope)
        {
            //if (dicts.Count == 0)
            if (dicts == null || dicts.Count == 0)
            {
                return ExceptionManager.createString(id, new Exception("Cannot dereference IDs without dictionaries"));
            }
            //string dereference = null;
            foreach (Dict dict in dicts)
            {
                if (dict.DefaultScope == scope)
                {
                    var value = dict.dereference(id);
                    if (value != null) return value;
                    /*
                    dereference = dict.dereference(id);
                    if (!string.ReferenceEquals(dereference, null))
                    {
                        return dereference;
                    }
                    else
                    {
                    }
                    */
                }
            }

            var returnValue = id + " (unresolved in scope : " + scope + ")";
            Debug.WriteLine("Warning dereference: " + returnValue);
            return returnValue; 

            //return ExceptionManager.createString(id, new Exception("Dictionary could not dereference key: " + id + " in scope " + scope));
        }


    }

}
