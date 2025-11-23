$(document).ready(function () {
    var $tabBg = $("#tab-bg");

    function kichhoatDangNhap() {
        // Form Đăng Nhập: Hiển thị (active-form)
        $("#dangnhapform").addClass("active-form");

        // Form Đăng Ký: Bị ẩn/Đẩy ra khỏi màn hình (Loại bỏ active-form)
        $("#dangkyform").removeClass("active-form");

        // Di chuyển thanh bg
        $tabBg.css("left", "0");
    }

    function kichhoatDangKy() {
        // Form Đăng Nhập: Bị ẩn/Đẩy ra khỏi màn hình (Loại bỏ active-form)
        $("#dangnhapform").removeClass("active-form");

        // Form Đăng Ký: Hiển thị (active-form)
        $("#dangkyform").addClass("active-form");

        // Di chuyển thanh bg
        $tabBg.css("left", "50%");
    }

    $("#nut-dangnhap").click(kichhoatDangNhap);
    $("#nut-dangky").click(kichhoatDangKy);

    // Mặc định hiển thị login
    // Giữ nguyên đoạn này để đảm bảo trạng thái khởi tạo
    kichhoatDangNhap();
});