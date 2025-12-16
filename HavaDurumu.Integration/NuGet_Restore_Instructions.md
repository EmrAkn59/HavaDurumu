# gRPC Paketlerini Yükleme Talimatları

Eğer gRPC paketleri yüklü değilse, aşağıdaki adımları izleyin:

## Visual Studio'da NuGet Paket Yöneticisi ile:

1. Solution Explorer'da `HavaDurumu.Integration` projesine sağ tıklayın
2. "Manage NuGet Packages..." seçeneğini seçin
3. "Browse" sekmesine gidin
4. Aşağıdaki paketleri yükleyin:
   - `Grpc.Core` (version 2.46.6)
   - `Google.Protobuf` (version 3.21.12)
   - `Grpc.Tools` (version 2.46.6) - Development dependency

## Package Manager Console ile:

```powershell
Install-Package Grpc.Core -Version 2.46.6 -ProjectName HavaDurumu.Integration
Install-Package Google.Protobuf -Version 3.21.12 -ProjectName HavaDurumu.Integration
Install-Package Grpc.Tools -Version 2.46.6 -ProjectName HavaDurumu.Integration
```

## NuGet CLI ile:

```bash
nuget install Grpc.Core -Version 2.46.6
nuget install Google.Protobuf -Version 3.21.12
nuget install Grpc.Tools -Version 2.46.6
```

## Paketlerin Yüklü Olduğunu Kontrol Etme:

Paketler şu klasörde olmalı:
- `packages\Grpc.Core.2.46.6\`
- `packages\Google.Protobuf.3.21.12\`
- `packages\Grpc.Tools.2.46.6\`

## Not:

Eğer paketler zaten yüklüyse ama proje hala hata veriyorsa:
1. Solution'ı kapatın
2. `packages` klasörünü kontrol edin
3. Visual Studio'yu yeniden açın
4. Solution'ı "Restore NuGet Packages" ile yeniden yükleyin

