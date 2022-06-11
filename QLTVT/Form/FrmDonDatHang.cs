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

namespace QLTVT
{

    public partial class FormDonDatHang : Form
    {
        /* vị trí của con trỏ trên grid view*/
        int viTri = 0;
        int vitriddh = 0;
        int vitrictdh = 0;
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



        /************************************************************
         * CheckExists:
         * Để tránh việc người dùng ấn vào 1 form đến 2 lần chúng ta 
         * cần sử dụng hàm này để kiểm tra xem cái form hiện tại đã 
         * có trong bộ nhớ chưa
         * Nếu có trả về "f"
         * Nếu không trả về "null"
         ************************************************************/
        private Form CheckExists(Type ftype)
        {
            foreach (Form f in this.MdiChildren)
                if (f.GetType() == ftype)
                    return f;
            return null;
        }
        public FormDonDatHang()
        {
            InitializeComponent();
        }
        // check lại cái này

        private void datHangBindingNavigatorSaveItem_Click(object sender, EventArgs e)
        {
            this.Validate();
            this.bdsDonDatHang.EndEdit();
            this.tableAdapterManager.UpdateAll(this.dataSet);

        }

        private void FormDonDatHang_Load(object sender, EventArgs e)
        {

            /*Step 1*/
            dataSet.EnforceConstraints = false; // tắt kích hoặc các ràng buộc

            this.chiTietDonDatHangTableAdapter.Connection.ConnectionString = Program.connstr;
            this.chiTietDonDatHangTableAdapter.Fill(this.dataSet.CTDDH);

            this.donDatHangTableAdapter.Connection.ConnectionString = Program.connstr;
            this.donDatHangTableAdapter.Fill(this.dataSet.DatHang);

            this.phieuNhapTableAdapter.Connection.ConnectionString = Program.connstr;
            this.phieuNhapTableAdapter.Fill(this.dataSet.PhieuNhap);
            /*van con ton tai loi chua sua duoc*/
            //maChiNhanh = ((DataRowView)bdsVatTu[0])["MACN"].ToString();

            /*Step 2*/
            cmbCHINHANH.DataSource = Program.bindingSource;/*sao chep bingding source tu form dang nhap*/
            cmbCHINHANH.DisplayMember = "TENCN";
            cmbCHINHANH.ValueMember = "TENSERVER";
            cmbCHINHANH.SelectedIndex = Program.brand;

            bds = bdsDonDatHang;
            gc = gcDonDatHang;



        }

        private void sOLUONGSpinEdit_EditValueChanged(object sender, EventArgs e)
        {

        }



        /*********************************************************
         * Step 0: Hiện chế độ làm việc
         * Step 1: cập nhật binding source và grid control
         * 
         * tắt các chức năng liên quan tới chi tiết đơn hàng
         *********************************************************/
        // mới đầu vô là bấm chế độ làm việc đầu tiên: có hai chế độ là: ĐƠN ĐẶT HÀNG VÀ CHI TIẾT ĐƠN ĐẶT HÀNG
        private void btnCheDoDonDatHang_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            /*Step 0*/
            btnMENU.Links[0].Caption = "Đơn Đặt Hàng"; // option: đơn đặt hàng

            /*Step 1*/
            bds = bdsDonDatHang;
            gc = gcDonDatHang;
            //MessageBox.Show("Chế Độ Làm Việc Đơn Đặt Hàng", "Thông báo", MessageBoxButtons.OK);

            /*Step 2*/
            //Bat chuc nang cua don dat hang*/ đã bật lại chỗ cmt lúc trước
            txtMaDonDatHang.Enabled = false;
            dteNGAY.Enabled = false;

            txtNhaCungCap.Enabled = true;
            txtMaNhanVien.Enabled = false;

            txtMaKho.Enabled = false;
            btnChonKhoHang.Enabled = true;

            /*Tat chuc nang cua chi tiet don hang*/
            txtMaVatTu.Enabled = false;
            btnChonVatTu.Enabled = false;
            txtSoLuong.Enabled = false;
            txtDonGia.Enabled = false;

            /*Bat cac grid control len*/
            gcDonDatHang.Enabled = true;
            gcChiTietDonDatHang.Enabled = true;


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
                this.btnMENU.Enabled = false;
                this.btnTHOAT.Enabled = true;

                this.groupBoxDonDatHang.Enabled = false;



            }

