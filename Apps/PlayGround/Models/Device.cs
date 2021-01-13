namespace PlayGround.Models
{
    public class Device
    {
        public string Address { get; }
        public string Name { get; }

        public Device(string address, string name)
        {
            Address = address;
            Name = name;
        }
    }
}