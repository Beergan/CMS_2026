using CMS_2026.Attributes;
using CMS_2026.Common;

namespace CMS_2026.Models
{
    public class Config
    {
        public Config()
        {
            ComplainItems = new ComplainItem[]
            {
                new ComplainItem {
                    Name ="Mr.Sang",
                    Phone ="0352889129"
                },
                new ComplainItem {
                    Name ="Mr.Pờm",
                    Phone ="0372461306"
                },
            };
            this.WebTitle = "VIET GROUP";
            this.Favicon = "/assets/images/pre-logo.png";
            this.GoogleGtag = "1234";
            this.LinkFacebook = "https://facebook.com";
            this.LinkInstagram = "https://www.instagram.com";
            this.LinkGoogle = "https://www.google.com";
            this.LinkTwitter = "#";
            this.Company = new CompanyInfo();
        }

        public string? email { get; set; }
        public string? pass { get; set; }

        [Field(
            Title = "Tiêu đề website",
            Required = false,
            Control = InputControlType.TextBox)]
        public string WebTitle { get; set; } = string.Empty;

        [Field(
            Title = "Tên website",
            Required = false,
            Control = InputControlType.TextBox)]
        public string? Description { get; set; }
        public string? WebDescription 
        { 
            get => Description; 
            set => Description = value; 
        }

        [Field(
            Title = "Favicon",
            Required = false,
            Control = InputControlType.Image)]
        public string Favicon { get; set; } = string.Empty;

        [Field(
            Title = "Logo",
            Required = false,
            Control = InputControlType.Image)]
        public string? Logo { get; set; }

        [Field(
            Title = "Google Gtag",
            Required = false,
            Control = InputControlType.TextBox)]
        public string GoogleGtag { get; set; } = string.Empty;

        [Field(
           Title = "Link facebook",
           Required = false,
           Control = InputControlType.TextBox)]
        public string LinkFacebook { get; set; } = string.Empty;

        [Field(
          Title = "Link twitter",
          Required = false,
          Control = InputControlType.TextBox)]
        public string LinkTwitter { get; set; } = string.Empty;

        [Field(
          Title = "Link youtube",
          Required = false,
          Control = InputControlType.TextBox)]
        public string? LinkYoutube { get; set; }

        [Field(
         Title = "Link Messenger",
         Required = false,
         Control = InputControlType.TextBox)]
        public string? LinkMessenger { get; set; }

        [Field(
           Title = "Link linkedin",
           Required = false,
           Control = InputControlType.TextBox)]
        public string? LinkLinkedin { get; set; }

        [Field(
           Title = "Link instagram",
           Required = false,
           Control = InputControlType.TextBox)]
        public string LinkInstagram { get; set; } = string.Empty;

        [Field(
           Title = "Link google",
           Required = false,
           Control = InputControlType.TextBox)]
        public string LinkGoogle { get; set; } = string.Empty;

        [Field(
           Title = "Giá lọc",
           Required = false,
           Control = InputControlType.Number)]
        public string? Price { get; set; }

        [Field(Title = "Danh sách menu", ChildTitle = "Menu chính")]
        public MenuItem[]? MainMenus { get; set; }

        [Field(Title = "Thông tin công ty")]
        public CompanyInfo Company { get; set; } = new();

        [Field(Title = "Thông tin tích hợp GHTK")]
        public GHTKConfig? GHTK { get; set; }

        //[Field(Title = "Thông tin tích hợp VNpay")]
        public VNpayConfig? VNPay { get; set; }

        [Field(Title = "Địa chỉ lấy hàng")]
        public PickAddress? PickAddress { get; set; }

        public ComplainItem[] ComplainItems { get; set; } = Array.Empty<ComplainItem>();

        [Field(Title = "Các liên kết")]
        public Link? Link { get; set; }

        //[Field(Title = "Cầu hình VietQR")]
        public BankKing? _BankKing { get; set; }

        [Field(Title = "Banner quảng cáo")]
        public Protomo? _Protomo { get; set; }

        public string? Advertisemant { get; set; }
    }

    public class Protomo
    {
        [Field(
           Title = "Tiêu đề",
           Required = false,
           Control = InputControlType.TextBox)]
        public string? TitleNew { get; set; }

        [Field(
           Title = "Quảng cáo",
           Required = false,
           Control = InputControlType.Image)]
        public string? BanerNew { get; set; }

        [Field(
           Title = "Đường dẫn",
           Required = false,
           Control = InputControlType.Link)]
        public string? LinkNews { get; set; }
    }

    public class BankKing
    {
        [Field(Title = "Mã ngân hàng", Required = false, Control = InputControlType.TextBox)]
        public string? Title { get; set; }

