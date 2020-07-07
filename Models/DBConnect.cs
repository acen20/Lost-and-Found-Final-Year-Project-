using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebApplication5.DataAccessLayer;
using System.Data;
using System.Globalization;
using WebApplication5.Models.AccountViewModels;
using System.IO;

namespace WebApplication5.Models
{
    public class DBConnect
    {
        private DBConnect() { }
        public static DBConnect instance = null;

        public static DBConnect getFController()
        {
            if (instance == null)
                instance = new DBConnect();
            return instance;

        }

        public string[] checkLogin(string email, string password, string role)
        {
            Database s = new Database();
            DataSet set = s.checkLogin(email, password, role);
            if (set.Tables["myTable"].Rows.Count == 1)
            {
                string Email = set.Tables["myTable"].Rows[0]["Email"].ToString();
                string Role = set.Tables["myTable"].Rows[0]["Role"].ToString();
                string Organization = set.Tables["myTable"].Rows[0]["Org"].ToString();
                return new string[3] { Email, Role, Organization };
            }
            else
                return null;
        }

        public List<string> getAllCategories()
        {
            Database s = new Database();
            DataSet set = s.getAllCategories();
            if (set.Tables["myTable"].Rows.Count > 0)
            {
                List<string> categories = new List<string>();
                foreach (DataRow r in set.Tables["myTable"].Rows)
                    categories.Add(r["name"].ToString());
                return categories;
            }
            else
                return new List<string>() { "" };
        }

        public int getHighestId()
        {
            Database s = new Database();
            DataTable dt = s.getHighestId().Tables["myTable"];
            if (dt.Rows[0]["id"] is int)
                return int.Parse(dt.Rows[0]["id"].ToString());
            else
                return 0;
        }

        public int insertNewAd(AdViewModel model, string postedby)
        {
            Database s = new Database();
            model = extractData(model);
            string latlng = model.Location.Trim('(', ')');
            string[] trimmed = latlng.Split(',');
            string lat = trimmed[0];
            string lng = trimmed[1].Remove(0, 1);
            if(model.Organization!=null)
            {
                model.Organization = s.getOrganizationID(model.Organization, model.City).Tables["myTable"].Rows[0]["id"].ToString();
            }
            int response = s.insertNewAd(model.Title, model.Description, model.Category, model.Color, 
                DateTime.Parse(model.Date).ToString("dd-MM-yyyy"),
            lat, lng, model.ImageName, model.Organization,
            model.Reward, model.Type, model.City, postedby);
            return response;
        }


        public List<string> getAllOrganizations()
        {
            Database s = new Database();
            DataSet set = s.getAllOrganizations();
            if (set.Tables["myTable"].Rows.Count > 0)
            {
                List<string> orgs = new List<string>();
                foreach (DataRow r in set.Tables["myTable"].Rows)
                    orgs.Add(r["Name"].ToString());
                return orgs;
            }
            else
                return new List<string>()
                {
                    "",
                };
        }

        public string getUserType(string user)
        {
            Database s = new Database();
            DataTable dt = s.getUserType(user).Tables["myTable"];
            if (dt.Rows[0]["Role"].ToString() != null)
                return dt.Rows[0]["Role"].ToString();
            else
                return " ";
        }

        public List<AdViewModel> getAllAds(string type)
        {
            Database s = new Database();
            List<AdViewModel> ads;
            DataTable dt = s.getAllAds(type).Tables["myTable"];
            if (dt != null)
            {
                ads = new List<AdViewModel>();
                foreach (DataRow r in dt.Rows)
                {
                    AdViewModel ad = createData(r);
                    ads.Add(ad);
                }

                return ads;
            }

            else
                return null;
        }

        public List<string> getAllCities()
        {
            Database s = new Database();
            DataSet set = s.getAllCities();
            if (set.Tables["myTable"].Rows.Count > 0)
            {
                List<string> cities = new List<string>();
                foreach (DataRow r in set.Tables["myTable"].Rows)
                    cities.Add(r["name"].ToString());
                return cities;
            }
            else
                return new List<string>() { "" };
        }


