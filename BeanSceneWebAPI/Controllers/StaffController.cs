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
using BCrypt.Net;


namespace BeanSCeneWebAPI.Controllers
{
    [BasicAuthentication]
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class StaffController : ApiController
    {
        MongoClient client;
        string dbName;
        string connString;
        public StaffController()
        {
            // capture connection string of name BeanSceneConn
            connString = ConfigurationManager.ConnectionStrings["BeanSceneConn"].ConnectionString;

            client = new MongoClient(connString);

            dbName = MongoUrl.Create(connString).DatabaseName;
        }
        // GET api/<controller>
        public HttpResponseMessage Get()
        {
            var collection = client.GetDatabase(dbName).GetCollection<Staff>("Staff").AsQueryable();
            string jsonResult = JsonConvert.SerializeObject(collection);

            var response = Request.CreateResponse(HttpStatusCode.OK);
            response.Content = new StringContent(jsonResult, Encoding.UTF8, "application/json");
            return response;
        }

        [Route("api/Staff/{username}/{password}")]
        public HttpResponseMessage Get (string username,string password)
        {
            #region
            // without hashing
            //var collection = client.GetDatabase(dbName).GetCollection<Staff>("Staff");

            //var result = collection.Find(x => x.username == username && x.password == password).FirstOrDefault();
            //string jsonResult = JsonConvert.SerializeObject(result);
            //var response = Request.CreateResponse(HttpStatusCode.OK);
            //response.Content = new StringContent(jsonResult, Encoding.UTF8, "application/json");
            //return response;
            #endregion
            // with hashing
            var collection = client.GetDatabase(dbName).GetCollection<Staff>("Staff");

            var result = collection.Find(x => x.username == username).FirstOrDefault();
            if (result != null)
            {
                try
                {

                    bool verified = BCrypt.Net.BCrypt.Verify(password, result.password);
                    if (!verified)
                    {
                        result = null;

                    }
                }
                catch (Exception ex)
                {

                    result = null;
                }
            }
            string jsonResult = JsonConvert.SerializeObject(result);
            var response = Request.CreateResponse(HttpStatusCode.OK);
            response.Content = new StringContent(jsonResult, Encoding.UTF8, "application/json");
            
            return response;

        }
        //[Route("api/Staff/{value}/{name}")]
        //public HttpResponseMessage Get(string value, string name)
        //{
        //    // gets all products
        //    var collection = client.GetDatabase(dbName).GetCollection<Staff>("Staff");

        //    var filteredResult = collection.AsQueryable().Where(x => x.firstname.ToLower().Contains(name)).ToList();
        //    string jsonResult = JsonConvert.SerializeObject(filteredResult);
        //    var response = Request.CreateResponse(HttpStatusCode.OK);
        //    response.Content = new StringContent(jsonResult, Encoding.UTF8, "application/json");
        //    return response;
        //}
        // GET api/<controller>/5
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<controller>
        [Route("api/Staff/{firstname}/{lastname}/{username}/{password}/{email}/{role}")]
        public HttpResponseMessage Post(string firstname, string lastname, string username, string password, string email, string role)
        {
            try
            {
                Staff s = new Staff();
                s.firstname = firstname;
                s.lastname = lastname;
                s.username = username;
                string hashPassword = BCrypt.Net.BCrypt.HashPassword(password);
                s.password = hashPassword;
                s.email = email;
                s.role = role;

                // returns the total count of items
                int lastStaffId = client.GetDatabase(dbName).GetCollection<Staff>("Staff").AsQueryable().Count();
                // returns the last item's id and parses to int               
                //int.TryParse(client.GetDatabase(dbName).GetCollection<Staff>("Staff").AsQueryable().Last().id, out int lastStaffId);

                s.id = (lastStaffId + 1).ToString();

                client.GetDatabase(dbName).GetCollection<Staff>("Staff").InsertOne(s);


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
        //[Route("api/Staff/addStaff")]
        //public HttpResponseMessage addStaff([FromBody] Staff s)
        //{
        //    try
        //    {
        //        string hashPassword = BCrypt.Net.BCrypt.HashPassword(s.password);
        //        s.password = hashPassword;

        //        // returns the total count of items
        //        int lastStaffId = client.GetDatabase(dbName).GetCollection<Staff>("Staff").AsQueryable().Count();
        //        // returns the last item's id and parses to int               
        //        //int.TryParse(client.GetDatabase(dbName).GetCollection<Staff>("Staff").AsQueryable().Last().id, out int lastStaffId);

        //        s.id = (lastStaffId + 1).ToString();

        //        client.GetDatabase(dbName).GetCollection<Staff>("Staff").InsertOne(s);


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

        // PUT api/<controller>/5 
        [Route("api/Staff/{id}/{firstname}/{lastname}/{username}/{password}/{email}/{role}")]
        public HttpResponseMessage Put(string id, string firstname, string lastname, string username, string password, string email, string role)
        {
            try
            {
                Staff s = new Staff();
                s.id = id;
                s.firstname = firstname;
                s.lastname = lastname;
                s.username = username;
                string hashPassword = BCrypt.Net.BCrypt.HashPassword(password);
                s.password = hashPassword;
                s.email = email;
                s.role = role;

                var filter = Builders<Staff>.Filter.Eq("id", s.id);
                var update = Builders<Staff>.Update.Set("firstname", s.firstname).Set("lastname", s.lastname).Set("username", s.username).Set("password", s.password).Set("email", s.email).Set("role", s.role);

                client.GetDatabase(dbName).GetCollection<Staff>("Staff").UpdateOne(filter, update);

                var response = Request.CreateResponse(HttpStatusCode.OK);

                var jObject = new JObject();
               // response.Headers.Append('Access-Control-Allow-Origin', '*', );
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

        // DELETE api/<controller>/5
        [Route("api/Staff/{id}")]
        public HttpResponseMessage Delete(int id)
        {
            try
            {
                var collection = client.GetDatabase(dbName).GetCollection<Staff>("Staff");

                var filter = Builders<Staff>.Filter.Eq("id", id);

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