        [Field(Title = "Số tài khoản", Required = false, Control = InputControlType.TextBox)]
        public string? Number { get; set; }

        [Field(Title = "Họ và tên người nhận", Required = false, Control = InputControlType.TextBox)]
        public string? Name { get; set; }
    }

    public class VNpayConfig
    {
        [Field(
            Title = "VNP_TmnCode",
            Required = false,
            Control = InputControlType.TextBox)]
        public string? VNP_TmnCode { get; set; }

        [Field(
            Title = "VNP_HashSecret",
            Required = false,
            Control = InputControlType.TextBox)]
        public string? VNP_HashSecret { get; set; }

        [Field(
            Title = "VNP_Url",
            Required = false,
            Control = InputControlType.TextBox)]
        public string? VNP_Url { get; set; }

        [Field(
            Title = "đường dẫn VNpay",
            Required = false,
            Control = InputControlType.Link)]
        public string? LinkVPN { get; set; }
    }

    public class Link
    {
        [Field(
            Title = "Trang đặt hàng",
            Required = false,
            Control = InputControlType.TextBox)]
        public string? PageMakeOrder { get; set; }

        [Field(
          Title = "Trang giỏ hàng",
          Required = false,
          Control = InputControlType.TextBox)]
        public string? PageCart { get; set; }

        [Field(
         Title = "Trang thông tin đơn hàng",
         Required = false,
         Control = InputControlType.TextBox)]
        public string? PageInfoOrder { get; set; }

        [Field(
            Title = "Trang các nhãn hiệu",
            Required = false,
            Control = InputControlType.TextBox)]
        public string? PageAllBrands { get; set; }

        [Field(
           Title = "Trang liên hệ",
           Required = false,
           Control = InputControlType.TextBox)]
        public string? PageContact { get; set; }

        [Field(
       Title = "Trang blog",
       Required = false,
       Control = InputControlType.Link)]
        public string? PageBlog { get; set; }

        [Field(
       Title = "Trang trạng thái",
       Required = false,
       Control = InputControlType.Link)]
        public string? OrderStat { get; set; }

        [Field(
       Title = "Trang Shop",
       Required = false,
       Control = InputControlType.Link)]
        public string? PageShop { get; set; }

        [Field(
       Title = "Tìm kiếm",
       Required = false,
       Control = InputControlType.Link)]
        public string? Seach { get; set; }

        [Field(
            Title = "Trang đăng ký",
            Required = false,
            Control = InputControlType.Link)]
        public string? Register { get; set; }

        [Field(
       Title = "Trang đăng nhập ",
       Required = false,
       Control = InputControlType.Link)]
        public string? Login { get; set; }

        [Field(
       Title = "Trang cá nhân",
       Required = false,
       Control = InputControlType.Link)]
        public string? Myacount { get; set; }
    }

    public class MenuItem
    {
        [Field(
           Title = "en:Title|vi:Tiêu đề",
           Required = false,
           Control = InputControlType.TextBox)]
        public string? Title { get; set; }

        [Field(
           Title = "en:Link|vi:Đường dẫn",
           Required = false,
           Control = InputControlType.Link)]
        public string? Href { get; set; }

        [Field(
           Title = "en:Menu1|vi:Kiểu đơn",
           Required = false,
           Control = InputControlType.CheckBox)]
        public bool Menu1 { get; set; }

        [Field(
       Title = "en:Menu2|vi:Bộ sưu tập",
       Required = false,
       Control = InputControlType.CheckBox)]
        public bool Collection { get; set; }

        [Field(
            Title = "en:Menu3|vi:Kiểu sản phẩm",
            Required = false,
            Control = InputControlType.CheckBox)]
        public bool ProductBuuton { get; set; }

        [Field(Title = "Các menu con", ChildTitle = "Menu con")]
        public SubMenu[]? SubMenus { get; set; }

        [Field(Title = "Đường dẫn sản phẩm", ChildTitle = "Menu con")]
        public ProductMenu[]? ProductMenus { get; set; }
    }

    public class ProductMenu
    {
        [Field(
           Title = "en:Link|vi:Đường dẫn",
           Required = false,
           Control = InputControlType.Link)]
        public string? Href { get; set; }
    }

    public class SubMenu
    {
        [Field(
           Title = "en:Title|vi:Tiêu đề",
           Required = false,
           Control = InputControlType.TextBox)]
        public string? Title { get; set; }

        [Field(
           Title = "en:Link|vi:Đường dẫn",
           Required = false,
           Control = InputControlType.Link)]
        public string? Href { get; set; }

        [Field(Title = "Các menu con", ChildTitle = "Menu con")]
        public SubMenu2[]? SubMenus { get; set; }

