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

namespace BeanSCeneWebAPI.Controllers
{
    [BasicAuthentication]
    //[EnableCors(origins:"*",headers:"*",methods:"*")]
    public class ProductsController : ApiController
    {
        MongoClient client;
        string dbName;
        string connString;
        public ProductsController()
        {
            // capture connection string of name BeanSceneConn
            connString = ConfigurationManager.ConnectionStrings["BeanSceneConn"].ConnectionString;

            client = new MongoClient(connString);

            dbName = MongoUrl.Create(connString).DatabaseName;

        }
        /// <summary>
        /// method used to get the products
        /// </summary>
        /// <returns></returns>

        // GET api/<controller>
        public HttpResponseMessage Get()
        {
            var collection = client.GetDatabase(dbName).GetCollection<Product>("Products").AsQueryable();
            string jsonResult = JsonConvert.SerializeObject(collection);

            var response = Request.CreateResponse(HttpStatusCode.OK);
            response.Content = new StringContent(jsonResult, Encoding.UTF8, "application/json");
            return response;
        }
        /// <summary>
        /// method used to get products based on name
        /// </summary>
        /// <param name="value"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        [Route("api/Products/{value}/{name}")]
        public HttpResponseMessage Get(string value, string name)
        {
            //gets all the products
            var collection = client.GetDatabase(dbName).GetCollection<Product>("Products");

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
        [Route("api/Products/{name}/{price}/{stock}/{description}/{brand}/{category}/{imageUrl}")]
        public HttpResponseMessage Post( string name, string price, string stock, string description, string brand, string category, string imageUrl)
        {
            try
            {
                Product p = new Product();             
                p.name = name;
                p.price = price;
                p.stock = stock;
                p.description = description;
                p.brand = brand;
                p.category = category;
                p.thumbnail = imageUrl;

                // returns the total count of items
                int lastProductId =  client.GetDatabase(dbName).GetCollection<Product>("Products").AsQueryable().Count();
                // returns the last item's id and parses to int
                
                //int.TryParse(client.GetDatabase(dbName).GetCollection<Product>("Products").AsQueryable().Last().id,out int lastProductId);
               
                p.id = (lastProductId+1).ToString();

                client.GetDatabase(dbName).GetCollection<Product>("Products").InsertOne(p);


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
        /// method used to add new product to db
        /// method takes obj as param
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        //adding a product by passing product object
        // POST api/<controller>
        [Route("api/Products/addProduct")]
        public HttpResponseMessage addProduct([FromBody] Product p)
        {
            try
            {

                int lastProductId = client.GetDatabase(dbName).GetCollection<Product>("Products").AsQueryable().Count();

                p.id = (lastProductId + 1).ToString();

                client.GetDatabase(dbName).GetCollection<Product>("Products").InsertOne(p);


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
        //[Route("api/Products/{id}")]
        //public HttpResponseMessage Put(string id, string name, string price, string stock, string description, string brand, string category, string imageUrl)
        //{
        //    try
        //    {
        //        Product p = new Product();
        //        p.id = id;
        //        p.name = name;
        //        p.price = price;
        //        p.stock = stock;
        //        p.description = description;
        //        p.brand = brand;
        //        p.category = category;
        //        p.thumbnail = imageUrl;

        //        var filter = Builders<Product>.Filter.Eq("id", p.id);
        //        var update = Builders<Product>.Update.Set("name", p.name).Set("price", p.price).Set("stock", p.stock).Set("description", p.description).Set("brand", p.brand).Set("category", p.category).Set("thumbnail", p.thumbnail);

        //        client.GetDatabase(dbName).GetCollection<Product>("Products").UpdateOne(filter, update);

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
        /// method used to update existing product in db
        /// method takes all details related to product that can be updated
        /// </summary>
        /// <param name="id"></param>
        /// <param name="name"></param>
        /// <param name="price"></param>
        /// <param name="stock"></param>
        /// <param name="description"></param>
        /// <param name="imageUrl"></param>
        /// <returns></returns>
        [Route("api/Products/{id}/{name}/{price}/{stock}/{description}/{imageUrl}")]
        public HttpResponseMessage Put(string id, string name, string price, string stock, string description, string imageUrl)
        {

            try
            {
                Product p = new Product();
                p.id = id;
                p.name = name;
                p.price = price;
                p.stock = stock;
                p.description = description;
                p.thumbnail = imageUrl;



                var filter = Builders<Product>.Filter.Eq("id", p.id);
                var update = Builders<Product>.Update.Set("name", p.name).Set("price", p.price).Set("stock", p.stock).Set("description", p.description).Set("thumbnail", p.thumbnail);

                client.GetDatabase(dbName).GetCollection<Product>("Products").UpdateOne(filter, update);

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

      
        //updating product by passing product object
        [Route("api/Products/updateProduct")]
        public HttpResponseMessage updateProduct([FromBody] Product p)
        {

            try
            {

                var filter = Builders<Product>.Filter.Eq("id", p.id);
                var update = Builders<Product>.Update.Set("name", p.name).Set("price", p.price).Set("stock", p.stock).Set("description", p.description).Set("thumbnail", p.thumbnail);

                client.GetDatabase(dbName).GetCollection<Product>("Products").UpdateOne(filter, update);

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
        /// method used to delete product from db
        /// takes ID
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        // DELETE api/<controller>/5
        [Route("api/Products/{id}")]
        public HttpResponseMessage Delete(int id)
        {
            try
            {
                var collection = client.GetDatabase(dbName).GetCollection<Product>("Products");

                var filter = Builders<Product>.Filter.Eq("id",id);

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