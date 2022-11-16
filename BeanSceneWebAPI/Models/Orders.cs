using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;

namespace BeanSCeneWebAPI.Models
{
    public class Orders
    {
        [BsonId]
        [BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)]
        public string _id { get; set; }
        public string id { get; set; }
        public string TableId { get; set; }
        public Table Table { get; set; }
        public string[] ItemIds { get; set; }
        public Item[] Items { get; set; }      
        public string[] Dierary { get; set; }
        public string[] Requests { get; set; }
        public BsonDateTime Date { get; set; }
        public BsonDateTime Time { get; set; }
        public string Status { get; set; }      
        public string Notes { get; set; }
    }
}