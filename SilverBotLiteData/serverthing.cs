using System;
using System.Collections.Generic;
using System.Text;
using LiteDB;
using SilverBotData;

namespace SilverBotLiteData
{
    public static class Serverthing
    {
        public static SilverBotData.Serverthing Getbyid(ulong id)
        {
            using LiteDatabase db = new LiteDatabase(@"Filename=serverdata.db; Connection=shared");
            ILiteCollection<SilverBotData.Serverthing> col = db.GetCollection<SilverBotData.Serverthing>();
            col.EnsureIndex(x => x.ServerId);
            return col.FindOne(x => x.ServerId == id);
        }

        public static int Removebyid(ulong channelid, ulong serverid)
        {
            using LiteDatabase db = new LiteDatabase(@"Filename=serverdata.db; Connection=shared");
            ILiteCollection<SilverBotData.Serverthing> col = db.GetCollection<SilverBotData.Serverthing>();
            col.EnsureIndex(x => x.ServerId);
            return col.DeleteMany(x => x.ChannelId == channelid && x.ServerId == serverid);
        }

        public static void Insert(SilverBotData.Serverthing thing)
        {
            using LiteDatabase db = new LiteDatabase(@"Filename=serverdata.db; Connection=shared");
            ILiteCollection<SilverBotData.Serverthing> col = db.GetCollection<SilverBotData.Serverthing>();
            col.Insert(thing);
        }
    }
}