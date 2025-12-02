# HƯỚNG DẪN TÁI SỬ DỤNG BỐ CỤC ADMIN LIST

Các partial views này cho phép bạn tái sử dụng bố cục từ cấu trúc cũ một cách dễ dàng.

## 📋 CÁC PARTIAL VIEWS CÓ SẴN

### 1. `_AdminListLayout.cshtml`
Wrapper layout cho admin list pages với breadcrumb và card-box.

**Usage:**
```razor
@await Html.PartialAsync("_Shared/_AdminListLayout", new {
    Title = "Danh sách sản phẩm",
    ShowBreadcrumb = true,
    BreadcrumbText = "Danh sách sản phẩm"
})

@section tabs {
    @await Html.PartialAsync("_Shared/_AdminListTabs", new { ... })
}

@section table {
    <table>...</table>
}
```

### 2. `_AdminListTabs.cshtml`
Tabs navigation với support cho categories, all items, và create button.

**Usage:**
```razor
@await Html.PartialAsync("_Shared/_AdminListTabs", new {
    ShowCategories = true,
    CategoryType = "product",
    ShowAllItems = true,
    AllItemsText = "Tất cả sản phẩm",
    AllItemsUrl = "/admin/product",
    CurrentCategory = Model.Category, // null nếu không có
    CreateUrl = "/admin/product/create",
    CreateText = "Thêm sản phẩm mới"
})
```

### 3. `_DataTable.cshtml`
DataTable với cấu hình đầy đủ (CSS, JS, sorting, pagination).

**Usage:**
```razor
@section head {
    @await Html.PartialAsync("_Shared/_DataTable", new {
        TableId = "datatable",
        IsHead = true
    })
}

@section scripts {
    @await Html.PartialAsync("_Shared/_DataTable", new {
        TableId = "datatable",
        IsHead = false,
        PageLength = 100,
        LengthChange = false
    })
}
```

### 4. `_DeleteConfirmation.cshtml`
SweetAlert delete confirmation với AJAX.

**Usage:**
```razor
@section head {
    @await Html.PartialAsync("_Shared/_DeleteConfirmation", new {
        IsHead = true
    })
}

@section scripts {
    @await Html.PartialAsync("_Shared/_DeleteConfirmation", new {
        IsHead = false,
        DeleteUrl = "/admin/product?handler=Delete",
        FunctionName = "deleteProduct",
        ItemType = "sản phẩm"
    })
}

<!-- Trong table row -->
<button onclick="deleteProduct(@product.Id, '@product.Title')">Xóa</button>
```

### 5. `_ProductListRow.cshtml`
Row template cho product list.

**Usage:**
```razor
<tbody>
    @foreach (var product in Model.Products)
    {
        @await Html.PartialAsync("_Shared/_ProductListRow", product, new ViewDataDictionary(ViewData) {
            { "ShowImage", true },
            { "ShowViewButton", true }
        })
    }
</tbody>
```

### 6. `_PostListRow.cshtml`
Row template cho post list.

**Usage:**
```razor
<tbody>
    @foreach (var post in Model.Posts)
    {
        @await Html.PartialAsync("_Shared/_PostListRow", post, new ViewDataDictionary(ViewData) {
            { "ShowImage", true },
            { "ShowFeatured", true },
            { "ShowViewButton", true }
        })
    }
</tbody>
```

## 🚀 VÍ DỤ HOÀN CHỈNH

### Product Index với đầy đủ tính năng:

```razor
@page
@model CMS_2026.Pages.Admin.Product.IndexModel
@{
    ViewData["Title"] = "Danh sách sản phẩm";
    Layout = "_Layout";
}

@await Html.PartialAsync("_Shared/_AdminListLayout", new {
    Title = "Danh sách sản phẩm",
    ShowBreadcrumb = true
})

@section tabs {
    @await Html.PartialAsync("_Shared/_AdminListTabs", new {
        ShowCategories = true,
        CategoryType = "product",
        ShowAllItems = true,
        AllItemsText = "Tất cả sản phẩm",
        AllItemsUrl = "/admin/product",
        CurrentCategory = Model.Category,
        CreateUrl = Model.CatId > 0 ? $"/admin/product/create?CategoryId={Model.CatId}" : "/admin/product/create",
        CreateText = "Thêm sản phẩm mới"
    })
}

@section table {
    <table id="datatable" class="table table-striped table-bordered dt-responsive">
        <thead>
            <tr>
                <th>Tiêu đề</th>
                <th>Hình ảnh</th>
                <th>Giá</th>
                <th>Danh mục</th>
                <th>Ngày tạo</th>
                <th style="width:1%;min-width:80px;">Thao tác</th>
            </tr>
        </thead>
        <tbody>
            @foreach (var product in Model.Products)
            {
                @await Html.PartialAsync("_Shared/_ProductListRow", product)
            }
        </tbody>
    </table>
}

@section head {
    @await Html.PartialAsync("_Shared/_DataTable", new { TableId = "datatable", IsHead = true })
    @await Html.PartialAsync("_Shared/_DeleteConfirmation", new { IsHead = true })
}

@section scripts {
    @await Html.PartialAsync("_Shared/_DataTable", new { 
        TableId = "datatable", 
        IsHead = false,
        PageLength = 100,
        LengthChange = false
    })
    @await Html.PartialAsync("_Shared/_DeleteConfirmation", new {
        IsHead = false,
        DeleteUrl = "/admin/product?handler=Delete",
        FunctionName = "deleteProduct",
        ItemType = "sản phẩm"
    })
}
```

## ⚙️ TÙY CHỈNH

Tất cả các partial views đều có các tham số tùy chọn để tùy chỉnh:
- `ShowBreadcrumb`, `ShowCategories`, `ShowAllItems`
- `ShowImage`, `ShowFeatured`, `ShowViewButton`
- `PageLength`, `LengthChange`, `ShowSearch`, `ShowPaging`
- Custom URLs, texts, icons

## 📝 LƯU Ý

1. **Entity Framework Core**: Tất cả các partial views này KHÔNG thay đổi Entity Framework Core, chỉ là UI components.
2. **ViewData**: Các tham số được truyền qua ViewData, có thể override bằng ViewDataDictionary.
3. **Sections**: Một số partial views cần được đặt trong `@section head` hoặc `@section scripts`.
4. **Constants**: Sử dụng `Constants.Admin_Url` để đảm bảo URL nhất quán.

