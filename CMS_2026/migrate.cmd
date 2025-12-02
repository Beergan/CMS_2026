@ECHO OFF
SETLOCAL

REM Chuyển vào thư mục project
CD /D "%~dp0CMS_2026"

REM Chạy script migrate.cmd trong thư mục project
CALL migrate.cmd %*

