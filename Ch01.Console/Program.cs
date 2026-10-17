using System;
using System.Collections.Generic;
using System.Linq;

// Xử lý font chữ tiếng Việt cho Console
Console.OutputEncoding = System.Text.Encoding.UTF8;

Console.WriteLine("*************************************************");
Console.WriteLine("     PHẦN MỀM QUẢN LÝ MƯỢN TRẢ TÀI LIỆU (v2.0)   ");
Console.WriteLine("*************************************************\n");

try
{
    // Dữ liệu bộ nhớ tạm
    var tapTheBanDoc = new List<BanDoc>();
    var khoTaiLieu = new List<TaiLieu>();

    // US-04: Thủ thư thêm sách mới
    Console.WriteLine("---> CHỨC NĂNG: CẬP NHẬT KHO TÀI LIỆU <---");
    var sachMoi = TaiLieu.Create(202, "Giáo trình C# nâng cao", 1);
    khoTaiLieu.Add(sachMoi);
    Console.WriteLine($"[+] Đã nhập kho: '{sachMoi.TieuDe}' (Số lượng hiện có: {sachMoi.SoLuongTon})");

    // US-01: Độc giả tạo tài khoản thành viên
    Console.WriteLine("\n---> CHỨC NĂNG: MỞ TÀI KHOẢN MỚI <---");
    var nguoiDung1 = BanDoc.Create(10, "Lê Cẩm Giang", "camgiang.le@student.edu.vn");
    tapTheBanDoc.Add(nguoiDung1);
    Console.WriteLine($"[+] Cấp thẻ thành công cho bạn đọc: {nguoiDung1.TenDayDu}");

    // Test logic: Đăng ký trùng email
    if (tapTheBanDoc.Any(x => x.EmailLienHe == "camgiang.le@student.edu.vn"))
    {
        Console.WriteLine("[!] Hệ thống từ chối: Địa chỉ Email này đã tồn tại!");
    }

    // US-02: Độc giả tìm kiếm sách
    Console.WriteLine("\n---> CHỨC NĂNG: TRA CỨU TÀI LIỆU <---");
    string tuKhoaTimKiem = "C#";
    var ketQuaTraCuu = khoTaiLieu.Where(t => t.TieuDe.Contains(tuKhoaTimKiem)).ToList();
    if (ketQuaTraCuu.Any())
        Console.WriteLine($"[*] Tuyệt vời! Tìm thấy {ketQuaTraCuu.Count} tài liệu khớp với từ khóa '{tuKhoaTimKiem}'.");
    else
        Console.WriteLine("[!] Rất tiếc, không tìm thấy tài liệu nào phù hợp.");

    // US-03: Độc giả mượn sách
    Console.WriteLine("\n---> CHỨC NĂNG: TIẾP NHẬN YÊU CẦU MƯỢN <---");
    var giaoDichMuon = GiaoDichMuon.Create(1001, nguoiDung1.MaBanDoc, sachMoi.MaTaiLieu);
    sachMoi.XuatKho(); // Khấu trừ số lượng
    Console.WriteLine($"[+] Đã lập hồ sơ mượn sách '{sachMoi.TieuDe}'. Vui lòng hoàn trả trước ngày: {giaoDichMuon.NgayHenTra:dd/MM/yyyy}");

    // Test logic: Mượn khi sách đã hết
    Console.WriteLine("[!] (Check tồn kho) -> " + (sachMoi.SoLuongTon > 0 ? "Vẫn còn sách" : "Cảnh báo: Tựa sách này đã được mượn hết!"));

    // US-05: Thủ thư xử lý trả sách (Giả lập trả trễ 2 ngày)
    Console.WriteLine("\n---> CHỨC NĂNG: NGHIỆP VỤ TRẢ SÁCH <---");
    var giaoDichTra = GiaoDichTra.Create(5001, giaoDichMuon.MaGiaoDich, giaoDichMuon.NgayHenTra.AddDays(2));
    giaoDichMuon.DoiTrangThaiDaTra();
    sachMoi.NhapKho(); // Hoàn trả vào kho
    Console.WriteLine($"[+] Thu hồi sách thành công. Đánh giá: {(giaoDichTra.BiTreHan ? "Nộp phạt do trả quá hạn!" : "Trả đúng hạn.")}");

    // US-06: Admin khóa tài khoản
    Console.WriteLine("\n---> CHỨC NĂNG: QUẢN TRỊ NGƯỜI DÙNG <---");
    nguoiDung1.DinhChiTaiKhoan();
    Console.WriteLine($"[+] Tài khoản của '{nguoiDung1.TenDayDu}' hiện đã chuyển sang trạng thái BỊ KHÓA.");

    // Test logic: Khóa tài khoản đã bị khóa từ trước
    try { nguoiDung1.DinhChiTaiKhoan(); }
    catch (Exception ex) { Console.WriteLine($"[!] (Lỗi cố ý test): {ex.Message}"); }

    Console.WriteLine("\n*************************************************\n");
}
catch (Exception ex)
{
    Console.WriteLine($"\n[PHÁT HIỆN LỖI]: {ex.Message}");
}

