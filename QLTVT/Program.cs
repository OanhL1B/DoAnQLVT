using DevExpress.Skins;
using DevExpress.UserSkins;
using QLTVT.ReportForm;
using QLTVT.SubForm;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Windows.Forms;

namespace QLTVT
{

    /*
    * Data Source=DESKTOP-405626K\\MAYCHU: tên server gốc
    * Initial Catalog=QLVT: tên cơ sở dữ liệu
    * Integrated Security=true: đăng nhập với chế độ Window Authentication
    */
    static class Program
    {
        /*FIX LỖI CHỈ CHỌN ĐƯỢC MAVATTU ĐANG EDIT KHI EDIT CTPN P1*/
        public static string MaVatTuDangCoOCTPN = "";
        /*FIX LỖI CHỈ CHỌN ĐƯỢC MAVATTU ĐANG EDIT KHI EDIT CTPX P1*/
        public static string MaVatTuDangCoOCTPX = "";
        /*ĐỂ BẮT LỖI GIỮA THÊM VÀ EDIT CHITIETPHIEUNHAP*/
        public static bool dangThemMoiPhieuNhap = false;
        /*ĐỂ BẮT LỖI GIỮA THÊM VÀ EDIT CHITIETPHIEUXUAT*/
        public static bool dangThemMoiPhieuXuat = false;
        /*THÊM BIẾN ĐỂ FIX LỖI KIỂM TRA CHITIETPHIEUNHAP đã tồn tại hay chưa với [SP_KiemTraChiTietPhieuNhapDaTonTaiHayChua]*/
        public static string maPhieuNhapDuocChon = "";
        /*THÊM BIẾN ĐỂ FIX LỖI KIỂM TRA CHITIETPHIEUXUAT đã tồn tại hay chưa với [SP_KiemTraChiTietPhieuXuatDaTonTaiHayChua]*/
        public static string maPhieuXuatDuocChon = "";

        /**********************************************
         * conn: biến để kết nối tới cơ sở dữ liệu
         * connstr: chuỗi kết nối động
         **********************************************/
        public static SqlConnection conn = new SqlConnection();
        public static String connstr = "";
        public static String connstrPublisher = @"Data Source=DESKTOP-53GB0IH\MAYCHU;Initial Catalog=QLVT;Integrated Security=true";
        public static SqlDataReader myReader;

        public static String serverName = "";
        public static String serverNameLeft = "";
        public static String userName = "";

        public static String loginName = "";
        public static String loginPassword = "";

        public static String database = "QLVT";

        public static String remoteLogin = "HTKN";
        public static String remotePassword = "123456";

        public static String currentLogin = "";
        public static String currentPassword = "";

        /* role, staff, brand đang login */
        public static String role = "";
        public static String staff = "";
        public static int brand = 0;

        /* chotrong phần tạo mới đơn đặt hàng */
        public static string maKhoDuocChon = "";
        public static string maVatTuDuocChon = "";

        /* cho in bao cao hoat dong nhan vien */
        public static int soLuongVatTu = 0;
        public static string maDonDatHangDuocChon = "";
        public static string maDonDatHangDuocChonChiTiet = "";
        public static int donGia = 0;

        /* Cho HOAT DONG NHAN VIEN */
        public static string maNhanVienDuocChon = "";
        public static string hoTen = "";
        public static string diaChi = "";
        public static string ngaySinh = "";

        /*BindingSource -> liên kết dữ liệu từ bảng dữ liệu vào chương trình*/
        /*có 2 cột: TENCN, TENSERVER*/
        /* tồn tại : (login->end)*/
        public static BindingSource bindingSource = new BindingSource();

        public static FormDangNhap formDangNhap;
        public static Formmain formChinh; /*Đây mới chỉ là con trỏ, chưa phải object, về sau sẽ là object của formMain*/
        public static FormNhanVien formNhanVien;

        public static FrmChuyenChiNhanh formChuyenChiNhanh;
        public static FormVatTu formVatTu;
        public static FormKho formKho;

        public static FormDonDatHang formDonDatHang;
        public static FrmChonKhoHang formChonKhoHang;
        public static FormPhieuNhap formPhieuNhap;

        public static FrmChonDonDatHang formChonDonDatHang;
        public static FrmChonChiTietDonHang formChonChiTietDonHang;
        public static FormPhieuXuat formPhieuXuat;

        public static frmDanhSachNhanVien formDanhSachNhanVien;
        public static frmDanhSachVatTu formDanhSachVatTu;
        public static frmDonDatHangKhongCoPhieuNhap formDonHangKhongPhieuNhap;

        public static frmChiTietSoLuongTriGiaHangHoaNhapHoacXuat formChiTietSoLuongTriGiaHangHoaNhapXuat;
        public static frmHoatDongNhanVien formHoatDongNhanVien;
        public static frmTongHopNhapXuat formTongHopNhapXuat;
        
