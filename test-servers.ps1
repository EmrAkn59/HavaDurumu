# Sunucu Test Scripti
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "Sunucu Test Scripti" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

# Node.js Test
Write-Host "1. Node.js Sunucusu Test (Port 3000)..." -ForegroundColor Yellow
$nodeOk = $false
try {
    $response = Invoke-WebRequest -Uri "http://127.0.0.1:3000?pm25=150" -UseBasicParsing -TimeoutSec 3
    Write-Host "   Status Code: $($response.StatusCode)" -ForegroundColor Green
    $json = $response.Content | ConvertFrom-Json
    Write-Host "   Mesaj: $($json.mesaj)" -ForegroundColor Green
    Write-Host "   Durum: $($json.durum)" -ForegroundColor Green
    Write-Host "   PM2.5: $($json.pm25)" -ForegroundColor Green
    Write-Host "   ✓ Node.js sunucusu çalışıyor!" -ForegroundColor Green
    $nodeOk = $true
} catch {
    Write-Host "   ✗ Node.js sunucusu çalışmıyor!" -ForegroundColor Red
    Write-Host "   Hata: $($_.Exception.Message)" -ForegroundColor Red
    Write-Host "   Çözüm: NodeServer klasöründe 'start-server.bat' dosyasını çalıştırın." -ForegroundColor Yellow
}

Write-Host ""

# LLM Test - Ana Sayfa
Write-Host "2. Python LLM Sunucusu Test (Port 5000)..." -ForegroundColor Yellow
$llmOk = $false
try {
    $response = Invoke-WebRequest -Uri "http://127.0.0.1:5000/" -UseBasicParsing -TimeoutSec 3
    Write-Host "   Ana Sayfa Status: $($response.StatusCode)" -ForegroundColor Green
} catch {
    Write-Host "   ✗ LLM sunucusu çalışmıyor!" -ForegroundColor Red
    Write-Host "   Hata: $($_.Exception.Message)" -ForegroundColor Red
    Write-Host "   Çözüm: LLm klasöründe 'start-llm-server.bat' dosyasını çalıştırın." -ForegroundColor Yellow
    Write-Host ""
    Write-Host "========================================" -ForegroundColor Cyan
    Write-Host "Özet:" -ForegroundColor Cyan
    Write-Host "  Node.js: $(if ($nodeOk) { '✓ Çalışıyor' } else { '✗ Çalışmıyor' })" -ForegroundColor $(if ($nodeOk) { 'Green' } else { 'Red' })
    Write-Host "  LLM: ✗ Çalışmıyor" -ForegroundColor Red
    Write-Host "========================================" -ForegroundColor Cyan
    exit
}

# LLM Test - Tahmin
try {
    $testData = @{
        sehir = "istanbul"
        tarih = "2025-12-16 15:00"
        nem = 65.0
        ruzgar = 12.0
        basinc = 1013.0
        yagis_var_mi = 0
    } | ConvertTo-Json
    
    $response = Invoke-WebRequest -Uri "http://127.0.0.1:5000/tahmin" -Method POST -Body $testData -ContentType "application/json" -UseBasicParsing -TimeoutSec 10
    $json = $response.Content | ConvertFrom-Json
    
    if ($json.basari -eq $true) {
        Write-Host "   ✓ LLM tahmin başarılı!" -ForegroundColor Green
        Write-Host "   Tahmin Edilen Sıcaklık: $($json.tahmin_edilen_sicaklik) °C" -ForegroundColor Green
        Write-Host "   Şehir: $($json.sehir)" -ForegroundColor Green
        $llmOk = $true
    } else {
        Write-Host "   ✗ LLM tahmin başarısız!" -ForegroundColor Red
        Write-Host "   Hata: $($json.hata)" -ForegroundColor Red
    }
} catch {
    Write-Host "   ✗ LLM tahmin hatası!" -ForegroundColor Red
    Write-Host "   Hata: $($_.Exception.Message)" -ForegroundColor Red
}

Write-Host ""
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "Özet:" -ForegroundColor Cyan
Write-Host "  Node.js: $(if ($nodeOk) { '✓ Çalışıyor' } else { '✗ Çalışmıyor' })" -ForegroundColor $(if ($nodeOk) { 'Green' } else { 'Red' })
Write-Host "  LLM: $(if ($llmOk) { '✓ Çalışıyor' } else { '✗ Çalışmıyor' })" -ForegroundColor $(if ($llmOk) { 'Green' } else { 'Red' })
Write-Host "========================================" -ForegroundColor Cyan
