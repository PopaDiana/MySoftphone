namespace MySoftphone.UI.Model
{
    internal class Caller
    {
        public Caller(string name, string phone)
        {
            this.Name = name;
            this.PhoneNumber = phone;
        }

        public string Name { get; set; }

        public string PhoneNumber { get; set; }
    }
}