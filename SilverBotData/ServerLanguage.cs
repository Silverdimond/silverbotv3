namespace SilverBotData
{
    public class ServerLanguage
    {
        public int Id { get; set; }

        /// <summary>
        /// two letter code as defined in 639-1 https://en.wikipedia.org/wiki/List_of_ISO_639-1_codes
        /// </summary>
        public string ISO_Code { get; set; }

        /// <summary>
        /// The guild id
        /// </summary>
        public ulong GuildID { get; set; }
    }
}