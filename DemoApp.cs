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
* Demo Application
*/
using phpipam;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Script.Serialization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace HTTP_POST
{
    /// <summary>
    /// IpamAPI Demo App 
    /// </summary>
    public partial class MainWindow : Window
    {
        public static string URL = @"http://127.0.0.1/api/";
        public static string AppID = "APP001";
        public static string AppCode = "bf9da2f8599a02824b2cf3cd3e3c0268";


        public IpamAPI ipam;

        public static List<string> APIMethodList;

        public static List<Classes.phpipamSection> CurrentSections;
        public static List<Classes.phpipamSubnet> CurrentSubnets;

        public MainWindow()
        {
            InitializeComponent();

            Startup();
        }

        private void Startup()
        {
            try
            {
                APIMethodList = GetMethods(typeof(IpamAPI));

                foreach (string Method in APIMethodList)
                {
                    TxtBox1.Text += Method + Environment.NewLine;
                }

                ipam = new IpamAPI();
                ipam.AppCode = AppCode;
                ipam.AppID = AppID;
                ipam.URL = URL;

                ipam.Load();

                ComboBox1.ItemsSource = ipam.APIitems;

            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
            }
        }

        private void Btn1_Click(object sender, RoutedEventArgs e)
        {
            IpamAPI.APIitem typeItem = (IpamAPI.APIitem)ComboBox1.SelectedItem;
            if (typeItem == null) return;
            string methodName = typeItem.Value.ToString();

            Type type = typeof(IpamAPI);
            //MessageBox.Show("type: " + "methodName: " + methodName);

            if (type != null)
            {
                MethodInfo methodInfo = type.GetMethod(methodName);
                if (methodInfo != null)
                {
                    object result = null;
                    ParameterInfo[] parameters = methodInfo.GetParameters();
                    //object classInstance = Activator.CreateInstance(type, null);
                    object classInstance = ipam;

                    if (parameters.Length == 0)
                    {
                        //MessageBox.Show("parameters.Length: " + parameters.Length.ToString());
                        result = methodInfo.Invoke(classInstance, null);
                    }
                    else
                    {
                        //if (parameters != null) MessageBox.Show("parameters.Length: " + parameters.Length.ToString());

                        object[] parametersArray = new object[] { Int32.Parse(TxtBoxInput.Text) };
                        result = methodInfo.Invoke(classInstance, parametersArray);
                    }

                    //if (result != null) MessageBox.Show(result.ToString());
                    if (result != null)
                    {
                        TxtBox2.Text = result.ToString();

                        string jsonInput = result.ToString();

                        if (methodName == "GetSectionsAll")
                        {
                            Classes.phpipamSections ThisSections = new Classes.phpipamSections(jsonInput);
                            if (ThisSections != null || ThisSections.data.Count > 0)
                            {
                                CurrentSections = ThisSections.data;
                                DataGrid1.ItemsSource = CurrentSections;

                                TxtBox1.Text = "";

                                foreach (Classes.phpipamSection section in ThisSections.data)
                                {
                                    TxtBox1.Text += section.name + Environment.NewLine;
                                }
                            }
                        }

                        if (methodName == "GetSubnetsFromSection")
                        {
                            Classes.phpipamSubnets ThisSubnets = new Classes.phpipamSubnets(jsonInput);
                            if (ThisSubnets != null || ThisSubnets.data.Count > 0)
                            {
                                CurrentSubnets = ThisSubnets.data;
                                DataGrid1.ItemsSource = CurrentSubnets;

                                TxtBox1.Text = "";

                                foreach (Classes.phpipamSubnet subnet in ThisSubnets.data)
                                {
                                    TxtBox1.Text += subnet.description + " (" + subnet.subnet + ") " + Environment.NewLine;
                                }
                            }
                        }
                    }
                }
            }
        }
        


        static List<string> GetMethods(Type type)
        {
            List<string> MethodList = new List<string>();
            
            foreach (var method in type.GetMethods())
            {
                var parameters = method.GetParameters();
                
                var parameterDescriptions = string.Join
                    (", ", method.GetParameters()
                                 .Select(x => x.ParameterType + " " + x.Name)
                                 .ToArray());

                Console.WriteLine("{0} {1} ({2})",
                                  method.ReturnType,
                                  method.Name,
                                  parameterDescriptions);

                MethodList.Add(method.Name);
            }

            return MethodList;
        }
    
    }

}
