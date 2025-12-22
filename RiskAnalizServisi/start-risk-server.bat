@echo off
echo ========================================
echo Risk Analiz Servisi Baslatiliyor...
echo ========================================
echo.

cd /d "%~dp0"

echo .NET kontrol ediliyor...
dotnet --version >nul 2>&1
if errorlevel 1 (
    echo HATA: .NET bulunamadi! .NET 9.0 veya uzeri SDK yuklu oldugundan emin olun.
    pause
    exit /b 1
)

echo.
echo ========================================
echo Sunucu baslatiliyor (Port 5058)...
echo HTTP Endpoint: http://localhost:5058/api/risk/analyze
echo gRPC Endpoint: http://localhost:5058
echo ========================================
echo.
echo Sunucu calisiyor... Durdurmak icin Ctrl+C basin.
echo.

dotnet run

pause
