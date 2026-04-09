# ETicaretAPI

> Onion Architecture, CQRS ve role-bazlı dinamik yetkilendirme prensipleri üzerine kurulu bir e-ticaret backend API'si. ASP.NET Core 7, Entity Framework Core ve PostgreSQL ile geliştirilmiştir.

Bu proje, bir bootcamp eğitimi kapsamında bir eğitmen rehberliğinde geliştirilmiştir. Amaç; modern .NET ekosisteminde clean architecture, CQRS, kimlik doğrulama (JWT + Refresh Token), dinamik yetkilendirme, dosya yönetimi, mail gönderimi, structured logging ve real-time iletişim gibi konuları uçtan uca uygulamalı olarak öğrenmektir.

---

## İçindekiler

- [Mimari](#mimari)
- [Kullanılan Teknolojiler](#kullanılan-teknolojiler)
- [Proje Yapısı](#proje-yapısı)
- [Özellikler](#özellikler)
- [Başlangıç](#başlangıç)
- [Yapılandırma](#yapılandırma)
- [Veritabanı ve Migration](#veritabanı-ve-migration)
- [API Endpoint'leri](#api-endpointleri)
- [Yetkilendirme Modeli](#yetkilendirme-modeli)
- [Loglama](#loglama)
- [Test](#test)
- [Bilinen Sınırlamalar](#bilinen-sınırlamalar)
- [Lisans](#lisans)

---

## Mimari

Proje **Onion Architecture (Clean Architecture)** prensiplerine göre tasarlanmıştır. Bağımlılıklar yalnızca dışarıdan içeriye akar; iç katmanlar dış katmanlardan habersizdir.

```
┌──────────────────────────────────────────────────────────┐
│                      Presentation                         │
│                   ETicaretAPI.API                         │
│   (Controllers, Filters, Middlewares, Program.cs)         │
└────────────────────────────┬─────────────────────────────┘
                             │
┌────────────────────────────┴─────────────────────────────┐
│                    Infrastructure                         │
│  ┌──────────────────┐ ┌──────────────┐ ┌──────────────┐  │
│  │   Persistence    │ │Infrastructure│ │   SignalR    │  │
│  │ (EF Core, Repos) │ │ (JWT, Mail,  │ │   (Hubs)     │  │
│  │                  │ │   Storage)   │ │              │  │
│  └──────────────────┘ └──────────────┘ └──────────────┘  │
└────────────────────────────┬─────────────────────────────┘
                             │
┌────────────────────────────┴─────────────────────────────┐
│                         Core                              │
│  ┌──────────────────────┐  ┌────────────────────────┐    │
│  │     Application      │  │        Domain          │    │
│  │ CQRS (MediatR), DTOs │  │   Entities, Common     │    │
│  │ Validators, Abstr.   │  │                        │    │
│  └──────────────────────┘  └────────────────────────┘    │
└──────────────────────────────────────────────────────────┘
```

**Tasarım kararları:**

- **CQRS + MediatR**: Komut (Command) ve Sorgu (Query) operasyonları net olarak ayrılır. Her use case kendi `Request`, `Response` ve `Handler` üçlüsü ile temsil edilir.
- **Repository Pattern**: `IReadRepository<T>` ve `IWriteRepository<T>` üzerinden generic veri erişimi. Domain entity başına özel repository'ler ihtiyaç olduğunda eklenir.
- **Dependency Inversion**: Tüm dış servislere (Storage, Token, Mail) Application katmanında interface tanımlanmış, implementasyonlar Infrastructure katmanında verilmiştir.
- **Dinamik Endpoint Yetkilendirmesi**: `AuthorizeDefinitionAttribute` ile işaretlenen action'lar runtime'da `RolePermissionFilter` üzerinden veritabanındaki rol-endpoint eşleştirmesine göre kontrol edilir.

---

## Kullanılan Teknolojiler

| Katman / Konu          | Teknoloji                                                       |
|------------------------|-----------------------------------------------------------------|
| Runtime                | .NET 7                                                          |
| Web Framework          | ASP.NET Core 7 (Web API)                                        |
| ORM                    | Entity Framework Core 7                                         |
| Veritabanı             | PostgreSQL (Npgsql provider)                                    |
| CQRS / Mediator        | MediatR 8                                                       |
| Validation             | FluentValidation 11                                             |
| Kimlik Yönetimi        | ASP.NET Core Identity                                           |
| Token                  | JWT Bearer (HS256) + Refresh Token                              |
| Real-Time              | SignalR                                                         |
| Logging                | Serilog (File + PostgreSQL sink)                                |
| API Dokümantasyonu     | Swashbuckle (Swagger / OpenAPI)                                 |
| Mail                   | System.Net.Mail (SMTP)                                          |

---

## Proje Yapısı

```
ETicaretAPI/
├── Core/
│   ├── ETicaretAPI.Domain/              # Entity'ler, Identity entity'leri, base sınıflar
│   └── ETicaretAPI.Application/         # CQRS feature'ları, abstractions, DTOs, validators
│
├── Infrastructure/
│   ├── ETicaretAPI.Persistence/         # DbContext, repository implementasyonları, migrations, services
│   ├── ETicaretAPI.Infrastructure/      # JWT, Mail, Local Storage, dış servisler
│   └── ETicaretAPI.SignalR/             # Real-time hub'lar
│
├── Presentation/
│   └── ETicaretAPI.API/                 # Controllers, filters, middleware, Program.cs, appsettings
│
└── tests/
    └── ETicaretAPI.Application.Tests/   # xUnit + Moq + FluentAssertions ile birim testleri
```

---

## Özellikler

- **Ürün Yönetimi**: Ürün CRUD, sayfalama, ürün görsellerini çoklu yükleme, vitrin görseli seçimi.
- **Sepet (Basket)**: Sepete ürün ekleme, miktar güncelleme, ürün çıkarma, sepet listeleme.
- **Sipariş Yönetimi**: Sipariş oluşturma, siparişleri listeleme, sipariş tamamlama (mail bildirimi ile).
- **Kimlik Doğrulama**: ASP.NET Core Identity üzerinden kullanıcı kaydı; JWT + Refresh Token akışı; şifre sıfırlama (mail ile).
- **Rol & Yetki Yönetimi**: Roller, kullanıcılara rol atama, endpoint bazlı dinamik yetkilendirme.
- **Dosya Depolama**: Soyut `IStorage` katmanı; şu an `LocalStorage` implementasyonu mevcut. Azure / AWS için yer ayrılmıştır.
- **Mail Servisi**: Şifre sıfırlama ve sipariş tamamlandı bildirimleri.
- **Loglama**: Serilog ile structured logging; loglar aynı anda hem dosyaya hem PostgreSQL `Logs` tablosuna yazılır. Her loga `user_name` enrich edilir.
- **Global Exception Handling**: Tüm yakalanmamış exception'lar tek bir middleware'de loglanıp standart JSON formatında döndürülür.
- **Swagger UI**: Geliştirme ortamında otomatik aktif.

---

## Başlangıç

### Ön Koşullar

- [.NET 7 SDK](https://dotnet.microsoft.com/download/dotnet/7.0)
- [PostgreSQL 14+](https://www.postgresql.org/download/)
- (Opsiyonel) [Visual Studio 2022](https://visualstudio.microsoft.com/) veya [JetBrains Rider](https://www.jetbrains.com/rider/)

### Klonla ve Hazırlık

```bash
git clone https://github.com/<kullanici>/ETicaretAPI.git
cd ETicaretAPI
```

### Yapılandırma Dosyasını Oluştur

`appsettings.json` `.gitignore` dahilindedir; gerçek hassas değerler depoya commit edilmez. Çalıştırmadan önce şablon dosyayı kopyalayıp kendi değerlerinle doldurmalısın:

```bash
cp Presentation/ETicaretAPI.API/appsettings.Example.json Presentation/ETicaretAPI.API/appsettings.json
```

Sonrasında `Presentation/ETicaretAPI.API/appsettings.json` içindeki şu alanları kendi değerlerinle güncelle:

- `ConnectionStrings:PostgreSQL` — yerel PostgreSQL bağlantı bilgilerin
- `Token:SecurityKey` — en az 32 karakter, kriptografik olarak güçlü rastgele bir string
- `Token:Audience`, `Token:Issuer` — kendi tanımladığın değerler
- `Mail:Username`, `Mail:Password`, `Mail:Host` — SMTP bilgilerin (kullanmıyorsan boş bırakabilirsin)

> 💡 **Üretim için öneri**: Hassas değerleri `appsettings.json` yerine [.NET User Secrets](https://learn.microsoft.com/en-us/aspnet/core/security/app-secrets), ortam değişkenleri veya Azure Key Vault gibi bir secret store içinde tut.

### Bağımlılıkları Yükle ve Çalıştır

```bash
dotnet restore
dotnet build
dotnet run --project Presentation/ETicaretAPI.API
```

API varsayılan olarak `https://localhost:7285` üzerinde ayağa kalkar. Swagger UI: `https://localhost:7285/swagger`

---

## Yapılandırma

### Ortam Değişkenleri

`Persistence` katmanındaki `DesignTimeDbContextFactory`, EF migration komutları için connection string'i şu sırayla çözer:

1. `ETICARETAPI_CONNECTION_STRING` ortam değişkeni (varsa)
2. `Presentation/ETicaretAPI.API/appsettings.json` içindeki `ConnectionStrings:PostgreSQL`

CI/CD ortamlarında 1. yöntemi kullanman tavsiye edilir.

### CORS

`Program.cs` içinde yalnızca `http://localhost:4200` ve `https://localhost:4200` (Angular istemci) origin'lerine izin verilir. Farklı bir frontend ile kullanacaksan oradaki politikayı güncellemen gerekir.

---

## Veritabanı ve Migration

EF Core migration'ları `Infrastructure/ETicaretAPI.Persistence/Migrations/` altında tutulur.

Yeni migration eklemek için:

```bash
dotnet ef migrations add <MigrationName> \
  --project Infrastructure/ETicaretAPI.Persistence \
  --startup-project Presentation/ETicaretAPI.API
```

Veritabanını güncellemek için:

```bash
dotnet ef database update \
  --project Infrastructure/ETicaretAPI.Persistence \
  --startup-project Presentation/ETicaretAPI.API
```

---

## API Endpoint'leri

> Tam ve etkileşimli liste için Swagger UI'a (`/swagger`) bakın. Aşağıda öne çıkanlar özetlenmiştir.

### Auth (`/api/auth`)
| Method | Endpoint                  | Açıklama                          |
|--------|---------------------------|-----------------------------------|
| POST   | `/login`                  | Kullanıcı girişi (JWT döner)      |
| POST   | `/refreshTokenLogin`      | Refresh token ile yeni access token |
| POST   | `/password-reset`         | Şifre sıfırlama maili tetikler    |
| POST   | `/verify-reset-token`     | Şifre sıfırlama token'ı doğrular  |

### Products (`/api/products`)
| Method | Endpoint                   | Yetki                |
|--------|----------------------------|----------------------|
| GET    | `/`                        | Public               |
| GET    | `/{id}`                    | Public               |
| POST   | `/`                        | Admin + Writing      |
| PUT    | `/`                        | Admin + Updating     |
| DELETE | `/{id}`                    | Admin + Deleting     |
| POST   | `/upload`                  | Admin + Writing      |
| GET    | `/getProductImages/{id}`   | Admin + Reading      |
| GET    | `/changeShowcaseImage`     | Admin + Updating     |
| GET    | `/deleteProductImage/{id}` | Admin + Deleting     |

### Baskets (`/api/baskets`)
Sepet item ekleme, listeleme, miktar güncelleme, silme.

### Orders (`/api/orders`)
Sipariş oluşturma, listeleme, detay, sipariş tamamlama.

### Users (`/api/users`)
Kullanıcı oluşturma, şifre güncelleme, kullanıcı listeleme, kullanıcıya rol atama.

### Roles (`/api/roles`)
Rol CRUD.

### AuthorizationEndpoints (`/api/authorizationendpoints`)
Endpoint'lere rol atama; endpoint'in mevcut rollerini sorgulama.

---

## Yetkilendirme Modeli

Bu projede klasik `[Authorize(Roles = "...")]` yerine **veritabanı tabanlı dinamik yetkilendirme** kullanılır:

1. Action method üstüne `[AuthorizeDefinition(Menu, Definition, ActionType)]` attribute'u eklenir.
2. Uygulama başlatıldığında bu attribute'lar reflection ile taranıp `Endpoints` tablosuna kaydedilir (`AuthorizationEndpointService` üzerinden).
3. Bir admin kullanıcısı, hangi rolün hangi endpoint'i çağırabileceğini `AuthorizationEndpointsController` üzerinden tanımlar.
4. Her istek `RolePermissionFilter` üzerinden geçer; kullanıcının rolleri ile endpoint'in izinli rolleri kesişiyorsa istek devam eder, aksi halde `401` döner.

Bu yapı, yetki tablosunu kod yerine veritabanında yönetmeyi sağlar ve admin paneli üzerinden runtime'da güncellenebilir.

---

## Loglama

- **Sink'ler**: `logs/log.txt` (rolling file) ve PostgreSQL `Logs` tablosu (`needAutoCreateTable: true`).
- **Custom column writers**: `RenderedMessageColumnWriter`, `MessageTemplateColumnWriter`, `LevelColumnWriter`, `TimestampColumnWriter`, `ExceptionColumnWriter`, `LogEventSerializedColumnWriter`, `UsernameColumnWriter`.
- **Enrichment**: Her log kaydına o anki kullanıcının `user_name`'i Serilog `LogContext` üzerinden push'lanır.
- **Request logging**: `app.UseSerilogRequestLogging()` ile her HTTP isteği otomatik loglanır.

---

## Test

Application katmanı için birim testleri `tests/ETicaretAPI.Application.Tests/` altında yer alır. Test stratejisi: **mock'lanabilir bağımlılıklar üzerinden saf iş mantığını doğrulamak**, integration test kompleksitesinden kaçınmak.

### Kullanılan Araçlar

| Paket | Görevi |
|---|---|
| **xUnit** | Test framework |
| **Moq** | Bağımlılıkları mock'lamak için |
| **FluentAssertions** | Okunabilir assertion DSL'i |
| **coverlet.collector** | Code coverage toplama |

### Mevcut Test Kapsamı

| Test Sınıfı | Hedef | Test Sayısı |
|---|---|---|
| `CreateProductCommandHandlerTests` | `CreateProductCommandHandler` (handler orkestrasyonu, mock verify, çağrı sırası) | 3 |
| `CreateUserCommandHandlerTests` | `CreateUserCommandHandler` (DTO mapping, başarı/başarısızlık akışları) | 2 |
| `CreateProductValidatorTests` | `CreateProductValidator` (FluentValidation kuralları, edge case'ler) | 9 |

### Çalıştırma

```bash
# Tüm testleri çalıştır
dotnet test

# Sadece bu projeyi
dotnet test tests/ETicaretAPI.Application.Tests

# Code coverage ile
dotnet test --collect:"XPlat Code Coverage"
```

Visual Studio'da Test Explorer otomatik olarak testleri keşfeder.

> **Not**: Test projesi `net7.0` hedefliyor ancak `<RollForward>Major</RollForward>` ayarı sayesinde .NET 7 runtime kurulu olmayan makinelerde de daha yeni .NET runtime'ları (8/9/10) üzerinde çalışır.

## Bilinen Sınırlamalar

Bu proje öğretim amaçlı geliştirildiği için bilinçli olarak bazı yerler "to-do" bırakılmıştır:

- **External Authentication** (Google/Facebook) interface seviyesinde tanımlıdır ancak implementasyonu yoktur (`NotImplementedException`).
- **SignalR Hub'ları** (`OrderHub`, `ProductHub`) iskelet olarak hazırdır; gerçek zamanlı bildirim akışı uçtan uca devreye alınmamıştır.
- **Cloud Storage** (Azure Blob / AWS S3) için interface ve enum değerleri vardır, fakat implementasyon yalnızca `LocalStorage` üzerindedir.
- **Unit / Integration test** projesi henüz eklenmemiştir.
- **API versioning** kullanılmamaktadır.

---

## Lisans

Bu proje MIT lisansı ile dağıtılmaktadır. Detaylar için [LICENSE](LICENSE) dosyasına bakınız.
