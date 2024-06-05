namespace MD.Net
{
    public static class Settings
    {
        static Settings()
        {
            Threads = -1;
        }

        public static int Threads { get; set; }
    }
}
