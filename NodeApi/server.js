// server.js
const http = require('http');

const hostname = '127.0.0.1'; // Localhost
const port = 3000; // Çalışacağı kapı numarası

const server = http.createServer((req, res) => {
  res.statusCode = 200;
  res.setHeader('Content-Type', 'application/json');
  
  // C# tarafına göndereceğimiz veri bu:
  const data = {
    mesaj: "Merhaba! Ben Node.js tarafından geliyorum.",
    tarih: new Date().toLocaleDateString(),
    durum: "Aktif"
  };

  res.end(JSON.stringify(data));
});

server.listen(port, hostname, () => {
  console.log(`Node.js Sunucusu çalışıyor: http://${hostname}:${port}/`);
});
