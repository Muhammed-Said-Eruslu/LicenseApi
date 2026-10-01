# License API

Ticari olarak dağıtılan web uygulamalarının yalnızca geçerli bir lisansla çalışmasını sağlayan merkezi
lisans sunucusu. Lisans üretme, doğrulama ve iptal işlemlerini; süre, cihaz limiti, makineye bağlama ve
modül bazlı özellik açma kurallarıyla yönetir.

*A central license server built with ASP.NET Core: issue, validate and revoke license keys with expiry,
device limits, machine binding and per-module feature flags, protected by JWT and role-based access.*

## Nasıl çalışır?

```text
Yönetici ──(JWT, ADMIN)──► License API ──► SQL Server
                               ▲
İstemci uygulama ──(POST /api/License/validate)──┘
(ör. BBD Garage e-ticaret sitesi)
```

İstemci tarafında (BBD Garage) arka planda çalışan bir servis lisansı düzenli aralıklarla doğrular,
sonucu önbellekte tutar; sunucuya ulaşılamazsa yapılandırılabilir bir tolerans süresi boyunca
uygulama çalışmaya devam eder, lisans geçersizse istekler HTTP 503 ile kapatılır.

## Özellikler

- Lisans anahtarı üretimi; bitiş tarihi, maksimum cihaz sayısı, aktif/pasif durumu
- Lisansı belirli bir makineye veya alan adına bağlama (`HostIdentifier`)
- Modül bazlı özellik açma (`EnabledModules`)
- Lisans iptali (`RevokedAt` kaydıyla)
- JWT kimlik doğrulama, ASP.NET Core Identity ile `USER` / `ADMIN` rolleri
- Yönetim uç noktaları yalnızca `ADMIN` rolüne açık; doğrulama uç noktası istemciler için anonim
- Swagger arayüzünde JWT ile deneme imkânı

## Mimari

Clean Architecture ile dört katman:

| Katman | İçerik |
|---|---|
| `Domain` | `License`, `AppUser` varlıkları |
| `Application` | DTO'lar ve `ILicenseService` arayüzü |
| `Infrastructure` | EF Core `AppDbContext`, migration'lar, `LicenseService`, bağımlılık kaydı |
| `Presentation` | Web API: `AuthController`, `LicenseController`, JWT ve Swagger yapılandırması |

## Uç noktalar

| Yöntem | Yol | Yetki | Açıklama |
|---|---|---|---|
| POST | `/api/Auth/register` | Herkes | Kullanıcı kaydı (`USER` rolü) |
| POST | `/api/Auth/login` | Herkes | JWT üretir; token'da kullanıcının gerçek rolleri bulunur |
| POST | `/api/License/validate` | Anonim | `licenseKey` + `machineId` ile lisans doğrulama |
| POST | `/api/License/generate` | ADMIN | E-posta, süre (gün) ve cihaz limitiyle lisans üretir |
| POST | `/api/License/issue` | ADMIN | Gelişmiş lisans: özel anahtar, makine bağlama, modüller, bitiş tarihi |
| POST | `/api/License/revoke` | ADMIN | Lisansı iptal eder |
| GET | `/api/License` | ADMIN | Tüm lisansları listeler |

Örnek doğrulama isteği:

```http
POST /api/License/validate
Content-Type: application/json

{ "licenseKey": "3F2A...", "machineId": "shop-server-01" }
```

## Kurulum

Gereksinimler: .NET 8 SDK, SQL Server (LocalDB veya Express).

1. `Presentation/appsettings.json` içindeki `ConnectionStrings:DefaultConnection` değerini kendi
   veritabanına göre düzenle.
2. JWT anahtarını ve ilk yönetici hesabını **kod dışında** tanımla (user-secrets veya ortam değişkeni):

   ```bash
   cd Presentation
   dotnet user-secrets init
   dotnet user-secrets set "Jwt:Key" "<en az 32 karakterlik rastgele anahtar>"
   dotnet user-secrets set "AdminSeed:Email" "admin@ornek.com"
   dotnet user-secrets set "AdminSeed:Password" "<güçlü parola>"
   ```

   `appsettings.json` içindeki `Jwt:Key` yalnızca yerel geliştirme içindir; canlı ortamda mutlaka değiştirilmelidir.
   `AdminSeed` verilmezse yönetici hesabı oluşturulmaz.
3. Veritabanını oluştur ve çalıştır:

   ```bash
   dotnet tool install --global dotnet-ef   # bir kez
   dotnet ef database update --project Infrastructure --startup-project Presentation
   dotnet run --project Presentation
   ```

4. Swagger: `https://localhost:7202/swagger` — `/api/Auth/login` ile alınan token'ı **Authorize** düğmesine
   `Bearer <token>` olarak gir.

## Teknolojiler

C#, ASP.NET Core 8 Web API, Entity Framework Core, SQL Server, ASP.NET Core Identity, JWT Bearer,
Swagger / OpenAPI

## Geliştirici

Muhammed Said Eruslu · [GitHub](https://github.com/Muhammed-Said-Eruslu)
