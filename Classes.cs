/**
* phpipam.sharp: C# Library for phpipam API (http://phpipam.net/)
*
* Copyright (C) Patrick Schlicher 2013-2014
*
* Licensed under the Apache License, Version 2.0 (the "License");
* you may not use this file except in compliance with the License.
* You may obtain a copy of the License at
*
* http://www.apache.org/licenses/LICENSE-2.0
*
* Unless required by applicable law or agreed to in writing, software
* distributed under the License is distributed on an "AS IS" BASIS,
* WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
* See the License for the specific language governing permissions and
* limitations under the License.
*
* phpipam Classes
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Script.Serialization;

namespace phpipam
{
    public class Classes
    {
        private static T JSON2Object<T>(string jsonstring)
        {
            string jsonInput = jsonstring;
            JavaScriptSerializer jsonSerializer = new JavaScriptSerializer();
            var obj = jsonSerializer.Deserialize<T>(jsonInput);
            return obj;
        }


        public class phpipamSections
        {
            public bool success { get; set; }
            public List<phpipamSection> data { get; set; }

            public phpipamSections() { }

            public phpipamSections(string jsonstring)
            {
                phpipamSections sections = new phpipamSections();
                sections = JSON2Object<phpipamSections>(jsonstring);
                this.success = sections.success;
                this.data = sections.data;
            }
        }

        public class phpipamSection
        {
            public string id { get; set; }
            public string name { get; set; }
            public string description { get; set; }
            public string permissions { get; set; }
            public string strictMode { get; set; }
            public string subnetOrdering { get; set; }
            public string order { get; set; }
            public string editDate { get; set; }
            public string showVLAN { get; set; }
            public string showVRF { get; set; }
        }

        public class phpipamSubnets
        {
            public bool success { get; set; }
            public List<phpipamSubnet> data { get; set; }

            public phpipamSubnets() { }

            public phpipamSubnets(string jsonstring)
            {
                phpipamSubnets subnets = new phpipamSubnets();
                subnets = JSON2Object<phpipamSubnets>(jsonstring);
                this.success = subnets.success;
                this.data = subnets.data;
            }
        }

        public class phpipamSubnet
        {
            public string id { get; set; }
            public string subnet { get; set; }
            public string mask { get; set; }
            public string sectionId { get; set; }
            public string description { get; set; }
            public string vrfId { get; set; }
            public string masterSubnetId { get; set; }
            public string allowRequests { get; set; }
            public string vlanId { get; set; }
            public string showName { get; set; }
            public string permissions { get; set; }
            public string pingSubnet { get; set; }
            public string isFolder { get; set; }
            public object editDate { get; set; }
        }

    }
}
