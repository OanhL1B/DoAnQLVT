using DevExpress.XtraBars;
using QLTVT.ReportForm;
using QLTVT.SubForm;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace QLTVT
{

    public partial class Formmain : DevExpress.XtraBars.Ribbon.RibbonForm
    {
        //public System.Windows.Forms.ToolStripStatusLabel MANHANVIEN { get; set; }
        //public System.Windows.Forms.ToolStripStatusLabel HOTEN { get; set; }
        //public System.Windows.Forms.ToolStripStatusLabel NHOM { get; set; }
        public Formmain()
        {
            InitializeComponent();
        }
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



        /************************************************************
         *enableButtons: kích hoạt các tab chức năng và nút đăng xuất
         ************************************************************/
        public void enableButtons()
        {

            btnDangNhap.Enabled = false;
            btnDangXuat.Enabled = true;

            pageNhapXuat.Visible = true;
            pageBaoCao.Visible = true;
            btnLapTaiKhoan.Enabled = true;

            if (Program.role == "USER")
            {
                btnLapTaiKhoan.Enabled = false; // chi nhánh vẫn lập được tài khoản
            }

            //pageTaiKhoan.Visible = true;


        }



        /************************************************************
         * Dispose: giải phóng các form khỏi bộ nhớ. Ví dụ form nhân viên,...
         * Close: đóng hoàn toàn chương trình lại
         ************************************************************/
        private void logout()
        {
            foreach (Form f in this.MdiChildren)
                f.Dispose();
        }



        /************************************************************
         * Step 1: giải phóng các form khỏi bộ nhớ
         * Step 2: vô hiệu hóa các tab
         * Step 3: gọi lại form đăng nhập
         ************************************************************/
        private void btnDangXuat_ItemClick(object sender, ItemClickEventArgs e)
        {
            logout();

            btnDangNhap.Enabled = true;
            btnDangXuat.Enabled = false;

            pageNhapXuat.Visible = false;
            pageBaoCao.Visible = false;

            //FIX LỖI FALSE ENABLE KHI ĐĂNG XUẤT
            btnLapTaiKhoan.Enabled = false;

            //pageTaiKhoan.Visible = false;

            Form f = this.CheckExists(typeof(FormDangNhap));
            if (f != null)
            {
                f.Activate();// kích hoạt form chính
            }
            else
            {
                FormDangNhap form = new FormDangNhap();
                //form.MdiParent = this;
                form.Show();
            }

            Program.formChinh.MANHANVIEN.Text = "MÃ NHÂN VIÊN:";
            Program.formChinh.HOTEN.Text = "HỌ TÊN:";
            Program.formChinh.NHOM.Text = "VAI TRÒ:";

        }




        /************************************************************
         * Kiểm tra xem form đăng nhập đã có trong hệ thống chưa?
         * Step 1: Nếu có thì chạy form đăng nhập
         * Step 2: Nếu không thì khởi tạo một form đăng nhập mới rồi 
         * ném vào đưa vào xtraTabbedMdiManager
         * 
         * f.MdiParent = this; cái form này có form cha là this - tức form chính
         ************************************************************/
        private void btnDangNhap_ItemClick(object sender, ItemClickEventArgs e)
        {
            Form f = this.CheckExists(typeof(FormDangNhap));
            if (f != null)
            {
                f.Activate();
            }
            else
            {
                FormDangNhap form = new FormDangNhap();
                //form.MdiParent = this;
                form.Show();
            }
        }

        private void FormChinh_Load(object sender, EventArgs e)
        {
            //this.TopMost = true;
            //this.FormBorderStyle = FormBorderStyle.None;
            //this.WindowState = FormWindowState.Maximized;
        }

        private void btnThoat_ItemClick(object sender, ItemClickEventArgs e)
        {
            this.Close();
        }

        private void statusStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

        }

        private void MANV_Click(object sender, EventArgs e)
        {

        }

        private void toolStripStatusLabel1_Click(object sender, EventArgs e)
        {

        }

        private void toolStripStatusLabel1_Click_1(object sender, EventArgs e)
        {

        }

        private void btnNhanVien_ItemClick(object sender, ItemClickEventArgs e)
        {
            Form f = this.CheckExists(typeof(FormNhanVien));
            if (f != null)
            {
                f.Activate();
            }
            else
            {
                FormNhanVien form = new FormNhanVien();
                form.MdiParent = this;
                form.Show();
            }
        }

        private void btnVatTu_ItemClick(object sender, ItemClickEventArgs e)
        {
            Form f = this.CheckExists(typeof(FormVatTu));
            if (f != null)
            {
                f.Activate();
            }
            else
            {
                FormVatTu form = new FormVatTu();
                form.MdiParent = this;
                form.Show();
            }
        }

        private void btnKho_ItemClick(object sender, ItemClickEventArgs e)
        {
            Form f = this.CheckExists(typeof(FormKho));
            if (f != null)
            {
                f.Activate();
            }
            else
            {
                FormKho form = new FormKho();
                form.MdiParent = this;
                form.Show();
            }
        }

        private void btnDonDatHang_ItemClick(object sender, ItemClickEventArgs e)
        {
            Form f = this.CheckExists(typeof(FormDonDatHang));
            if (f != null)
            {
                f.Activate();
            }
            else
            {
                FormDonDatHang form = new FormDonDatHang();
                form.MdiParent = this;
                form.Show();
            }
        }

        private void btnPhieuNhap_ItemClick(object sender, ItemClickEventArgs e)
        {
            Form f = this.CheckExists(typeof(FormPhieuNhap));
            if (f != null)
            {
                f.Activate();
            }
            else
            {
                FormPhieuNhap form = new FormPhieuNhap();
                form.MdiParent = this;
                form.Show();
            }
        }

        private void btnPhieuXuat_ItemClick(object sender, ItemClickEventArgs e)
        {
            Form f = this.CheckExists(typeof(FormPhieuXuat));
            if (f != null)
            {
                f.Activate();
            }
            else
            {
                FormPhieuXuat form = new FormPhieuXuat();
                form.MdiParent = this;
                form.Show();
            }
        }

        private void btnDanhSachNhanVien_ItemClick(object sender, ItemClickEventArgs e)
        {
            Form f = this.CheckExists(typeof(frmDanhSachNhanVien));
            if (f != null)
            {
                f.Activate();
            }
            else
            {
                frmDanhSachNhanVien form = new frmDanhSachNhanVien();
                //form.MdiParent = this;
                form.Show();
            }
        }

        private void btnDanhSachVatTu_ItemClick(object sender, ItemClickEventArgs e)
        {
            Form f = this.CheckExists(typeof(frmDanhSachVatTu));
            if (f != null)
            {
                f.Activate();
            }
            else
            {
                frmDanhSachVatTu form = new frmDanhSachVatTu();
                //form.MdiParent = this;
                form.Show();
            }
        }

        private void btnDonHangKhongPhieuNhap_ItemClick(object sender, ItemClickEventArgs e)
        {
            Form f = this.CheckExists(typeof(frmDonDatHangKhongCoPhieuNhap));
            if (f != null)
            {
                f.Activate();
            }
            else
            {
                frmDonDatHangKhongCoPhieuNhap form = new frmDonDatHangKhongCoPhieuNhap();
                //form.MdiParent = this;
                form.Show();
            }
        }

        private void btnChiTietNhapXuat_ItemClick(object sender, ItemClickEventArgs e)
        {
            Form f = this.CheckExists(typeof(frmChiTietSoLuongTriGiaHangHoaNhapHoacXuat));
            if (f != null)
            {
                f.Activate();
            }
            else
            {
                frmChiTietSoLuongTriGiaHangHoaNhapHoacXuat form = new frmChiTietSoLuongTriGiaHangHoaNhapHoacXuat();
                //form.MdiParent = this;
                form.Show();
            }
        }

        private void btnHoatDongNhanVien_ItemClick(object sender, ItemClickEventArgs e)
        {
            Form f = this.CheckExists(typeof(frmHoatDongNhanVien));
            if (f != null)
            {
                f.Activate();
            }
            else
            {
                frmHoatDongNhanVien form = new frmHoatDongNhanVien();
                //form.MdiParent = this;
                form.Show();
            }
        }

        private void btnTongHopNhapXuat_ItemClick(object sender, ItemClickEventArgs e)
        {
            Form f = this.CheckExists(typeof(frmTongHopNhapXuat));
            if (f != null)
            {
                f.Activate();
            }
            else
            {
                frmTongHopNhapXuat form = new frmTongHopNhapXuat();
                //form.MdiParent = this;
                form.Show();
            }
        }

        private void btnLapTaiKhoan_ItemClick(object sender, ItemClickEventArgs e)
        {
            Form f = this.CheckExists(typeof(FormTaoTaiKhoan));
            if (f != null)
            {
                f.Activate();
            }
            else
            {
                FormTaoTaiKhoan form = new FormTaoTaiKhoan();
                //form.MdiParent = this;
                form.Show();
            }
        }

        private void statusStrip1_ItemClicked_1(object sender, ToolStripItemClickedEventArgs e)
        {

        }

        private void NHOM_Click(object sender, EventArgs e)
        {

        }
    }
}