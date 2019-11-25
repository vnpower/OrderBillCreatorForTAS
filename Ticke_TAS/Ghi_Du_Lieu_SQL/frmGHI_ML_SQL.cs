using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

//--- cho connect đến SQL ---
using System.Data.SqlClient;

//--- cho hàm loại bỏ dấu space của chuỗi ---
using System.Text.RegularExpressions;

namespace Ghi_Du_Lieu_SQL
{
    
    public partial class frmMain : Form
    {
        public frmMain()
        {
            InitializeComponent();
        }

        public static string dieukien = "where ma_lenh = 2";
        
        //--- Connection Database SQL ---
        //string connection_SQL = "server=(local);database=Loadingpsinnovation;user=sa;password=vinh@123";
        string connection_SQL = "server=(local);database=3_ngay44;user=sa;password=vinh@123";
        //string connection_SQL = "server=(local);database=3_ngay44;user=sa;password=psbinh";


        Boolean enable_save = false;
        Boolean enable_save_nhap = false;
        int malenhxuattoday = 0;
        int malenhnhaptoday = 0;

        private void frmMain_Load(object sender, EventArgs e)
        {
            //Truy_xuat_thu_gon();
            Truy_xuat_Bang_ma_lenh();
            txtBanghi.Text = "SỐ LỆNH TẠO HÔM NAY: " + (dataGridView1.RowCount-1).ToString();

          }

        public void Ket_noi_SQL()
        {
            //Trạng thái nối SQL: ...
            //string connection_SQL = "server=(local);database=3_ngay44;user=sa;password=psbinh";

            SqlConnection con = new SqlConnection(connection_SQL);

            if (con.State != ConnectionState.Open)
            {
                try
                {
                    con.Open();
                    if (con.State == ConnectionState.Open)
                    {
                        txtSQL_Status.Text = "KẾT NỐI SQL: TỐT";
                        txtSQL_Status.BackColor = Color.LightGreen;
                    }
                    //MessageBox.Show("Kết nối DataBase SQL thành công");
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                    txtSQL_Status.Text = "KẾT NỐI SQL: LỖI";
                }
            }
        }

        public void Truy_xuat_Bang_ma_lenh()
        {
            //string connection_SQL = "server=(local);database=3_ngay44;user=sa;password=psbinh";
            SqlConnection conn = new SqlConnection(connection_SQL);
            SqlCommand comm;
            SqlDataAdapter adapter;
            DataSet ds = new DataSet();

            if (conn.State != ConnectionState.Open)
            {
                try
                {
                    conn.Open();
                    if (conn.State == ConnectionState.Open)
                    {
                        txtSQL_Status.Text = "KẾT NỐI SQL: TỐT";
                        txtSQL_Status.BackColor = Color.LightGreen;
                    }
                    //MessageBox.Show("Kết nối DataBase SQL thành công");
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                    txtSQL_Status.Text = "KẾT NỐI SQL: LỖI";
                }
            }

            comm = new SqlCommand("SELECT Time_tao_lenh AS 'T.Gian TẠO LỆNH', Ma_lenh AS 'MÃ LỆNH', Ma_hang AS 'MÃ HÀNG', Ma_hong AS 'MÃ HỌNG', so_ptien AS 'SỐ XE', ma_ngan AS 'MÃ NGĂN', Nhiet_do AS 'NHIỆT ĐỘ', Luong_dat AS 'LƯỢNG ĐẶT', Luong_thuc_te AS 'LƯỢNG THỰC XUẤT'  FROM BX_BangMaLenh ORDER BY Time_tao_lenh", conn);
            adapter = new SqlDataAdapter(comm);
            adapter.Fill(ds);

            var GiaTriLay = from GiaTri in ds.Tables[0].AsEnumerable()
                            where (GiaTri.Field<DateTime>("T.Gian TẠO LỆNH") >= DateTime.Today)
                            select GiaTri;

            dataGridView1.DataSource = null;
            dataGridView1.DataSource = GiaTriLay.AsDataView();
            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                row.HeaderCell.Value = String.Format("{000}", row.Index + 1);
            }


            conn.Dispose();
            adapter.Dispose();
            comm.Dispose();
            malenhxuattoday = dataGridView1.RowCount;

            tinh_tong_luong_thuc_xuat();

        }


