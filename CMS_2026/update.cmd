@ECHO OFF
SETLOCAL

REM Chuyển vào thư mục project
CD /D "%~dp0CMS_2026"

REM Chạy script update.cmd trong thư mục project
CALL update.cmd %*

