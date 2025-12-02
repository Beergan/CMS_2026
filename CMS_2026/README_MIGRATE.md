# 🚀 Hướng dẫn chạy Migration Scripts

## Vị trí scripts

Scripts được đặt ở 2 nơi để tiện sử dụng:
- `CMS_2026/CMS_2026/migrate.cmd` - Wrapper script (tự động chuyển vào thư mục project)
- `CMS_2026/CMS_2026/CMS_2026/migrate.cmd` - Script chính

## Cách chạy

### Trong PowerShell (từ thư mục `CMS_2026\CMS_2026`)

```powershell
# Dùng dấu chấm và backslash
.\migrate.cmd

# Hoặc với tên migration
.\migrate.cmd AddNewTable
```

### Trong Command Prompt (CMD)

```cmd
REM Từ thư mục CMS_2026\CMS_2026
migrate.cmd

REM Hoặc với tên migration
migrate.cmd AddNewTable
```

### Trong PowerShell (từ thư mục `CMS_2026\CMS_2026\CMS_2026`)

```powershell
# Vào thư mục project trước
cd CMS_2026
.\migrate.cmd
```

## Lưu ý

- ✅ Script sẽ tự động chuyển vào thư mục project nếu chạy từ `CMS_2026\CMS_2026`
- ✅ Trong PowerShell, luôn dùng `.\` trước tên file
- ✅ Đảm bảo đang ở đúng thư mục có file `migrate.cmd`

## Troubleshooting

### Lỗi: "The term './migrate.cmd' is not recognized"

**Nguyên nhân**: PowerShell không nhận diện `./` như bash

**Giải pháp**: Dùng `.\migrate.cmd` thay vì `./migrate.cmd`

### Lỗi: "Cannot find the path specified"

**Nguyên nhân**: Đang ở sai thư mục

**Giải pháp**: 
```powershell
# Kiểm tra thư mục hiện tại
pwd

# Chuyển vào thư mục đúng
cd D:\Iambee\update\CMS_2026\CMS_2026

# Chạy script
.\migrate.cmd
```