        public List<AdViewModel> getAllAds(string type, string organization, string category, string color, string city, string q)
        {
            Database s = new Database();
            List<AdViewModel> ads;
            DataTable dt = s.getAllAds(type, organization, category, color, city, q).Tables["myTable"];
            if (dt != null)
            {
                ads = new List<AdViewModel>();
                foreach (DataRow r in dt.Rows)
                {
                    AdViewModel ad = createData(r);
                    ads.Add(ad);
                }

                return ads;
            }
            else
                return null;
        }


        public AdViewModel createData(DataRow r)
        {
            AdViewModel ad = new AdViewModel();
            ad.Title = r["Title"].ToString();
            ad.Description = r["description"].ToString();
            ad.ImageName = r["ImageName"].ToString();
            ad.Reward = int.Parse(r["Reward"].ToString());
            ad.City = r["City"].ToString();
            ad.Date = r["date"].ToString();
            ad.Id = int.Parse(r["id"].ToString());
            ad.Location = "{lat:" + r["locationlat"].ToString() + ", lng:" + r["locationlong"].ToString() + "}";
            ad.Type = r["type"].ToString();
            ad.PostedBy = r["postedby"].ToString();
            ad.Category = r["category"].ToString();
            ad.Color = r["color"].ToString();
            ad.Organization = r["org"].ToString();
            return ad;
        }

        public AdViewModel getAd(int id)
        {
            Database s = new Database();
            if (s.getAd(id).Tables["myTable"].Rows.Count > 0)
            {
                AdViewModel ad = createData(s.getAd(id).Tables["myTable"].Rows[0]);
                return ad;
            }
            else
                return null;
        }

        public RegisterViewModel getUserDetails(string email)
        {
            Database s = new Database();
            DataTable dt = s.getUserDetails(email).Tables["myTable"];
            if (dt != null)
            {
                RegisterViewModel user = new RegisterViewModel();
                user.Name = dt.Rows[0]["name"].ToString();
                user.Phone = dt.Rows[0]["phone"].ToString();
                user.Organization = dt.Rows[0]["org"].ToString();
                user.Role = dt.Rows[0]["Role"].ToString();
                return user;
            }
            else
                return null;
        }

        public int updateAd(int id, AdViewModel model)
        {
            Database s = new Database();
            model = extractData(model);
            s.updateAd(id, model.Title, model.Description, model.Category, model.Color,
            DateTime.Parse(model.Date).ToString("dd-MM-yyyy"), model.ImageName, model.Organization, model.Reward, model.Type, model.City);
            return 0;
        }

        public AdViewModel extractData(AdViewModel model)
        {

            if (model.Organization == null)
                model.Organization = "-";
            if (model.Reward.ToString() == "0")
                model.Reward = 0;
            return model;
        }

        public string getImageName(int id)
        {
            Database s = new Database();
            DataTable dt = s.getImageName(id).Tables["myTable"];
            if (dt.Rows.Count > 0)
            {
                return dt.Rows[0]["ImageName"].ToString();
            }
            else
                return null;
        }

        public int deleteAd(int id)
        {
            Database s = new Database();
            int response = s.deleteAd(id);
            return response;
        }

        public int registerUser(RegisterViewModel reg)
        {
            Database s = new Database();
            int? orgID=null;
            if(reg.Organization!=null)
            {
                DataTable dt=s.checkOrganization(reg.Organization, reg.City).Tables["myTable"];
                if(dt.Rows.Count>0 && dt.Rows[0]!=null)
                {
                    if(reg.Organization.ToLower()==dt.Rows[0]["Name"].ToString().ToLower())
                        return 3;
                }

                dt = s.checkEmail(reg.Email).Tables["myTable"];
                if (dt.Rows.Count > 0 && dt.Rows[0] != null)
                {
                    if (reg.Email.ToLower() == dt.Rows[0]["Email"].ToString().ToLower())
                        return 2;
                }
                orgID= registerOrganization(reg);
            }

            int response = s.registerUser(reg.Name, reg.Email, reg.Password, reg.Phone, reg.Role, orgID);
            return response;
        }

