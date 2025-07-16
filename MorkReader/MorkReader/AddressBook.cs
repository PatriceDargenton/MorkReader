using System.Collections.Generic;
using System.IO;

namespace _4n6MorkReader
{
    /// <summary>
	/// An address book is a container for addresses loaded from a Mozilla
	/// Thunderbird address book, which is stored in the Mork file format.
	/// 
	/// @author mhaller
	/// </summary>
	public class AddressBook
    {

        /// <summary>
        /// Internal container for Addresses </summary>
        private readonly IList<Address> addresses = new List<Address>();
        private ExceptionHandler exceptionHandler;

        /// <summary>
        /// Loads a Mork database from the given input and parses it as being a
        /// Mozilla Thunderbird Address Book. The file is usually called abook.mab
        /// and is located in the Thunderbird user profile.
        /// 
        /// If additional address books are loaded into the same Address Book
        /// instance, the addresses get collected into the same address book.
        /// </summary>
        /// <param name="inputStream">
        ///            the stream to load the address book from. </param>
        //JAVA TO C# CONVERTER WARNING: 'final' parameters are ignored unless the option to convert to C# 7.2 'in' parameters is selected:
        //ORIGINAL LINE: public void load(final java.io.InputStream inputStream)
        public void load(Stream inputStream)
        {
            if (inputStream == null)
            {
                throw new System.ArgumentException("InputStream must not be null");
            }
            //JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
            //ORIGINAL LINE: final mork.MorkDocument morkDocument = new mork.MorkDocument(new java.io.InputStreamReader(inputStream), exceptionHandler);
            MorkDocument morkDocument = new MorkDocument(new StreamReader(inputStream), exceptionHandler);
            //foreach (Row row in morkDocument.Rows)
            //{
            //    //JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
            //    //ORIGINAL LINE: final Address address = new Address(row.getAliases());
            //    Address address = new Address(row.Aliases);
            //    addresses.Add(address);
            //}
            foreach (Table table in morkDocument.Tables)
            {
                foreach (Row row in table.Rows)
                {
                    if (row.getValue("DisplayName") != null)
                    {
                        //JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
                        //ORIGINAL LINE: final Address address = new Address(row.getAliases());
                        Address address = new Address(row.Aliases);
                        addresses.Add(address);
                    }
                }
            }
        }

        /// <summary>
        /// Returns an unmodifiable list of <seealso cref="Address"/>es.
        /// </summary>
        /// <returns> an unmodifiable list of <seealso cref="Address"/>es, might be empty, never
        ///         null. </returns>
        public IList<Address> Addresses
        {
            get
            {
                return addresses;
            }
        }

        public ExceptionHandler ExceptionHandler
        {
            set
            {
                this.exceptionHandler = value;
            }
        }

    }

}