        [Field(
        Title = "en:Image|vi:Hình ảnh",
        Required = false,
        Control = InputControlType.Image)]
        public string? Image { get; set; }
    }

    public class SubMenu2
    {
        [Field(
           Title = "en:Title|vi:Tiêu đề",
           Required = false,
           Control = InputControlType.TextBox)]
        public string? Title { get; set; }

        [Field(
           Title = "en:Link|vi:Đường dẫn",
           Required = false,
           Control = InputControlType.Link)]
        public string? Href { get; set; }
    }

    public class ComplainItem
    {
        [Field(
           Title = "en:Name|vi:Tên",
           Required = false,
           Control = InputControlType.TextBox)]
        public string? Name { get; set; }

        [Field(
           Title = "en:Phone|vi:Số điện thoại",
           Required = false,
           Control = InputControlType.TextBox)]
        public string? Phone { get; set; }
    }

    public class CompanyInfo
    {
        [Field(
            Title = "en:Slogan|vi:Slogan",
            Required = false,
            Control = InputControlType.TextBox)]
        public string? Slogan { get; set; }

        [Field(
            Title = "en:Company name|vi:Tên công ty",
            Required = false,
            Control = InputControlType.TextBox)]
        public string? CompanyName { get; set; }

        [Field(
            Title = "en:Số Fax|vi:Số Fax",
            Required = false,
            Control = InputControlType.TextBox)]
        public string? Fax { get; set; }

        [Field(
            Title = "en:Bank number|vi:Số tài khoản",
            Required = false,
            Control = InputControlType.TextBox)]
        public string? BankNumber { get; set; }

        [Field(
           Title = "en:Bank name|vi:Tên ngân hàng",
           Required = false,
           Control = InputControlType.TextBox)]
        public string? BankName { get; set; }

        [Field(
            Title = "Whatsapp",
            Required = false,
            Control = InputControlType.TextBox)]
        public string? Whatsapp { get; set; }

        [Field(
            Title = "Zalo",
            Required = false,
            Control = InputControlType.TextBox)]
        public string? Zalo { get; set; }

        [Field(
            Title = "Hotline",
            Required = false,
            Control = InputControlType.TextBox)]
        public string? Hotline { get; set; }

        [Field(
            Title = "Số điện thoại",
            Required = false,
            Control = InputControlType.TextBox)]
        public string? ContactPhone { get; set; }

        [Field(
          Title = "Website",
          Required = false,
          Control = InputControlType.TextBox)]
        public string? Website { get; set; }

        [Field(
          Title = "Email",
          Required = false,
          Control = InputControlType.TextBox)]
        public string? Email { get; set; }

        [Field(
            Title = "Copyright Text",
            Required = false,
            Control = InputControlType.TextBox)]
        public string? CopyrightText { get; set; }

        [Field(
            Title = "Giờ làm việc",
            Required = false,
            Control = InputControlType.TextBox)]
        public string? WorkingHours { get; set; }

        [Field(
            Title = "en:Địa chỉ|vi:Địa chỉ",
            Required = false,
            Control = InputControlType.TextBox)]
        public string? Address { get; set; }

        [Field(
            Title = "en:Tên công ty bảo vệ bản quyền|vi:Tên công ty bảo vệ bản quyền",
            Required = false,
            Control = InputControlType.TextBox)]
        public string? CompanyNameGuardCopyRight { get; set; }

        [Field(
            Title = "en:Link công ty bảo vệ bản quyền|vi:Link công ty bảo vệ bản quyền",
            Required = false,
            Control = InputControlType.Link)]
        public string? LinkCompanyGuardCopyRight { get; set; }

        [Field(
            Title = "en:Bản đồ|vi:Bản đồ",
            Required = false,
            Control = InputControlType.TextBox)]
        public string? Map { get; set; }
    }

    public class GHTKConfig
    {
        [Field(Title = "Mã token GHTK", Control = InputControlType.TextBox)]
        public string? GhtkToken { get; set; }

        [Field(Title = "URL tích hợp API ", Control = InputControlType.TextBox)]
        public string? GhtkApi { get; set; }

        public string? Token => GhtkToken;
        public string? ApiUrl => GhtkApi;
    }

    public class PickAddress
    {
        [Field(Title = "Số nhà, tên đường ", Control = InputControlType.TextBox)]
        public string? Address { get; set; }

        [Field(Title = "Tỉnh/thành phố", Control = InputControlType.TextBox)]
        public string? Province { get; set; }

        [Field(Title = "Quận/huyện", Control = InputControlType.TextBox)]
        public string? District { get; set; }

        [Field(Title = "Phường/xã", Control = InputControlType.TextBox)]
        public string? Ward { get; set; }
    }
}

