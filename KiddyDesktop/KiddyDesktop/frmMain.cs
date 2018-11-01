﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using KiddyDesktop.Models;
using System.Net.Http;
using Newtonsoft.Json;

namespace KiddyDesktop
{
    public partial class frmMain : Form
    {
        tblEmployee emp;
        private KiddyStore data = new KiddyStore();
        private List<tblEmployee> dtEmployee;
        private IEnumerable<ToyDTO> listToys;
        string empImgString;
        int ordIDConfirm;
        private static HttpClient client = new HttpClient();
        private static readonly string BASE_URL = "http://localhost:50815/api/";

        public static void initClient()
        {
            client.DefaultRequestHeaders.Clear();
            client.DefaultRequestHeaders.Accept
                .Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
        }

        public frmMain()
        {
            InitializeComponent();
        }

        public frmMain(string username)
        {
            InitializeComponent();
            
            loadToys();
            dtEmployee = data.tblEmployees.Where(em => em.role.Equals("Employee") && em.isActived == true).ToList();
            emp = data.tblEmployees.Single(em => em.username.Equals(username));
        }
        //Add Employee button
        private void button2_Click(object sender, EventArgs e)
        {
            SetEmployeeTabBlank();
            btnEmployeeSave.Enabled = true;
            gvEmployee.ClearSelection();
            empImgString = null;
            ordIDConfirm = -1;
            txtUsername.Enabled = true;

        }

        #region Employee_functions
        //<----------------------------Employee function----------------------------------->

        private void SetEmployeeTabBlank()
        {
            txtUsername.Text = "";
            txtFirstName.Text = "";
            txtLastName.Text = "";
            PBEmployee.Image = null;
        }

        private bool CheckEmployeeTabBlank()
        {
            bool result = false;
            if (txtUsername.Text.Equals(""))
            {
                txtUsername.Focus();
                result = true;
            }
            if (txtFirstName.Text.Equals(""))
            {
                txtUsername.Focus();
                result = true;
            }
            if (txtLastName.Text.Equals(""))
            {
                txtUsername.Focus();
                result = true;
            }
            if(empImgString == null)
            {
                result = true;
                btnUploadImage.Focus();
            }
            return result;
        }

        private bool CheckEmployeeExistedUsername(string username)
        {
            bool result = false;
            try
            {
                tblEmployee emp = data.tblEmployees.Single(em => em.username.Equals(username));
                result = true;
            }
            catch (Exception)
            {


            }

            return result;
        }

        private bool CheckValidDate(string date)
        {
            bool result = true;
            try
            {
                DateTime dateTime = DateTime.Parse(date);
            }
            catch (Exception)
            {
                result = false;
            }

            return result;

        }

        private void SetUpEmployeeData()
        {
            LoadEmployeeData(dtEmployee);
            dtDOB.Format = DateTimePickerFormat.Custom;
            dtDOB.CustomFormat = "yyyy-MMM-dd";
            btnEmployeeSave.Enabled = false;
            if (emp.role.Equals("Employee"))
            {
                TabControl.TabPages.Remove(tabEmployee);
            }
        }

        private void LoadEmployeeData(List<tblEmployee> listOfEmp)
        {
            try
            {
                gvEmployee.DataSource = listOfEmp;
                gvEmployee.Columns["username"].HeaderText = "Username";
                gvEmployee.Columns["firstname"].HeaderText = "First name";
                gvEmployee.Columns["lastname"].HeaderText = "Last name";
                gvEmployee.Columns["Role"].Visible = false;
                gvEmployee.Columns["password"].Visible = false;
                gvEmployee.Columns["dob"].Visible = false;
                gvEmployee.Columns["gender"].Visible = false;
                gvEmployee.Columns["isActived"].Visible = false;
                gvEmployee.Columns["image"].Visible = false;
                gvEmployee.Columns["tblFeedbacks"].Visible = false;
                gvEmployee.Columns["tblOrders"].Visible = false;
                gvEmployee.Columns["tblToys"].Visible = false;
            }catch(Exception )
            {
            }


        }

