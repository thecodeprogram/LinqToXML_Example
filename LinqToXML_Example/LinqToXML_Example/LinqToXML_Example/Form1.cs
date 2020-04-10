using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Linq;

namespace LinqToXML_Example
{
    public partial class Form1 : Form
    {

        public const string xml_path = "employees.xml";
        public const string root_Element = "Employees";
        public const string second_element = "Employee";

        public const string elementName_ID = "ID";
        public const string elementName_Name = "Name";
        public const string elementName_Surname = "Surname";
        public const string elementName_Role = "Role";
        public const string elementName_Salary = "Salary";


        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            //Load the employees at first to see who recorded.
            loadEmployees();
        }

        //First I will write a function which will create the XML file if there is no XML file
        public void createXMLFile()
        {
            //In this function we will use the XmlTextWriter to create an XML file.
            XmlTextWriter xmlWriter = new XmlTextWriter(xml_path, UTF8Encoding.UTF8);
            xmlWriter.WriteStartDocument();
            //First, Here we will create it and we write the created date inside the file as a comment.
            xmlWriter.WriteComment("XML File Created at first time at : " + DateTime.Now.ToString());
            //We must write the root element of the XML firstly
            //If we do not the program will not run and throw an exception 
            //which specifies there is no root element. 
            xmlWriter.WriteStartElement(root_Element);
            //And we finis the writing the XML file
            xmlWriter.WriteEndDocument();
            xmlWriter.Close();
        }

        //We write a method to add a new element into the XML file under the main element with secon layer element.
        public void addNewEmployee()
        {
            //To make anything with the XML file, first we have to check the file exist
            //and if not exist, we must create the XML file with required root element
            if (File.Exists(xml_path) == false)
                createXMLFile();

            //And then load the xml document.
            XDocument document = XDocument.Load(xml_path);
            if (document != null)
            {
                //After loading the element we must select the root element which we will work on.
                XElement main_element = document.Element(root_Element);
                //One of the most important thing of  a record is RECORD ID value.
                //XML has no such thing Auto Identity, so we will count how many record exist in the XML
                //and we will increase the element count for new record.
                int employeeID;
                try { employeeID = document.Descendants(second_element).Max(id => (int)id.Attribute(elementName_ID)); }
                catch { employeeID = 0; }
                employeeID++;
                //And then we will create new Element under selected Root Element.
                //This element layer will be second layer and all employee elements will be this element
                //So we create all employee elements with this tag.
                XElement element = new XElement(new XElement(second_element,
                    new XElement(elementName_Name, txtName.Text),
                    new XElement(elementName_Surname, txtSurname.Text),
                    new XElement(elementName_Role, txtRole.Text),
                    new XElement(elementName_Salary, txtSalary.Text)));
                //We will set the element id as Attribute of the element.
                element.SetAttributeValue(elementName_ID, employeeID);
                //You can add comments above of all new elements with XComment.
                element.AddFirst(new XComment("New Employee ID : " + employeeID));
                //And add it under thhe main_element
                main_element.Add(element);
                //After all process do not forget to save the document.
                document.Save(xml_path);

                MessageBox.Show("Employee added successfully");

                txtID.Text = "";
                txtName.Text = "";
                txtSurname.Text = "";
                txtRole.Text = "";
                txtSalary.Text = "";
                //To show the user the updates, load the records again.
                loadEmployees();
            }
        }

        public void updateEmployee()
        {
            //First load the xml document.
            XDocument document = XDocument.Load(xml_path);
            if (document != null)
            {
                //First select the element with the specified ID value to update.
                //Here we created an XElement from the document's root element. And then we select the second layer element
                //with where condition we specified the selected second layer element must have a Attribute value that we specified in txtID
                //and then select appropraite employee record in here.
                XElement element = (from f in document.Element(root_Element).Elements(second_element)
                                      where f.Attribute(elementName_ID).Value == txtID.Text
                                      select f).SingleOrDefault();
                //If selected element is not null, I mean there is a record with specified ID value
                if (element != null)
                {
                    //Start to update elements' value from textboxes
                    element.SetElementValue(elementName_Name, txtName.Text);
                    element.SetElementValue(elementName_Surname, txtSurname.Text);
                    element.SetElementValue(elementName_Role, txtRole.Text);
                    element.SetElementValue(elementName_Salary, txtSalary.Text);
                    //Last do not forget to save the XML file.
                    document.Save(xml_path);

                    MessageBox.Show("Employee updated successfully.");

                    txtID.Text = "";
                    txtName.Text = "";
                    txtSurname.Text = "";
                    txtRole.Text = "";
                    txtSalary.Text = "";
                    //Loading the entire records will be good.
                    loadEmployees();

                }
            }
        }

