using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BeanSceneWebAPI.Models
{
    internal class Reports
    {
        [BsonId]
        [BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)]
        public string _id { get; set; }
        public string id { get; set; }
        public string date { get; set; }
        public string orders { get; set; }
        public string revenue { get; set; }
    }
}