        private void ViewEmployeeDetail(DataGridViewCellEventArgs e)
        {
            try
            {
                btnEmployeeSave.Enabled = false;
                string gender = gvEmployee.Rows[e.RowIndex].Cells["gender"].Value.ToString();
                txtUsername.Text = gvEmployee.Rows[e.RowIndex].Cells["username"].Value.ToString();
                txtFirstName.Text = gvEmployee.Rows[e.RowIndex].Cells["firstname"].Value.ToString();
                txtLastName.Text = gvEmployee.Rows[e.RowIndex].Cells["lastname"].Value.ToString();
                byte[] imageByte = (byte[])gvEmployee.Rows[e.RowIndex].Cells["image"].Value;
                dtDOB.Text = gvEmployee.Rows[e.RowIndex].Cells["dob"].Value.ToString();
                if (gender.Equals("female"))
                {
                    rdFemail.Checked = true;
                }
                else
                {
                    rdMale.Checked = true;
                }
                Image img = byteArrayToImage(imageByte);
                PBEmployee.Image = img;
                PBEmployee.SizeMode = PictureBoxSizeMode.StretchImage;
            }
            catch(Exception ex)
            {

            }
            
        }

        public Image byteArrayToImage(byte[] byteArrayIn)
        {
            MemoryStream ms = new MemoryStream(byteArrayIn);
            Image returnImage = Image.FromStream(ms);
            return returnImage;
        }

        public byte[] imageToByteArray(System.Drawing.Image imageIn)
        {
            MemoryStream ms = new MemoryStream();
            imageIn.Save(ms, System.Drawing.Imaging.ImageFormat.Gif);
            return ms.ToArray();
        }