        public void Them_lenh()
        {

            if (txtLuongDat.Text == string.Empty )
            {
                MessageBox.Show("Chưa nhập lượng đặt");
            }
            else
            {
                //--- cho sql ---
                //string connection_SQL = "server=(local);database=3_ngay44;user=sa;password=psbinh";
                SqlConnection conn = new SqlConnection(connection_SQL);

                if (conn.State != ConnectionState.Open)
                {
                    conn.Open();
                }

                //Xoa lenh trong bang BX_Malenhtemp
                SqlCommand cmd_delete = new SqlCommand();
                cmd_delete.Connection = conn;
                cmd_delete.CommandText=@"delete  from BX_MaLenhTemp";
                cmd_delete.ExecuteNonQuery();

                //Them lenh vao bang BX_Malenhtemp
                SqlCommand cmd_insert = new SqlCommand();
                cmd_insert.Connection = conn;
                cmd_insert.CommandText = @"insert into BX_Malenhtemp(STT,Time_tao_lenh,Ma_hang,Ma_lenh,Luong_dat,Trang_thai_lenh, so_ptien, lai_xe, ma_ngan) values
            (@STT,@Time_tao_lenh,@Ma_hang,@Ma_lenh,@Luong_dat,@Trang_thai_lenh,@so_ptien,@lai_xe,@ma_ngan)";

                cmd_insert.Parameters.Add("@STT", SqlDbType.NVarChar).Value = Guid.NewGuid().ToString();
                cmd_insert.Parameters.Add("@Ma_hang", SqlDbType.NVarChar).Value = txtmahang.Text;

                cmd_insert.Parameters.Add("@Time_tao_lenh", SqlDbType.DateTime).Value = DateTime.Now;
                //cmd_insert.Parameters.Add("@Ma_hong", SqlDbType.TinyInt).Value = ccbHong.SelectedItem;

                cmd_insert.Parameters.Add("@Ma_lenh", SqlDbType.Int).Value = malenhxuattoday;//int.Parse(txtMaLenh.Text);
                cmd_insert.Parameters.Add("@Luong_dat", SqlDbType.Float).Value = float.Parse(txtLuongDat.Text);

                cmd_insert.Parameters.Add("@Trang_thai_lenh", SqlDbType.Int).Value = 0;

                //cmd_insert.Parameters.Add("@TT_Foxpro", SqlDbType.Int).Value = 0;

                cmd_insert.Parameters.Add("@so_ptien", SqlDbType.NVarChar).Value = txtsoptien.Text;
                cmd_insert.Parameters.Add("@lai_xe", SqlDbType.NVarChar).Value = txtlaixe.Text;
                cmd_insert.Parameters.Add("@ma_ngan", SqlDbType.NVarChar).Value = txtmangan.Text;
                
                cmd_insert.ExecuteNonQuery();
                conn.Dispose();
                cmd_insert.Dispose();
                cmd_delete.Dispose();

                enable_save = true;
                Truy_xuat_Bang_ma_lenh();

            }
        }

        private void btnThucHien_Click(object sender, EventArgs e)
        {
            Them_lenh();
            txtBanghi.Text = "SỐ LỆNH TẠO HÔM NAY: " + (dataGridView1.RowCount - 1).ToString();
            
        }

        private void button1_Click(object sender, EventArgs e)
        {
            dataGridView1.DataSource = null;
        }

        private void frmMain_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            Truy_xuat_Bang_ma_lenh();
            txtBanghi.Text = "SỐ BẢN GHI: " + (dataGridView1.RowCount - 1).ToString();

        }






        #region <>--- Khối cho họng nhập <>---

        private void them_lenh_nhap()
        {
            if ((txtNhap_ML.Text == string.Empty) || (txtNhap_tenBe.Text == string.Empty))
            {
                MessageBox.Show("Yêu cầu nhập đủ giá trị các trường \n Mã Lệnh \n Lượng Đặt");
            }
            else
            {
                //--- cho sql ---
                //string connection_SQL = "server=(local);database=3_ngay44;user=sa;password=psbinh";
                SqlConnection conn = new SqlConnection(connection_SQL);

                if (conn.State != ConnectionState.Open)
                {
                    conn.Open();
                }
                
                //Xoa lenh trong bang BN_Malenhtemp
                SqlCommand cmd_delete = new SqlCommand();
                cmd_delete.Connection = conn;
                cmd_delete.CommandText = @"delete  from BN_MaLenhTemp";
                cmd_delete.ExecuteNonQuery();

                //Them lenh vao bang BN_Malenhtemp
                SqlCommand cmd_insert = new SqlCommand();
                cmd_insert.Connection = conn;
                cmd_insert.CommandText = @"insert into BN_Malenhtemp(keys,Time_tao,Ma_lenh,Ten_be,Trang_thai,Bien_so_xe_nhap) values
                    (@keys,@Time_tao,@Ma_lenh,@Ten_be,@Trang_thai,@Bien_so_xe_nhap)";

                cmd_insert.Parameters.Add("@keys", SqlDbType.NVarChar).Value = Guid.NewGuid().ToString();

                cmd_insert.Parameters.Add("@Time_tao", SqlDbType.DateTime).Value = DateTime.Now;

                cmd_insert.Parameters.Add("@Ma_lenh", SqlDbType.Int).Value = int.Parse(txtNhap_ML.Text);

                cmd_insert.Parameters.Add("@Trang_thai", SqlDbType.Int).Value = 0;
                cmd_insert.Parameters.Add("@Ten_be", SqlDbType.Int).Value = int.Parse(txtNhap_tenBe.Text);

                cmd_insert.Parameters.Add("@Bien_so_xe_nhap", SqlDbType.NVarChar).Value = txtxenhap.Text;

                cmd_insert.ExecuteNonQuery();

                conn.Dispose();
                cmd_insert.Dispose();
                cmd_delete.Dispose();
            }
        }

        private void btnNhap_Click(object sender, EventArgs e)
        {
            //them_lenh_nhap();
            //Show_BN_BangMaLenh();
            //enable_save_nhap = true;
        }

        public void Show_BN_BangMaLenh()
        {
            SqlConnection conn = new SqlConnection(connection_SQL);
            SqlCommand comm;
            SqlDataAdapter adapter;
            DataSet ds = new DataSet();

            if (conn.State != ConnectionState.Open)
            {
                try
                {
                    conn.Open();
                    if (conn.State == ConnectionState.Open)
                    {
                        txtSQL_Status.Text = "KẾT NỐI SQL: TỐT";
                        txtSQL_Status.BackColor = Color.LightGreen;
                    }
                    //MessageBox.Show("Kết nối DataBase SQL thành công");
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                    txtSQL_Status.Text = "KẾT NỐI SQL: LỖI";
                }
            }

            comm = new SqlCommand("SELECT * FROM BN_BangMaLenh", conn);
            adapter = new SqlDataAdapter(comm);
            adapter.Fill(ds);

            var GiaTriLay = from GiaTri in ds.Tables[0].AsEnumerable()
                            where (GiaTri.Field<DateTime>("Time_tao") >= DateTime.Today) 
                            select GiaTri;

            dataGridView1.DataSource = null;
            dataGridView1.DataSource = GiaTriLay.AsDataView();

            conn.Dispose();
            adapter.Dispose();
            comm.Dispose();
        }

        private void btnShowBN_Click(object sender, EventArgs e)
        {
            //Show_BN_BangMaLenh();
        }

        #endregion
