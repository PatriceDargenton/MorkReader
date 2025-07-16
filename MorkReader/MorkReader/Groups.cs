using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace _4n6MorkReader
{
    /// <summary>
	/// Parses the part of a Mork file containing Group definitions. A group is
	/// something like a transaction.
	/// 
	/// @author mhaller
	/// </summary>
	public class Groups
    {
        /// <summary>
        /// Internal container for all parsed groups </summary>
        private IList<Group> groups = new List<Group>();

        /// <summary>
        /// Parse the given String for Group definitions
        /// </summary>
        /// <param name="content">
        ///            the Mork content with Group definitions </param>
        public Groups(string content)
        {
            content = StringUtils.removeCommentLines(content);
            content = StringUtils.removeNewlines(content);
            //Pattern pattern = Pattern.compile("@\\$\\$\\{([0-9A-F]*)\\{@(.*)@\\$\\$\\}(\\x0001)\\}@");
            Match matcher = Regex.Match(content, "@\\$\\$\\{([0-9A-F]*)\\{@(.*)@\\$\\$\\}(\\x0001)\\}@");
            while ((matcher = matcher.NextMatch()).Success)
            {
                string transactionId1 = matcher.Groups[1].Value;
                string transactionContent = matcher.Groups[2].Value;
                // String transactionId2 = matcher.group(3);
                groups.Add(new Group(transactionId1, transactionContent));

            }
        }

        /// <summary>
        /// Returns the number of groups found in the content
        /// </summary>
        /// <returns> the number of groups </returns>
        public virtual int countGroups()
        {
            return groups.Count;
        }

        /// <summary>
        /// Return a Group by its position in the internal list.
        /// </summary>
        /// <param name="i">
        ///            the index position (This is NOT the Group ID!) </param>
        /// <returns> the Group </returns>
        public virtual Group getGroup(int i)
        {
            return groups[i];
        }

    }

}