        public void loadEmployees()
        {
            //First clear the listview to load records
            lstEployees.Items.Clear();
            //and we check the is exist
            if (File.Exists(xml_path))
            {
                //if the file is exist load the XML document
                XDocument document = XDocument.Load(xml_path);
                if (document != null)
                {
                    //We load  all second layer elements with XElement type in an Enumerable under Root Element
                    IEnumerable<XElement> employees = from f in document.Element(root_Element).Elements(second_element)
                                                 select f;
                    //If there is a record
                    if (employees.Count() > 0)
                    {
                        //Load all records to the listview
                        foreach (XElement emp in employees) 
                        {
                            ListViewItem item1 = new ListViewItem(emp.Attribute(elementName_ID).Value);
                            item1.SubItems.Add(emp.Element(elementName_Name).Value);
                            item1.SubItems.Add(emp.Element(elementName_Surname).Value);
                            item1.SubItems.Add(emp.Element(elementName_Role).Value);
                            item1.SubItems.Add(emp.Element(elementName_Salary).Value);
                            lstEployees.Items.Add(item1);
                        }
                        lblTotalEmployee.Text = "Total Employee : " + employees.Count();
                    }
                }
            }
        }

        //To remove a record from the XML file.
        public void removeEmployee()
        {
            //First we load the element 
            XElement element = XElement.Load(xml_path);
            //and we select the specified element with the specified attribute value.
            var employee = from f in element.Elements(second_element)
                       where f.Attribute(elementName_ID).Value == txtID.Text
                       select f;
            //If there is record with specified ID value
            if (employee != null)
            {
                //Remove the element
                employee.Remove();
                //and save the XML document
                element.Save(xml_path);

                MessageBox.Show("Employee removed successfully.");

                txtID.Text = "";
                txtName.Text = "";
                txtSurname.Text = "";
                txtRole.Text = "";
                txtSalary.Text = "";
                //When operation is finished, reload the all records.
                loadEmployees();
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            if (txtName.Text != "" && txtSurname.Text != "" && txtRole.Text != "" && txtSalary.Text != "")
            {
                //To add new record
                addNewEmployee();
            }

        }

        private void lstEployees_Click(object sender, EventArgs e)
        {
            //Load the record data to the textboxes
            txtID.Text = lstEployees.SelectedItems[0].SubItems[0].Text.ToString();
            txtName.Text = lstEployees.SelectedItems[0].SubItems[1].Text.ToString();
            txtSurname.Text = lstEployees.SelectedItems[0].SubItems[2].Text.ToString();
            txtRole.Text = lstEployees.SelectedItems[0].SubItems[3].Text.ToString();
            txtSalary.Text = lstEployees.SelectedItems[0].SubItems[4].Text.ToString();
        }

        private void btnRemove_Click(object sender, EventArgs e)
        {
            if (txtID.Text != "")
            {
                //To remove selected record
                removeEmployee();
            }
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            if (txtName.Text != "" && txtSurname.Text != "" && txtRole.Text != "" && txtSalary.Text != "")
            {
                //To update selected record.
                updateEmployee();
            }
            
        }

        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            //To make a live search first clear the listview
            lstEployees.Items.Clear();
            //and the check file is exist
            if (File.Exists(xml_path))
            {
                //Load the document
                XDocument document = XDocument.Load(xml_path);
                if (document != null)
                {
                    //Here we get the records under root element that contains the text in Searchbox.
                    //Here you should make it all texts toLower, 
                    //Because C# is capital letter sensitive language. 
                    IEnumerable<XElement> employees = from f in document.Element(root_Element).Elements(second_element)
                                                 where f.Element(elementName_Name).Value.ToLower().Contains(txtSearch.Text.ToLower())
                                                 select f;
                    //And load the records in the listview if there is record more than one.
                    if (employees.Count() > 0)
                    {
                        foreach (XElement emp in employees) 
                        {
                            ListViewItem item1 = new ListViewItem(emp.Attribute(elementName_ID).Value);
                            item1.SubItems.Add(emp.Element(elementName_Name).Value);
                            item1.SubItems.Add(emp.Element(elementName_Surname).Value);
                            item1.SubItems.Add(emp.Element(elementName_Role).Value);
                            item1.SubItems.Add(emp.Element(elementName_Salary).Value);
                            lstEployees.Items.Add(item1);
                        }
                    }
                }
            }
        }
    }
}