        public List<AdViewModel> getAds(AdViewModel model, string type)
        {
            Database s = new Database();
            DataTable dt = s.getAds(model.Id, model.Category, model.Date, 
                model.Organization, type, model.City).Tables["myTable"];
            if (dt.Rows.Count > 0)
            {
                List<AdViewModel> list = new List<AdViewModel>();
                foreach (DataRow r in dt.Rows)
                {
                    AdViewModel ad = createData(r);
                    list.Add(ad);
                }

                return list;
            }
            else
                return null;
        }

        public DataSet getDictionary()
        {
            Database s = new Database();
            return s.getDictionary();
        }


        public List<AdViewModel> getSimilarAds(int id , string type)
        {
            Database db = new Database();
            AdViewModel model = getAd(id);
            Similarity s = new Similarity();
            s.currentAd = model;
            int response = s.getAllAds(type);
            if (response == -1)
                return null;
            else
                return s.getMatches();
        }

        public RegisterViewModel getUser(string email)
        {
            Database s = new Database();
            DataTable dt = s.getUser(email).Tables["myTable"];
            if (dt.Rows.Count == 1)
            {
                return new RegisterViewModel()
                {
                    Name = dt.Rows[0]["name"].ToString(),
                    Email = email,
                    Phone = dt.Rows[0]["phone"].ToString(),
                    Role = dt.Rows[0]["role"].ToString(),
                    Organization = dt.Rows[0]["organization"].ToString(),
                    City = dt.Rows[0]["city"].ToString()
                };
            }

            else
                return null;
        }

        public int registerOrganization(RegisterViewModel reg)
        {
            Database s = new Database();
            DataTable dt=s.registerOrganization(reg.Organization, reg.City).Tables["myTable"];
            return int.Parse(dt.Rows[0]["id"].ToString());
        }


        public bool Blocked(string email, AdViewModel ad)
        {
            if (ad.Organization == "-")
                return false;

            Database s = new Database();
            DataTable dt = s.getOrganizationID(ad.Organization, ad.City).Tables["myTable"];
            int orgID = int.Parse(dt.Rows[0]["id"].ToString());

            dt = s.checkBlocked(email, orgID).Tables["myTable"];
            if (dt.Rows.Count > 0 && dt.Rows[0] != null)
            {
                return true;
            }
            else
                return false;
        }

        public List<RegisterViewModel> getAllRequests()
        {
            Database s = new Database();
            DataSet set = s.getAllRequests();
            if (set.Tables["myTable"].Rows.Count > 0)
            {
                List<RegisterViewModel> requests = new List<RegisterViewModel>();
                foreach (DataRow r in set.Tables["myTable"].Rows)
                {
                    RegisterViewModel usr = new RegisterViewModel();
                    usr.Name = r["Name"].ToString();
                    usr.Organization = r["OrgName"].ToString();
                    usr.City = r["City"].ToString();
                    usr.Phone = r["Phone"].ToString();
                    usr.id = int.Parse(r["id"].ToString());
                    requests.Add(usr);
                }
                return requests;
            }
            else
                return null;
        }

        public int approveRequest(int id)
        {
            Database s = new Database();
            return s.approveRequest(id);
        }

        public int deleteRequest(int id)
        {
            Database s = new Database();
            return s.deleteRequest(id);
        }

        public List<string> getOrganization(string email)
        {
            Database s = new Database();
            DataTable dt=s.getOrganization(email).Tables["myTable"];
            return new List<string>
            {
                dt.Rows[0]["Name"].ToString(),
                dt.Rows[0]["City"].ToString()
            };
        }

    }
}