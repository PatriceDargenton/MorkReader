using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _4n6MorkReader
{
    /// <summary>
    /// Represents a single address in a Mozilla Thunderbird address book.
    /// 
    /// The following list of properties is known in Thunderbird 1.5.0.9
    /// <ul>
    /// <li>FirstName</li>
    /// <li>LastName</li>
    /// <li>PrimaryEmail</li>
    /// <li>DisplayName</li>
    /// <li>Company</li>
    /// 
    /// <li>ListName</li>
    /// <li>ListNickName</li>
    /// <li>ListDescription</li>
    /// <li>ListTotalAddresses</li>
    /// <li>LowercaseListName</li>
    /// <li>ns:addrbk:db:table:kind:deleted</li>
    /// <li>ns:addrbk:db:row:scope:card:all</li>
    /// <li>ns:addrbk:db:row:scope:list:all</li>
    /// <li>ns:addrbk:db:row:scope:data:all</li>
    /// <li>PhoneticFirstName</li>
    /// <li>PhoneticLastName</li>
    /// <li>NickName</li>
    /// <li>LowercasePrimaryEmail</li>
    /// <li>SecondEmail</li>
    /// <li>DefaultEmail</li>
    /// <li>CardType</li>
    /// <li>PreferMailFormat</li>
    /// <li>PopularityIndex</li>
    /// <li>WorkPhone</li>
    /// <li>HomePhone</li>
    /// <li>FaxNumber</li>
    /// <li>PagerNumber</li>
    /// <li>CellularNumber</li>
    /// <li>WorkPhoneType</li>
    /// <li>HomePhoneType</li>
    /// <li>FaxNumberType</li>
    /// <li>PagerNumberType</li>
    /// <li>CellularNumberType</li>
    /// <li>HomeAddress</li>
    /// <li>HomeAddress2</li>
    /// <li>HomeCity</li>
    /// <li>HomeState</li>
    /// <li>HomeZipCode</li>
    /// <li>HomeCountry</li>
    /// <li>WorkAddress</li>
    /// <li>WorkAddress2</li>
    /// <li>WorkCity</li>
    /// <li>WorkState</li>
    /// <li>WorkZipCode</li>
    /// <li>WorkCountry</li>
    /// <li>JobTitle</li>
    /// <li>Department</li>
    /// <li>_AimScreenName</li>
    /// <li>AnniversaryYear</li>
    /// <li>AnniversaryMonth</li>
    /// <li>AnniversaryDay</li>
    /// <li>SpouseName</li>
    /// <li>FamilyName</li>
    /// <li>DefaultAddress</li>
    /// <li>Category</li>
    /// <li>WebPage1</li>
    /// <li>WebPage2</li>
    /// <li>BirthYear</li>
    /// <li>BirthMonth</li>
    /// <li>BirthDay</li>
    /// </ul>
    /// 
    /// @author mhaller
    /// </summary>
    public class Address
    {
        private readonly IDictionary<string, Alias> aliases;

        public Address(IDictionary<string, Alias> map)
        {
            this.aliases = map;
        }

        public virtual string get(string id)
        {
            try
            {
                Alias a = aliases[id];
                if (a != null)
                {
                    return a.Value;
                }
            }
            catch (Exception ex)
            {
                
            }
            return null;
        }

        public string FirstName => get("FirstName");
        public string PrimaryEmail => get("PrimaryEmail");
        public string LastName => get("LastName");
        public string DisplayName => get("DisplayName");
        public string Company => get("Company");
        public string PhotoURI => get("PhotoURI");

        public string ListName => get("ListName");
        public string ListNickName => get("ListNickName");
        public string ListDescription => get("ListDescription");
        public string ListTotalAddresses => get("ListTotalAddresses");
        public string LowercaseListName => get("LowercaseListName");
        public string ns_addrbk_db_table_kind_deleted => get("ns:addrbk:db:table:kind:deleted");
        public string ns_addrbk_db_row_scope_card_all => get("ns:addrbk:db:row:scope:card:all");
        public string ns_addrbk_db_row_scope_list_all => get("ns:addrbk:db:row:scope:list:all");
        public string ns_addrbk_db_row_scope_data_all => get("ns:addrbk:db:row:scope:data:all");
        public string PhoneticFirstName => get("PhoneticFirstName");
        public string PhoneticLastName => get("PhoneticLastName");
        public string NickName => get("NickName");
        public string LowercasePrimaryEmail => get("LowercasePrimaryEmail");
        public string SecondEmail => get("SecondEmail");
        public string DefaultEmail => get("DefaultEmail");
        public string CardType => get("CardType");
        public string PreferMailFormat => get("PreferMailFormat");
        public string PopularityIndex => get("PopularityIndex");
        public string WorkPhone => get("WorkPhone");
        public string HomePhone => get("HomePhone");
        public string FaxNumber => get("FaxNumber");
        public string PagerNumber => get("PagerNumber");
        public string CellularNumber => get("CellularNumber");
        public string WorkPhoneType => get("WorkPhoneType");
        public string HomePhoneType => get("HomePhoneType");
        public string FaxNumberType => get("FaxNumberType");
        public string PagerNumberType => get("PagerNumberType");
        public string CellularNumberType => get("CellularNumberType");
        public string HomeAddress => get("HomeAddress");
        public string HomeAddress2 => get("HomeAddress2");
        public string HomeCity => get("HomeCity");
        public string HomeState => get("HomeState");
        public string HomeZipCode => get("HomeZipCode");
        public string HomeCountry => get("HomeCountry");
        public string WorkAddress => get("WorkAddress");
        public string WorkAddress2 => get("WorkAddress2");
        public string WorkCity => get("WorkCity");
        public string WorkState => get("WorkState");
        public string WorkZipCode => get("WorkZipCode");
        public string WorkCountry => get("WorkCountry");
        public string JobTitle => get("JobTitle");
        public string Department => get("Department");
        public string AimScreenName => get("_AimScreenName");
        public string AnniversaryYear => get("AnniversaryYear");
        public string AnniversaryMonth => get("AnniversaryMonth");
        public string AnniversaryDay => get("AnniversaryDay");
        public string SpouseName => get("SpouseName");
        public string FamilyName => get("FamilyName");
        public string DefaultAddress => get("DefaultAddress");
        public string Category => get("Category");
        public string WebPage1 => get("WebPage1");
        public string WebPage2 => get("WebPage2");
        public string BirthYear => get("BirthYear");
        public string BirthMonth => get("BirthMonth");
        public string BirthDay => get("BirthDay");

        public string _Yahoo => get("_Yahoo");
        public string _MSN => get("_MSN");
        public string _ICQ => get("_ICQ");
        public string _IRC => get("_IRC");
        public string _JabberId => get("_JabberId");
        public string _Skype => get("_Skype");
        public string _QQ => get("_QQ");
        public string _GoogleTalk => get("_GoogleTalk");
    }

}


