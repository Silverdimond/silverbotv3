using System;
using System.Collections.Generic;

namespace SilverBotData
{
    public class ServerInfractions
    {/// <summary>
     /// needed so litedb doesnt take a dump
     /// </summary>
        public ulong Id { get; set; }

        /// <summary>
        /// the server id or as idiots call it guild
        /// </summary>
        public ulong Server_id { get; set; }

        /// <summary>
        /// List of infrations of users in a guild
        /// </summary>
        public List<Infraction> Infractions { get; set; }
    }

    public class Infraction
    {
        /// <summary>
        /// Id of the Infraction. Randomly generated
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// The id of the person that gave the infraction.
        /// </summary>
        public ulong Punisher_id { get; set; }

        /// <summary>
        /// The id of the person that got the infraction
        /// </summary>
        public ulong User_id { get; set; }

        /// <summary>
        /// The time. DUHHHH in utc for you cunts that dont use utc
        /// </summary>
        public DateTime Time { get; set; }

        /// <summary>
        /// The reason the person got punished.
        /// </summary>
        public string Reason { get; set; }

        /// <summary>
        /// The urls of attachents of the thing idk man i am not a programmer
        /// </summary>
        public List<string> Attachment_urls { get; set; }
    }
}