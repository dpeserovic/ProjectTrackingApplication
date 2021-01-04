using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Linq;

namespace ProjectTrackingApplication
{
    class Program
    {
        static int tableWidth = 195;
        static void PrintLine()
        {
            Console.WriteLine(new string('-', tableWidth));
        }

        static void PrintRow(params string[] columns)
        {
            int width = (tableWidth - columns.Length) / columns.Length;
            string row = "|";

            foreach (string column in columns)
            {
                row += AlignCentre(column, width) + "|";
            }

            Console.WriteLine(row);
        }

        static string AlignCentre(string text, int width)
        {
            text = text.Length > width ? text.Substring(0, width - 3) + "..." : text;

            if (string.IsNullOrEmpty(text))
            {
                return new string(' ', width);
            }
            else
            {
                return text.PadRight(width - (width - text.Length) / 2).PadLeft(width);
            }
        }
        public struct User
        {
            public string username;
            public string password;
            public User(string sUsername, string sPassword)
            {
                username = sUsername;
                password = sPassword;
            }
        }
        public struct Project
        {
            public string name;
            public string carrier;
            public int value;
            public string status;
            public string director;
            public string location;
            public Project(string sName, string sCarrier, int nValue, string sStatus, string sDirector, string sLocation)
            {
                name = sName;
                carrier = sCarrier;
                value = nValue;
                status = sStatus;
                director = sDirector;
                location = sLocation;
            }
        }
        public static List<User> GetAllUsers()
        {
            List<User> lUsers = new List<User>();
            string sXml = "";
            StreamReader oSr = new StreamReader("config.xml");
            using (oSr)
            {
                sXml = oSr.ReadToEnd();
            }
            XmlDocument oXml = new XmlDocument();
            oXml.LoadXml(sXml);
            XmlNodeList oNodes = oXml.SelectNodes("//users/user");
            foreach (XmlNode oNode in oNodes)
            {
                string sUsername = oNode.Attributes["username"].Value;
                string sPassword = oNode.Attributes["password"].Value;
                lUsers.Add
                    (
                        new User(sUsername, sPassword)
                    );
            }
            return lUsers;
        }
        public static bool LogIn(string username, string password)
        {
            bool bUserExists = false;
            List<User> lUsers = GetAllUsers();
            for (int i = 0; i < lUsers.Count(); i++)
            {
                if ((username == lUsers[i].username) && (password == lUsers[i].password))
                {
                    bUserExists = true;
                }
            }
            return bUserExists;
        }
        public static void GetMenu()
        {
            Console.WriteLine("1 - Show all projects");
            Console.WriteLine("2 - Group all projects");
            Console.WriteLine("3 - Add project");
            Console.WriteLine("4 - Delete project");
            Console.WriteLine("5 - Sign out");
            int n = int.Parse(Console.ReadLine());
            switch (n)
            {
                case 1:
                    ShowAllProjects();
                    break;
                case 2:
                    GroupAllProjects();
                    break;
                case 3:
                    AddProject();
                    break;
                case 4:
                    DeleteProject();
                    break;
                case 5:
                    SignOut();
                    break;
                default:
                    Console.WriteLine("Wrong number!");
                    break;
            }
        }
        public static List<Project> GetAllProjects()
        {
            List<Project> lProjects = new List<Project>();
            string sXml = "";
            StreamReader oSr = new StreamReader("projects.xml");
            using (oSr)
            {
                sXml = oSr.ReadToEnd();
            }
            XmlDocument oXml = new XmlDocument();
            oXml.LoadXml(sXml);
            XmlNodeList oNodes = oXml.SelectNodes("//projects/project");
            foreach (XmlNode oNode in oNodes)
            {
                string sName = oNode.Attributes["name"].Value;
                string sCarrier = oNode.Attributes["carrier"].Value;
                int nValue = Convert.ToInt32(oNode.Attributes["value"].Value);
                string sStatus = oNode.Attributes["status"].Value;
                string sDirector = oNode.Attributes["director"].Value;
                string sLocation = oNode.Attributes["location"].Value;
                lProjects.Add
                    (
                       new Project(sName, sCarrier, nValue, sStatus, sDirector, sLocation)
                    );
            }
            return lProjects;
        }
        public static void ShowAllProjects()
        {
            List<Project> lProjects = GetAllProjects();
            PrintLine();
            PrintRow("NAME", "CARRIER", "VALUE", "STATUS", "DIRECTOR", "LOCATION");
            PrintLine();
            for (int i = 0; i < lProjects.Count(); i++)
            {
                lProjects.Sort((x, y) => string.Compare(x.name, y.name));
                PrintRow(
                lProjects[i].name,
                lProjects[i].carrier,
                lProjects[i].value + " kn",
                lProjects[i].status,
                lProjects[i].director,
                lProjects[i].location);
                PrintLine();

            }
        }
        public static void GroupAllProjects()
        {
            List<Project> lProjects = GetAllProjects();
            Console.WriteLine("Group by location or director: ");
            Console.WriteLine("1 - Location");
            Console.WriteLine("2 - Director");
            int a = int.Parse(Console.ReadLine());
            if (a == 1)
            {
                PrintLine();
                PrintRow("LOCATION", "CARRIER", "VALUE", "STATUS", "DIRECTOR", "NAME");
                PrintLine();
                for (int i = 0; i < lProjects.Count(); i++)
                {
                    lProjects.Sort((x, y) => string.Compare(x.location, y.location));
                    PrintRow(lProjects[i].location,
                    lProjects[i].carrier,
                    lProjects[i].value + " kn",
                    lProjects[i].status,
                    lProjects[i].director,
                    lProjects[i].name);
                    PrintLine();
                }
            }
            else if (a == 2)
            {
                PrintLine();
                PrintRow("DIRECTOR", "CARRIER", "VALUE", "STATUS", "LOCATION", "NAME");
                PrintLine();
                for (int i = 0; i < lProjects.Count(); i++)
                {
                    lProjects.Sort((x, y) => string.Compare(x.director, y.director));
                    PrintRow(lProjects[i].director,
                    lProjects[i].carrier,
                    lProjects[i].value + " kn",
                    lProjects[i].status,
                    lProjects[i].location,
                    lProjects[i].name);
                    PrintLine();
                }
            }
            else
            {
                Console.WriteLine("Wrong number!");
            }
        }
        public static void AddProject()
        {
            XDocument doc = XDocument.Load("projects.xml");
            XElement root = new XElement("project");
            Console.WriteLine("Name: ");
            root.Add(new XAttribute("name", Console.ReadLine()));
            Console.WriteLine("Carrier: ");
            root.Add(new XAttribute("carrier", Console.ReadLine()));
            Console.WriteLine("Value (whole number in HRK): ");
            root.Add(new XAttribute("value", Console.ReadLine()));
            Console.WriteLine("Status: ");
            root.Add(new XAttribute("status", Console.ReadLine()));
            Console.WriteLine("Director: ");
            root.Add(new XAttribute("director", Console.ReadLine()));
            Console.WriteLine("Location: ");
            root.Add(new XAttribute("location", Console.ReadLine()));
            doc.Element("projects").Add(root);
            doc.Save("projects.xml");
            Console.WriteLine("You have successfully added a project!");
        }
        public static void DeleteProject()
        {
            List<Project> lProjects = GetAllProjects();
            Console.WriteLine("Enter the name of the project you want to delete: ");
            string name = Console.ReadLine();
            string sName = "";
            var xDoc = XDocument.Load("projects.xml");
            for (int i = 0; i < lProjects.Count(); i++)
            {
                if (name == lProjects[i].name)
                {
                    xDoc.Descendants("project").First(c => c.Attribute("name").Value == name).Remove();
                    xDoc.Save("projects.xml");
                    name = sName;
                }

            }
            if (name == sName)
                Console.WriteLine("You have successfully deleted the project!");
            else
                Console.WriteLine("The entered project name does not exist, pay attention to uppercase letters, lowercase letters and spaces!");
        }
        public static void SignOut()
        {
            Environment.Exit(0);
        }
        static void Main(string[] args)
        {
            string pathConfig = "config.xml";
            using (FileStream fs1 = File.Open(pathConfig, FileMode.Open, FileAccess.ReadWrite, FileShare.None))
            {
                Console.WriteLine("config.xml file has been successfully loaded!");
            }
            string pathProjects = "projects.xml";
            using (FileStream fs2 = File.Open(pathProjects, FileMode.Open, FileAccess.ReadWrite, FileShare.None))
            {
                Console.WriteLine("projects.xml file has been successfully loaded!");
            }
            Console.WriteLine();
            string sUsername;
            string sPassword;
            Console.WriteLine("Enter username:");
            sUsername = Console.ReadLine();
            Console.WriteLine("Enter password:");
            sPassword = Console.ReadLine();
            while (true)
            {
                if (LogIn(sUsername, sPassword))
                {
                    Console.WriteLine();
                    GetMenu();
                }
                else
                {
                    Console.WriteLine("Wrong credentials, try again!");
                    Console.WriteLine("Username: ");
                    sUsername = Console.ReadLine();
                    Console.WriteLine("Password: ");
                    sPassword = Console.ReadLine();
                }
            }
        }
    }
}