/*
        private void timer_Main_Tick(object sender, EventArgs e)
        {
            int R = this.Width;
            int H = this.Height;
            this.tabControl1.Size = new System.Drawing.Size(1320*R/1338, 213*H/668);
            this.dataGridView1.Size = new System.Drawing.Size(1320 * R / 1338, 218*H/668);
            this.groupBox1.Size = new System.Drawing.Size(1320 * R / 1338, 36*H/668);
            this.tabControl1.Location = new System.Drawing.Point(0, 106*H/668);
            this.dataGridView1.Location = new System.Drawing.Point(0, 318*H/668);
            this.groupBox1.Location = new System.Drawing.Point(0, 541*H/668);
        }
  
 */

        private void btnexit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnInXuat_Click(object sender, EventArgs e)
        {
            //Chuyen lenh tu BX_Malenhtemp vao BX_BangMaLenh
            //--- cho sql ---
            //string connection_SQL = "server=(local);database=3_ngay44;user=sa;password=psbinh";
            
            //Neu chua luu lenh thi luu lenh roi goi form in lenh, neu luu roi thi chi goi form in lenh
            if (enable_save)
            {
                SqlConnection conn = new SqlConnection(connection_SQL);

                if (conn.State != ConnectionState.Open)
                {
                    conn.Open();
                }
                SqlCommand cmd_copy = new SqlCommand();
                cmd_copy.Connection = conn;
                cmd_copy.CommandText = @"insert into BX_BangMaLenh select * from BX_MaLenhTemp";
                cmd_copy.ExecuteNonQuery();
                conn.Dispose();
                cmd_copy.Dispose();
                enable_save = false;
            };
            Truy_xuat_Bang_ma_lenh();
            txtBanghi.Text = "SỐ LỆNH TẠO HÔM NAY: " + dataGridView1.RowCount.ToString();


            //Goi form In lenh
            frmInlenhxuat frmInlenhxuat = new frmInlenhxuat();
            frmInlenhxuat.Show();
//            frmInlenhxuat.Close();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Truy_xuat_Bang_ma_lenh();
            txtBanghi.Text = "SỐ BẢN GHI: " + (dataGridView1.RowCount - 1).ToString();

        }

        private void button2_Click(object sender, EventArgs e)
        {
            txtLuongDat.Text = "";
            txtlaixe.Text = "";
            txtsoptien.Text = "";
        }

        private void btninlenhnhap_Click(object sender, EventArgs e)
        {

            /*/Neu chua luu lenh thi luu lenh roi goi form in lenh, neu luu roi thi chi goi form in lenh
            if (enable_save_nhap)
            {
                SqlConnection conn = new SqlConnection(connection_SQL);

                if (conn.State != ConnectionState.Open)
                {
                    conn.Open();
                }
                SqlCommand cmd_copy = new SqlCommand();
                cmd_copy.Connection = conn;
                cmd_copy.CommandText = @"insert into BN_BangMaLenh select * from BN_Malenhtemp";
                cmd_copy.ExecuteNonQuery();
                conn.Dispose();
                cmd_copy.Dispose();
                enable_save_nhap = false;
            };
            
            //Goi form In lenh
            frmInlenhnhap frmInlenhnhap = new frmInlenhnhap();
            frmInlenhnhap.Show();
            //            frmInlenhxuat.Close();
             */
        }


        private void btnInticke_Click(object sender, EventArgs e)
        {

            SqlConnection conn = new SqlConnection(connection_SQL);

            if (conn.State != ConnectionState.Open)
            {
                conn.Open();
            }
            //Xoa lenh trong bang Temp
            SqlCommand cmd_delete = new SqlCommand();
            cmd_delete.Connection = conn;
            cmd_delete.CommandText = @"delete  from BX_Malenhtemp";
            cmd_delete.ExecuteNonQuery();
            //Ghi vao bang temp
            SqlCommand cmd_copy = new SqlCommand();
            cmd_copy.Connection = conn;
            cmd_copy.CommandText = @"insert into BX_MaLenhtemp select * from BX_BangMaLenh where (Ma_lenh = @Ma_lenh)and(Time_tao_lenh >= @Time_tao_lenh) ";
            cmd_copy.Parameters.Add("@Ma_lenh", SqlDbType.Int).Value = int.Parse(txttichkexuat.Text);
            cmd_copy.Parameters.Add("@Time_tao_lenh", SqlDbType.DateTime).Value = DateTime.Today.AddHours(0);
            cmd_copy.ExecuteNonQuery();
            conn.Dispose();
            cmd_copy.Dispose();

            formtickexuat formtickexuat = new formtickexuat();
            formtickexuat.Show();
        }

        private void btn_dangnhap_Click(object sender, EventArgs e)
        {
            if (txt_UserName.Text == "" || txt_Password.Text == "")
            {
                MessageBox.Show("Chưa nhập UserName and Password");
                return;
            }
            try
            {
                //Create SqlConnection
                SqlConnection con = new SqlConnection(connection_SQL);
                SqlCommand cmd = new SqlCommand("Select * from user_name_hh where user_name=@user_name and pass_word=@pass_word", con);
                cmd.Parameters.AddWithValue("@user_name", txt_UserName.Text);
                cmd.Parameters.AddWithValue("@pass_word", txt_Password.Text);
                con.Open();
                SqlDataAdapter adapt = new SqlDataAdapter(cmd);
                DataSet ds = new DataSet();
                adapt.Fill(ds);
                con.Close();
                int count = ds.Tables[0].Rows.Count;
                //If count is equal to 1, than show frmMain form
                if (count == 1)
                {
                    MessageBox.Show("Đăng nhập thành công!");
                    group_nhapmalenh.Visible = true;
                    group_intichke.Visible = true;
                    txt_UserName.Text = "";
                    txt_Password.Text = "";
                    lbl_password.Text = "NEW PASSWORD";
                    btn_dangnhap.Visible = false;
                    btn_changepass.Visible = true;
                }
                else
                {
                    MessageBox.Show("Sai mật khẩu hoặc username!");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btn_changepass_Click(object sender, EventArgs e)
        {
            if (txt_UserName.Text == "" || txt_Password.Text == "")
            {
                MessageBox.Show("Chưa nhập UserName and Password");
                return;
            }
            try
            {
                //Create SqlConnection
                SqlConnection con = new SqlConnection(connection_SQL);
                SqlCommand cmd = new SqlCommand("UPDATE user_name_hh SET pass_word=@pass_word WHERE user_name=@user_name", con);
                cmd.Parameters.AddWithValue("@user_name", txt_UserName.Text);
                cmd.Parameters.AddWithValue("@pass_word", txt_Password.Text);
                con.Open();
                SqlDataAdapter adapt = new SqlDataAdapter(cmd);
                DataSet ds = new DataSet();
                adapt.Fill(ds);
                con.Close();
                MessageBox.Show("Đổi password thành công!");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void cbbMaH_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
        private void tinh_tong_luong_thuc_xuat()
        {
            Double result = 0;
            foreach (DataGridViewRow row in this.dataGridView1.Rows)
            {
                if (row.Cells[8].Value != null)
                {
                    try
                    {
                        result += Convert.ToDouble(row.Cells[8].Value);
                    }
                    catch { }
                }
            }

            this.textBox1.Text = result.ToString();
        }
        private void btn_Xem_Click(object sender, EventArgs e)
        {
            SqlConnection conn = new SqlConnection(connection_SQL);
            SqlCommand comm;
            SqlDataAdapter adapter;
            DataSet ds = new DataSet();

            if (conn.State != ConnectionState.Open)
            {
                try
                {
                    conn.Open();
                    if (conn.State == ConnectionState.Open)
                    {
                        txtSQL_Status.Text = "KẾT NỐI SQL: TỐT";
                        txtSQL_Status.BackColor = Color.LightGreen;
                    }
                    //MessageBox.Show("Kết nối DataBase SQL thành công");
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                    txtSQL_Status.Text = "KẾT NỐI SQL: LỖI";
                }
            }
            comm = new SqlCommand("SELECT Time_tao_lenh AS 'T.Gian TẠO LỆNH', Ma_lenh AS 'MÃ LỆNH', Ma_hang AS 'MÃ HÀNG', Ma_hong AS 'MÃ HỌNG', so_ptien AS 'SỐ XE', ma_ngan AS 'MÃ NGĂN', Nhiet_do AS 'NHIỆT ĐỘ', Luong_dat AS 'LƯỢNG ĐẶT', Luong_thuc_te AS 'LƯỢNG THỰC XUẤT'  FROM BX_BangMaLenh ORDER BY Time_tao_lenh", conn);
            adapter = new SqlDataAdapter(comm);
            adapter.Fill(ds);

            //1000 THEO NGAY
            if ((checkBox_THEONGAY.Checked) && (!checkBox_THEOHONG.Checked) && (!checkBox_THEOMAHANG.Checked) && (!checkBox_THEOMALENH.Checked))
            {
                var GiaTriLay = from GiaTri in ds.Tables[0].AsEnumerable()
                                where (GiaTri.Field<DateTime>("T.Gian TẠO LỆNH") >= dateTimePicker3.Value.Date) && (GiaTri.Field<DateTime>("T.Gian TẠO LỆNH") < dateTimePicker4.Value.Date)
                                select GiaTri;

                dataGridView1.DataSource = null;
                dataGridView1.DataSource = GiaTriLay.AsDataView();

                tinh_tong_luong_thuc_xuat();
            }
            //0100 THEO HONG
            else if ((!checkBox_THEONGAY.Checked) && (checkBox_THEOHONG.Checked) && (!checkBox_THEOMAHANG.Checked) && (!checkBox_THEOMALENH.Checked))
            {
                var GiaTriLay = from GiaTri in ds.Tables[0].AsEnumerable()
                                where GiaTri.Field<int?>("MÃ HỌNG") == (comboBox2.SelectedIndex + 1)
                                select GiaTri;

                dataGridView1.DataSource = null;
                dataGridView1.DataSource = GiaTriLay.AsDataView();

                tinh_tong_luong_thuc_xuat();
            }
            
            //1100 THEO NGAY & THEO HONG
            else if ((checkBox_THEONGAY.Checked) & (checkBox_THEOHONG.Checked) & (!checkBox_THEOMAHANG.Checked) & (!checkBox_THEOMALENH.Checked))
            {
                var GiaTriLay = from GiaTri in ds.Tables[0].AsEnumerable()
                                where (GiaTri.Field<DateTime>("T.Gian TẠO LỆNH") >= dateTimePicker3.Value.Date) && (GiaTri.Field<DateTime>("T.Gian TẠO LỆNH") < dateTimePicker4.Value.Date)
                                where GiaTri.Field<int?>("MÃ HỌNG") == (comboBox2.SelectedIndex + 1)
                                select GiaTri;

                dataGridView1.DataSource = null;
                dataGridView1.DataSource = GiaTriLay.AsDataView();

                tinh_tong_luong_thuc_xuat();
            }

            //0010 THEO MA HANG
            else if ((!checkBox_THEONGAY.Checked) & (!checkBox_THEOHONG.Checked) & (checkBox_THEOMAHANG.Checked) & (!checkBox_THEOMALENH.Checked))
            {
                var GiaTriLay = from GiaTri in ds.Tables[0].AsEnumerable()
                                where GiaTri.Field<string>("MÃ HÀNG") == textBox3.Text
                                select GiaTri;

                dataGridView1.DataSource = null;
                dataGridView1.DataSource = GiaTriLay.AsDataView();

                tinh_tong_luong_thuc_xuat();
            }
            
            //1010 THEO NGAY & THEO MA HANG
            else if ((checkBox_THEONGAY.Checked) & (!checkBox_THEOHONG.Checked) & (checkBox_THEOMAHANG.Checked) & (!checkBox_THEOMALENH.Checked))
            {
                var GiaTriLay = from GiaTri in ds.Tables[0].AsEnumerable()
                                where (GiaTri.Field<DateTime>("T.Gian TẠO LỆNH") >= dateTimePicker3.Value.Date) && (GiaTri.Field<DateTime>("T.Gian TẠO LỆNH") < dateTimePicker4.Value.Date)
                                where GiaTri.Field<string>("MÃ HÀNG") == textBox3.Text
                                select GiaTri;

                dataGridView1.DataSource = null;
                dataGridView1.DataSource = GiaTriLay.AsDataView();

                tinh_tong_luong_thuc_xuat();
            }


            //0110 THEO HONG & THEO MA HANG
            else if ((!checkBox_THEONGAY.Checked) & (checkBox_THEOHONG.Checked) & (checkBox_THEOMAHANG.Checked) & (!checkBox_THEOMALENH.Checked))
            {
                var GiaTriLay = from GiaTri in ds.Tables[0].AsEnumerable()
                                where GiaTri.Field<int?>("MÃ HỌNG") == (comboBox2.SelectedIndex + 1)
                                where GiaTri.Field<string>("MÃ HÀNG") == textBox3.Text
                                select GiaTri;

                dataGridView1.DataSource = null;
                dataGridView1.DataSource = GiaTriLay.AsDataView();

                tinh_tong_luong_thuc_xuat();
            }
            
            //1110 THEO NGAY & THEO HONG & THEO MA HANG
            else if ((checkBox_THEONGAY.Checked) & (checkBox_THEOHONG.Checked) & (checkBox_THEOMAHANG.Checked) & (!checkBox_THEOMALENH.Checked))
            {
                var GiaTriLay = from GiaTri in ds.Tables[0].AsEnumerable()
                                where (GiaTri.Field<DateTime>("T.Gian TẠO LỆNH") >= dateTimePicker3.Value.Date) && (GiaTri.Field<DateTime>("T.Gian TẠO LỆNH") < dateTimePicker4.Value.Date)
                                
                                where GiaTri.Field<int?>("MÃ HỌNG") == (comboBox2.SelectedIndex + 1)
                                where GiaTri.Field<string>("MÃ HÀNG") == textBox3.Text
                                select GiaTri;

                dataGridView1.DataSource = null;
                dataGridView1.DataSource = GiaTriLay.AsDataView();

                tinh_tong_luong_thuc_xuat();
            }
            
            //0001 THEO MA LENH
            else if ((!checkBox_THEONGAY.Checked) & (!checkBox_THEOHONG.Checked) & (!checkBox_THEOMAHANG.Checked) & (checkBox_THEOMALENH.Checked))
            {
                var GiaTriLay = from GiaTri in ds.Tables[0].AsEnumerable()
                                where GiaTri.Field<int?>("MÃ LỆNH") == int.Parse(textBox2.Text)
                                select GiaTri;

                dataGridView1.DataSource = null;
                dataGridView1.DataSource = GiaTriLay.AsDataView();

                tinh_tong_luong_thuc_xuat();
            }
            
            //1001 THEO NGAY & THEO MA LENH
            else if ((checkBox_THEONGAY.Checked) & (!checkBox_THEOHONG.Checked) & (!checkBox_THEOMAHANG.Checked) & (checkBox_THEOMALENH.Checked))
            {
                var GiaTriLay = from GiaTri in ds.Tables[0].AsEnumerable()
                                where (GiaTri.Field<DateTime>("T.Gian TẠO LỆNH") >= dateTimePicker3.Value.Date) && (GiaTri.Field<DateTime>("T.Gian TẠO LỆNH") < dateTimePicker4.Value.Date)
                                //where GiaTri.Field<int?>("MÃ HỌNG") == (comboBox2.SelectedIndex + 1)
                                //where GiaTri.Field<string>("MÃ HÀNG") == textBox3.Text
                                where GiaTri.Field<int?>("MÃ LỆNH") == int.Parse(textBox2.Text)
                                select GiaTri;

                dataGridView1.DataSource = null;
                dataGridView1.DataSource = GiaTriLay.AsDataView();

                tinh_tong_luong_thuc_xuat();
            }
            
            //0101 THEO HONG & THEO MA LENH
            else if ((!checkBox_THEONGAY.Checked) & (checkBox_THEOHONG.Checked) & (!checkBox_THEOMAHANG.Checked) & (checkBox_THEOMALENH.Checked))
            {
                var GiaTriLay = from GiaTri in ds.Tables[0].AsEnumerable()
                                //where (GiaTri.Field<DateTime>("T.Gian TẠO LỆNH") >= dateTimePicker3.Value.Date) && (GiaTri.Field<DateTime>("T.Gian TẠO LỆNH") < dateTimePicker4.Value.Date)
                                where GiaTri.Field<int?>("MÃ HỌNG") == (comboBox2.SelectedIndex + 1)
                                //where GiaTri.Field<string>("MÃ HÀNG") == textBox3.Text
                                where GiaTri.Field<int?>("MÃ LỆNH") == int.Parse(textBox2.Text)
                                select GiaTri;

                dataGridView1.DataSource = null;
                dataGridView1.DataSource = GiaTriLay.AsDataView();

                tinh_tong_luong_thuc_xuat();
            }
            
            //1101 THEO NGAY & THEO HONG & THEO MA LENH
            else if ((checkBox_THEONGAY.Checked) & (checkBox_THEOHONG.Checked) & (!checkBox_THEOMAHANG.Checked) & (checkBox_THEOMALENH.Checked))
            {
                var GiaTriLay = from GiaTri in ds.Tables[0].AsEnumerable()
                                where (GiaTri.Field<DateTime>("T.Gian TẠO LỆNH") >= dateTimePicker3.Value.Date) && (GiaTri.Field<DateTime>("T.Gian TẠO LỆNH") < dateTimePicker4.Value.Date)
                                where GiaTri.Field<int?>("MÃ HỌNG") == (comboBox2.SelectedIndex + 1)
                                //where GiaTri.Field<string>("MÃ HÀNG") == textBox3.Text
                                where GiaTri.Field<int?>("MÃ LỆNH") == int.Parse(textBox2.Text)
                                select GiaTri;

                dataGridView1.DataSource = null;
                dataGridView1.DataSource = GiaTriLay.AsDataView();

                tinh_tong_luong_thuc_xuat();
            }
            
            //0011 THEO MA HANG & THEO MA LENH
            else if ((!checkBox_THEONGAY.Checked) & (!checkBox_THEOHONG.Checked) & (checkBox_THEOMAHANG.Checked) & (checkBox_THEOMALENH.Checked))
            {
                var GiaTriLay = from GiaTri in ds.Tables[0].AsEnumerable()
                                //where (GiaTri.Field<DateTime>("T.Gian TẠO LỆNH") >= dateTimePicker3.Value.Date) && (GiaTri.Field<DateTime>("T.Gian TẠO LỆNH") < dateTimePicker4.Value.Date)
                                //where GiaTri.Field<int?>("MÃ HỌNG") == (comboBox2.SelectedIndex + 1)
                                where GiaTri.Field<string>("MÃ HÀNG") == textBox3.Text
                                where GiaTri.Field<int?>("MÃ LỆNH") == int.Parse(textBox2.Text)
                                select GiaTri;

                dataGridView1.DataSource = null;
                dataGridView1.DataSource = GiaTriLay.AsDataView();

                tinh_tong_luong_thuc_xuat();
            }
            
            //1011 THEO NGAY & THEO MA HANG & THEO MA LENH
            else if ((checkBox_THEONGAY.Checked) & (!checkBox_THEOHONG.Checked) & (checkBox_THEOMAHANG.Checked) & (checkBox_THEOMALENH.Checked))
            {
                var GiaTriLay = from GiaTri in ds.Tables[0].AsEnumerable()
                                where (GiaTri.Field<DateTime>("T.Gian TẠO LỆNH") >= dateTimePicker3.Value.Date) && (GiaTri.Field<DateTime>("T.Gian TẠO LỆNH") < dateTimePicker4.Value.Date)
                                //where GiaTri.Field<int?>("MÃ HỌNG") == (comboBox2.SelectedIndex + 1)
                                where GiaTri.Field<string>("MÃ HÀNG") == textBox3.Text
                                where GiaTri.Field<int?>("MÃ LỆNH") == int.Parse(textBox2.Text)
                                select GiaTri;

                dataGridView1.DataSource = null;
                dataGridView1.DataSource = GiaTriLay.AsDataView();

                tinh_tong_luong_thuc_xuat();
            }
            
            //0111 THEO HONG & THEO MA HANG & THEO MA LENH
            else if ((!checkBox_THEONGAY.Checked) & (checkBox_THEOHONG.Checked) & (checkBox_THEOMAHANG.Checked) & (checkBox_THEOMALENH.Checked))
            {
                var GiaTriLay = from GiaTri in ds.Tables[0].AsEnumerable()
                                //where (GiaTri.Field<DateTime>("T.Gian TẠO LỆNH") >= dateTimePicker3.Value.Date) && (GiaTri.Field<DateTime>("T.Gian TẠO LỆNH") < dateTimePicker4.Value.Date)
                                where GiaTri.Field<int?>("MÃ HỌNG") == (comboBox2.SelectedIndex + 1)
                                where GiaTri.Field<string>("MÃ HÀNG") == textBox3.Text
                                where GiaTri.Field<int?>("MÃ LỆNH") == int.Parse(textBox2.Text)
                                select GiaTri;

                dataGridView1.DataSource = null;
                dataGridView1.DataSource = GiaTriLay.AsDataView();

                tinh_tong_luong_thuc_xuat();
            }
            
            //1111 THEO NGAY & THEO HONG & THEO MA HANG & THEO MA LENH
            else if ((checkBox_THEONGAY.Checked) & (checkBox_THEOHONG.Checked) & (checkBox_THEOMAHANG.Checked) & (checkBox_THEOMALENH.Checked))
            {
                var GiaTriLay = from GiaTri in ds.Tables[0].AsEnumerable()
                                where (GiaTri.Field<DateTime>("T.Gian TẠO LỆNH") >= dateTimePicker3.Value.Date) && (GiaTri.Field<DateTime>("T.Gian TẠO LỆNH") < dateTimePicker4.Value.Date)
                                where GiaTri.Field<int?>("MÃ HỌNG") == (comboBox2.SelectedIndex + 1)
                                where GiaTri.Field<string>("MÃ HÀNG") == textBox3.Text
                                where GiaTri.Field<int?>("MÃ LỆNH") == int.Parse(textBox2.Text)
                                select GiaTri;
                dataGridView1.DataSource = null;
                dataGridView1.DataSource = GiaTriLay.AsDataView();

                tinh_tong_luong_thuc_xuat();
            }
            
            // Xem tat ca bang ma lenh
            else
            {
                var GiaTriLay = from GiaTri in ds.Tables[0].AsEnumerable()
                                select GiaTri;

                dataGridView1.DataSource = null;
                dataGridView1.DataSource = GiaTriLay.AsDataView();

                tinh_tong_luong_thuc_xuat();
             }
            
            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                row.HeaderCell.Value = String.Format("{000}", row.Index + 1);
            }


            conn.Dispose();
            adapter.Dispose();
            comm.Dispose();
            malenhxuattoday = dataGridView1.RowCount;

        }

        private void checkBox_THEONGAY_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox_THEONGAY.Checked)
            {
                label8.Visible = true;
                label9.Visible = true;
                this.dateTimePicker3.Visible = true;
                this.dateTimePicker4.Visible = true;
                txtML_timestart.Visible = true;
                txtML_timestop.Visible = true;
            }
            else
            {
                label8.Visible = false;
                label9.Visible = false;
                this.dateTimePicker3.Visible = false;
                this.dateTimePicker4.Visible = false;
                txtML_timestart.Visible = false;
                txtML_timestop.Visible = false;
            }

        }

        private void checkBox_THEOHONG_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox_THEOHONG.Checked)
            {
                label12.Visible = true;
                comboBox2.Visible = true;
            }
            else
            {
                label12.Visible = false;
                comboBox2.Visible = false;
            }
        }




       

        private void checkBox_THEOMALENH_CheckedChanged_1(object sender, EventArgs e)
        {
            if (checkBox_THEOMALENH.Checked)
            {
                label14.Visible = true;
                textBox2.Visible = true;
            }
            else
            {
                label14.Visible = false;
                textBox2.Visible = false;
            }
        }

        private void checkBox_THEOMAHANG_CheckedChanged_1(object sender, EventArgs e)
        {
            if (checkBox_THEOMAHANG.Checked)
            {
                label15.Visible = true;
                textBox3.Visible = true;
            }
            else
            {
                label15.Visible = false;
                textBox3.Visible = false;
            }
        }

        private void dateTimePicker3_ValueChanged(object sender, EventArgs e)
        {
            txtML_timestart.Text = dateTimePicker3.Value.ToString();
        }

        private void dateTimePicker4_ValueChanged(object sender, EventArgs e)
        {
            txtML_timestop.Text = dateTimePicker4.Value.ToString();
        }

        private void btnViewBX_alrm_Click(object sender, EventArgs e)
        {
            SqlConnection conn = new SqlConnection(connection_SQL);
            SqlCommand comm;
            SqlDataAdapter adapter;
            DataSet ds = new DataSet();

            if (conn.State != ConnectionState.Open)
            {
                try
                {
                    conn.Open();
                    if (conn.State == ConnectionState.Open)
                    {
                        txtSQL_Status.Text = "KẾT NỐI SQL: TỐT";
                        txtSQL_Status.BackColor = Color.LightGreen;
                    }
                    //MessageBox.Show("Kết nối DataBase SQL thành công");
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                    txtSQL_Status.Text = "KẾT NỐI SQL: LỖI";
                }
            }

            comm = new SqlCommand("SELECT Time AS 'THỜI GIAN', Ma_hong AS 'MÃ HỌNG', Ma_lenh AS 'MÃ LỆNH', Luong_dat AS 'LƯỢNG ĐẶT', Luong_tt AS 'LƯỢNG THỰC', ToC AS 'NHIỆT ĐỘ', ToC_tb AS 'NHIỆT ĐỘ TB', Luu_toc AS 'LƯU TỐC', BOM AS 'BƠM', Van1 AS 'VAN NO', Van2 AS 'VAN NC', Auto_man AS 'AUTO/MAN', Dung_su_co AS 'DỪNG SỰ CỐ'  FROM BX_Historys ORDER BY Time", conn);
            adapter = new SqlDataAdapter(comm);
            adapter.Fill(ds);

            var GiaTriLay = from GiaTri in ds.Tables[0].AsEnumerable()
                            where GiaTri.Field<int?>("MÃ HỌNG")== (cbbMaH.SelectedIndex + 1)
                            where (GiaTri.Field<DateTime>("THỜI GIAN") >= dateTimePicker5.Value)&&(GiaTri.Field<DateTime>("THỜI GIAN")<dateTimePicker6.Value)
                            select GiaTri;

            dataGridView1.DataSource = null;
            dataGridView1.DataSource = GiaTriLay.AsDataView();
            
            conn.Dispose();
            adapter.Dispose();
            comm.Dispose();
        }

        private void dateTimePicker5_ValueChanged(object sender, EventArgs e)
        {
            txtbatdau.Text = dateTimePicker5.Value.ToString();
        }

        private void dateTimePicker6_ValueChanged(object sender, EventArgs e)
        {
            txtKetthuc.Text = dateTimePicker6.Value.ToString();
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            SqlConnection conn = new SqlConnection(connection_SQL);
            SqlCommand comm;
            SqlDataAdapter adapter;
            DataSet ds = new DataSet();

            if (conn.State != ConnectionState.Open)
            {
                try
                {
                    conn.Open();
                    if (conn.State == ConnectionState.Open)
                    {
                        txtSQL_Status.Text = "KẾT NỐI SQL: TỐT";
                        txtSQL_Status.BackColor = Color.LightGreen;
                    }
                    //MessageBox.Show("Kết nối DataBase SQL thành công");
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                    txtSQL_Status.Text = "KẾT NỐI SQL: LỖI";
                }
            }

            comm = new SqlCommand("SELECT Time AS 'THỜI GIAN', Ma_hong AS 'MÃ HỌNG', Canh_bao AS 'CẢNH BÁO'  FROM BX_Canhbao ORDER BY Time", conn);
            adapter = new SqlDataAdapter(comm);
            adapter.Fill(ds);

            
            if (comboBox1.SelectedIndex == 0)
            {
                var GiaTriLay = from GiaTri in ds.Tables[0].AsEnumerable()
                                //where GiaTri.Field<int?>("MÃ HỌNG") == (comboBox1.SelectedIndex + 1)
                                where (GiaTri.Field<DateTime>("THỜI GIAN") >= dateTimePicker1.Value) && (GiaTri.Field<DateTime>("THỜI GIAN") < dateTimePicker2.Value)
                                select GiaTri;
                dataGridView1.DataSource = null;
                dataGridView1.DataSource = GiaTriLay.AsDataView();
            }
            else if (comboBox1.SelectedIndex > 0)
            {
                var GiaTriLay = from GiaTri in ds.Tables[0].AsEnumerable()
                                where GiaTri.Field<int?>("MÃ HỌNG") == (comboBox1.SelectedIndex - 1)
                                where (GiaTri.Field<DateTime>("THỜI GIAN") >= dateTimePicker1.Value) && (GiaTri.Field<DateTime>("THỜI GIAN") < dateTimePicker2.Value)
                                select GiaTri;
                dataGridView1.DataSource = null;
                dataGridView1.DataSource = GiaTriLay.AsDataView();
            }
            else
            {
                var GiaTriLay = from GiaTri in ds.Tables[0].AsEnumerable()
                                //where GiaTri.Field<int?>("MÃ HỌNG") == (comboBox1.SelectedIndex)
                                where (GiaTri.Field<DateTime>("THỜI GIAN") >= dateTimePicker1.Value) && (GiaTri.Field<DateTime>("THỜI GIAN") < dateTimePicker2.Value)
                                select GiaTri;
                dataGridView1.DataSource = null;
                dataGridView1.DataSource = GiaTriLay.AsDataView();
            }


            

            conn.Dispose();
            adapter.Dispose();
            comm.Dispose();
        }

        private void dateTimePicker1_ValueChanged(object sender, EventArgs e)
        {
            this.textBox5.Text = dateTimePicker1.Value.ToString();
        }

        private void dateTimePicker2_ValueChanged(object sender, EventArgs e)
        {
            this.textBox4.Text = dateTimePicker2.Value.ToString();
        }
        
    }
}