        /*******************************************************************
         * - mở kết nối tới server 
         * - y như đoạn code kết nối về csdl gốc, chỉ khác duy
         * nhất 1 điều là chạy trên đối tượng sqlconnection là conn toàn cục
         *******************************************************************/
        public static int KetNoi()
        {
            if (Program.conn != null && Program.conn.State == ConnectionState.Open) 
                Program.conn.Close();
            try
            {
                /*có 4 attribute giống y như bên csdl gốc chỉ # là USERID, password*/
                /*nếu báo sai thì chỉ có thể sai ở userName và password
                ko thể sai : Program.serverName được vì đâu có gõ tay đâu mà sai, chọn thôi mà
                và Program.database cũng ko thể sai được vì đã gán trực tiếp biến toàn cục là 
                1 csdl duy nhất xuyên suốt trên toàn dự án của ta*/
                Program.connstr = "Data Source=" + Program.serverName + ";Initial Catalog=" +
                       Program.database + ";User ID=" +
                       Program.loginName + ";password=" + Program.loginPassword;
                Program.conn.ConnectionString = Program.connstr;

                Program.conn.Open();
                return 1;
            }

            catch (Exception e)
            {
                MessageBox.Show("Kiểm tra lại tài khoản và mật khẩu!\nError : " + e.Message, "", MessageBoxButtons.OK);
                return 0;
            }
        }


        /* ExecSqlDataReader: thực hiện câu lệnh mà dữ liệu trả về chỉ dùng để xem & không thao tác với nó */
        public static SqlDataReader ExecSqlDataReader(String strLenh)
        {
            SqlDataReader myreader;
            /* VẤN ĐÁP: giờ ta muốn thực thi 1 câu lệnh SP (function or view) ở trong CSDL thì làm cách nào?
             * => chỉ có 1 cách là tạo ra đối tượng SqlCommand và sẽ nhúng vào trong đối tượng SqlCommand 
             * đó 2 tham số dưới dạng phương thức thiết lập, phương thức thứ nhất là chuỗi lệnh, 
             * phương thức tham số thứ 2 là kết nối conn.*/
            SqlCommand sqlcmd = new SqlCommand(strLenh, Program.conn);
            sqlcmd.CommandType = CommandType.Text; /*luôn luôn là chuỗi lệnh => dùng dạng TEXT*/
            if (Program.conn.State == ConnectionState.Closed)
                Program.conn.Open();
            try
            {
                myreader = sqlcmd.ExecuteReader(); return myreader;
            }
            catch (SqlException ex)
            {
                Program.conn.Close();
                MessageBox.Show(ex.Message);
                return null;
            }
        }

        /* ExecSqlDataTable: thực hiện câu lệnh mà dữ liệu trả về có thể xem - thêm - xóa - sửa tùy ý */
        /* Còn 1 cách tạo dt nữa là sẽ tải về dưới dạng là 1 dataReader trước 
           sau đó load dl đó vào dt (dùng LoadData) (ko cần dùng SqlDataAdapter nữa) */
        public static DataTable ExecSqlDataTable(String cmd)
        {
            DataTable dt = new DataTable();
            if (Program.conn.State == ConnectionState.Closed) Program.conn.Open();
            SqlDataAdapter da = new SqlDataAdapter(cmd, conn);
            da.Fill(dt);
            conn.Close();
            return dt;
        }

        /* Cập nhật trên một SP và không trả về giá trị */
        public static int ExecSqlNonQuery(String strlenh)
        {
            SqlCommand Sqlcmd = new SqlCommand(strlenh, conn);
            Sqlcmd.CommandType = CommandType.Text;
            Sqlcmd.CommandTimeout = 600;/* //10 phut   /*Vì những câu lệnh thực thi mà ko truy vấn thì
                                                        có khả năng là làm tự động hàng loạt bên csdl
                                                        Cho câu lệnh chạy có time max = 10p (vì mặc định 
                                                        chỉ có 60s mà đối vs dl trăm ngàn mẩu tin -> ko đủ)
                                                        Nếu trong 10p chạy unsucceess -> false */
            if (conn.State == ConnectionState.Closed) conn.Open();
            try
            {
                Sqlcmd.ExecuteNonQuery(); 
                conn.Close(); //chạy auto, ko return kq trả về, nếu return 0 -> success
                return 0;
            }
            catch (SqlException ex)
            {
                if (ex.Message.Contains("Error converting data type varchar to int"))
                    MessageBox.Show("Bạn format Cell lại cột \"Ngày Thi\" qua kiểu Number hoặc mở File Excel."); /*ko sài tới có thể bỏ qua*/
                else MessageBox.Show(ex.Message);
                conn.Close();
                return ex.State; //Trạng thái lỗi gửi từ RAISERROR trong SQL Server qua 
                                 //Chuỗi thông báo từ server gửi đến client thông qua cái ex này.
            }
        }
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            /*làm ntn thay vì truyền thằng " new FormChinh()" làm tham số vì : 
             Đang có ý đồ là giữa formDangNhap & formMain là sẽ giao tiếp với nhau
             và trao đổi dl với nhau. Cụ thể formDangNhap về sau này khi mà ta lấy về
             maNV, hoTen, tenNhom, ... thì ta phải gửi dl đó về cho formMain để nó hiển thị
             => Muốn như vậy thì phải có cái tên object để ta gọi.*/
            Program.formChinh = new Formmain();
            Application.Run(formChinh);
        }
    }
}
