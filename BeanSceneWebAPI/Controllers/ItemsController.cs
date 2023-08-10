using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using BeanSCeneWebAPI.Models;
using Newtonsoft.Json;
using System.Text;
using Newtonsoft.Json.Linq;
using System.Web;
using System.Web.Helpers;
using System.Drawing.Drawing2D;
using System.Web.Http.Cors;

namespace BeanSCeneWebAPI.Controllers
{
    [BasicAuthentication]
    [EnableCors(origins:"*",headers:"*",methods:"*")]
    public class ItemsController : ApiController
    {
        MongoClient client;
        string dbName;
        string connString;
        public ItemsController()
        {
            // capture connection string of name BeanSceneConn
            connString = ConfigurationManager.ConnectionStrings["BeanSceneConn"].ConnectionString;

            client = new MongoClient(connString);

            dbName = MongoUrl.Create(connString).DatabaseName;

        }
        /// <summary>
        /// method used to get the Items
        /// </summary>
        /// <returns></returns>

        // GET api/<controller>
        public HttpResponseMessage Get()
        {
            var collection = client.GetDatabase(dbName).GetCollection<Item>("Items").AsQueryable();
            string jsonResult = JsonConvert.SerializeObject(collection);

            var response = Request.CreateResponse(HttpStatusCode.OK);
            response.Content = new StringContent(jsonResult, Encoding.UTF8, "application/json");
            return response;
        }
        /// <summary>
        /// method used to get Items based on name
        /// </summary>
        /// <param name="value"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        [Route("api/Items/{value}/{name}")]
        public HttpResponseMessage Get(string value, string name)
        {
            //gets all the Items
            var collection = client.GetDatabase(dbName).GetCollection<Item>("Items");

            var filteredResult = collection.AsQueryable().Where(x => x.name.ToLower().Contains(name)).ToList();

            string jsonResult = JsonConvert.SerializeObject(filteredResult);


            var response = Request.CreateResponse(HttpStatusCode.OK);
            response.Content = new StringContent(jsonResult, Encoding.UTF8, "application/json");

            return response;
        }

        // GET api/<controller>/5
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<controller>
        //{name}/{price}/{stock}/{description}/{brand}/{category}/{imageUrl}
        [Route("api/Items/{name}/{description}/{dietary}/{price}/{category}/{imageUrl}/{availability}")]
        public HttpResponseMessage Post(string name, string description, string dietary, string price, string category, string imageUrl, string availability)
        {
            try
            {
                Item i = new Item();             
                i.name = name;               
                i.description = description; 
                i.dietary = dietary;
                i.price = price;
                i.category = category;
                i.thumbnail = imageUrl;
                i.availability = availability;

                // returns the total count of items
                int lastItemId =  client.GetDatabase(dbName).GetCollection<Item>("Items").AsQueryable().Count();
                // returns the last item's id and parses to int
                
                //int.TryParse(client.GetDatabase(dbName).GetCollection<Item>("Items").AsQueryable().Last().id,out int lastItemId);
               
                i.id = (lastItemId+1).ToString();

                client.GetDatabase(dbName).GetCollection<Item>("Items").InsertOne(i);


                var response = Request.CreateResponse(HttpStatusCode.OK);

                var jObject = new JObject();
                response.Content = new StringContent(jObject.ToString(), Encoding.UTF8, "application/json");



                return response;

            }
            catch (Exception ex)
            {

                var response = Request.CreateResponse(HttpStatusCode.BadRequest);

                var jObject = new JObject();
                response.Content = new StringContent(jObject.ToString(), Encoding.UTF8, "application/json");



                return response;
            }
        }
        /// <summary>
        /// method used to add new Item to db
        /// method takes obj as param
        /// </summary>
        /// <param name="i"></param>
        /// <returns></returns>
        //adding a Item by passing Item object
        // POST api/<controller>
        [Route("api/Items/addItem")]
        public HttpResponseMessage addItem([FromBody] Item i)
        {
            try
            {

                int lastItemId = client.GetDatabase(dbName).GetCollection<Item>("Items").AsQueryable().Count();

                i.id = (lastItemId + 1).ToString();

                client.GetDatabase(dbName).GetCollection<Item>("Items").InsertOne(i);


                var response = Request.CreateResponse(HttpStatusCode.OK);

                var jObject = new JObject();
                response.Content = new StringContent(jObject.ToString(), Encoding.UTF8, "application/json");

                return response;
            }

            catch (Exception ex)
            {
                var response = Request.CreateResponse(HttpStatusCode.BadRequest);

                var jObject = new JObject();
                response.Content = new StringContent(jObject.ToString(), Encoding.UTF8, "application/json");

                return response;
            }
        }

        // PUT api/<controller>/5
        //{name}/{price}/{stock}/{description}/{brand}/{category}/{imageUrl}
        //add description between stock and imageurl
        //[Route("api/Items/{id}")]
        //public HttpResponseMessage Put(string id, string name, string price, string stock, string description, string brand, string category, string imageUrl)
        //{
        //    try
        //    {
        //        Item i = new Item();
        //        i.id = id;
        //        i.name = name;
        //        i.price = price;
        //        i.stock = stock;
        //        i.description = description;
        //        i.brand = brand;
        //        i.category = category;
        //        i.thumbnail = imageUrl;

