@echo off
echo ========================================
echo Tüm Sunucuları Başlatıyor...
echo ========================================
echo.

REM Node.js Sunucusu
echo [1/3] Node.js sunucusu başlatılıyor (Port 3000)...
start "Node.js Server" cmd /k "cd /d %~dp0NodeServer && node server.js"
timeout /t 2 /nobreak >nul

REM Python LLM Sunucusu
echo [2/3] Python LLM sunucusu başlatılıyor (Port 5000)...
start "Python LLM Server" cmd /k "cd /d %~dp0LLm && python app.py"
timeout /t 2 /nobreak >nul

REM Risk Analiz Servisi (gRPC)
echo [3/3] Risk Analiz Servisi başlatılıyor (Port 5058)...
start "Risk Analiz Servisi" cmd /k "cd /d %~dp0RiskAnalizServisi && dotnet run --urls http://localhost:5058"
timeout /t 2 /nobreak >nul

echo.
echo ========================================
echo Sunucular başlatıldı!
echo ========================================
echo.
echo Node.js: http://127.0.0.1:3000
echo Python LLM: http://127.0.0.1:5000
echo Risk Analiz (gRPC): http://127.0.0.1:5058
echo.
echo Sunucuları kapatmak için açılan pencereleri kapatın.
echo.
pause

