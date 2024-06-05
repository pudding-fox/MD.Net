namespace MD.Net
{
    public class Device : IDevice
    {
        public Device(string name)
        {
            this.Name = name;
        }

        public string Name { get; private set; }

        public static IDevice None
        {
            get
            {
                return new Device(string.Empty);
            }
        }
    }
}
