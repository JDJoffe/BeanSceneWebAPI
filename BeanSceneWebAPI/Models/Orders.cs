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
        //public string name { get; set; }
        public string Table { get; set; }
        public BsonDocument Items { get; set; }   
        public BsonDocument Dietary { get; set; }
        public BsonDocument Requests { get; set; }
        public string Date { get; set; }
        public string Time { get; set; }
        public string Status { get; set; }      
        public string Notes { get; set; }
    }
}