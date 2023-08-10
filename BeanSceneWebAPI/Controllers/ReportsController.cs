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
using BeanSceneWebAPI.Models;

namespace BeanSCeneWebAPI.Controllers
{
    [BasicAuthentication]
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class ReportsController : ApiController
    {
        MongoClient client;
        string dbName;
        string connString;
        public ReportsController()
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
            var collection = client.GetDatabase(dbName).GetCollection<Reports>("Reports").AsQueryable();
            string jsonResult = JsonConvert.SerializeObject(collection);

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
       /// <summary>
       /// 
       /// </summary>
       /// <param name="date"></param>
       /// <param name="orders"></param>
       /// <param name="revenue"></param>
       /// <returns></returns>
        [Route("api/Reports/{date}/{orders}/{revenue}")]
        public HttpResponseMessage Post(string date, string orders, string revenue)
        {
            try
            {
                Reports r = new Reports();
                r.date = date;
                r.orders = orders;
                r.revenue = revenue;

                // returns the total count of items
                int lastReportId = client.GetDatabase(dbName).GetCollection<Reports>("Reports").AsQueryable().Count();
                // returns the last item's id and parses to int

                //int.TryParse(client.GetDatabase(dbName).GetCollection<Item>("Items").AsQueryable().Last().id,out int lastItemId);

                r.id = (lastReportId + 1).ToString();

                client.GetDatabase(dbName).GetCollection<Reports>("Reports").InsertOne(r);


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
        /// <param name="date"></param>
        /// <param name="orders"></param>
        /// <param name="revenue"></param>
        /// <returns></returns>
        [Route("api/Reports/{id}/{date}/{orders}/{revenue}")]
        public HttpResponseMessage Put(string id, string date, string orders, string revenue)
        {
            try
            {
                Reports r = new Reports();
                r.id = id;
                r.date = date;
                r.orders = orders;
                r.revenue = revenue;

                var filter = Builders<Reports>.Filter.Eq("id", r.id);
                var update = Builders<Reports>.Update.Set("date", r.date).Set("orders", r.orders).Set("revenue", r.revenue);

                client.GetDatabase(dbName).GetCollection<Reports>("Reports").UpdateOne(filter, update);

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
        /// method used to delete Item from db
        /// takes ID
        /// </summary>
        /// <param name="i"></param>
        /// <returns></returns>
        // DELETE api/<controller>/5
        [Route("api/Reports/{id}")]
        public HttpResponseMessage Delete(int id)
        {
            try
            {
                var collection = client.GetDatabase(dbName).GetCollection<Reports>("Reports");

                var filter = Builders<Reports>.Filter.Eq("id", id);

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