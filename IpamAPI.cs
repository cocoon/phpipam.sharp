using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq; // System.Xml and System.Xml.Linq needs to be added to references
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Web; // System.Web needs to be added to references
using System.Web.Script.Serialization; // System.Web.Extensions needs to be added to references

namespace phpipam
{
    /*
     * 
     *  Example
     *  
        use phpipam;
        
        public static string URL = @"http://127.0.0.1/phpipam/api/";
        public static string AppID = "MyAppName";
        public static string AppCode = "48c3fe2b4e43d4578456abede1e2e40d";
     
        ipam = new IpamAPI();
        ipam.AppCode = AppCode;
        ipam.AppID = AppID;
        ipam.URL = URL;
        ipam.Load();
    */

    public class IpamAPI
    {
        private IDictionary<String, String> _store;
        
        private string _url;
        private string _appid;
        private string _appcode;

        private bool _isLoaded = false;

        public IpamAPI()
        {
            _store = new Dictionary<String, String>();
        }

        private String GetOrDefault(String propertyName)
        {
            if (_store.ContainsKey(propertyName))
            {
                return _store[propertyName];
            }

            return String.Empty;
        }

        public string URL
        {
            get { return GetOrDefault("URL"); }
            set { _store["URL"] = value; }
        }

        public string AppID
        {
            get { return GetOrDefault("_appid"); }
            set { _store["_appid"] = value; }
        }

        public string AppCode
        {
            get { return GetOrDefault("_appcode"); }
            set { _store["_appcode"] = value; }
        }

        public bool IsLoaded
        {
            get { return _isLoaded; }
        }


        public List<APIitem> APIitems
        {
            get { return _apiitems(); }
        }
        
        private List<APIitem> _apiitems()
        {
            List<APIitem> ReturnList = new List<APIitem>();

            APIitem NewItem = new APIitem();

            NewItem.Text = "GetSectionsAll";
            NewItem.Value = "GetSectionsAll";
            ReturnList.Add(NewItem);

            NewItem = new APIitem();
            NewItem.Text = "GetSubnetsFromSection(int SectionID)";
            NewItem.Value = "GetSubnetsFromSection";
            ReturnList.Add(NewItem);

            return ReturnList;
        }

        public class APIitem
        {
            public string Text { get; set; }
            public object Value { get; set; }

            public override string ToString()
            {
                return Text;
            }
        }

        private T JSON2Object<T>(string jsonstring)
        {
            string jsonInput = jsonstring;
            JavaScriptSerializer jsonSerializer = new JavaScriptSerializer();
            var obj = jsonSerializer.Deserialize<T>(jsonInput);
            return obj;
        }

        public Classes.phpipamSections Convert2Sections(string jsonstring)
        {
            Classes.phpipamSections Sections = new Classes.phpipamSections();
            Sections = JSON2Object<Classes.phpipamSections>(jsonstring);
            return Sections;
        }

        public Classes.phpipamSubnets Convert2Subnets(string jsonstring)
        {
            Classes.phpipamSubnets Subnets = new Classes.phpipamSubnets();
            Subnets = JSON2Object<Classes.phpipamSubnets>(jsonstring);
            return Subnets; 
        }


        public static readonly IpamAPI Empty = new IpamAPI();

        public Boolean HasData()
        {
            return !Empty.Equals(this);
        }

        public Boolean IsEmpty()
        {
            return Empty.Equals(this);
        }

        public Boolean HasAllData()
        {
            return _store.Any(x => !String.IsNullOrWhiteSpace(x.Value));
        }

        public Boolean HasNoData()
        {
            return _store.All(x => String.IsNullOrWhiteSpace(x.Value));
        }   



        public bool Load()
        {
            bool RetVal = false;

            if (!this.HasAllData())
            {
                Debug.WriteLine("Not all parameter set, missing: URL or AppID or AppCode!");
                return RetVal;
            }

            this._isLoaded = true;
            RetVal = true;
            return RetVal;
        }



        public string GetSectionsAll()
        {
            string RequestURL = null;
            string Response = null;

            IDictionary<string, string> req = new Dictionary<string, string>();
            req["controller"] = "sections";
            req["action"] = "read";
            req["all"] = "true";

            Response = SendPOST(RequestURL, req);
            
            return Response;
        }
        
        
        public string GetSubnetsFromSection(int SectionID)
        {
            //string RequestURL = "&sectionId=" + SectionID.ToString();
            string RequestURL = null;
            string Response = null;

            /*
                $req['controller'] 	= "subnets";
                $req['action']		= "read";
                $req['format']		= "ip";

                # set id
                if(!isset($_REQUEST['sectionId'])) 	{ $req['sectionId'] = 1; }
                else 								{ $req['sectionId'] = $_REQUEST['sectionId']; }
            */


            IDictionary<string, string> req = new Dictionary<string, string>();
            req["controller"] = "subnets";
            req["action"] = "read";
            req["format"] = "ip";
            req["sectionId"] = "1"; 

            Response = SendPOST(RequestURL, req);

            return Response;
        }