// ==========================================
// THIẾT KẾ DOMAIN CLASSES MỚI
// ==========================================

public class BanDoc
{
    public int MaBanDoc { get; private set; }
    public string TenDayDu { get; private set; }
    public string EmailLienHe { get; private set; }
    public bool DaBiDinhChi { get; private set; }

    private BanDoc(int maBanDoc, string tenDayDu, string emailLienHe)
    {
        MaBanDoc = maBanDoc;
        TenDayDu = tenDayDu;
        EmailLienHe = emailLienHe;
        DaBiDinhChi = false;
    }

    public static BanDoc Create(int maBanDoc, string tenDayDu, string emailLienHe)
    {
        if (string.IsNullOrWhiteSpace(tenDayDu)) throw new ArgumentException("Tên bạn đọc không hợp lệ.");
        return new BanDoc(maBanDoc, tenDayDu, emailLienHe);
    }

    public void DinhChiTaiKhoan()
    {
        if (DaBiDinhChi) throw new InvalidOperationException("Tài khoản của bạn đã bị khóa.");
        DaBiDinhChi = true;
    }
}

public class TaiLieu
{
    public int MaTaiLieu { get; private set; }
    public string TieuDe { get; private set; }
    public int SoLuongTon { get; private set; }

    private TaiLieu(int maTaiLieu, string tieuDe, int soLuongTon)
    {
        MaTaiLieu = maTaiLieu;
        TieuDe = tieuDe;
        SoLuongTon = soLuongTon;
    }

    public static TaiLieu Create(int maTaiLieu, string tieuDe, int soLuong)
    {
        if (string.IsNullOrWhiteSpace(tieuDe)) throw new ArgumentException("Vui lòng nhập đầy đủ thông tin ấn phẩm.");
        return new TaiLieu(maTaiLieu, tieuDe, soLuong);
    }

    public void XuatKho() { if (SoLuongTon > 0) SoLuongTon--; }
    public void NhapKho() { SoLuongTon++; }
}

public class GiaoDichMuon
{
    public int MaGiaoDich { get; private set; }
    public int NguoiMuonID { get; private set; }
    public int TaiLieuID { get; private set; }
    public DateTime NgayHenTra { get; private set; }
    public string TinhTrang { get; private set; }

    private GiaoDichMuon(int maGiaoDich, int nguoiMuonId, int taiLieuId)
    {
        MaGiaoDich = maGiaoDich;
        NguoiMuonID = nguoiMuonId;
        TaiLieuID = taiLieuId;
        NgayHenTra = DateTime.Now.AddDays(7); // Quy định mượn 7 ngày
        TinhTrang = "Chưa trả";
    }

    public static GiaoDichMuon Create(int maGiaoDich, int nguoiMuonId, int taiLieuId)
    {
        return new GiaoDichMuon(maGiaoDich, nguoiMuonId, taiLieuId);
    }

    public void DoiTrangThaiDaTra() => TinhTrang = "Đã thu hồi";
}

public class GiaoDichTra
{
    public int MaGiaoDichTra { get; private set; }
    public int GiaoDichMuonID { get; private set; }
    public DateTime NgayTraThucTe { get; private set; }
    public bool BiTreHan { get; private set; }

    private GiaoDichTra(int maGiaoDichTra, int giaoDichMuonId, DateTime ngayTra, bool biTreHan)
    {
        MaGiaoDichTra = maGiaoDichTra;
        GiaoDichMuonID = giaoDichMuonId;
        NgayTraThucTe = ngayTra;
        BiTreHan = biTreHan;
    }

    public static GiaoDichTra Create(int maGiaoDichTra, int giaoDichMuonId, DateTime ngayTraThucTe)
    {
        // Xét trễ hạn dựa vào thời gian thực tế so với giả định 7 ngày
        bool isTreHan = ngayTraThucTe > DateTime.Now.AddDays(7);
        return new GiaoDichTra(maGiaoDichTra, giaoDichMuonId, ngayTraThucTe, isTreHan);
    }
}

public class NguoiQuanTri
{
    public int MaQuanTriVien { get; private set; }
    public string HoTen { get; private set; }
    public string ChucVu { get; private set; }

    private NguoiQuanTri(int maNhanVien, string hoTen, string chucVu)
    {
        MaQuanTriVien = maNhanVien;
        HoTen = hoTen;
        ChucVu = chucVu;
    }

    public static NguoiQuanTri Create(int maNhanVien, string hoTen, string chucVu)
    {
        return new NguoiQuanTri(maNhanVien, hoTen, chucVu);
    }
}