            /* CHI NHANH & USER co the xem - xoa - sua du lieu nhung khong the 
             chuyen sang chi nhanh khac*/
            if (Program.role == "CHINHANH" || Program.role == "USER")
            {
                cmbCHINHANH.Enabled = false;

                this.btnTHEM.Enabled = true;
                bool turnOn = (bdsDonDatHang.Count > 0) ? true : false;
                this.btnXOA.Enabled = turnOn;
                this.btnGHI.Enabled = true;

                this.btnHOANTAC.Enabled = false;
                this.btnLAMMOI.Enabled = true;
                this.btnMENU.Enabled = true;
                this.btnTHOAT.Enabled = true;

                this.txtMaDonDatHang.Enabled = false;

            }
        }

        // chọn chế độ làm việc là CHI TIẾT ĐƠN ĐẶT HÀNG
        private void btnCheDoChiTietDonDatHang_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            /*Step 0*/
            btnMENU.Links[0].Caption = "Chi Tiết Đơn Đặt Hàng";

            /*Step 1*/
            bds = bdsChiTietDonDatHang;
            gc = gcChiTietDonDatHang; // chỗ này bên chi tiết phiếu nhập mà gc = gcPhieuNhap

            //MessageBox.Show("Chế Độ Làm Việc Chi Tiết Đơn Đặt Hàng", "Thông báo", MessageBoxButtons.OK);

            /*Step 2*/
            /*Tat chuc nang don dat hang*/
            txtMaDonDatHang.Enabled = false;
            dteNGAY.Enabled = false;

            txtNhaCungCap.Enabled = false;
            txtMaNhanVien.Enabled = false;

            txtMaKho.Enabled = false;
            btnChonKhoHang.Enabled = false;

            /*Bat chuc nang cua chi tiet don hang*/
            txtMaVatTu.Enabled = false;
            btnChonVatTu.Enabled = false;
            txtSoLuong.Enabled = true;
            txtDonGia.Enabled = true;


            /*Bat cac grid control len*/
            // cho các cái grid sáng lên
            gcDonDatHang.Enabled = true;
            gcChiTietDonDatHang.Enabled = true;

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
                this.btnMENU.Enabled = false;
                this.btnTHOAT.Enabled = true;

                this.groupBoxDonDatHang.Enabled = false;


            }

            /* CHI NHANH & USER co the xem - xoa - sua du lieu nhung khong the 
             chuyen sang chi nhanh khac*/
            if (Program.role == "CHINHANH" || Program.role == "USER")
            {
                cmbCHINHANH.Enabled = false;

                this.btnTHEM.Enabled = true;
                bool turnOn = (bdsChiTietDonDatHang.Count > 0) ? true : false;
                this.btnXOA.Enabled = turnOn;

                this.btnGHI.Enabled = true;  // đã sửa false thành true

                this.btnHOANTAC.Enabled = false;
                this.btnLAMMOI.Enabled = true;
                this.btnMENU.Enabled = true;
                this.btnTHOAT.Enabled = true;

                this.txtMaDonDatHang.Enabled = false;

            }
        }

        private void btnTHOAT_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            this.Dispose();
        }

        private void btnTHEM_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            /*Step 1*/
            /*lấy vị trí hiện tại của con trỏ*/
            viTri = bds.Position;
            dangThemMoi = true;


            /*Step 2*/
            /*AddNew tự động nhảy xuống cuối thêm 1 dòng mới*/
            //   bds.AddNew(); đã cmt

            if (btnMENU.Links[0].Caption == "Đơn Đặt Hàng")
            {
                bdsDonDatHang.AddNew(); // đã thêm
                this.txtMaDonDatHang.Enabled = true;
                //this.txtMaKho.Text = "";
                this.dteNGAY.EditValue = DateTime.Now;
                this.dteNGAY.Enabled = false;
                this.txtNhaCungCap.Enabled = true;
                this.txtMaNhanVien.Text = Program.userName;
                this.btnChonKhoHang.Enabled = true;

                /*Gan tu dong may truong du lieu nay*/
                ((DataRowView)(bdsDonDatHang.Current))["MANV"] = Program.userName;
                ((DataRowView)(bdsDonDatHang.Current))["NGAY"] = DateTime.Now;
            }

            if (btnMENU.Links[0].Caption == "Chi Tiết Đơn Đặt Hàng")
            {
                bdsChiTietDonDatHang.AddNew(); // đã thêm
                // check coi người đăng nhập với người lâp phiếu có phải cùng một người không nếu
                // không cùng thì k cho sửa đơn đặt hàng do người khác lập phiếu
                DataRowView drv = ((DataRowView)bdsDonDatHang[bdsDonDatHang.Position]);
                String maNhanVien = drv["MANV"].ToString();
                if (Program.userName != maNhanVien) // user name cũng là mã nhân viên chỗ này cận thận nếu như nhập tay là dễ sai
                {
                    MessageBox.Show("Bạn không thêm chi tiết đơn hàng trên phiếu không phải do mình tạo", "Thông báo", MessageBoxButtons.OK);
                    bdsChiTietDonDatHang.RemoveCurrent();
                    return;
                }




                this.txtMaVatTu.Enabled = false;
                this.btnChonVatTu.Enabled = true;

                this.txtSoLuong.Enabled = true;
                this.txtSoLuong.EditValue = 1;

                this.txtDonGia.Enabled = true;
                this.txtDonGia.EditValue = 1;
            }


            /*Step 3*/
            this.btnTHEM.Enabled = false;
            this.btnXOA.Enabled = false;
            this.btnGHI.Enabled = true;

            this.btnHOANTAC.Enabled = true;
            this.btnLAMMOI.Enabled = false;
            this.btnMENU.Enabled = false;
            this.btnTHOAT.Enabled = false;
        }


        /**************************************************
         * ham nay kiem tra du lieu dau vao
         * true là qua hết
         * false là thiếu một dữ liệu nào đó
         **************************************************/
        private bool kiemTraDuLieuDauVao(String cheDo)
        {
            if (cheDo == "Đơn Đặt Hàng")
            {
                if (txtMaDonDatHang.Text == "")
                {
                    MessageBox.Show("Không thể bỏ trống mã đơn hàng", "Thông báo", MessageBoxButtons.OK);
                    return false;
                }
                if (txtMaDonDatHang.Text.Length > 8)
                {
                    MessageBox.Show("Mã đơn đặt hàng không quá 8 kí tự", "Thông báo", MessageBoxButtons.OK);
                    return false;
                }
                if (txtMaNhanVien.Text == "")
                {
                    MessageBox.Show("Không thể bỏ trống mã nhân viên", "Thông báo", MessageBoxButtons.OK);
                    return false;
                }
                if (txtNhaCungCap.Text == "")
                {
                    MessageBox.Show("Không thể bỏ trống nhà cung cấp", "Thông báo", MessageBoxButtons.OK);
                    return false;
                }
                if (txtNhaCungCap.Text.Length > 100)
                {
                    MessageBox.Show("Tên nhà cung cấp không quá 100 kí tự", "Thông báo", MessageBoxButtons.OK);
                    return false;
                }
                if (txtMaKho.Text == "")
                {
                    MessageBox.Show("Không thể bỏ trống mã kho", "Thông báo", MessageBoxButtons.OK);
                    return false;
                }
            }

            if (cheDo == "Chi Tiết Đơn Đặt Hàng")
            {
                if (txtMaVatTu.Text == "")
                {
                    MessageBox.Show("Không thể bỏ trống mã vật tư", "Thông báo", MessageBoxButtons.OK);
                    return false;
                }
                if (txtSoLuong.Value < 0 || txtDonGia.Value < 0)
                {
                    MessageBox.Show("Không thể nhỏ hơn 1", "Thông báo", MessageBoxButtons.OK);
                    return false;
                }
                /*
                if( txtSoLuong.Value > Program.soLuongVatTu)
                {
                    MessageBox.Show("Sô lượng đặt mua lớn hơn số lượng vật tư hiện có", "Thông báo", MessageBoxButtons.OK);
                    return false;
                }*/
            }
            return true;
        }



        /**************************************************
         * trả về một câu truy vấn để phục hồi dữ liệu
         * 
         * kết quả trả về dựa trên chế độ đang sử dụng
         **************************************************/
        private String taoCauTruyVanHoanTac(String cheDo)
        {
            String cauTruyVan = "";
            DataRowView drv;


            /*đang chỉnh sửa đơn đặt hàng*/
            if (cheDo == "Đơn Đặt Hàng" && dangThemMoi == false)
            {
                drv = ((DataRowView)bdsDonDatHang[bdsDonDatHang.Position]);
                vitriddh = bdsDonDatHang.Position; // đã thêm ngày 15/5

                /*Ngày cần được xử lý đặc biệt*/
                DateTime ngay = ((DateTime)drv["NGAY"]);
                // SỬA THÌ UPDATE
                cauTruyVan = "UPDATE DBO.DATHANG " +
                    "SET " +
                    "NGAY = CAST('" + ngay.ToString("yyyy-MM-dd") + "' AS DATETIME), " +
                    "NhaCC = '" + drv["NhaCC"].ToString().Trim() + "', " +
                    "MANV = '" + drv["MANV"].ToString().Trim() + "', " +
                    "MAKHO = '" + drv["MAKHO"].ToString().Trim() + "' " +
                    "WHERE MasoDDH = '" + drv["MasoDDH"].ToString().Trim() + "'";
            }
            /*Dang xoa don dat hang*/
            if (cheDo == "Đơn Đặt Hàng" && dangThemMoi == true)
            {
                drv = ((DataRowView)bdsDonDatHang[bdsDonDatHang.Position]);
                DateTime ngay = ((DateTime)drv["NGAY"]);
                //XÓA THÌ INSERT 
                cauTruyVan = "INSERT INTO DBO.DATHANG(MasoDDH, NGAY, NhaCC, MaNV, MaKho) " +
                    "VALUES('" + drv["MasoDDH"] + "', '" +
                    ngay.ToString("yyyy-MM-dd") + "', '" +
                    drv["NhaCC"].ToString() + "', '" +
                    drv["MaNV"].ToString() + "', '" +
                    drv["MaKho"].ToString() + "' )";

            }

            /*Dang chinh sua chi tiet don dat hang*/
            //update
            if (cheDo == "Chi Tiết Đơn Đặt Hàng" && dangThemMoi == false)
            {
                drv = ((DataRowView)bdsChiTietDonDatHang[bdsChiTietDonDatHang.Position]);
                vitrictdh = bdsChiTietDonDatHang.Position; // đã thêm ngày 15/5
                cauTruyVan = "UPDATE DBO.CTDDH " +
                    "SET " +
                    "SOLUONG = " + drv["SOLUONG"].ToString() + " , " +
                    "DONGIA = " + drv["DONGIA"].ToString() + " " +
                    "WHERE MasoDDH = '" + drv["MasoDDH"].ToString().Trim() + "'" +
                    " AND MAVT = '" + drv["MAVT"].ToString().Trim() + "'";

            }
            // đã  thêm if dưới đã sửa
            if (cheDo == "Chi Tiết Đơn Đặt Hàng" && dangThemMoi == true)
            {
                drv = ((DataRowView)bdsChiTietDonDatHang[bdsChiTietDonDatHang.Position]);
                //      DateTime ngay = ((DateTime)drv["NGAY"]);
                //XÓA THÌ INSERT 
                cauTruyVan = "INSERT INTO DBO.CTDDH(MasoDDH, MAVT, SOLUONG, DONGIA)" +
                    "VALUES('" + drv["MasoDDH"] + "', '" +
                    drv["MAVT"].ToString() + "', '" +
                    drv["SOLUONG"].ToString() + "', '" +
                    drv["DONGIA"].ToString() + "' )";

            }
            return cauTruyVan;
        }



        /**************************************************
         * Step 1: Kiem tra xem day co phai nguoi lap don hang hay không
         * Step 2: lay che do dang lam viec, kiem tra du lieu dau vao. Neu OK thi 
         * tiep tuc tao cau truy van hoan tac neu dangThemMoi = false
         * Step 3: kiem tra xem cai ma don hang nay da ton tai chua ?
         *          Neu co thi ket thuc luon
         *          Neu khong thi cho them moi
         **************************************************/
        private void btnGHI_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            //  viTri = bdsDonDatHang.Position;đã cmt ngày 15/5
            vitriddh = bdsDonDatHang.Position;
            vitrictdh = bdsChiTietDonDatHang.Position;

            /*Step 1*/
            DataRowView drv = ((DataRowView)bdsDonDatHang[bdsDonDatHang.Position]);
            /*lay maNhanVien & maDonDatHang de phong truong hop them chi tiet don hang thi se co ngay*/
            String maNhanVien = drv["MANV"].ToString();
            String maDonDatHang = drv["MasoDDH"].ToString().Trim();

            if (Program.userName != maNhanVien && dangThemMoi == false)
            {
                MessageBox.Show("Bạn không thể sửa phiếu do người khác lập", "Thông báo", MessageBoxButtons.OK);
                return;
            }
            // đã thêm 
            if (bdsChiTietDonDatHang.Count == 0 && btnMENU.Links[0].Caption == "Chi Tiết Đơn Đặt Hàng")
            {

                MessageBox.Show("Không có chi tiết đơn hàng nào để sửa", "Thông báo", MessageBoxButtons.OK);
                return;
            }


            /*Step 2*/
            String cheDo = (btnMENU.Links[0].Caption == "Đơn Đặt Hàng") ? "Đơn Đặt Hàng" : "Chi Tiết Đơn Đặt Hàng";

            bool ketQua = kiemTraDuLieuDauVao(cheDo);
            if (ketQua == false) return;

            String cauTruyVanHoanTac = taoCauTruyVanHoanTac(cheDo);
            //Console.WriteLine(cauTruyVanHoanTac);


            /*Step 3*/
            String maDonDatHangMoi = txtMaDonDatHang.Text;
            String cauTruyVan =
                   "DECLARE	@result int " +
                    "EXEC @result = sp_TimMaSoDonDangHang '" +
                    maDonDatHangMoi + "' " +
                    "SELECT 'Value' = @result";
            SqlCommand sqlCommand = new SqlCommand(cauTruyVan, Program.conn);

            try
            {
                Program.myReader = Program.ExecSqlDataReader(cauTruyVan);
                /*khong co ket qua tra ve thi ket thuc luon*/
                if (Program.myReader == null)
                {
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

            //==================THỬ NGHIÊM=================================
            String maVatTuMoi = txtMaVatTu.Text;
            String masoDDHMoi = txtMaDonDatHang.Text;
            String cauTruyVanVattu =
                   /*  "DECLARE	@ketqua int " +
                    "EXEC @ketqua = sp_KiemTraMaVatTuOCTDD '" +
                   " '" + masoDDHMoi + "' ,"+
                   "'" + maVatTuMoi + "'" +
                    "SELECT 'Value' = @result"; */

                   "DECLARE	@ketqua int " +
                    "EXEC @ketqua = sp_KiemTraMaVatTuOCTDD  '" + masoDDHMoi + "' , '" + maVatTuMoi + "'" +
                    "SELECT 'Value' = @ketqua";


            SqlCommand sqlCommand1 = new SqlCommand(cauTruyVanVattu, Program.conn);

            try
            {
                Program.myReader = Program.ExecSqlDataReader(cauTruyVanVattu);
                /*khong co ket qua tra ve thi ket thuc luon*/
                if (Program.myReader == null)
                {
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
            int ketqua = int.Parse(Program.myReader.GetValue(0).ToString());
            Program.myReader.Close();

            //=============================================================hết chỗ thử nghiệm===================

            /*Step 4*/
            //Console.WriteLine(txtMaNhanVien.Text);
            int viTriHienTai = bds.Position;
            int viTriMaDonDatHang = bdsDonDatHang.Find("MasoDDH", txtMaDonDatHang.Text);
            //   int ViTriMaVatTu = bdsChiTietDonDatHang.Find("MAVT", txtMaVatTu.Text); // đã thêm
            /******************************************************************
             * trường hợp thêm mới đơn đặt hàng  mới quan tâm xem nó đã tồn tại hay chưa??
             * 
             ******************************************************************/

            if (result == 1 && cheDo == "Đơn Đặt Hàng" && viTriHienTai != viTriMaDonDatHang)
            {
                MessageBox.Show("Mã đơn hàng này đã được sử dụng !\n\n", "Thông báo",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            else if (ketqua == 1 && cheDo == "Chi Tiết Đơn Đặt Hàng" && dangThemMoi == true)//&&  viTriHienTai != ViTriMaVatTu
            {
                MessageBox.Show("Không thể thêm trùng mã vật tư!\n\n", "Thông báo",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            /*****************************************************************
             * tất cả các trường hợp khác không cần quan tâm !!
             *****************************************************************/

            else
            {
                DialogResult dr = MessageBox.Show("Bạn có chắc muốn ghi dữ liệu vào cơ sở dữ liệu ?", "Thông báo",
                         MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
                if (dr == DialogResult.OK)
                {
                    try
                    {
                        //Console.WriteLine(txtMaNhanVien.Text);
                        /*TH1: thêm mới đơn đặt hàng*/
                        if (cheDo == "Đơn Đặt Hàng" && dangThemMoi == true)
                        {
                            cauTruyVanHoanTac =
                                "DELETE FROM DBO.DATHANG " +
                                "WHERE MasoDDH = '" + maDonDatHang + "'";
                        }

                        /*TH2: them moi chi tiet don hang*/
                        if (cheDo == "Chi Tiết Đơn Đặt Hàng" && dangThemMoi == true)
                        {

                            // gán tự động mấy trường dữ liệu này
                            ((DataRowView)(bdsChiTietDonDatHang.Current))["MasoDDH"] = ((DataRowView)(bdsDonDatHang.Current))["MasoDDH"];
                            ((DataRowView)(bdsChiTietDonDatHang.Current))["MAVT"] = Program.maVatTuDuocChon;
                            ((DataRowView)(bdsChiTietDonDatHang.Current))["SOLUONG"] =
                                txtSoLuong.Value;
                            ((DataRowView)(bdsChiTietDonDatHang.Current))["DONGIA"] =
                                (int)txtDonGia.Value;

                            cauTruyVanHoanTac =
                                "DELETE FROM DBO.CTDDH " +
                                "WHERE MasoDDH = '" + maDonDatHang + "' " +
                                "AND MAVT = '" + txtMaVatTu.Text.Trim() + "'";
                        }

                        /*TH3: chinh sua don hang */

                        /*TH4: chinh sua chi tiet don hang - > thi chi can may dong lenh duoi la xong*/

                        undoList.Push(cauTruyVanHoanTac);
                        //Console.WriteLine("cau truy van hoan tac");
                        //Console.WriteLine(cauTruyVanHoanTac);

                        this.bdsDonDatHang.EndEdit();
                        this.bdsChiTietDonDatHang.EndEdit();
                        this.donDatHangTableAdapter.Update(this.dataSet.DatHang);
                        this.chiTietDonDatHangTableAdapter.Update(this.dataSet.CTDDH);

                        this.btnTHEM.Enabled = true;
                        this.btnXOA.Enabled = true;
                        this.btnGHI.Enabled = true;

                        this.btnHOANTAC.Enabled = true;
                        this.btnLAMMOI.Enabled = true;
                        this.btnMENU.Enabled = true;
                        this.btnTHOAT.Enabled = true;

                        //this.groupBoxDonDatHang.Enabled = false;

                        /*cập nhật lại trạng thái thêm mới cho chắc*/
                        dangThemMoi = false;
                        this.btnChonVatTu.Enabled = false; // đã thêm ngày 17/4 kiểu ghi xong thì tắt cái chọn vật tư đi

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



        private void panelControl1_Paint(object sender, PaintEventArgs e)
        {

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
            /* Step 0 */ // đã ấn thêm nhưng chưa ấn ghi
            if (dangThemMoi == true && this.btnTHEM.Enabled == false)
            {
                dangThemMoi = false;

                /*dang o che do Don Dat Hang*/
                if (btnMENU.Links[0].Caption == "Đơn Đặt Hàng")
                {
                    this.txtMaDonDatHang.Enabled = false;

                    //this.dteNGAY.EditValue = DateTime.Now;
                    this.dteNGAY.Enabled = false;
                    this.txtNhaCungCap.Enabled = true;
                    //this.txtMaNhanVien.Text = Program.userName;
                    this.btnChonKhoHang.Enabled = true;

                    // đã thêm
                    this.btnTHEM.Enabled = true;
                    this.btnXOA.Enabled = true;
                    this.btnGHI.Enabled = true;

                    //this.btnHOANTAC.Enabled = false;
                    this.btnLAMMOI.Enabled = true;
                    this.btnMENU.Enabled = true;
                    this.btnTHOAT.Enabled = true;


                    bdsDonDatHang.CancelEdit();
                    /*xoa dong hien tai*/
                    bdsDonDatHang.RemoveCurrent();
                    // bdsDonDatHang.Position = viTri;
                    this.donDatHangTableAdapter.Fill(this.dataSet.DatHang); // đã thêm
                    bdsDonDatHang.Position = vitriddh;
                    return; // đã thêm
                }
                /*dang o che do Chi Tiet Don Dat Hang*/
                if (btnMENU.Links[0].Caption == "Chi Tiết Đơn Đặt Hàng")
                {
                    this.txtMaVatTu.Enabled = false;
                    this.btnChonVatTu.Enabled = false; // đã sửa ngày 17/5 true => false

                    this.txtSoLuong.Enabled = true;
                    this.txtSoLuong.EditValue = 1;
                    //this.txtSoLuong.EditValue = 1;

                    this.txtDonGia.Enabled = true;
                    this.txtDonGia.EditValue = 1;

                    //đã thêm 
                    this.btnTHEM.Enabled = true;
                    this.btnXOA.Enabled = true;
                    this.btnGHI.Enabled = true;

                    //this.btnHOANTAC.Enabled = false;
                    this.btnLAMMOI.Enabled = true;
                    this.btnMENU.Enabled = true;
                    this.btnTHOAT.Enabled = true;

                    //đã thêm
                    bdsChiTietDonDatHang.CancelEdit();

                    /*xoa dong hien tai*/
                    //  bdsChiTietDonDatHang.RemoveCurrent(); // lỗi ở chỗ này nè
                    this.chiTietDonDatHangTableAdapter.Fill(this.dataSet.CTDDH); // đã thêm
                    bdsChiTietDonDatHang.Position = vitrictdh;
                    //   bdsChiTietDonDatHang.Position = vitriddh; // sửa vitri
                    return;
                }
                /*
                this.btnTHEM.Enabled = true;
                this.btnXOA.Enabled = true;
                this.btnGHI.Enabled = true;
                //this.btnHOANTAC.Enabled = false;
                this.btnLAMMOI.Enabled = true;
                this.btnMENU.Enabled = true;
                this.btnTHOAT.Enabled = true;
                */

                //  bds.CancelEdit();
                /*xoa dong hien tai*/
                // bds.RemoveCurrent(); // lỗi ở chỗ này nè
                /* trở về lúc đầu con trỏ đang đứng*/
                // bds.Position = viTri;
                //return; đã cmt
            }
            // đã thêm ngày 15/5

            if (btnMENU.Links[0].Caption == "Đơn Đặt Hàng" && dangThemMoi == false)
            {
                bds.CancelEdit();
                this.donDatHangTableAdapter.Fill(this.dataSet.DatHang);
                this.chiTietDonDatHangTableAdapter.Fill(this.dataSet.CTDDH);
                bdsDonDatHang.Position = vitriddh;
            }

            if (btnMENU.Links[0].Caption == "Chi Tiết Đơn Đặt Hàng" && dangThemMoi == false)
            {
                bds.CancelEdit();
                this.donDatHangTableAdapter.Fill(this.dataSet.DatHang);
                this.chiTietDonDatHangTableAdapter.Fill(this.dataSet.CTDDH);
                bdsChiTietDonDatHang.Position = vitrictdh;
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
            // chỗ  này n 
            int n = Program.ExecSqlNonQuery(cauTruyVanHoanTac);

            this.donDatHangTableAdapter.Fill(this.dataSet.DatHang);
            this.chiTietDonDatHangTableAdapter.Fill(this.dataSet.CTDDH);

            // bdsDonDatHang.Position = vitriddh;// đã sửa 
            //bdsChiTietDonDatHang.Position = vitrictdh;// đã thêm

            // thử nghiệm ================
            this.btnChonVatTu.Enabled = false;// đã thêm ngày 17/5 hoàn tác thì tắt cái chọn vật tư đi

            bdsDonDatHang.Position = vitriddh;
            bdsChiTietDonDatHang.Position = vitrictdh;


            //=============================




        }

        private void btnLAMMOI_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                // do du lieu moi tu dataSet vao gridControl NHANVIEN
                this.donDatHangTableAdapter.Fill(this.dataSet.DatHang);
                this.chiTietDonDatHangTableAdapter.Fill(this.dataSet.CTDDH);

                this.gcDonDatHang.Enabled = true;
                this.gcChiTietDonDatHang.Enabled = true;

                bdsDonDatHang.Position = viTri;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi Làm mới" + ex.Message, "Thông báo", MessageBoxButtons.OK);
                return;
            }
        }

        /***************************************************************
         * ShowDialog is useful when you want to present info to a user, or let him change it, or get info from him before you do anything else.
         * 
         * Show is useful when you want to show information to the user but it is not important that you wait fro him to be finished.
         ***************************************************************/
        private void btnChonKhoHang_Click(object sender, EventArgs e)
        {
            // bấm vô chuyển tới form chọn kho hàng
            FrmChonKhoHang form = new FrmChonKhoHang();
            form.ShowDialog();


            this.txtMaKho.Text = Program.maKhoDuocChon;
        }

        private void btnChonVatTu_Click(object sender, EventArgs e)
        {
            FrmChonVatTu form = new FrmChonVatTu();
            form.ShowDialog();
            this.txtMaVatTu.Text = Program.maVatTuDuocChon;
        }



        /**
         * Step 1: lấy chế độ đang sử dụng và đặt dangThemMoi = true để phục vụ điều kiện tạo câu truy
         * vấn hoàn tác
         * Step 2: kiểm tra điều kiện theo chế độ đang sử dụng
         * Step 3: nạp câu truy vấn hoàn tác vào undolist
         * Step 4: Thực hiện xóa nếu OK
         */
        private void btnXOA_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            string cauTruyVan = "";
            string cheDo = (btnMENU.Links[0].Caption == "Đơn Đặt Hàng") ? "Đơn Đặt Hàng" : "Chi Tiết Đơn Đặt Hàng";

            dangThemMoi = true;// bật cái này lên để ứng với điều kiện tạo câu truy vấn

            if (cheDo == "Đơn Đặt Hàng")
            {
                /*Cái bdsChiTietDonHangHang là đại diện cho binding source riêng biệt của CTDDH
                 *Còn cTDDHBindingSource là lấy ngay từ trong data source DATHANG
                 */
                if (bdsChiTietDonDatHang.Count > 0)
                {
                    MessageBox.Show("Không thể xóa đơn đặt hàng này vì có chi tiết đơn đặt hàng", "Thông báo", MessageBoxButtons.OK);
                    return;
                }

                if (bdsPhieuNhap.Count > 0)
                {
                    MessageBox.Show("Không thể xóa đơn đặt hàng này vì có phiếu nhập", "Thông báo", MessageBoxButtons.OK);
                    return;
                }


            }
            if (cheDo == "Chi Tiết Đơn Đặt Hàng")
            {
                DataRowView drv = ((DataRowView)bdsDonDatHang[bdsDonDatHang.Position]);
                String maNhanVien = drv["MANV"].ToString();
                if (Program.userName != maNhanVien)
                {
                    MessageBox.Show("Bạn không xóa chi tiết đơn hàng trên phiếu không phải do mình tạo", "Thông báo", MessageBoxButtons.OK);
                    //  bdsChiTietDonDatHang.RemoveCurrent();
                    return;
                }
                // Oanh đã thêm
                if (bdsChiTietDonDatHang.Count == 0)
                {
                    MessageBox.Show("Không còn chi tiết đơn đặt hàng nào để xóa", "Thông báo", MessageBoxButtons.OK);
                    return;
                }

                /*FIX LỖI ĐƠN ĐH MÀ ĐƯỢC LẬP TRONG PHIẾU NHẬP THÌ KO CHO XÓA */
                DataRowView drvCT = ((DataRowView)bdsChiTietDonDatHang[bdsChiTietDonDatHang.Position]);
                //MessageBox.Show("maDDH : " + drvCT["MasoDDH"].ToString(), "Thông báo", MessageBoxButtons.OK);
                String maDDH = drvCT["MasoDDH"].ToString();
               
                String maVT = drvCT["MaVT"].ToString();
                //MessageBox.Show("maVT : " + maVT, "Thông báo", MessageBoxButtons.OK);

                String cauTruyVantemp =
               "DECLARE	@result int " +
                "EXEC @result = sp_KiemTraRangBuocCTDDHDaDuocSuDungTrongPNHayChua '" +
                maDDH + "', '" + maVT + "'" +
                "SELECT 'Value' = @result";
                SqlCommand sqlCommand = new SqlCommand(cauTruyVantemp, Program.conn);

                try
                {
                    Program.myReader = Program.ExecSqlDataReader(cauTruyVantemp);
                    /*khong co ket qua tra ve thi ket thuc luon*/
                    if (Program.myReader == null)
                    {
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
                //MessageBox.Show("result : " + result, "Thông báo", MessageBoxButtons.OK);

                if(result ==1)
                {
                    MessageBox.Show("Không thể xóa CTDDH vì đã tồn tại trong Phiếu nhập!", "Thông báo", MessageBoxButtons.OK);
                    return;
                }

                /*END FIX LỖI ĐƠN ĐH MÀ ĐƯỢC LẬP TRONG PHIẾU NHẬP THÌ KO CHO XÓA */
            }


            cauTruyVan = taoCauTruyVanHoanTac(cheDo);
            //Console.WriteLine("Line 753");
            //Console.WriteLine(cauTruyVan);
            undoList.Push(cauTruyVan);// thêm phần tử vào đỉnh stack

            /*Step 2*/
            if (MessageBox.Show("Bạn có chắc chắn muốn xóa không ?", "Thông báo",
                MessageBoxButtons.OKCancel) == DialogResult.OK)
            {
                try
                {
                    /*Step 3*/
                    //  viTri = bds.Position;
                    vitriddh = bdsDonDatHang.Position; // đã thêm

                    if (cheDo == "Đơn Đặt Hàng")
                    {
                        bdsDonDatHang.RemoveCurrent();
                    }
                    // đã thêm
                    vitrictdh = bdsChiTietDonDatHang.Position;
                    if (cheDo == "Chi Tiết Đơn Đặt Hàng")
                    {
                        bdsChiTietDonDatHang.RemoveCurrent();
                    }

                    // đổ lại dữ liệu
                    this.donDatHangTableAdapter.Connection.ConnectionString = Program.connstr;
                    this.donDatHangTableAdapter.Update(this.dataSet.DatHang);

                    this.chiTietDonDatHangTableAdapter.Connection.ConnectionString = Program.connstr;
                    this.chiTietDonDatHangTableAdapter.Update(this.dataSet.CTDDH);

                    /*Cap nhat lai do ben tren can tao cau truy van nen da dat dangThemMoi = true*/
                    dangThemMoi = false;
                    MessageBox.Show("Xóa thành công ", "Thông báo", MessageBoxButtons.OK);
                    this.btnHOANTAC.Enabled = true;
                }
                catch (Exception ex)
                {
                    /*Step 4*/
                    MessageBox.Show("Lỗi xóa nhân viên. Hãy thử lại\n" + ex.Message, "Thông báo", MessageBoxButtons.OK);
                    this.donDatHangTableAdapter.Connection.ConnectionString = Program.connstr;
                    this.donDatHangTableAdapter.Update(this.dataSet.DatHang);

                    this.chiTietDonDatHangTableAdapter.Connection.ConnectionString = Program.connstr;
                    this.chiTietDonDatHangTableAdapter.Update(this.dataSet.CTDDH);
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
        // combobox chi nhánh 
        private void cmbCHINHANH_SelectedIndexChanged(object sender, EventArgs e)
        {
            /*
            /*Neu combobox khong co so lieu thi ket thuc luon*/
            // coi lại chỗ này  1 xíu
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
                this.chiTietDonDatHangTableAdapter.Connection.ConnectionString = Program.connstr;
                this.chiTietDonDatHangTableAdapter.Fill(this.dataSet.CTDDH);

                this.donDatHangTableAdapter.Connection.ConnectionString = Program.connstr;
                this.donDatHangTableAdapter.Fill(this.dataSet.DatHang);

                this.phieuNhapTableAdapter.Connection.ConnectionString = Program.connstr;
                this.phieuNhapTableAdapter.Fill(this.dataSet.PhieuNhap);
            }
        }

        private void gcDonDatHang_Click(object sender, EventArgs e)
        {

        }

        private void bdsChiTietDonDatHang_CurrentChanged(object sender, EventArgs e)
        {

        }
    }
}