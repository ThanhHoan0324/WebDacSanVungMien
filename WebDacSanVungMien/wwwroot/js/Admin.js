// Hàm mở Modal Sửa Đặc Sản (Sử dụng AJAX để tải form)
function openEditDacSanModal(id) {
    // 1. Đặt nội dung 'Đang tải'
    document.getElementById('editDacSanBody').innerHTML =
        '<div class="text-center py-5">Đang tải dữ liệu Đặc Sản ID: ' + id + '...</div>';

    // 2. Mở Modal Sửa
    var editModal = new bootstrap.Modal(document.getElementById('editDacSanModal'), {});
    editModal.show();

    // 3. Gửi yêu cầu AJAX để tải Partial View
    $.get('/Admin/GetEditDacSanPartial', { id: id }) // SỬ DỤNG ĐƯỜNG DẪN TUYỆT ĐỐI
        .done(function (data) {
            document.getElementById('editDacSanBody').innerHTML = data;

            // Re-parse validation scripts sau khi tải AJAX content (QUAN TRỌNG)
            var form = $('#editDacSanBody form');
            form.removeData('validator');
            form.removeData('unobtrusiveValidation');
            $.validator.unobtrusive.parse(form);

        })
        .fail(function (jqXHR, textStatus, errorThrown) {
            document.getElementById('editDacSanBody').innerHTML =
                '<div class="text-red-500 py-5">Lỗi tải dữ liệu. Vui lòng kiểm tra console hoặc Action GetEditDacSanPartial.</div>';
        });
}

// Hàm xác nhận và gửi Form Xóa
function confirmDeleteDacSan(id) {
    if (confirm("Bạn có chắc chắn muốn xóa Đặc Sản ID: " + id + "? Hành động này không thể hoàn tác!")) {
        // Lưu ý: Cần thêm input hidden cho CSRF token vào form deleteForm trong View
        document.getElementById('deleteId').value = id;
        document.getElementById('deleteForm').submit();
    }
}

// Hàm này sẽ gửi yêu cầu duyệt hoặc chưa duyệt (AJAX)
function toggleApproval(specialtyId, isApproved) {

    const checkbox = document.getElementById('isApproved_' + specialtyId);
    checkbox.disabled = true;

    $.ajax({
        url: '/Admin/ToggleApproval', // SỬ DỤNG ĐƯỜNG DẪN TUYỆT ĐỐI
        type: 'POST',
        data: {
            id: specialtyId,
            isApproved: isApproved
        },
        headers: {
            'RequestVerificationToken': $('input[name="__RequestVerificationToken"]').val() // Lấy token từ form
        },
        success: function (response) {
            if (response.success) {
                const msg = isApproved ? "Đã duyệt đặc sản ID: " : "Đã chuyển sang Chờ duyệt ID: ";

                // HIỂN THỊ VÀ ẨN THÔNG BÁO AJAX
                $('<div class="bg-emerald-500 text-white p-2 rounded-lg shadow-xl mb-4 transition duration-300 transform animate-pulse-fadeout">' + msg + specialtyId + '</div>')
                    .appendTo('.fixed.top-20.right-5.z-50.w-full.max-w-sm')
                    .delay(1000)
                    .fadeOut(500, function () {
                        $(this).remove();
                    });

                checkbox.disabled = false;
            } else {
                alert('Lỗi: ' + response.message);
                checkbox.checked = !isApproved;
                checkbox.disabled = false;
            }
        },
        error: function (xhr, status, error) {
            alert('Lỗi kết nối hoặc lỗi server: Vui lòng kiểm tra lại.');
            checkbox.checked = !isApproved;
            checkbox.disabled = false;
        }
    });
}
// Đảm bảo mã tự động ẩn TempData cũng nằm trong View, hoặc di chuyển vào đây nếu cần