using System;

namespace HavaDurumu.Integration
{
    /// <summary>
    /// gRPC Servis Host Program - Servisi başlatmak için kullanılır
    /// </summary>
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                NodeService nodeServis = new NodeService();
                HavaDurumuServisi havaServis = new HavaDurumuServisi();
                
                Console.WriteLine("Servisler başarıyla oluşturuldu!");
                Console.WriteLine("Çıkmak için bir tuşa basın...");
                Console.ReadKey();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Hata: {ex.Message}");
                Console.WriteLine(ex.StackTrace);
            }
        }
    }
}