        //        var filter = Builders<Item>.Filter.Eq("id", i.id);
        //        var update = Builders<Item>.Update.Set("name", i.name).Set("price", i.price).Set("stock", i.stock).Set("description", i.description).Set("brand", i.brand).Set("category", i.category).Set("thumbnail", i.thumbnail);

        //        client.GetDatabase(dbName).GetCollection<Item>("Items").UpdateOne(filter, update);

        //        var response = Request.CreateResponse(HttpStatusCode.OK);

        //        var jObject = new JObject();
        //        response.Content = new StringContent(jObject.ToString(), Encoding.UTF8, "application/json");



        //        return response;

        //    }
        //    catch (Exception ex)
        //    {

        //        var response = Request.CreateResponse(HttpStatusCode.BadRequest);

        //        var jObject = new JObject();
        //        response.Content = new StringContent(jObject.ToString(), Encoding.UTF8, "application/json");



        //        return response;
        //    }
        //}
        /// <summary>
        /// method used to update existing Item in db
        /// method takes all details related to Item that can be updated
        /// </summary>
        /// <param name="id"></param>
        /// <param name="name"></param>
        /// <param name="price"></param>
        /// <param name="description"></param>
        /// <param name="dietary"></param>
        /// <param name="imageUrl"></param>
        /// <param name="availability"></param>
        /// <returns></returns>
        [Route("api/Items/{id}/{name}/{description}/{dietary}/{price}/{category}/{imageUrl}/{availability}")]
        public HttpResponseMessage Put(string id, string name,  string description, string dietary,string price,string category, string imageUrl, string availability)
        {

            try
            {
                Item i = new Item();
                i.id = id;
                i.name = name;                
                i.description = description;
                i.dietary = dietary;
                i.price = price;
                i.category = category;
                i.thumbnail = imageUrl;
                i.availability = availability;



                var filter = Builders<Item>.Filter.Eq("id", i.id);
                var update = Builders<Item>.Update.Set("name", i.name).Set("description", i.description).Set("dietary", i.dietary).Set("price", i.price).Set("category", i.category).Set("thumbnail", i.thumbnail).Set("availability", i.availability);

                client.GetDatabase(dbName).GetCollection<Item>("Items").UpdateOne(filter, update);

                var response = Request.CreateResponse(HttpStatusCode.OK);

                var jObject = new JObject();
                response.Content = new StringContent(jObject.ToString(), Encoding.UTF8, "application/json");

                return response;

            }
            catch (Exception ex)
            {
                var response = Request.CreateResponse(HttpStatusCode.BadRequest);

                var jObject = new JObject();
                response.Content = new StringContent(jObject.ToString(), Encoding.UTF8, "application/json");

                return response;
            }
        }


        //updating Item by passing Item object
        //[Route("api/Items/updateItem")]
        //public HttpResponseMessage updateItem([FromBody] Item i)
        //{

        //    try
        //    {

        //        var filter = Builders<Item>.Filter.Eq("id", i.id);
        //        var update = Builders<Item>.Update.Set("name", i.name).Set("description", i.description).Set("dietary", i.dietary).Set("price", i.price).Set("category", i.category).Set("thumbnail", i.thumbnail).Set("availability", i.availability);

        //        client.GetDatabase(dbName).GetCollection<Item>("Items").UpdateOne(filter, update);

        //        var response = Request.CreateResponse(HttpStatusCode.OK);

        //        var jObject = new JObject();
        //        response.Content = new StringContent(jObject.ToString(), Encoding.UTF8, "application/json");

        //        return response;

        //    }
        //    catch (Exception ex)
        //    {
        //        var response = Request.CreateResponse(HttpStatusCode.BadRequest);

        //        var jObject = new JObject();
        //        response.Content = new StringContent(jObject.ToString(), Encoding.UTF8, "application/json");

        //        return response;
        //    }
        //}

        /// <summary>
        /// method used to delete Item from db
        /// takes ID
        /// </summary>
        /// <param name="i"></param>
        /// <returns></returns>
        // DELETE api/<controller>/5
        [Route("api/Items/{id}")]
        public HttpResponseMessage Delete(int id)
        {
            try
            {
                var collection = client.GetDatabase(dbName).GetCollection<Item>("Items");

                var filter = Builders<Item>.Filter.Eq("id",id);

                collection.DeleteOne(filter);

                var response = Request.CreateResponse(HttpStatusCode.OK);

                var jObject = new JObject();
                response.Content = new StringContent(jObject.ToString(), Encoding.UTF8, "application/json");



                return response;
            }
            catch (Exception ex)
            {

                var response = Request.CreateResponse(HttpStatusCode.BadRequest);

                var jObject = new JObject();
                response.Content = new StringContent(jObject.ToString(), Encoding.UTF8, "application/json");



                return response;
            }
        }
    }
}