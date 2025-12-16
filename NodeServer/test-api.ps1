Write-Host "Node.js API Test Ediliyor..." -ForegroundColor Yellow
Write-Host ""

try {
    $response = Invoke-RestMethod -Uri "http://127.0.0.1:3000" -Method Get -TimeoutSec 3
    Write-Host "✓ Bağlantı Başarılı!" -ForegroundColor Green
    Write-Host ""
    Write-Host "Yanıt:" -ForegroundColor Cyan
    $response | ConvertTo-Json
    Write-Host ""
    Write-Host "PM2.5 ile test:" -ForegroundColor Yellow
    $response2 = Invoke-RestMethod -Uri "http://127.0.0.1:3000?pm25=25" -Method Get -TimeoutSec 3
    $response2 | ConvertTo-Json
}
catch {
    Write-Host "✗ Hata: $($_.Exception.Message)" -ForegroundColor Red
    Write-Host ""
    Write-Host "Node.js sunucusunu başlatmak için:" -ForegroundColor Yellow
    Write-Host "  cd NodeServer" -ForegroundColor White
    Write-Host "  node server.js" -ForegroundColor White
}