        public string SendPOST(string URLReqest, IDictionary<string, string> POSTdata)
        {
            try
            {
                return sendPOST(URLReqest, POSTdata);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
                return null;
            }
        }

        private string sendPOST(string URLReqest, IDictionary<string, string> req)
        {
            /* request
             *
             * # commands
                $req['controller'] 	= "sections";
                $req['action']		= "read";
                $req['all']			= true;
            */

            /*
            IDictionary<string, string> req = new Dictionary<string, string>();
            req["controller"]   = "sections";
            req["action"]       = "read";
            req["all"]			= "true";
            */

            if (URLReqest != null && URLReqest.Length > 0)
            {
                this.URL += "?" + URLReqest;
            }
            Debug.WriteLine("URL is: " + URL);

            //string json = serializer.Serialize(new { controller =  req["controller"], action = req["action"], all = req["all"] });
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            Dictionary<string, string> dataToSerialize = req.Keys.ToDictionary(p => p.ToString(), p => req[p]);
            string json = serializer.Serialize(dataToSerialize);

            HttpWebRequest httpWReq = (HttpWebRequest)WebRequest.Create(URL);

            ASCIIEncoding encoding = new ASCIIEncoding();



            //encrypt the request parameters
            string enc_request = encrypt(json, AppCode, AppID);

            //decrypt the encrypted string (only for demonstration how to decode)
            //string dec_request = DecryptIt(enc_request, System.Text.ASCIIEncoding.ASCII.GetBytes(AppCode), System.Text.ASCIIEncoding.ASCII.GetBytes(AppID));
            //MessageBox.Show(dec_request);

            //create the params array, which will
            //be the POST parameters
            //$params = array();
            //$params['enc_request'] = $enc_request;
            //$params['app_id'] = $this->_app_id;

            IDictionary<string, string> POSTParams = new Dictionary<string, string>();
            POSTParams["enc_request"] = enc_request;
            POSTParams["app_id"] = AppID;

            string POSTString = dictionaryToPostString(POSTParams);

            byte[] data = encoding.GetBytes(POSTString);

            httpWReq.Method = "POST";
            httpWReq.ContentType = "application/x-www-form-urlencoded";
            httpWReq.ContentLength = data.Length;

            using (Stream stream = httpWReq.GetRequestStream())
            {
                stream.Write(data, 0, data.Length);
            }

            HttpWebResponse response = (HttpWebResponse)httpWReq.GetResponse();

            string responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();

            return responseString;
        }

        public static string dictionaryToPostString(IDictionary<string, string> postVariables)
        {
            string postString = "";
            foreach (KeyValuePair<string, string> pair in postVariables)
            {
                postString += HttpUtility.UrlEncode(pair.Key) + "=" + HttpUtility.UrlEncode(pair.Value) + "&";
            }

            return postString;
        }


        static public string base64_encode(string toEncode)
        {
            byte[] toEncodeAsBytes = System.Text.ASCIIEncoding.ASCII.GetBytes(toEncode);
            string returnValue = System.Convert.ToBase64String(toEncodeAsBytes);
            return returnValue;
        }


        public static string encrypt(string encryptionString, string Key, string IV)
        {
            byte[] clearTextBytes = Encoding.UTF8.GetBytes(encryptionString);

            SymmetricAlgorithm rijn = SymmetricAlgorithm.Create();
            rijn.Mode = CipherMode.ECB;
            rijn.Padding = PaddingMode.Zeros;
            rijn.BlockSize = 256;

            MemoryStream ms = new MemoryStream();
            byte[] rgbIV = Encoding.ASCII.GetBytes(IV);

            byte[] key = Encoding.ASCII.GetBytes(Key);
            CryptoStream cs = new CryptoStream(ms, rijn.CreateEncryptor(key, rgbIV), CryptoStreamMode.Write);

            cs.Write(clearTextBytes, 0, clearTextBytes.Length);

            cs.Close();

            return Convert.ToBase64String(ms.ToArray());
        }

        private static String DecryptIt(String s, byte[] key, byte[] IV)
        {
            String result;

            RijndaelManaged rijn = new RijndaelManaged();
            rijn.Mode = CipherMode.ECB;
            rijn.Padding = PaddingMode.Zeros;
            rijn.BlockSize = 256;

            using (MemoryStream msDecrypt = new MemoryStream(Convert.FromBase64String(s)))
            {
                using (ICryptoTransform decryptor = rijn.CreateDecryptor(key, IV))
                {
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader swDecrypt = new StreamReader(csDecrypt))
                        {
                            result = swDecrypt.ReadToEnd();
                        }
                    }
                }
            }
            rijn.Clear();

            return result;
        }
    }
}