        private void SaveEmployee()
        {
            try
            {
                if (!CheckEmployeeTabBlank())
                {
                    if (!CheckEmployeeExistedUsername(txtUsername.Text))
                    {
                        if (CheckValidDate(dtDOB.Text))
                        {
                            DateTime myDate = DateTime.Parse(dtDOB.Text);
                            string mydob = string.Format("{0}-{1}-{2}", myDate.Year, myDate.Month, myDate.Day);
                            string myGender = "female";
                            if (rdMale.Checked == true)
                            {
                                myGender = "male";
                            }
                            Image img = Image.FromFile(empImgString);
                            byte[] imgByte = imageToByteArray(img);
                            tblEmployee addEmp = new tblEmployee();
                            addEmp.username = txtUsername.Text;
                            addEmp.password = "1";
                            addEmp.dob = mydob;
                            addEmp.gender = myGender;
                            addEmp.role = "Employee";
                            addEmp.isActived = true;
                            addEmp.firstname = txtFirstName.Text;
                            addEmp.lastname = txtLastName.Text;
                            addEmp.image = imgByte;
                            data.tblEmployees.Add(addEmp);
                            data.SaveChanges();
                            dtEmployee = data.tblEmployees.Where(em => em.role.Equals("Employee") && em.isActived == true).ToList();
                            LoadEmployeeData(dtEmployee);

                            MessageBox.Show("Save employee complete!");
                        }
                        else
                        {
                            MessageBox.Show("Invalid date");
                        }
                    }
                    else
                    {
                        MessageBox.Show("Username has already existed!");
                    }
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }

}

        private void EditEmployee()
        {
            try
            {
                if (!CheckEmployeeTabBlank())
                {
                    if (CheckEmployeeExistedUsername(txtUsername.Text))
                    {
                        if (CheckValidDate(dtDOB.Text))
                        {
                            DateTime myDate = DateTime.Parse(dtDOB.Text);

                            string mydob = string.Format("{0}-{1}-{2}", myDate.Year, myDate.Month, myDate.Day);
                            string myGender = "female";
                            if (rdMale.Checked == true)
                            {
                                myGender = "male";
                            }
                            Image image = Image.FromFile(empImgString);
                            byte[] imagebyte = imageToByteArray(image);
                            tblEmployee editEmp = data.tblEmployees.Single(emp => emp.username.Equals(txtUsername.Text));
                            editEmp.firstname = txtFirstName.Text;
                            editEmp.lastname = txtLastName.Text;
                            editEmp.dob = mydob;
                            editEmp.gender = myGender;
                            editEmp.image = imagebyte;
                            data.SaveChanges();
                            dtEmployee = data.tblEmployees.Where(em => em.role.Equals("Employee") && em.isActived == true).ToList();
                            LoadEmployeeData(dtEmployee);
                            MessageBox.Show("Edit employee complete!");
                        }
                    }
                    else
                    {
                        MessageBox.Show("Username is not existed!");
                    }

                }
            }
            catch (Exception e)
            {

            }

        }

        private void DeleteEmployee()
        {
            string username = txtUsername.Text;
            try
            {
                if (CheckEmployeeExistedUsername(txtUsername.Text))
                {
                    tblEmployee delEmp = data.tblEmployees.Single(emp => emp.username.Equals(username));
                    delEmp.isActived = false;
                    data.SaveChanges();
                    dtEmployee = data.tblEmployees.Where(em => em.role.Equals("Employee") && em.isActived == true).ToList();
                    LoadEmployeeData(dtEmployee);
                    MessageBox.Show("Delete employee complete");
                    SetEmployeeTabBlank();
                }
                else
                {
                    MessageBox.Show("Username is not existed!");
                }
            }
            catch (Exception e)
            {
            }
        }


        //<----------------------------end------------------------------------------------>     
        #endregion

        #region Customer_functions
        //<----------------------------Customer function----------------------------------->

        private void LoadCustomerData()
        {
            try
            {
                gvCustomer.DataSource = data.tblCustomers.Where(cus => cus.isActived == true).Select(cus => new
                {
                    username = cus.username,
                    name = cus.firstname + " " + cus.lastname

                }).ToList();
            }catch(Exception e)
            {
            }
        }

        private void ViewCustomerOrders(string cusID)
        {
            try
            {
                gvOrders.DataSource = data.tblOrders.Where(ord => ord.cusID.Equals(cusID)).Select(ord => new
                {
                    id = ord.id,
                    Date = ord.datetime,
                    Payment = ord.payment
                }).ToList();
                gvOrders.Columns["id"].Visible = false;
            }
            catch (Exception e)
            {
            }
            
        }

        private void ViewOrderDetail(int orderIDRef)
        {
            try
            {
                gvOrderDetail.ColumnCount = 2;
                gvOrderDetail.Columns[0].Name = "Toy";
                gvOrderDetail.Columns[1].Name = "Quantity";
                gvOrderDetail.Rows.Clear();
                List<tblOrderDetail> listOfOrderDetails = data.tblOrderDetails.Where(ord => ord.orderID == orderIDRef).ToList();
                foreach (tblOrderDetail orderDetail in listOfOrderDetails)
                {
                    ArrayList row = new ArrayList();
                    string toyName = data.tblToys.Single(toy => toy.id == orderDetail.toyID).name;
                    row.Add(toyName);
                    row.Add(orderDetail.quantity);
                    gvOrderDetail.Rows.Add(row.ToArray());
                }
            }
            catch (Exception e)
            {
            }
            
        }


        //</----------------------------end------------------------------------------------> 
        #endregion
            
        #region Order&Feedback_functions
        private void SetUpConfirmOrder()
        {
            gvConfirmOrder.Rows.Clear();
            gvConfirmOrder.ColumnCount = 4;
            gvConfirmOrder.Columns[0].Name = "Customer";
            gvConfirmOrder.Columns[1].Name = "Date";
            gvConfirmOrder.Columns[2].Name = "Address";
            gvConfirmOrder.Columns[3].Name = "id";

            List<tblOrder> listOfOrders = data.tblOrders.Where(ord => ord.payment.Equals("confirm")).ToList();
            foreach(tblOrder ord in listOfOrders)
            {
                ArrayList row = new ArrayList();
                string cusName = data.tblCustomers.Single(cus => cus.username.Equals(ord.cusID)).firstname + " " + data.tblCustomers.Single(cus => cus.username.Equals(ord.cusID)).lastname;
                row.Add(cusName);
                row.Add(ord.datetime);
                row.Add(ord.address);
                row.Add(ord.id);
                gvConfirmOrder.Rows.Add(row.ToArray());
            }
            gvConfirmOrder.Columns["id"].Visible = false;
        }
        private void ViewOrderDetail2(int orderIDRef)
        {
            gvOrderDetail2.ColumnCount = 2;
            gvOrderDetail2.Columns[0].Name = "Toy";
            gvOrderDetail2.Columns[1].Name = "Quantity";
            gvOrderDetail2.Rows.Clear();
            List<tblOrderDetail> listOfOrderDetails = data.tblOrderDetails.Where(ord => ord.orderID == orderIDRef).ToList();
            int i = 0;
            foreach (tblOrderDetail orderDetail in listOfOrderDetails)
            {
                string toyName = data.tblToys.Single(toy => toy.id == orderDetail.toyID).name;
                if (toyName == "") { break; };
                ArrayList row = new ArrayList();      
                row.Add(toyName);
                row.Add(orderDetail.quantity);
                gvOrderDetail2.Rows.Add(row.ToArray());
                i++;
            }
            
        }
        private void ConfirmOrder()
        {
            tblOrder ord = data.tblOrders.Single(order => order.id == ordIDConfirm);
            ord.payment = "success";
            data.SaveChanges();
            SetUpConfirmOrder();
            gvOrderDetail2.Rows.Clear();
        }
        private void RejectOrder()
        {
            tblOrder ord = data.tblOrders.Single(order => order.id == ordIDConfirm);
            ord.payment = "fail";
            data.SaveChanges();
            SetUpConfirmOrder();
            gvOrderDetail2.Rows.Clear();
        }

        #endregion

        private void dataGridView2_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            ViewEmployeeDetail(e);
            txtUsername.Enabled = false;
        }

