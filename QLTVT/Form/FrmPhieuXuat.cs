using DevExpress.XtraGrid;
using QLTVT.SubForm;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Text.RegularExpressions; //KIỂM TRA LỖI

namespace QLTVT
{
    public partial class FormPhieuXuat : Form
    {
        /* vị trí của con trỏ trên grid view*/
        int viTri = 0;
        int viTriPhieuXuat = 0;
        int viTriCTPX = 0;
        /********************************************
         * đang thêm mới -> true -> đang dùng btnTHEM
         *              -> false -> có thể là btnGHI( chỉnh sửa) hoặc btnXOA
         *              
         * Mục đích: dùng biến này để phân biệt giữa btnTHEM - thêm mới hoàn toàn
         * và việc chỉnh sửa nhân viên( do mình ko dùng thêm btnXOA )
         * Trạng thái true or false sẽ được sử dụng 
         * trong btnGHI - việc này để phục vụ cho btnHOANTAC
         ********************************************/
        bool dangThemMoi = false;
        public string makho = "";
        string maChiNhanh = "";
        /**********************************************************
         * undoList - phục vụ cho btnHOANTAC -  chứa các thông tin của đối tượng bị tác động 
         * 
         * nó là nơi lưu trữ các đối tượng cần thiết để hoàn tác các thao tác
         * 
         * nếu btnGHI sẽ ứng với INSERT
         * nếu btnXOA sẽ ứng với DELETE
         * nếu btnCHUYENCHINHANH sẽ ứng với CHANGEBRAND
         **********************************************************/
        Stack undoList = new Stack();



        /********************************************************
         * chứa những dữ liệu hiện tại đang làm việc
         * gc chứa grid view đang làm việc
         ********************************************************/
        BindingSource bds = null;
        GridControl gc = null;
        string type = "";
        public FormPhieuXuat()
        {
            InitializeComponent();
        }

        private void phieuXuatBindingNavigatorSaveItem_Click(object sender, EventArgs e)
        {
            this.Validate();
            this.bdsPhieuXuat.EndEdit();
            this.tableAdapterManager.UpdateAll(this.dataSet);

        }

        private void FormPhieuXuat_Load(object sender, EventArgs e)
        {
            /*Step 1*/
            /*không kiểm tra khóa ngoại nữa*/
            dataSet.EnforceConstraints = false;

            this.phieuXuatTableAdapter.Connection.ConnectionString = Program.connstr;
            this.phieuXuatTableAdapter.Fill(this.dataSet.PhieuXuat);

            this.chiTietPhieuXuatTableAdapter.Connection.ConnectionString = Program.connstr;
            this.chiTietPhieuXuatTableAdapter.Fill(this.dataSet.CTPX);

            /*Step 2*/
            cmbCHINHANH.DataSource = Program.bindingSource;/*sao chep bingding source tu form dang nhap*/
            cmbCHINHANH.DisplayMember = "TENCN";
            cmbCHINHANH.ValueMember = "TENSERVER";
            cmbCHINHANH.SelectedIndex = Program.brand;
        }

        private void btnTHOAT_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            this.Dispose();
        }

        private void btnCheDoPhieuXuat_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            /*Step 0*/
            btnMENU.Links[0].Caption = "Phiếu Xuất";

            /*Step 1*/
            bds = bdsPhieuXuat;
            gc = gcChiTietPhieuXuat;


            /*Step 2*/
            /*Bat chuc nang cua phieu xuat*/
            txtMaPhieuXuat.Enabled = false;
            dteNgay.Enabled = false;

            txtTenKhachHang.Enabled = true;
            txtMaNhanVien.Enabled = false;

            btnChonKhoHang.Enabled = true;
            txtMaKho.Enabled = false;


            /*Tat chuc nang cua chi tiet phieu nhap*/
            txtMaVatTuChiTietPhieuXuat.Enabled = false;
            btnChonVatTu.Enabled = false;
            txtSoLuongChiTietPhieuXuat.Enabled = false;
            txtDonGiaChiTietPhieuXuat.Enabled = false;

            /*Bat cac grid control len*/
            gcPhieuXuat.Enabled = true;
            gcChiTietPhieuXuat.Enabled = true;


            /*Step 3*/
            /*CONG TY chi xem du lieu*/
            if (Program.role == "CONGTY")
            {
                cmbCHINHANH.Enabled = true;

                this.btnTHEM.Enabled = false;
                this.btnXOA.Enabled = false;
                this.btnGHI.Enabled = false;

                this.btnHOANTAC.Enabled = false;
                this.btnLAMMOI.Enabled = true;
                this.btnMENU.Enabled = true;
                this.btnTHOAT.Enabled = true;

                this.groupBoxPhieuNhap.Enabled = false;


            }

