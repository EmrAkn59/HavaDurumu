@echo off
echo ========================================
echo Sunucu Test Ediliyor...
echo ========================================
echo.

echo Node.js Test (Port 3000)...
powershell -Command "try { $r = Invoke-WebRequest -Uri 'http://127.0.0.1:3000?pm25=150' -UseBasicParsing -TimeoutSec 2; Write-Host '✓ Node.js ÇALIŞIYOR! Status:' $r.StatusCode; $json = $r.Content | ConvertFrom-Json; Write-Host '  Mesaj:' $json.mesaj; Write-Host '  Durum:' $json.durum } catch { Write-Host '✗ Node.js çalışmıyor' }"

echo.
echo Python LLM Test (Port 5000)...
powershell -Command "try { $r = Invoke-WebRequest -Uri 'http://127.0.0.1:5000/' -UseBasicParsing -TimeoutSec 2; Write-Host '✓ LLM ÇALIŞIYOR! Status:' $r.StatusCode } catch { Write-Host '✗ LLM çalışmıyor' }"

echo.
pause

