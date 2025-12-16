const http = require('http');

console.log('Node.js API Test Ediliyor...\n');

// Test 1: Basit GET isteği
http.get('http://127.0.0.1:3000', (res) => {
    let data = '';
    
    res.on('data', (chunk) => {
        data += chunk;
    });
    
    res.on('end', () => {
        console.log('✅ Test 1 - Basit GET:');
        console.log('Status:', res.statusCode);
        console.log('Response:', data);
        console.log('');
        
        // Test 2: PM2.5 ile GET isteği
        http.get('http://127.0.0.1:3000?pm25=25', (res2) => {
            let data2 = '';
            
            res2.on('data', (chunk) => {
                data2 += chunk;
            });
            
            res2.on('end', () => {
                console.log('✅ Test 2 - PM2.5 ile GET:');
                console.log('Status:', res2.statusCode);
                console.log('Response:', data2);
            });
        }).on('error', (err) => {
            console.log('❌ Test 2 Hata:', err.message);
        });
    });
}).on('error', (err) => {
    console.log('❌ Test 1 Hata:', err.message);
    console.log('Sunucu çalışmıyor olabilir. Lütfen sunucuyu başlatın: node server.js');
});