        private void frmMain_Load(object sender, EventArgs e)
        {
            SetUpEmployeeData();
            LoadCustomerData();
            SetUpConfirmOrder();
        }
         
        private void btnEmployeeSave_Click(object sender, EventArgs e)
        {
            SaveEmployee();
        }

        private void btnEmployeeEdit_Click(object sender, EventArgs e)
        {
            EditEmployee();
        }

        private void btnEmployeeDelete_Click(object sender, EventArgs e)
        {
            DeleteEmployee();
        }

        private void frmMain_FormClosed(object sender, FormClosedEventArgs e)
        {
            Application.Exit();
        }

        private void textBox7_TextChanged(object sender, EventArgs e)
        {
            dtEmployee = data.tblEmployees.Where(em => em.role.Equals("Employee") && em.isActived == true && (em.firstname + " " + em.lastname).Contains(txtSearch.Text)).ToList();
            LoadEmployeeData(dtEmployee);
        }

        private void gvCustomer_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            string cusID = gvCustomer.CurrentRow.Cells[0].Value.ToString();
            ViewCustomerOrders(cusID);
            gvOrderDetail.DataSource = null;
            
        }

        private void gvOrders_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            int orderID = int.Parse(gvOrders.CurrentRow.Cells[0].Value.ToString());
            ViewOrderDetail(orderID);
        }

        private void btnChangePassword_Click(object sender, EventArgs e)
        {
            ChangePasswordForm CPasswordform = new ChangePasswordForm(emp.username);
            CPasswordform.Show();
        }

        private void txtUsername_Validating(object sender, CancelEventArgs e)
        {
            if (txtUsername.Text.Equals(""))
            {
                usernameValidate.SetError(txtUsername, "Username can not be blank!");
                txtUsername.Focus();
            }
            else
            {
                this.usernameValidate.SetError(txtUsername, "");
            }
        }

        private void txtFirstName_Validating(object sender, CancelEventArgs e)
        {
            if (txtFirstName.Text.Equals(""))
            {
                firstnameValidate.SetError(txtFirstName, "Firstname can not be blank!");
                txtFirstName.Focus();

            }
            else
            {
                this.firstnameValidate.SetError(txtFirstName, "");
            }
        }

        private void txtLastName_Validating(object sender, CancelEventArgs e)
        {
            if (txtLastName.Text.Equals(""))
            {
                lastnameValidate.SetError(txtLastName, "Firstname can not be blank!");
                txtLastName.Focus();
            }
            else
            {
                this.lastnameValidate.SetError(txtLastName, "");
            }
        }

        private void textBox12_TextChanged(object sender, EventArgs e)
        {
            gvCustomer.DataSource = data.tblCustomers.Where(cus => cus.isActived == true && (cus.firstname + " " + cus.lastname).Contains(txtCustomerSearch.Text)).Select(cus => new
            {
                username = cus.username,
                name = cus.firstname + " " + cus.lastname

            }).ToList();
        }

        private void gvConfirmOrder_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            ordIDConfirm = int.Parse(gvConfirmOrder.Rows[e.RowIndex].Cells[3].Value.ToString());
            ViewOrderDetail2(ordIDConfirm);
        }

        private void button9_Click(object sender, EventArgs e)
        {
            if (ordIDConfirm != -1)
            {
                ConfirmOrder();
            }
        }

        private void btnRejectOrder_Click(object sender, EventArgs e)
        {
            RejectOrder();
        }

        private void btnUploadImage_Click(object sender, EventArgs e)
        {
            OpenFileDialog fileChooser = new OpenFileDialog();
            fileChooser.Title = "Please select a photo";
            fileChooser.Filter = "JPG|*.jpg|PNG|*.png|GIF|*gif";
            fileChooser.Multiselect = false;
            if(fileChooser.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                this.PBEmployee.ImageLocation = fileChooser.FileName;
                empImgString = fileChooser.FileName;
                PBEmployee.SizeMode = PictureBoxSizeMode.StretchImage;
            }
        }

        private void btnUploadImage_Validating(object sender, CancelEventArgs e)
        {
            if(empImgString == null && PBEmployee.Image == null)
            {
                imageValidate.SetError(btnUploadImage, "Please choose employee image");
                btnUploadImage.Focus();
            }
            else
            {
                imageValidate.SetError(btnUploadImage, "");
            }
        }

        #region Toy_Functions
        private async void loadToys()
        {
            HttpResponseMessage response = await client.GetAsync(BASE_URL + "Toys");
            if(response.IsSuccessStatusCode)
            {
                string strResponse = response.Content.ReadAsStringAsync().Result;
                listToys = JsonConvert.DeserializeObject<IEnumerable<ToyDTO>>(strResponse);
                dgvProducts.DataSource = null;
                dgvProducts.DataSource = listToys;
                dgvProducts.Columns["image"].Visible = false;
                dgvProducts.Columns["createdBy"].Visible = false;
                dgvProducts.Columns["isActived"].Visible = false;
                dgvProducts.Columns["description"].Visible = false;
                dgvProducts.Columns["category"].Visible = false;
                clearDataBindingForProduct();
                addDataBindingForProduct();
            } else
            {
                MessageBox.Show("Load product failed! Please check your connection!");
            }
        }

        private async void addToy(ToyDTO dto)
        {
            HttpResponseMessage response = await client.PostAsJsonAsync(BASE_URL + "Toys", dto);
            try
            {
                response.EnsureSuccessStatusCode();
            } catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            
        }

        private async void updateToy(ToyDTO dto)
        {
            HttpResponseMessage response = await client.PutAsJsonAsync(BASE_URL + "Toys/" + dto.id, dto);
            try
            {
                response.EnsureSuccessStatusCode();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private async void deleteToy(int id)
        {
            HttpResponseMessage response = await client.PutAsJsonAsync(BASE_URL + "Toys/Delete?id=" + id, "");
            try
            {
                response.EnsureSuccessStatusCode();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnClearPro_Click(object sender, EventArgs e)
        {
            clearDataBindingForProduct();

            txtProID.Text = "";
            txtProName.Text = "";
            txtProPrice.Text = "";
            txtProQuantity.Text = "";
            txtProDescription.Text = "";
            cbProCategory.SelectedIndex = 0;
        }

        private void addDataBindingForProduct()
        {
            txtProID.DataBindings.Add("Text", listToys, "id");
            txtProName.DataBindings.Add("Text", listToys, "name");
            txtProPrice.DataBindings.Add("Text", listToys, "price");
            txtProQuantity.DataBindings.Add("Text", listToys, "quantity");
            txtProDescription.DataBindings.Add("Text", listToys, "description");
            cbProCategory.DataBindings.Add("Text", listToys, "category");
            pbProImage.DataBindings.Add("Image", listToys, "image", true);
        }

        private void clearDataBindingForProduct()
        {
            txtProID.DataBindings.Clear();
            txtProName.DataBindings.Clear();
            txtProPrice.DataBindings.Clear();
            txtProQuantity.DataBindings.Clear();
            txtProDescription.DataBindings.Clear();
            cbProCategory.DataBindings.Clear();
            pbProImage.DataBindings.Clear();
        }

        private void btnAddPro_Click(object sender, EventArgs e)
        {
            string proName = txtProName.Text.Trim();
            float proPrice = float.Parse(txtProPrice.Text.Trim());
            int proQuantity = int.Parse(txtProQuantity.Text.Trim());
            string proCategory = (string)cbProCategory.SelectedItem;
            string proDescription = txtProDescription.Text.Trim();
            byte[] image = imageToByteArray(pbProImage.Image);

            ToyDTO dto = new ToyDTO
            {
                name = proName,
                price = proPrice,
                quantity = proQuantity,
                category = proCategory,
                description = proDescription,
                image = image
            };

            addToy(dto);
            MessageBox.Show("Add a toy success!");
            btnClearPro_Click(sender, e);
            loadToys();
        }

        private void dgvProducts_MouseClick(object sender, MouseEventArgs e)
        {
            clearDataBindingForProduct();
            addDataBindingForProduct();
        }

        private void btnSavePro_Click(object sender, EventArgs e)
        {
            int id = int.Parse(txtProID.Text);
            string proName = txtProName.Text.Trim();
            float proPrice = float.Parse(txtProPrice.Text.Trim());
            int proQuantity = int.Parse(txtProQuantity.Text.Trim());
            string proCategory = (string)cbProCategory.SelectedItem;
            string proDescription = txtProDescription.Text.Trim();
            byte[] image = imageToByteArray(pbProImage.Image);

            ToyDTO dto = new ToyDTO
            {
                id = id,
                name = proName,
                price = proPrice,
                quantity = proQuantity,
                category = proCategory,
                description = proDescription,
                image = image
            };

            updateToy(dto);
            MessageBox.Show("Update toy id" + id + " success!");
            btnClearPro_Click(sender, e);
            loadToys();
        }

        private void btnDeletePro_Click(object sender, EventArgs e)
        {
            int id = int.Parse(txtProID.Text);
            DialogResult confirm = MessageBox.Show("Are you sure to delete toy id " + id + " ?",
                                                    "Confirmation", MessageBoxButtons.YesNo);
            if (confirm == DialogResult.Yes)
            {
                deleteToy(id);
                MessageBox.Show("Delete toy id " + id + " success!");
                loadToys();
            }
        }

        private void txtProName_Enter(object sender, EventArgs e)
        {
            txtProName.DataBindings.Clear();
        }

        private void txtProPrice_Enter(object sender, EventArgs e)
        {
            txtProPrice.DataBindings.Clear();
        }

        private void txtProQuantity_Enter(object sender, EventArgs e)
        {
            txtProQuantity.DataBindings.Clear();
        }

        private void txtProDescription_Enter(object sender, EventArgs e)
        {
            txtProDescription.DataBindings.Clear();
        }

        private void txtSearchPro_TextChanged(object sender, EventArgs e)
        {
            //clearDataBindingForProduct();
            //string search = txtSearchPro.Text;
            //BindingSource bs = new BindingSource();
            //bs.DataSource = dgvProducts.DataSource;
            //bs.Filter = "[name] = '" + search + "'";
            //dgvProducts.DataSource = bs;
        }

        #endregion

        private void btnUploadProImage_Click(object sender, EventArgs e)
        {
            OpenFileDialog fileChooser = new OpenFileDialog();
            fileChooser.Title = "Please select a photo";
            fileChooser.Filter = "JPG|*.jpg|PNG|*.png|GIF|*gif";
            fileChooser.Multiselect = false;
            if (fileChooser.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                this.pbProImage.ImageLocation = fileChooser.FileName;
                pbProImage.SizeMode = PictureBoxSizeMode.StretchImage;
            }
        }
    }
}