            /* CHI NHANH & USER co the xem - xoa - sua du lieu nhung khong the 
             chuyen sang chi nhanh khac*/
            if (Program.role == "CHINHANH" || Program.role == "USER")
            {
                cmbCHINHANH.Enabled = false;

                this.btnTHEM.Enabled = true;
                bool turnOn = (bdsPhieuXuat.Count > 0) ? true : false;
                this.btnXOA.Enabled = turnOn;
                this.btnGHI.Enabled = true;

                this.btnHOANTAC.Enabled = false;
                this.btnLAMMOI.Enabled = true;
                this.btnMENU.Enabled = true;
                this.btnTHOAT.Enabled = true;

                //this.txtMaDonDatHang.Enabled = false;

            }
        }

        private void btnCheDoChiTietPhieuXuat_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            /*Step 0*/
            btnMENU.Links[0].Caption = "Chi Tiết Phiếu Xuất";

            /*Step 1*/
            bds = bdsChiTietPhieuXuat;
            //gc = gcChiTietPhieuXuat;


            /*Step 2*/
            /*Tat het chuc nang cua phieu xuat*/
            txtMaPhieuXuat.Enabled = false;
            dteNgay.Enabled = false;

            txtTenKhachHang.Enabled = false;
            txtMaNhanVien.Enabled = false;

            btnChonKhoHang.Enabled = false;
            txtMaKho.Enabled = false;


            /*Bat chuc nang cua chi tiet phieu nhap*/
            txtMaVatTuChiTietPhieuXuat.Enabled = false;
            txtSoLuongChiTietPhieuXuat.Enabled = true;
            txtDonGiaChiTietPhieuXuat.Enabled = true;
            btnChonVatTu.Enabled = true;

            /*Bat cac grid control len*/
            gcPhieuXuat.Enabled = true;
            gcChiTietPhieuXuat.Enabled = true;


            /*Step 3*/
            /*CONG TY chi xem du lieu*/
            if (Program.role == "CONGTY")
            {
                cmbCHINHANH.Enabled = true;

                this.btnTHEM.Enabled = false;
                this.btnXOA.Enabled = true;
                this.btnGHI.Enabled = false;

                this.btnHOANTAC.Enabled = false;
                this.btnLAMMOI.Enabled = true;
                this.btnMENU.Enabled = true;
                this.btnTHOAT.Enabled = true;

                this.groupBoxPhieuNhap.Enabled = false;


            }

            /* CHI NHANH & USER co the xem - xoa - sua du lieu nhung khong the 
             chuyen sang chi nhanh khac*/
            if (Program.role == "CHINHANH" || Program.role == "USER")
            {
                cmbCHINHANH.Enabled = false;

                this.btnTHEM.Enabled = true;
                bool turnOn = (bdsPhieuXuat.Count > 0) ? true : false;
                this.btnXOA.Enabled = true;
                this.btnGHI.Enabled = true;

                this.btnHOANTAC.Enabled = false;
                this.btnLAMMOI.Enabled = true;
                this.btnMENU.Enabled = true;
                this.btnTHOAT.Enabled = true;

                //this.txtMaDonDatHang.Enabled = false;
            }
        }

        private void cmbCHINHANH_SelectedIndexChanged(object sender, EventArgs e)
        {
            /*
            /*Neu combobox khong co so lieu thi ket thuc luon*/
            if (cmbCHINHANH.SelectedValue.ToString() == "System.Data.DataRowView")
                return;

            Program.serverName = cmbCHINHANH.SelectedValue.ToString();

            /*Neu chon sang chi nhanh khac voi chi nhanh hien tai*/
            if (cmbCHINHANH.SelectedIndex != Program.brand)
            {
                Program.loginName = Program.remoteLogin;
                Program.loginPassword = Program.remotePassword;
            }
            /*Neu chon trung voi chi nhanh dang dang nhap o formDangNhap*/
            else
            {
                Program.loginName = Program.currentLogin;
                Program.loginPassword = Program.currentPassword;
            }

            if (Program.KetNoi() == 0)
            {
                MessageBox.Show("Xảy ra lỗi kết nối với chi nhánh hiện tại", "Thông báo", MessageBoxButtons.OK);
            }
            else
            {
                this.phieuXuatTableAdapter.Connection.ConnectionString = Program.connstr;
                this.phieuXuatTableAdapter.Fill(this.dataSet.PhieuXuat);

                this.chiTietPhieuXuatTableAdapter.Connection.ConnectionString = Program.connstr;
                this.chiTietPhieuXuatTableAdapter.Fill(this.dataSet.CTPX);
            }
        }

        private void btnTHEM_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {            
            /*Step 1*/
            /*lấy vị trí hiện tại của con trỏ*/
            viTri = bds.Position;
            dangThemMoi = true;
            Program.dangThemMoiPhieuXuat = true;

            /*Step 2*/
            /*AddNew tự động nhảy xuống cuối thêm 1 dòng mới*/
            bds.AddNew();
            if (btnMENU.Links[0].Caption == "Phiếu Xuất")
            {
                this.txtMaPhieuXuat.Enabled = true;

                this.dteNgay.EditValue = DateTime.Now;
                this.dteNgay.Enabled = false;

                this.txtTenKhachHang.Enabled = true;
                this.txtMaNhanVien.Text = Program.userName;

                this.btnChonKhoHang.Enabled = true;
                this.txtMaKho.Text = Program.maKhoDuocChon;

                this.txtMaVatTuChiTietPhieuXuat.Enabled = false;
                this.btnChonVatTu.Enabled = false;
                this.txtSoLuongChiTietPhieuXuat.Enabled = false;
                this.txtDonGiaChiTietPhieuXuat.Enabled = false;
                
                /*Gan tu dong may truong du lieu nay*/
                ((DataRowView)(bdsPhieuXuat.Current))["NGAY"]  = DateTime.Now;
                ((DataRowView)(bdsPhieuXuat.Current))["MANV"]  = Program.userName;
                ((DataRowView)(bdsPhieuXuat.Current))["MAKHO"] = Program.maKhoDuocChon;

            }

            if (btnMENU.Links[0].Caption == "Chi Tiết Phiếu Xuất")
            {

                DataRowView drv = ((DataRowView)bdsPhieuXuat[bdsPhieuXuat.Position]);
                String maNhanVien = drv["MANV"].ToString();
                if (Program.userName != maNhanVien)
                {
                    MessageBox.Show("Không thể thêm chi tiết phiếu xuất trên phiếu  không phải do mình tạo", "Thông báo", MessageBoxButtons.OK);
                    return;
                }

               /*Gan tu dong may truong du lieu nay*/
                ((DataRowView)(bdsChiTietPhieuXuat.Current))["MAPX"] = ((DataRowView)(bdsPhieuXuat.Current))["MAPX"];
                ((DataRowView)(bdsChiTietPhieuXuat.Current))["MAVT"] = Program.maVatTuDuocChon;


                /*THÊM ĐỂ FIX LỖI KIỂM TRA CHITIETPHIEUXUATđã tồn tại hay chưa với [SP_KiemTraChiTietPhieuXuatDaTonTaiHayChua]*/
                Program.maPhieuXuatDuocChon = (String)((DataRowView)(bdsPhieuXuat.Current))["MAPX"];
                //MessageBox.Show("maPhieuXuatDuocChon : " + Program.maPhieuXuatDuocChon, "Thông báo", MessageBoxButtons.OK);


                this.txtMaVatTuChiTietPhieuXuat.Enabled = false;
                this.btnChonVatTu.Enabled = true;

                this.txtSoLuongChiTietPhieuXuat.Enabled = true;
                this.txtSoLuongChiTietPhieuXuat.EditValue = 1;

                this.txtDonGiaChiTietPhieuXuat.Enabled = true;
                this.txtDonGiaChiTietPhieuXuat.EditValue = 1;
            }

            

            /*Step 3*/
            this.btnTHEM.Enabled = false;
            this.btnXOA.Enabled = false;
            this.btnGHI.Enabled = true;

            this.btnHOANTAC.Enabled = true;
            this.btnLAMMOI.Enabled = false;
            this.btnMENU.Enabled = false;
            this.btnTHOAT.Enabled = false;

            gcPhieuXuat.Enabled = false;
            gcChiTietPhieuXuat.Enabled = false;
        }

        private void btnChonKhoHang_Click(object sender, EventArgs e)
        {
            FrmChonKhoHang form = new FrmChonKhoHang();
            form.ShowDialog();

            this.txtMaKho.Text = Program.maKhoDuocChon;
        }

        private void btnChonVatTu_Click(object sender, EventArgs e)
        {
            if (null != ((DataRowView)(bdsChiTietPhieuXuat.Current)))
            {
                /*FIX LỖI CHỈ CHỌN ĐƯỢC MAVATTU ĐANG EDIT KHI EDIT CTPX P2*/
                Program.MaVatTuDangCoOCTPX = ((DataRowView)(bdsChiTietPhieuXuat.Current))["MaVT"].ToString().Trim();
                //MessageBox.Show("Program.MaVatTuDangCoOCTPX : " + Program.MaVatTuDangCoOCTPX, "Thông báo", MessageBoxButtons.OK);

                FrmChonVatTu form = new FrmChonVatTu();
                form.ShowDialog();
                this.txtMaVatTuChiTietPhieuXuat.Text = Program.maVatTuDuocChon;
            }
            else
            {
                MessageBox.Show("Chưa có chi tiết phiếu xuất, mời bạn chọn chức năng THÊM!", "Thông báo", MessageBoxButtons.OK);
                return;
            }
        }


        private bool kiemTraDuLieuDauVao(string cheDo)
        {
            if (cheDo == "Phiếu Xuất")
            {
                DataRowView drv = ((DataRowView)bdsPhieuXuat[bdsPhieuXuat.Position]);
                String maNhanVien = drv["MANV"].ToString();
                if (Program.userName != maNhanVien)
                {
                    MessageBox.Show("Không thể sửa phiếu xuất do người khác tạo", "Thông báo", MessageBoxButtons.OK);
                    return false;
                }
                //----------
                if (txtMaPhieuXuat.Text == "")
                {
                    MessageBox.Show("Không bỏ trống mã phiếu xuất !", "Thông báo", MessageBoxButtons.OK);
                    txtMaPhieuXuat.Focus();
                    return false;
                }

                if (Regex.IsMatch(txtMaPhieuXuat.Text, @"^[a-zA-Z0-9]+$") == false)
                {
                    MessageBox.Show("Mã phiếu xuất chỉ chấp nhận số, chữ (A-Z, a-z), không chấp nhận khoảng trắng và ký tự đặc biệt!", "Thông báo", MessageBoxButtons.OK);
                    txtMaPhieuXuat.Focus();
                    return false;
                }

                if (txtMaPhieuXuat.Text.Length > 8)
                {
                    MessageBox.Show("Mã phiếu xuất không thể quá 8 kí tự !", "Thông báo", MessageBoxButtons.OK);
                    txtMaPhieuXuat.Focus();
                    return false;
                }
                //----------
                if (txtTenKhachHang.Text == "")
                {
                    MessageBox.Show("Không bỏ trống tên khách hàng !", "Thông báo", MessageBoxButtons.OK);
                    txtTenKhachHang.Focus();
                    return false;
                }

                if( txtTenKhachHang.Text.Length > 100)
                {
                    MessageBox.Show("Tên khách hàng không quá 100 kí tự !", "Thông báo", MessageBoxButtons.OK);
                    txtTenKhachHang.Focus();
                    return false;
                }

                if (Regex.IsMatch(txtTenKhachHang.Text, @"^[a-zA-Z0-9áàạảãâấầậẩẫăắằặẳẵÁÀẠẢÃÂẤẦẬẨẪĂẮẰẶẲẴéèẹẻẽêếềệểễÉÈẸẺẼÊẾỀỆỂỄóòọỏõôốồộổỗơớờợởỡÓÒỌỎÕÔỐỒỘỔỖƠỚỜỢỞỠúùụủũưứừựửữÚÙỤỦŨƯỨỪỰỬỮíìịỉĩÍÌỊỈĨđĐýỳỵỷỹÝỲỴỶỸ ]+$") == false)
                {
                    MessageBox.Show("Tên khách hàng không nhận ký tự đặc biệt!", "Thông báo", MessageBoxButtons.OK);
                    txtTenKhachHang.Focus();
                    return false;
                }
                //----------
                if (txtMaKho.Text == "")
                {
                    MessageBox.Show("Không bỏ trống mã kho !", "Thông báo", MessageBoxButtons.OK);
                    return false;
                }

            }

            if(cheDo == "Chi Tiết Phiếu Xuất")
            {
                DataRowView drv = ((DataRowView)bdsPhieuXuat[bdsPhieuXuat.Position]);
                String maNhanVien = drv["MANV"].ToString();
                if (Program.userName != maNhanVien)
                {
                    MessageBox.Show("Không thể thêm chi tiết phiếu xuất với phiếu xuất do người khác tạo !", "Thông báo", MessageBoxButtons.OK);
                    bdsChiTietPhieuXuat.RemoveCurrent();
                    return false;
                }

                if (txtMaPhieuXuat.Text == "")
                {
                    MessageBox.Show("Không bỏ trống mã phiếu nhập !", "Thông báo", MessageBoxButtons.OK);
                    txtMaPhieuXuat.Focus();
                    return false;
                }

                if (txtMaPhieuXuat.Text.Length > 8)
                {
                    MessageBox.Show("Mã phiếu xuất không thể quá 8 kí tự !", "Thông báo", MessageBoxButtons.OK);
                    txtMaPhieuXuat.Focus();
                    return false;
                }

                if (txtMaVatTuChiTietPhieuXuat.Text == "")
                {
                    MessageBox.Show("Thiếu mã vật tư !", "Thông báo", MessageBoxButtons.OK);
                    txtMaVatTuChiTietPhieuXuat.Focus();
                    return false;
                }

                if( txtMaVatTuChiTietPhieuXuat.Text.Length > 4)
                {
                    MessageBox.Show("Mã vật tư không quá 4 kí tự !", "Thông báo", MessageBoxButtons.OK);
                    txtMaVatTuChiTietPhieuXuat.Focus();
                    return false;
                }

                if( txtSoLuongChiTietPhieuXuat.Value < 0 || txtSoLuongChiTietPhieuXuat.Value > Program.soLuongVatTu)
                {
                    MessageBox.Show("Số lượng vật tư không thể bé hơn 0 & lớn hơn số lượng vật tư đang có trong kho hàng !", "Thông báo", MessageBoxButtons.OK);
                    txtSoLuongChiTietPhieuXuat.Focus();
                    return false;
                }

                if (txtDonGiaChiTietPhieuXuat.Value < 0)
                {
                    MessageBox.Show("Đơn giá không thể bé hơn 0 VND !", "Thông báo", MessageBoxButtons.OK);
                    txtDonGiaChiTietPhieuXuat.Focus();
                    return false;
                }
            }

            return true;
        }

        private string taoCauTruyVanHoanTac(string cheDo)
        {
            String cauTruyVan = "";
            DataRowView drv;
            
            /*TH1: dang sua phieu xuat*/
            if (cheDo == "Phiếu Xuất" && dangThemMoi == false)
            {
                drv = ((DataRowView)(bdsPhieuXuat.Current));
                DateTime ngay = (DateTime) drv["NGAY"];


                cauTruyVan = "UPDATE DBO.PHIEUXUAT " +
                    "SET " +
                    "NGAY = CAST('" + ngay.ToString("yyyy-MM-dd") + "' AS DATETIME), " +
                    "HOTENKH = N'" + drv["HOTENKH"].ToString().Trim() + "', " +
                    "MANV = '" + drv["MANV"].ToString().Trim() + "', " +
                    "MAKHO = '" + drv["MAKHO"].ToString().Trim() + "' " +
                    "WHERE MAPX = '" + drv["MAPX"].ToString().Trim() + "' "; 
            }

            /*TH2: them moi phieu xuat*/
            if (cheDo == "Phiếu Xuất" && dangThemMoi == true)
            {
                // tao trong btnGHI thi hon
            }

            /*TH3: them moi chi tiet phieu xuat*/
            if (cheDo == "Chi Tiết Phiếu Xuất" && dangThemMoi == true)
            {
                // tao trong btnGHI thi hon
            }

            /*TH4: dang sua chi tiet phieu nhap*/
            if (cheDo == "Chi Tiết Phiếu Xuất" && dangThemMoi == false)
            {
                drv = ((DataRowView)(bdsChiTietPhieuXuat.Current));
                //int soLuong = int.Parse(drv["SOLUONG"].ToString().Trim());
                float donGia = float.Parse(drv["DONGIA"].ToString().Trim());
                String maPhieuXuat = drv["MAPX"].ToString().Trim();
                String maVatTu = drv["MAVT"].ToString().Trim();

                /*FIX LỖI UPDATE ĐƯỢC SỐ LƯỢNG TỒN KHI HOÀN UPDATE SỐ LƯỢNG*/
                string demsoluongcu = " SELECT SOLUONG FROM DBO.CTPX " +
                                      "WHERE MAPX = '" + maPhieuXuat + "' " +
                                      "AND MAVT = '" + maVatTu + "' ";
                SqlCommand sqlCommand = new SqlCommand(demsoluongcu, Program.conn);
                try{
                    Program.myReader = Program.ExecSqlDataReader(demsoluongcu);
                }
                catch (Exception ex){
                    MessageBox.Show("Thực thi database thất bại!\n\n" + ex.Message, "Thông báo",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                Program.myReader.Read();
                int SOLUONGBANDAU = int.Parse(Program.myReader.GetValue(0).ToString());
                Program.myReader.Close();

                int soluongthaydoi = (int)txtSoLuongChiTietPhieuXuat.Value;
                int soluongchenhlenh = SOLUONGBANDAU - soluongthaydoi;

                cauTruyVan = "UPDATE DBO.CTPX " +
                    "SET " +
                    "SOLUONG = " + SOLUONGBANDAU + ", " +
                    "DONGIA = " + donGia + " " +
                    "WHERE MAPX = '" + maPhieuXuat + "' " +
                    "AND MAVT = '" + maVatTu + "' " +
                    " UPDATE DBO.Vattu " +
                    "SET SOLUONGTON = SOLUONGTON - " + soluongchenhlenh +
                    " WHERE MAVT = '" + maVatTu + "'";
            }

            return cauTruyVan;
        }




        private void capNhatSoLuongVatTu(string mode, string maVatTu, int soLuong)
        {
            string cauTruyVan = "EXEC sp_UpdateSoLuongTonCuaVatTu '" + mode + "', '" + maVatTu + "', " + soLuong;
            int n = Program.ExecSqlNonQuery(cauTruyVan);
            Console.WriteLine(cauTruyVan);
        }
        /*
         *Step 1: xac dinh xem minh dang GHI o che do nao
         *Step 2: kiem tra du lieu dau vao
         *Step 3: tao cau truy van hoan tac
         *Step 4: dung stored procedure kiem tra xem phieu nhap da ton tai chua ?
         *Step 5: xu ly du lieu neu co
         */
        private void btnGHI_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            /*Step 1*/
            String cheDo = (btnMENU.Links[0].Caption == "Phiếu Xuất") ? "Phiếu Xuất" : "Chi Tiết Phiếu Xuất";

            /*Step 2*/
            bool ketQua = kiemTraDuLieuDauVao(cheDo);
            if (ketQua == false) return;

            

            /*Step 3*/
            string cauTruyVanHoanTac = taoCauTruyVanHoanTac(cheDo);
            //Console.WriteLine("CAU TRUY VAN HOAN TAC");
            //Console.WriteLine(cauTruyVanHoanTac);


            /*Step 4*/
            String maPhieuXuat = txtMaPhieuXuat.Text.Trim();
            //Console.WriteLine(maPhieuNhap);
            String cauTruyVan =
                    "DECLARE	@result int " +
                    "EXEC @result = SP_KiemTraMaPhieuNhapPhieuXuatDaTonTaiHayChua XUAT, '" +
                    maPhieuXuat + "' " +
                    "SELECT 'Value' = @result";
            SqlCommand sqlCommand = new SqlCommand(cauTruyVan, Program.conn);

            MessageBox.Show("CAUTRUYVAN AT STEP4 :  " + cauTruyVan, "Thông báo", MessageBoxButtons.OK);
            //MessageBox.Show("MÃ PHIẾU XUẤT : " + maPhieuXuat, "Thông báo", MessageBoxButtons.OK);
            try
            {
                Program.myReader = Program.ExecSqlDataReader(cauTruyVan);
                /*khong co ket qua tra ve thi ket thuc luon*/
                //MessageBox.Show(Program.myReader + "", "Thông báo", MessageBoxButtons.OK);
                if (Program.myReader == null)
                {
                    MessageBox.Show("stop at step4, do ko có kết quả mã phiếu XUẤT tồn tại! ", "Thông báo", MessageBoxButtons.OK);
                    return;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Thực thi database thất bại!\n\n" + ex.Message, "Thông báo",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                Console.WriteLine(ex.Message);
                return;
            }
            Program.myReader.Read();
            int result = int.Parse(Program.myReader.GetValue(0).ToString());
            Program.myReader.Close();

            /*Step 5*/
            int viTriConTro = bdsPhieuXuat.Position; // ĐANG NHẬP VÔ
            int viTriMaPhieuXuat = bdsPhieuXuat.Find("MAPX", maPhieuXuat); //ĐÃ TỒN TẠI

            /*Dang them moi phieu XUẤT, RESULT = 1 : ĐÃ TỒN TẠI*/
            if (result == 1 && cheDo == "Phiếu Xuất" && viTriMaPhieuXuat != viTriConTro)
            {
                MessageBox.Show("Mã phiếu xuất đã được sử dụng !", "Thông báo", MessageBoxButtons.OK);
                txtMaPhieuXuat.Focus();
                return;
            }
            else
            {
                DialogResult dr = MessageBox.Show("Bạn có chắc muốn ghi dữ liệu vào cơ sở dữ liệu ?", "Thông báo",
                         MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
                if (dr == DialogResult.OK)
                {
                    try
                    {
                        /*TH1: them moi phieu XUẤT*/
                        if (cheDo == "Phiếu Xuất" && dangThemMoi == true)
                        {
                            cauTruyVanHoanTac =
                                "DELETE FROM DBO.PHIEUXUAT " +
                                "WHERE MAPX = '" + maPhieuXuat + "'";
                        }

                        /*TH2: them moi chi tiet don hang*/
                        if (cheDo == "Chi Tiết Phiếu Xuất" && dangThemMoi == true)
                        {
                            string maVatTu = txtMaVatTuChiTietPhieuXuat.Text.Trim();
                            int soLuong = (int)txtSoLuongChiTietPhieuXuat.Value;

                            cauTruyVanHoanTac =
                                "DELETE FROM DBO.CTPX " +
                                "WHERE MAPX = '" + maPhieuXuat + "' " +
                                "AND MAVT = '" + Program.maVatTuDuocChon + "'" + 
                                " UPDATE DBO.Vattu " + //FIX LỖI HOÀN TÁC SAU KHI THÊM LẠI THÌ PHẢI UPDATE SỐ LƯỢNG TỒN
                                "SET SOLUONGTON = SOLUONGTON + " + soLuong +
                                "WHERE MAVT = '" + maVatTu + "'";

                            capNhatSoLuongVatTu("XUATVATTU", maVatTu, soLuong);
                        }

                        /*TH3: chinh sua phieu nhap -> chang co gi co the chinh sua
                         * duoc -> chang can xu ly*/
                        /*TH4: chinh sua chi tiet phieu nhap - > thi chi can may dong lenh duoi la xong*/
                        /*FIX LỖI UPDATE ĐƯỢC SỐ LƯỢNG TỒN KHI UPDATE SỐ LƯỢNG*/
                        if (cheDo == "Chi Tiết Phiếu Xuất" && dangThemMoi == false){
                            DataRowView drv = ((DataRowView)(bdsChiTietPhieuXuat.Current));
                            String maVatTu = drv["MAVT"].ToString().Trim();

                            string demsoluongcu = " SELECT SOLUONG FROM DBO.CTPX " +
                                                  "WHERE MAPX = '" + maPhieuXuat + "' " +
                                                  "AND MAVT = '" + maVatTu + "' ";
                            sqlCommand = new SqlCommand(demsoluongcu, Program.conn);
                            try
                            {
                                Program.myReader = Program.ExecSqlDataReader(demsoluongcu);
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show("Thực thi database thất bại!\n\n" + ex.Message, "Thông báo",
                                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                            Program.myReader.Read();
                            int SOLUONGBANDAU = int.Parse(Program.myReader.GetValue(0).ToString());
                            Program.myReader.Close();

                            int soluongthaydoi = (int)txtSoLuongChiTietPhieuXuat.Value;
                            int soluongchenhlenh = SOLUONGBANDAU - soluongthaydoi;

                            capNhatSoLuongVatTu("UPDATEXUAT", maVatTu, soluongchenhlenh);
                        }

                        /*-------------*/
                        undoList.Push(cauTruyVanHoanTac);
                        Console.WriteLine("cau truy van hoan tac");
                        Console.WriteLine(cauTruyVanHoanTac);

                        this.bdsPhieuXuat.EndEdit();
                        this.bdsChiTietPhieuXuat.EndEdit();
                        this.phieuXuatTableAdapter.Update(this.dataSet.PhieuXuat);
                        this.chiTietPhieuXuatTableAdapter.Update(this.dataSet.CTPX);

                        this.txtMaPhieuXuat.Enabled = false;

                        this.btnTHEM.Enabled = true;
                        this.btnXOA.Enabled = true;
                        this.btnGHI.Enabled = true;

                        this.btnHOANTAC.Enabled = true;
                        this.btnLAMMOI.Enabled = true;
                        this.btnMENU.Enabled = true;
                        this.btnTHOAT.Enabled = true;

                        this.gcPhieuXuat.Enabled = true;
                        this.gcChiTietPhieuXuat.Enabled = true;
                        /*cập nhật lại trạng thái thêm mới cho chắc*/
                        dangThemMoi = false;
                        Program.dangThemMoiPhieuXuat = false;
                        MessageBox.Show("Ghi thành công", "Thông báo", MessageBoxButtons.OK);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                        bds.RemoveCurrent();
                        MessageBox.Show("Da xay ra loi !\n\n" + ex.Message, "Lỗi",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                }
            }
        }



        /**********************************************************************
         * moi lan nhan btnHOANTAC thi nen nhan them btnLAMMOI de 
         * tranh bi loi khi an btnTHEM lan nua
         * 
         * statement: chua cau y nghia chuc nang ngay truoc khi an btnHOANTAC.
         * Vi du: statement = INSERT | DELETE | CHANGEBRAND
         * 
         * bdsNhanVien.CancelEdit() - phuc hoi lai du lieu neu chua an btnGHI
         * Step 0: trường hợp đã ấn btnTHEM nhưng chưa ấn btnGHI
         * Step 1: kiểm tra undoList có trông hay không ?
         * Step 2: Neu undoList khong trống thì lấy ra khôi phục
         *********************************************************************/
        private void btnHOANTAC_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {

            if (btnMENU.Links[0].Caption == "Phiếu Xuất")
            {
                viTriPhieuXuat = bds.Position;
            }
            if (btnMENU.Links[0].Caption == "Chi Tiết Phiếu Xuất")
            {
                viTriPhieuXuat = bdsPhieuXuat.Position;
                viTriCTPX = bds.Position;
                MessageBox.Show("viTriPhieuXuat CON TRO DANG DUNG : " + viTriPhieuXuat, "Thông báo", MessageBoxButtons.OK);
                MessageBox.Show("viTriCTPX CON TRO DANG DUNG : " + viTriCTPX, "Thông báo", MessageBoxButtons.OK);
            }

            /* Step 0 */
            if (dangThemMoi == true && this.btnTHEM.Enabled == false)
            {
                dangThemMoi = false;
                Program.dangThemMoiPhieuXuat = false;

                /*dang o che do Phiếu XUẤT*/
                if (btnMENU.Links[0].Caption == "Phiếu Xuất")
                {
                    this.txtMaPhieuXuat.Enabled = false;
                    this.dteNgay.Enabled = false;
                    this.txtTenKhachHang.Enabled = true;

                    this.txtMaNhanVien.Enabled = false;
                    
                    this.txtMaKho.Enabled = false;
                    this.btnChonKhoHang.Enabled = true;
                }
                /*dang o che do Chi Tiết Phiếu xuất*/
                if (btnMENU.Links[0].Caption == "Chi Tiết Phiếu Nhập")
                {
                    this.txtMaPhieuXuat.Enabled = false;
                    this.txtMaVatTuChiTietPhieuXuat.Enabled = false;
                    this.btnChonVatTu.Enabled = true;

                    this.txtSoLuongChiTietPhieuXuat.Enabled = true;
                    this.txtDonGiaChiTietPhieuXuat.Enabled = true;
                }

                this.btnTHEM.Enabled = true;
                this.btnXOA.Enabled = true;
                this.btnGHI.Enabled = true;

                //this.btnHOANTAC.Enabled = false;
                this.btnLAMMOI.Enabled = true;
                this.btnMENU.Enabled = true;
                this.btnTHOAT.Enabled = true;

                this.gcPhieuXuat.Enabled = true;
                this.gcChiTietPhieuXuat.Enabled = true;

                bds.CancelEdit();
                /*xoa dong hien tai*/
                if (btnMENU.Links[0].Caption == "Phiếu Xuất")
                {
                    bds.RemoveCurrent();
                }
                /* trở về lúc đầu con trỏ đang đứng*/
                bds.Position = viTri;
                return;
            }

            /*Step 1*/
            if (undoList.Count == 0)
            {
                MessageBox.Show("Không còn thao tác nào để khôi phục", "Thông báo", MessageBoxButtons.OK);
                btnHOANTAC.Enabled = false;
                return;
            }

            /*Step 2*/
            bds.CancelEdit();
            String cauTruyVanHoanTac = undoList.Pop().ToString();

            Console.WriteLine(cauTruyVanHoanTac);
            int n = Program.ExecSqlNonQuery(cauTruyVanHoanTac);


            /*FIX LỖI con trỏ ko trỏ đến vị trí phiếu nhập khi ấn hoàn tác chức năng ghi CTPN*/
            /*this.phieuXuatTableAdapter.Fill(this.dataSet.PhieuXuat);
            this.chiTietPhieuXuatTableAdapter.Fill(this.dataSet.CTPX);*/

            /*bdsPhieuXuat.Position = viTri;*/
            /*viTri = bds.Position;
            bdsChiTietPhieuXuat.RemoveCurrent();*/

            //FIX LỖI HOÀN TÁC CHỨC NĂNG XÓA -> TRỎ ĐẾN ĐÚNG VỊ TRÍ
            this.phieuXuatTableAdapter.Fill(this.dataSet.PhieuXuat);
            this.chiTietPhieuXuatTableAdapter.Fill(this.dataSet.CTPX);

            //HOÀN TÁC CHỨC NĂNG THÊM MỚI PHIẾU XUẤT


            if (cauTruyVanHoanTac.Contains("DBO.PHIEUXUAT")){
                bds.Position = viTriPhieuXuat;
                return;
            }

            if (cauTruyVanHoanTac.Contains("DBO.CTPX")){
                bdsPhieuXuat.Position = viTriPhieuXuat;
                bds.Position = viTriCTPX;
                return;
            }

/*            if (cauTruyVanHoanTac.Contains("INSERT INTO FROM DBO.CTPX"))
            {
                bdsPhieuXuat.Position = viTriPhieuXuat;
                bds.Position = viTriCTPX;
                return;
            }

            if (cauTruyVanHoanTac.Contains("UPDATE FROM DBO.CTPX"))
            {
                bdsPhieuXuat.Position = viTriPhieuXuat;
                bds.Position = viTriCTPX;
                return;
            }*/

        }

        private void btnLAMMOI_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                this.phieuXuatTableAdapter.Fill(this.dataSet.PhieuXuat);
                this.chiTietPhieuXuatTableAdapter.Fill(this.dataSet.CTPX);
            }
            catch(Exception ex)
            {
                MessageBox.Show("Loi lam moi \n\n" + ex.Message, "Thông báo", MessageBoxButtons.OK);
            }
        }

        private void btnXOA_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (btnMENU.Links[0].Caption == "Phiếu Xuất")
            {
                viTriPhieuXuat = bds.Position;
            }
            if (btnMENU.Links[0].Caption == "Chi Tiết Phiếu Xuất")
            {
                viTriPhieuXuat = bdsPhieuXuat.Position;
                viTriCTPX = bds.Position;
                //MessageBox.Show("viTriPhieuXuat CON TRO DANG DUNG : " + viTriPhieuXuat, "Thông báo", MessageBoxButtons.OK);
                //MessageBox.Show("viTriCTPX CON TRO DANG DUNG : " + viTriCTPX, "Thông báo", MessageBoxButtons.OK);
            }


            DataRowView drv;
            string cauTruyVanHoanTac = "";
            string cheDo = (btnMENU.Links[0].Caption == "Phiếu Xuất") ? "Phiếu Xuất" : "Chi Tiết Phiếu Xuất";

            if (cheDo == "Phiếu Xuất")
            {
                drv = ((DataRowView)bdsPhieuXuat[bdsPhieuXuat.Position]);
                String maNhanVien = drv["MANV"].ToString();
                if (Program.userName != maNhanVien)
                {
                    MessageBox.Show("Không xóa chi tiết phiếu xuất không phải do mình tạo", "Thông báo", MessageBoxButtons.OK);
                    return;
                }

                if (bdsChiTietPhieuXuat.Count > 0)
                {
                    MessageBox.Show("Không thể xóa vì có chi tiết phiếu xuất", "Thông báo", MessageBoxButtons.OK);
                    return;
                }


                DateTime ngay = ((DateTime)drv["NGAY"]);

                cauTruyVanHoanTac = "INSERT INTO DBO.PHIEUXUAT(MAPX, NGAY, HOTENKH, MANV, MAKHO) " +
                    "VALUES( '" + drv["MAPX"].ToString().Trim() + "', '" +
                    ngay.ToString("yyyy-MM-dd") + "', '" +
                    drv["HOTENKH"].ToString() + "', '" +
                    drv["MANV"].ToString() + "', '" +
                    drv["MAKHO"].ToString() + "')";
            }

            if (cheDo == "Chi Tiết Phiếu Xuất")
            {
                drv = ((DataRowView)bdsPhieuXuat[bdsPhieuXuat.Position]);
                String maNhanVien = drv["MANV"].ToString();
                if (Program.userName != maNhanVien)
                {
                    MessageBox.Show("Bạn không xóa chi tiết phiếu xuất không phải do mình tạo", "Thông báo", MessageBoxButtons.OK);
                    return;
                }


                drv = ((DataRowView)bdsChiTietPhieuXuat[bdsChiTietPhieuXuat.Position]);
                cauTruyVanHoanTac = "INSERT INTO DBO.CTPX (MAPX, MAVT, SOLUONG, DONGIA) " +
                    "VALUES('" + drv["MAPX"].ToString().Trim() + "', '" +
                    drv["MAVT"].ToString().Trim() + "', " +
                    drv["SOLUONG"].ToString().Trim() + ", " +
                    drv["DONGIA"].ToString().Trim() + ")"+
                    " UPDATE DBO.Vattu " +      //FIX LỖI HOÀN TÁC SAU KHI XÓA LẠI THÌ PHẢI UPDATE SỐ LƯỢNG TỒN
                    "SET SOLUONGTON = SOLUONGTON - " + drv["SOLUONG"].ToString().Trim() +
                    "  WHERE MAVT = '" + drv["MAVT"].ToString().Trim() + "'";
            }

            undoList.Push(cauTruyVanHoanTac);
            MessageBox.Show("cauTruyVanHoanTac : " + cauTruyVanHoanTac, "Thông báo",
                MessageBoxButtons.OK);
            /*Step 2*/
            if (MessageBox.Show("Bạn có chắc chắn muốn xóa không ?", "Thông báo",
                MessageBoxButtons.OKCancel) == DialogResult.OK)
            {
                try
                {
                    /*Step 3*/
                    viTri = bds.Position;
                    if (cheDo == "Phiếu Xuất")
                    {
                        bdsPhieuXuat.RemoveCurrent();
                    }
                    if (cheDo == "Chi Tiết Phiếu Xuất")
                    {
                        /*FIX LỖI SAU KHI XÓA LÀ PHẢI GIẢM SỐ LƯỢNG TỒN*/
                        drv = ((DataRowView)bdsChiTietPhieuXuat[bdsChiTietPhieuXuat.Position]);
                        /*string xoasoluongton = " UPDATE DBO.Vattu " +
                        "SET SOLUONGTON = SOLUONGTON + " + drv["SOLUONG"].ToString().Trim() +
                        "  WHERE MAVT = '" + drv["MAVT"].ToString().Trim() + "'";
                        int n2 = Program.ExecSqlNonQuery(xoasoluongton);*/
                        capNhatSoLuongVatTu("DELETEXUAT", drv["MAVT"].ToString().Trim(), int.Parse(drv["SOLUONG"].ToString().Trim()));

                        //MessageBox.Show("n2 : " + n2 ,"Thông báo", MessageBoxButtons.OK);
                        bdsChiTietPhieuXuat.RemoveCurrent();
                    }


                    this.phieuXuatTableAdapter.Connection.ConnectionString = Program.connstr;
                    this.phieuXuatTableAdapter.Update(this.dataSet.PhieuXuat);

                    this.chiTietPhieuXuatTableAdapter.Connection.ConnectionString = Program.connstr;
                    this.chiTietPhieuXuatTableAdapter.Update(this.dataSet.CTPX);

                    //bdsPhieuNhap.Position = viTri;
                    /*Cap nhat lai do ben tren can tao cau truy van nen da dat dangThemMoi = true*/
                    dangThemMoi = false;
                    Program.dangThemMoiPhieuXuat = false;
                    MessageBox.Show("Xóa thành công ", "Thông báo", MessageBoxButtons.OK);
                    this.btnHOANTAC.Enabled = true;
                }
                catch (Exception ex)
                {
                    /*Step 4*/
                    MessageBox.Show("Lỗi xóa nhân viên. Hãy thử lại\n" + ex.Message, "Thông báo", MessageBoxButtons.OK);
                    this.phieuXuatTableAdapter.Connection.ConnectionString = Program.connstr;
                    this.phieuXuatTableAdapter.Update(this.dataSet.PhieuXuat);

                    this.chiTietPhieuXuatTableAdapter.Connection.ConnectionString = Program.connstr;
                    this.chiTietPhieuXuatTableAdapter.Update(this.dataSet.CTPX);
                    // tro ve vi tri cua nhan vien dang bi loi
                    bds.Position = viTri;
                    //bdsNhanVien.Position = bdsNhanVien.Find("MANV", manv);
                    return;
                }
            }
            else
            {
                // xoa cau truy van hoan tac di
                undoList.Pop();
            }
        }

        private void gcPhieuXuat_Click(object sender, EventArgs e)
        {

        }
    }
}
