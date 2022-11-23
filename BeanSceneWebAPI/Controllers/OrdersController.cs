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
using System.Web.Http.Cors;
using System.Web;
using System.Drawing.Drawing2D;
using System.Web.Helpers;
using MongoDB.Bson;
using BeanSceneWebAPI.Models;

namespace BeanSCeneWebAPI.Controllers
{
    [BasicAuthentication]
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class OrdersController : ApiController
    {
        MongoClient client;
        string dbName;
        string connString;
        public OrdersController()
        {
            // capture connection string of name BeanSceneConn
            connString = ConfigurationManager.ConnectionStrings["BeanSceneConn"].ConnectionString;

            client = new MongoClient(connString);

            dbName = MongoUrl.Create(connString).DatabaseName;

        }
        /// <summary>
        /// method used to get the Orders
        /// </summary>
        /// <returns></returns>

        // GET api/<controller>
        public HttpResponseMessage Get()
        {
            var collection = client.GetDatabase(dbName).GetCollection<Orders>("Orders").AsQueryable();
            string jsonResult = JsonConvert.SerializeObject(collection);

            var response = Request.CreateResponse(HttpStatusCode.OK);
            response.Content = new StringContent(jsonResult, Encoding.UTF8, "application/json");
            return response;
        }
       /// <summary>
       /// 
       /// </summary>
       /// <param name="value"></param>
       /// <param name="Table"></param>
       /// <returns></returns>
        [Route("api/Orders/{value}/{Table}")]
        public HttpResponseMessage Get(string value, string Table)
        {
            //gets all the Orders
            var collection = client.GetDatabase(dbName).GetCollection<Orders>("Orders");

            var filteredResult = collection.AsQueryable().Where(x => x.Table.ToLower().Contains(Table)).ToList();

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
        [Route("api/Orders/{Table}/{Items}/{Dietary}/{Requests}/{Date}/{Time}/{Status}/{Notes}")]
        public HttpResponseMessage Post(string Table, string[] itemId, string[] dietary, string[] requests, string date, string time, string status, string notes)
        {
            try
            {
                Orders o = new Orders();
                o.Table = Table;
                for (int i = 0; i < itemId.Length; i++)
                {
                    o.Items[i] = itemId[i];
                }
                for (int i = 0; i < dietary.Length; i++)
                {
                    o.Dietary[i] = dietary[i];
                }
                for (int i = 0; i < requests.Length; i++)
                {
                    o.Requests[i] = requests[i];
                }
                o.Date = date;
                o.Time =  time;
                o.Status = status;
                o.Notes = notes;             
                
                // returns the total count of items
                int lastItemId = client.GetDatabase(dbName).GetCollection<Orders>("Orders").AsQueryable().Count();
                // returns the last item's id and parses to int

                //int.TryParse(client.GetDatabase(dbName).GetCollection<Orders>("Orders").AsQueryable().Last().id,out int lastItemId);

                o.id = (lastItemId + 1).ToString();

                client.GetDatabase(dbName).GetCollection<Orders>("Orders").InsertOne(o);


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
       /// 
       /// </summary>
       /// <param name="o"></param>
       /// <returns></returns>
        //adding a Orders by passing Orders object
        // POST api/<controller>
        [Route("api/Orders/addOrder")]
        public HttpResponseMessage addItem([FromBody] Orders o)
        {
            try
            {

                int lastItemId = client.GetDatabase(dbName).GetCollection<Orders>("Orders").AsQueryable().Count();

                o.id = (lastItemId + 1).ToString();

                client.GetDatabase(dbName).GetCollection<Orders>("Orders").InsertOne(o);


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
        //[Route("api/Orders/{id}")]
        //public HttpResponseMessage Put(string id, string name, string price, string stock, string description, string brand, string category, string imageUrl)
        //{
        //    try
        //    {
        //        Orders i = new Orders();
        //        i.id = id;
        //        i.name = name;
        //        i.price = price;
        //        i.stock = stock;
        //        i.description = description;
        //        i.brand = brand;
        //        i.category = category;
        //        i.thumbnail = imageUrl;

        //        var filter = Builders<Orders>.Filter.Eq("id", i.id);
        //        var update = Builders<Orders>.Update.Set("name", i.name).Set("price", i.price).Set("stock", i.stock).Set("description", i.description).Set("brand", i.brand).Set("category", i.category).Set("thumbnail", i.thumbnail);

        //        client.GetDatabase(dbName).GetCollection<Orders>("Orders").UpdateOne(filter, update);

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
       /// 
       /// </summary>
       /// <param name="id"></param>
       /// <param name="Table"></param>
       /// <param name="itemId"></param>
       /// <param name="dietary"></param>
       /// <param name="requests"></param>
       /// <param name="date"></param>
       /// <param name="time"></param>
       /// <param name="status"></param>
       /// <param name="notes"></param>
       /// <returns></returns>
        [Route("api/Orders/{Id}/{Table}/{Items}/{Dietary}/{Requests}/{Date}/{Time}/{Status}/{Notes}")]
        public HttpResponseMessage put(string id,string Table, string[] itemId, string[] dietary, string[] requests, string date, string time, string status, string notes)
        {
            try
            {
                Orders o = new Orders();
                o.id = id;
                o.Table = Table;
                for (int i = 0; i < itemId.Length; i++)
                {
                    o.Items[i] = itemId[i];
                }
                for (int i = 0; i < dietary.Length; i++)
                {
                    o.Dietary[i] = dietary[i];
                }
                for (int i = 0; i < requests.Length; i++)
                {
                    o.Requests[i] = requests[i];
                }
                o.Date = date;
                o.Time = time;
                o.Status = status;
                o.Notes = notes;



                var filter = Builders<Orders>.Filter.Eq("id", o.id);
                var update = Builders<Orders>.Update.Set("Table", o.Table).Set("Items", o.Items).Set("Dietary", o.Dietary).Set("Requests", o.Requests).Set("Date", o.Date).Set("Time", o.Time).Set("Status", o.Status).Set("Notes", o.Notes);

                client.GetDatabase(dbName).GetCollection<Orders>("Orders").UpdateOne(filter, update);

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


        //updating Orders by passing Orders object
        [Route("api/Orders/updateOrder")]
        public HttpResponseMessage updateItem([FromBody] Orders o)
        {

            try
            {

                var filter = Builders<Orders>.Filter.Eq("id", o.id);
                var update = Builders<Orders>.Update.Set("Table", o.Table).Set("Items", o.Items).Set("Dietary", o.Dietary).Set("Requests", o.Requests).Set("Date", o.Date).Set("Time", o.Time).Set("Status", o.Status).Set("Notes", o.Notes);

                client.GetDatabase(dbName).GetCollection<Orders>("Orders").UpdateOne(filter, update);

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
       /// 
       /// </summary>
       /// <param name="id"></param>
       /// <returns></returns>
        // DELETE api/<controller>/5
        [Route("api/Orders/{id}")]
        public HttpResponseMessage Delete(int id)
        {
            try
            {
                var collection = client.GetDatabase(dbName).GetCollection<Orders>("Orders");

                var filter = Builders<Orders>.Filter.Eq("id", id);

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