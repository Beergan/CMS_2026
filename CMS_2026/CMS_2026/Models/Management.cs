using CMS_2026.Attributes;

namespace CMS_2026.Models
{
    [Feature(Name = "Management", TextEn = "MANAGEMENT", TextVi = "QUẢN TRỊ")]
    public class Management
    {
        [Feature(Name = "Management", TextEn = "MANAGEMENT", TextVi = "QUẢN TRỊ")]
        public enum PermissionAccount
        {
            [Function(TextEn = "management", TextVi = "Quản trị")]
            Management,
        }

        [Feature(Name = "Dashboard", TextEn = "DASHBOARD", TextVi = "TRANG CHỦ")]
        public enum PermissionDashboard
        {
            [Function(TextEn = "View dashboard", TextVi = "Xem trang chủ")]
            Dashboard_View
        }

        [Feature(Name = "OnlineUsers", TextEn = "WHO'S ONLINE", TextVi = "AI ĐANG ONLINE")]
        public enum PermissionOnlineUsers
        {
            [Function(TextEn = "View online users", TextVi = "Xem người đang online")]
            OnlineUsers_View
        }

        [Feature(Name = "VisitorHistory", TextEn = "VISITOR HISTORY", TextVi = "LỊCH SỬ TRUY CẬP")]
        public enum PermissionVisitorHistory
        {
            [Function(TextEn = "View visit history", TextVi = "Xem lịch sử truy cập")]
            VisitorHistory_View
        }

        [Feature(Name = "MediaLibrary", TextEn = "MEDIA LIBRARY", TextVi = "THƯ VIỆN HÌNH ẢNH")]
        public enum PermissionMedia
        {
            [Function(TextEn = "View media", TextVi = "Xem thư viện")]
            Media_View,

            [Function(TextEn = "Upload media", TextVi = "Tải lên hình ảnh")]
            Media_Upload,

            [Function(TextEn = "Delete media", TextVi = "Xoá hình ảnh")]
            Media_Delete
        }

        [Feature(Name = "Languages", TextEn = "LANGUAGES", TextVi = "DANH SÁCH NGÔN NGỮ")]
        public enum PermissionLanguage
        {
            [Function(TextEn = "View languages", TextVi = "Xem danh sách ngôn ngữ")]
            Language_View,

            [Function(TextEn = "Edit languages", TextVi = "Sửa ngôn ngữ")]
            Language_Edit
        }

        [Feature(Name = "Settings", TextEn = "SETTINGS", TextVi = "CẤU HÌNH CHUNG")]
        public enum PermissionSetting
        {
            [Function(TextEn = "View settings", TextVi = "Xem cấu hình")]
            Setting_View,

            [Function(TextEn = "Edit settings", TextVi = "Sửa cấu hình")]
            Setting_Edit
        }

        [Feature(Name = "Pages", TextEn = "PAGES", TextVi = "DANH SÁCH TRANG")]
        public enum PermissionPage
        {
            [Function(TextEn = "View pages", TextVi = "Xem danh sách trang")]
            Page_View,

            [Function(TextEn = "Add/Edit pages", TextVi = "Thêm - Sửa trang")]
            Page_Add_Edit,

            [Function(TextEn = "Delete pages", TextVi = "Xoá trang")]
            Page_Delete
        }

        [Feature(Name = "Category", TextEn = "CATEGORY", TextVi = "CHUYÊN MỤC")]
        public enum PermissionCategory
        {
            [Function(TextEn = "View categories", TextVi = "Danh sách chuyên mục")]
            Category_View,

            [Function(TextEn = "Add/Edit category", TextVi = "Thêm - Sửa chuyên mục")]
            Category_Save_Edit,

            [Function(TextEn = "Delete category", TextVi = "Xoá chuyên mục")]
            Category_Delete
        }

        [Feature(Name = "Products", TextEn = "PRODUCTS", TextVi = "SẢN PHẨM")]
        public enum PermissionProduct
        {
            [Function(TextEn = "View products", TextVi = "Danh sách sản phẩm")]
            Product_View,

            [Function(TextEn = "Add/Edit product", TextVi = "Thêm - Sửa sản phẩm")]
            Product_Add_Edit,

            [Function(TextEn = "Delete product", TextVi = "Xoá sản phẩm")]
            Product_Delete
        }

        [Feature(Name = "Posts", TextEn = "POSTS", TextVi = "BÀI VIẾT")]
        public enum PermissionPost
        {
            [Function(TextEn = "View posts", TextVi = "Danh sách bài viết")]
            Post_View,

            [Function(TextEn = "Add/Edit post", TextVi = "Thêm - Sửa bài viết")]
            Post_Add_Edit,

            [Function(TextEn = "Delete post", TextVi = "Xoá bài viết")]
            Post_Delete
        }

        [Feature(Name = "Registration", TextEn = "REGISTRATIONS", TextVi = "DANH SÁCH ĐĂNG KÝ")]
        public enum PermissionRegistration
        {
            [Function(TextEn = "View registrations", TextVi = "Xem danh sách đăng ký")]
            Register_View,

            [Function(TextEn = "Delete registrations", TextVi = "Xoá đăng ký")]
            Register_Delete
        }

        [Feature(Name = "Contact", TextEn = "CONTACT", TextVi = "LIÊN HỆ")]
        public enum PermissionContact
        {
            [Function(TextEn = "View contacts", TextVi = "Xem danh sách liên hệ")]
            Contact_View,

            [Function(TextEn = "Edit contact", TextVi = "Chỉnh sửa liên hệ")]
            Contact_Edit
        }

        [Feature(Name = "Orders", TextEn = "ORDERS", TextVi = "ĐƠN HÀNG")]
        public enum PermissionOrder
        {
            [Function(TextEn = "View orders", TextVi = "Xem danh sách đơn hàng")]
            Order_View,

            [Function(TextEn = "Edit order status", TextVi = "Cập nhật trạng thái đơn hàng")]
            Order_Status_Update,

            [Function(TextEn = "Delete orders", TextVi = "Xoá đơn hàng")]
            Order_Delete
        }
    }
}

