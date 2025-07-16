namespace _4n6MorkReader
{
    public class Alias
    {
        public string ValueRef { get; set; }
        public string Id { get; set; }
        public string Value { get; set; }
        public string RefId { get; set; }

        public Alias(string refId, string id, string value, string valueref) : base()
        {
            RefId = refId;
            Id = id;
            Value = value;
            ValueRef = valueref;
        }

    }
}
