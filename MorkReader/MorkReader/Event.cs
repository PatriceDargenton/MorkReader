using System;
using System.Runtime.Serialization;

namespace _4n6MorkReader
{
    public class Event : ISerializable
    {

        /**
         * SUID
         */
        private static long serialVersionUID = -953791935775686254L;

        /**
         * The type of the Event, one of {@link EventType}, must never be null
         */
        public EventType eventType;

        /**
         * A literal value associated with the event. Some events (of the
         * end-of-element events) contain values, others use <code>null</code>
         * here if no additional data is available.
         */
        public String value;

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            //throw new NotImplementedException();
        }
    }
}
