# OBS / ÜBYS Backend — Mimari ve İş Gereksinimleri

## 1. Rolün ve ana hedefin

Deneyimli bir .NET backend mimarı ve yazılım geliştiricisi olarak mevcut OBS / ÜBYS (Öğrenci Bilgi Sistemi / Üniversite Bilgi Yönetim Sistemi) projesinin backend mimarisini inceleyecek, mevcut mimariyle uyumlu şekilde akademik işlemleri, kullanıcı yetkilendirmesini ve ilgili iş kurallarını geliştireceksin.

Bu sistem sıradan bir CRUD uygulaması değildir. Üniversite organizasyon yapısı, akademik yıl, müfredat sürümleri, dersler, öğretim elemanları, öğrenci kayıtları, ders seçimleri, ders programları ve değerlendirme kuralları birbiriyle ilişkilidir.

Temel amaç; yeni fakülteler, bölümler, akademik yıllar, ders türleri, kullanıcı rolleri ve değerlendirme politikaları eklendiğinde sistemin büyük çaplı yeniden yazım gerektirmemesidir.

**Öncelik sırası:**

1. Doğru ve güvenli yetkilendirme.
2. Akademik verilerin yıllar arasında tutarlılığını korumak.
3. Domain iş kurallarını doğru uygulamak.
4. Mevcut universal backend mimarisini korumak.
5. Test edilebilir, sürdürülebilir ve genişletilebilir kod üretmek.
6. Kullanılabilir API sözleşmeleri oluşturmak.

İlk olarak repository'yi, solution yapısını ve mevcut modülleri incele. Daha sonra gerçek dosya yapısına ve mevcut kod standartlarına uygun bir uygulama planı çıkar. Mevcut yapı incelenmeden yeni katmanlar, servisler, repository'ler veya soyutlamalar oluşturma.

## 2. Mimari ilkeler

Projede mevcut olan aşağıdaki mimari yaklaşımı esas al:

- .NET ve C# tabanlı backend.
- Vertical Slice Architecture.
- CQRS ve mevcut MediatR kullanımı.
- Domain-Driven Design (DDD).
- Rich Domain Model: iş kuralları yalnızca controller veya handler içerisinde dağınık şekilde bulunmamalı.
- Entity, Value Object ve Aggregate Root kullanımı gerektiği yerde uygulanmalı.
- EF Core ile persistence.
- FluentValidation ile istek ve kullanım senaryosu doğrulamaları.
- Mevcut Unit of Work, repository ve application abstraction yapıları.
- Mevcut API versioning yaklaşımı.
- Projede bulunan `ServiceResult<T>`, `Error`, `ErrorType` ve API hata standardı.
- Projede bulunan `BuildingBlocks.Domain`, `BuildingBlocks.Application`, `BuildingBlocks.Application.Abstractions` ve API yapılandırmaları.

Bu isimler ve yapılar hâlihazırda mevcutsa onları kullan. Gerçek implementasyonlarını incelemeden yeni alternatiflerini yazma.

### Mimari kurallar

- Controller'lar HTTP katmanında kalmalı; iş kuralları controller'lara taşınmamalı.
- Her kullanım senaryosu anlaşılabilir bir Command/Query ve handler üzerinden yürümeli.
- Domain katmanı EF Core, HTTP, JWT, controller veya dış sistemlere bağımlı olmamalı.
- Application katmanı domain kurallarını koordine etmeli; authorization ve kullanım senaryosu doğrulamaları burada doğru biçimde uygulanmalı.
- Infrastructure katmanı veritabanı ve dış bağımlılıkların implementasyonunu üstlenmeli.
- API katmanı request/response sözleşmelerini ve HTTP davranışlarını yönetmeli.
- Birden fazla katmanda aynı iş kuralını farklı şekillerde tekrar etme.
- Salt CRUD işlemleri için gereksiz aggregate, repository veya soyutlama üretme.
- Aynı sorumluluğa sahip ikinci bir result, error, validation veya authorization sistemi oluşturma.
- Her özelliği tek bir büyük servise yığma; ilgili kullanım senaryolarını ayrı tut.
- Modüler monolit yaklaşımını tercih et. Gerçek bir ihtiyaç bulunmadıkça mikroservis oluşturma.
- Mevcut kodla çelişen bir mimari karar görürsen doğrudan yeni bir paralel yapı kurmak yerine çelişkiyi belirle ve en küçük tutarlı çözümü seç.

Tüm uygulama boyunca mevcut naming convention, namespace, DI, configuration, migration, logging ve test düzenine uy.

## 3. Temel domain ve organizasyon yapısı

Aşağıdaki kavramları birbirinden ayır.

### Üniversite organizasyonu

- Fakülte (Faculty)
- Bölüm (Department)
- Akademik program veya öğretim programı (Program)
- Akademik yıl (AcademicYear)
- Akademik dönem (AcademicTerm / Semester)

Temel ilişki:

- Bir fakültenin birden fazla bölümü olabilir.
- Bir bölüm bir fakülteye bağlıdır.
- Akademik programlar kendi akademik yapıları ve müfredatlarıyla ilişkilendirilebilir.
- Akademik yıllar ve dönemler birbirinden ayrılmalıdır.

Organizasyon kayıtları için oluşturma, düzenleme, etkin/pasif duruma alma ve gerekli görüntüleme senaryolarını tasarla.

Fakülte veya bölümün silinmesi, geçmiş akademik kayıtların bozulmasına yol açmamalıdır. Kullanımda olan kayıtlar için fiziksel silme yerine uygun durum yönetimini değerlendir.

### Fakülte ve bölüm yönetimi

Yetki matrisi şu şekilde olmalıdır:

**Öğrenci işleri:**
- Fakülte oluşturabilir.
- Fakülte bilgilerini düzenleyebilir.
- Bölüm oluşturabilir.
- Bölüm bilgilerini düzenleyebilir.
- Yetkisi dahilindeki akademik kayıt ve öğrenci işlemlerini gerçekleştirebilir.

**Fakülte sorumlusu:**
- Atandığı fakülteyi görüntüleyebilir ve düzenleyebilir.
- Kendi fakültesine bağlı bölümleri oluşturabilir ve düzenleyebilir.
- Başka bir fakültenin kayıtlarını yönetemez.

**Bölüm sorumlusu:**
- Yalnızca atandığı bölümü görüntüleyebilir ve düzenleyebilir.
- Bölümün bağlı olduğu fakültenin veya başka bölümlerin yönetim yetkisine sahip değildir.
- Kendisine ayrıca izin verilmediği sürece yeni bölüm veya fakülte oluşturamaz.

**Sistem yöneticisi:**
- Sistem genelinde açıkça tanımlanmış yönetim yetkilerine sahip olabilir.
- Bu yetki normal kullanıcı rollerinden ayrı değerlendirilmelidir.

Bu kurallar yalnızca listeleme ekranlarında uygulanmamalı; okuma, oluşturma, düzenleme, silme, ilişkilendirme ve diğer tüm API işlemlerinde geçerli olmalıdır.

Örneğin bölüm yöneticisi, URL veya request gövdesinde başka bir `DepartmentId` göndererek başka bir bölümü düzenleyememelidir.

## 4. Permission-based authorization

Bu projenin en önemli noktası yetkilendirme sistemidir.

Sadece `Admin`, `Teacher` veya `Student` şeklinde birkaç rol kontrolü yeterli değildir. Rol bazlı yapı ile ayrıntılı permission bazlı yetkilendirme birlikte çalışmalıdır.

### 4.1. Rol ve permission ayrımı

Roller, kullanıcılara atanabilen yetki gruplarıdır. Permission'lar ise tekil işlemleri tanımlar.

Örnek roller:

- System Administrator
- Student Affairs
- Faculty Manager
- Department Manager
- Instructor
- Student

Örnek permission'lar:

- `faculty.read`
- `faculty.create`
- `faculty.update`
- `faculty.deactivate`
- `department.read`
- `department.create`
- `department.update`
- `department.deactivate`
- `curriculum.read`
- `curriculum.create`
- `curriculum.update`
- `curriculum.publish`
- `student.read`
- `student.create`
- `student.update`
- `student.enrollment.manage`
- `course.read`
- `course.create`
- `course.update`
- `course.assignment.manage`
- `course.schedule.manage`
- `course.assessment.manage`
- `course.grading.bell-curve.manage`
- `course-selection.read`
- `course-selection.manage`
- `course-selection.ects-limit.override`
- `exam-template.read`
- `exam-template.create`
- `exam-template.update`
- `academic-term.manage`
- `authorization.role.manage`
- `authorization.permission.manage`

Bunlar başlangıç önerileridir. Gerçek permission kataloğunu kullanım senaryolarına göre netleştir; aşırı genel veya anlamsız derecede küçük izinler üretme.

Örneğin `faculty.manage` adı altında oluşturma, düzenleme ve pasifleştirmeyi birleştirme. Bunların farklı kişiler tarafından yürütülebilmesi gerekir.

### 4.2. Kapsamlı yetkilendirme

Permission'ın var olması, kullanıcının sistemdeki her kayda erişebileceği anlamına gelmez.

Yetki kontrolü en az iki boyutlu olmalıdır:

1. **İşlem yetkisi:** Kullanıcı hangi eylemi gerçekleştirebilir?
2. **Kaynak kapsamı:** Kullanıcı bu eylemi hangi fakülte, bölüm, ders veya akademik dönem üzerinde gerçekleştirebilir?

Örnekler:

- `department.update` izni olan bölüm sorumlusu yalnızca kendisine atanmış bölümü düzenleyebilir.
- Atanmış fakültede geçerli yetkilere sahip fakülte sorumlusu, o fakülteye bağlı bölümlerde işlem yapabilir.
- Öğretmen `course.assessment.manage` iznine sahip olsa bile yalnızca görevlendirildiği dersin değerlendirme koşullarını yönetebilir.
- Öğrenci, yalnızca kendi öğrencilik bilgilerine ve kayıtlarına erişebilir.
- Öğrenci işleri, kendisine tanımlanan kurumsal kapsamda fakülte, bölüm ve öğrenci işlemleri gerçekleştirebilir.

Veri kapsamları gerektiğinde aşağıdaki gibi modellenebilir:

- Üniversite geneli
- Fakülte
- Bölüm
- Ders veya ders açımı
- Kullanıcının kendisi

Mevcut Security modülünü incele. Sistemimizde kullanıcı, rol, permission ve kapsam atamalarını destekleyen bir yapı varsa onu kullanarak ilerle. Bunların uygulaması henüz yoksa OBS içinde kendi başına ikinci bir IAM sistemi kurmak yerine mevcut universal Security modülüyle entegrasyonun eksiklerini belirle.

JWT'ye her işlem için çok sayıda, sürekli güncellenen kapsam bilgisini doldurmak zorunda değilsin. Güncel yetki iptali, kaynak bazlı erişim ve kapsam kontrolü ihtiyacına uygun çözümü değerlendir.

### 4.3. Yetkilendirme kuralları

- Varsayılan davranış izin vermemek olmalıdır.
- Hem listeleme hem tek kayıt sorguları kapsam denetiminden geçmelidir.
- Kaynak kimliğine sahip olmak o kaynak üzerinde yetkili olmak anlamına gelmemelidir.
- İlişkili kaynaklar değiştirildiğinde de yetki kontrolü yapılmalıdır.
- Bir kaydın başka bir fakülteye veya bölüme taşınması ayrıca kontrol edilmelidir.
- Kullanıcı arayüzünden gizlenen butonlar güvenlik kontrolü yerine geçmez.
- Kritik işlemler için audit kaydı tutulmalıdır.
- Yetkisiz işlemler veri değişikliği yapmamalıdır.
- Yetkilendirme mantığını her handler içine farklı şekilde kopyalama; mevcut yapıya uygun, test edilebilir ortak bir çözüm üret.

## 5. Akademik yıl ve müfredat yönetimi

Müfredat konusunda akademik yıl kavramı kritik önemdedir.

2025-2026 ve 2026-2027 dönemlerinde aynı bölüm için farklı müfredatlar bulunabilir. Aynı akademik yıl içinde de birden fazla müfredat sürümü veya program bulunması ihtimali göz önünde bulundurulmalıdır.

### 5.1. Müfredat modeli

Aşağıdaki kavramların birbirinden ayrılmasını değerlendir:

- Bölüm veya program
- Müfredat (Curriculum)
- Müfredat sürümü (CurriculumVersion)
- Müfredatın geçerli olduğu akademik yıl
- Müfredat dersleri
- Dersin zorunlu/seçmeli niteliği
- Dersin AKTS değeri
- Dersin dönemi ve sınıf seviyesi
- Ön koşullar

Bir akademik program için farklı akademik yıllara ait müfredatlar oluşturulabilmelidir.

Örnek:

- Bilgisayar Mühendisliği — 2025-2026 müfredatı
- Bilgisayar Mühendisliği — 2026-2027 müfredatı

Yeni akademik yılın müfredatı oluşturulduğunda eski müfredatın geçmiş kayıtları değiştirilmemelidir.

Müfredat düzenleme işlemi, önceki akademik yıllara ait öğrenci kayıtlarını sessizce değiştirmemelidir. Bunun için sürümleme, yayınlama ve gerektiğinde yeni sürüm oluşturma yaklaşımı kullan.

Önemli ilişkilerde akademik yıl veya sürüm referansını yalnızca isim olarak tutma; uygun bir kalıcı kayıt ve foreign key ilişkisi kur.

### 5.2. Müfredat izinleri

- Yetkili kullanıcı müfredat oluşturabilmelidir.
- Oluşturma ve düzenleme ayrı permission'lar olmalıdır.
- Taslak ve yayınlanmış müfredat durumları gerektiğinde ayrılmalıdır.
- Yayınlanmış ve öğrenciler tarafından kullanılan müfredatın değiştirilmesinde kurallar uygulanmalıdır.
- Bir müfredatın dersleri ve AKTS bilgileri tutarlı biçimde yönetilmelidir.
- Müfredatın hangi program, bölüm ve akademik yıl için geçerli olduğu doğrulanmalıdır.
- Başka bir bölümün müfredatına yetkisiz erişim veya değişiklik engellenmelidir.

Bu işlemleri fakülte ve bölüm kapsamlarıyla birlikte değerlendir. Yeni müfredat oluşturma ve yayınlama işlemlerini aynı yetkiye bağlama.

## 6. Öğrenci yönetimi ve manuel kayıt

Öğrenciler her zaman dışarıdan bir otomasyonla sisteme gelmeyebilir. Öğrenci işleri tarafından manuel öğrenci kaydı açılması gerçek bir gereksinimdir.

Manuel kayıt hem gerçek operasyonlarda hem de geliştirme/test ortamlarındaki mock verilerde kullanılabilmelidir.

### 6.1. Öğrenci oluşturma

En yüksek yetki seviyesinde yetkilendirilmiş kullanıcı, sistem üzerinden manuel öğrenci oluşturabilmelidir.

Öğrenci oluşturma sırasında gerekli olduğu ölçüde aşağıdaki bilgiler ele alınmalıdır:

- Öğrenci numarası
- Ad ve soyad
- İlişkili kullanıcı hesabı
- Bölüm veya akademik program
- Üniversiteye giriş yılı
- Öğrencilik durumu
- Kayıt tarihi
- Öğrencinin bağlı olduğu müfredat sürümü

Gerçek sistemin ihtiyaç duymadığı kişisel verileri gereksiz yere toplama. Hassas veriler, kimlik bilgileri ve hesap güvenliği için mevcut Security ve uygulama kurallarını kullan.

Kurallar:

- Öğrenci numarası için tekillik kontrolü yapılmalıdır.
- İlişkili bölüm ve program gerçek olmalıdır.
- Öğrenciye doğru akademik program ve müfredat sürümü bağlanmalıdır.
- Öğrenci oluşturma ve kullanıcı hesabı oluşturma işlemleri aynı şey olarak kabul edilmemelidir.
- Bir öğrenciye ait kullanıcı hesabının açılması gerekiyorsa bu işlem mevcut Security modülü üzerinden yürütülmelidir.
- Başarısız işlemler yarım öğrenci veya tutarsız ilişkiler bırakmamalıdır.
- Kayıt, silme ve durum değişikliği işlemlerinin yetki kontrolleri ayrı ayrı yapılmalıdır.

Öğrenci oluşturma, sadece request DTO'sunu veritabanına yazmak şeklinde tasarlanmamalıdır.

### 6.2. Mock veri

Test verileri için gerçek öğrenci oluşturma iş akışını kullanabilecek senaryolar geliştirilebilir.

Ancak:

- Production ortamında otomatik olarak mock öğrenci üretme.
- Seed mekanizmasını gerçek öğrenci oluşturma endpoint'iyle karıştırma.
- Mock veriler için ayrı ve güvenli bir geliştirme/test stratejisi oluştur.
- Gerçek endpoint'lerin yetkilendirme kurallarını sırf test amacıyla gevşetme.
- Aynı seed işleminin tekrar çalıştırılması mükerrer kayıt oluşturmamalıdır.

## 7. Ders kataloğu ve dönemlik ders açımları

Ders kavramını, o dersin belirli bir akademik dönemde açılmasından ayır.

Örneğin aynı ders farklı akademik yıllarda, farklı öğretim elemanlarıyla ve farklı programlarda açılabilir.

Aşağıdaki ayrımı değerlendir:

**Ders kataloğu (Course):**
- Ders kodu
- Ders adı
- Açıklama
- İlgili akademik bölüm veya akademik sahiplik
- Varsayılan AKTS ve kredi bilgileri
- Ders türü
- Gerekli ön koşullar

**Ders açımı (CourseOffering):**
- İlgili ders
- Akademik yıl ve dönem
- Dersi açan bölüm/program
- Atanmış öğretim elemanı veya öğretim elemanları
- Derslik veya program bilgileri
- Kontenjan
- Dönemlik değerlendirme planı
- Dersin kayıt durumu

İsimlendirmeyi mevcut domain'e göre belirle; önemli olan kavramların birbirine karıştırılmamasıdır.

Böylece 2025-2026 yılında açılan dersin geçmiş öğretmen ve değerlendirme bilgileri, 2026-2027 açımı oluşturulduğunda bozulmaz.

Aynı dersin iki dönemde farklı değerlendirme şartlarıyla yürütülebilmesi desteklenmelidir.

## 8. Öğretim elemanı ve ders görevlendirmeleri

Bir öğretim elemanının hangi dersleri verdiği, ilgili dönem için açık ve doğrulanabilir bir ilişkiyle tutulmalıdır.

- Yetkili kullanıcı ders açımı oluşturabilmelidir.
- Yetkili kullanıcı öğretim elemanı atayabilmelidir.
- Ders atamaları akademik dönemle ilişkili olmalıdır.
- Öğretmen sadece gerçekten görevlendirildiği dersleri yönetebilmelidir.
- Görevlendirme iptal edildiğinde veya değiştiğinde eski kayıtlar korunmalıdır.
- Bir öğretim elemanının başka bir öğretmenin dersini yalnızca URL'deki kimliği değiştirerek yönetmesi engellenmelidir.
- Gerektiğinde bir derse birden fazla öğretim elemanı atanması değerlendirilebilir; ancak bunun için gerçek iş gereksinimi ve net bir yetki modeli bulunmalıdır.

Öğretim elemanının ders listesini, görevli olduğu ders açımları üzerinden sorgulayan API'ler oluştur.

## 9. Öğrenci ders seçimi ve AKTS sınırı

Öğrenci, kendisine tanımlı müfredat ve akademik dönem kapsamında ders seçebilmelidir.

Ders seçimi yalnızca bir tabloya öğrenci ve ders ID'si eklemekten ibaret değildir.

### 9.1. Ders seçimi kuralları

En azından aşağıdaki kontrolleri değerlendir:

- Ders öğrencinin ilgili müfredatında bulunuyor mu?
- Ders ilgili akademik dönemde açılmış mı?
- Öğrenci ders seçimine uygun durumda mı?
- Aynı ders daha önce seçilmiş mi?
- Dersin kontenjanı dolmuş mu?
- Dersin ön koşulları karşılanıyor mu?
- Ders programında çakışma var mı?
- Öğrencinin seçtiği toplam AKTS geçerli limiti aşıyor mu?
- Akademik kayıt dönemi açık mı?
- Öğrencinin ders için gerekli akademik uygunluğu var mı?

İhtiyaç duyulmayan kuralları sırf kapsamlı görünmek adına sabit ve değiştirilemez şekilde kodlama. Örneğin bütün programlarda aynı kayıt takvimini veya aynı ders seçimi politikasını varsayma.

### 9.2. AKTS hesaplama

AKTS kontrolü yapılandırılabilir bir kurala dayanmalıdır.

Öğrencinin ilgili akademik dönem için seçtiği derslerin AKTS değerlerini topla ve geçerli limitle karşılaştır.

Sistem genelinde tek bir sayıyı her senaryoya zorla uygulama. Uygun yapılandırma seviyesini (akademik dönem, program veya ilgili politika) inceleyerek belirle.

AKTS hesabında hangi durumların toplama dahil edildiğini açıkça tanımla. Taslak, bekleyen, onaylanmış ve iptal edilmiş ders kayıtlarının aynı şekilde değerlendirilmesi gerekmeyebilir.

### 9.3. AKTS limitini aşma yetkisi

Öğrenci işleri, özel durumlarda AKTS limitini esnetebilmelidir.

Bu işlem için ayrı bir `course-selection.ects-limit.override` benzeri permission kullanılmalıdır.

Esnetme şu şekilde çalışmalıdır:

- Yetkili kullanıcı mevcut limiti geçici veya öğrenci/dönem bazında değiştirilebilir bir istisna kapsamında aşabilmelidir.
- Yetkisiz kullanıcı aynı işlemi gerçekleştirememelidir.
- İşlem nedeni/açıklaması tutulmalıdır.
- İşlemi kimin yaptığı ve ne zaman yaptığı kaydedilmelidir.
- Mümkünse önceki limit, yeni limit ve ilgili ders seçimi bilgileri audit kaydında bulunmalıdır.
- İstisnanın kapsamı belirgin olmalıdır; tek bir öğrencinin istisnası tüm öğrencileri etkilememelidir.
- Bir istisna, ön koşullar ve program çakışmaları gibi ilgisiz kuralları otomatik olarak devre dışı bırakmamalıdır.
- Limit yükseltme, seçilen dersleri otomatik olarak eklemekle aynı işlem kabul edilmemelidir.

İş kuralını birden fazla farklı endpoint'te ayrı ayrı uygulama. Öğrenci kendi kaydını oluştururken ve öğrenci işleri manuel kayıt yaparken tutarlı davranış gösteren bir çözüm geliştir.

### 9.4. Manuel ders kaydı

Öğrenci işleri, uygun izinlere sahip olduğunda öğrenci adına ders kaydı oluşturabilmelidir.

Manuel kayıt da aynı kritik kontrollerden geçmelidir. Yetkili bir istisna verildiğinde yalnızca açıkça izin verilen kural esnetilmelidir.

Eşzamanlı işlemlerde kontenjanın veya AKTS limitinin aşılması gibi sorunlar için gerekli transaction ve concurrency önlemlerini değerlendir.

## 10. Ders programı ve haftalık çizelge

Öğrenciler seçtikleri derslerin haftalık programını görebilmelidir.

Öğretim elemanları da görevli oldukları derslerin programlarını görüntüleyebilmelidir.

Programlama modelinde şu unsurlar değerlendirilmeli:

- Ders açımı
- Gün
- Başlangıç ve bitiş saati
- Derslik veya çevrim içi ders bilgisi
- Akademik dönem
- Program çakışmaları

Bir dersin birden fazla haftalık oturumu olabilir. Laboratuvar veya uygulama oturumlarıyla normal ders saatleri ayrışabilir.

Gerektiğinde ders programı birden fazla zaman diliminden oluşabilmelidir.

Kontroller:

- Başlangıç saati bitiş saatinden önce olmalıdır.
- Aynı öğrenci için aynı zaman diliminde çakışan derslerin seçilmesi engellenmeli veya açıkça yönetilmelidir.
- Öğretim elemanı aynı anda iki farklı yerde derse atanamamalıdır.
- Aynı derslik aynı zaman diliminde birden fazla çakışan derse tahsis edilmemelidir.
- Çakışma kontrolü ilgili akademik dönem ve gerçek zaman aralıklarıyla çalışmalıdır.

Ders programının öğrenciler ve öğretmenler için nasıl sorgulanacağına yönelik API'ler tasarla.

## 11. Sınav ve değerlendirme sistemi

Bir dersin değerlendirilmesi sadece `Vize` ve `Final` gibi iki sabit alandan oluşmamalıdır.

Derslerde aşağıdaki değerlendirme türleri kullanılabilir:

- Ara sınav
- Final sınavı
- Bütünleme sınavı
- Kısa sınav
- Ödev
- Proje
- Laboratuvar
- Uygulama
- Katılım
- Diğer yapılandırılabilir değerlendirmeler

Bunları kod içerisinde sabit bir listeye bağlama. Ancak tamamen serbest ve doğrulanmayan bir model de oluşturma.

Değerlendirme türü, ağırlık, zorunluluk, maksimum puan ve gerektiğinde geçerlilik kuralları olan yönetilebilir bir yapı tasarla.

### 11.1. Varsayılan sınav şablonu

Ders açıldığında kullanılabilecek varsayılan bir değerlendirme şablonu bulunmalıdır.

Örnek bir şablon:

- Vize: %40
- Final: %60

Bu yalnızca örnektir. Değerleri uygulamanın her yerinde sabitleme.

Şablonlar üzerinden:

- Değerlendirme bileşenleri tanımlanabilmeli.
- Bileşenlerin ağırlıkları düzenlenebilmeli.
- Gerekli değerlendirme türleri belirtilebilmeli.
- Uygun olduğunda final veya bütünleme gibi sınav türleri işaretlenebilmeli.
- Şablonlar yönetilebilir ve sürümlenebilir olmalı.
- Şablonun geçerli olduğu fakülte, program veya diğer kapsamlar açıkça belirlenmeli.

### 11.2. Fakülteye özel şablonlar

Her fakülte varsayılan sınav/değerlendirme şablonlarını yapılandırabilmelidir.

Örneğin bir fakültede vize-final ağırlıklı bir şablon, başka bir fakültede ödev-proje-laboratuvar ağırlıklı bir şablon varsayılan olabilir.

Yetki davranışı:

- Öğrenci işleri, uygun yetki kapsamı içinde gerekli akademik ayarlamaları yapabilir.
- Fakülte sorumlusu yalnızca kendi fakültesinin şablonlarını düzenleyebilir.
- Bölüm sorumlusu, açıkça yetki verilmediği sürece fakülte genelindeki şablonları değiştiremez.
- Öğretmen, fakülte genelindeki varsayılan şablonları yönetemez.

Varsayılan şablonun güncellenmesi, daha önce açılmış ve değerlendirmeleri kullanılmakta olan tüm dersleri kendiliğinden değiştirmemelidir.

Yeni şablon sürümüyle geçmiş ders açımlarındaki değerlendirme kurallarının ilişkisini dikkatle tasarla. Ders açımı oluşturulurken varsayılan şablondan uygun bir dönemlik değerlendirme planı oluşturulması veya bir sürüm referansı alınması değerlendirilebilir.

### 11.3. Dersin değerlendirme koşulları

Öğretim elemanı kendi dersi için izin verilen değerlendirme koşullarını düzenleyebilmelidir.

Örneğin:

- Ara sınav ağırlığı
- Final ağırlığı
- Ödev ve proje ağırlıkları
- Değerlendirme türlerinin etkinliği
- Dersin değerlendirme planında izin verilen diğer sınırlı ayarlar

Ancak her öğretmen bütün akademik politikalara müdahale edememelidir.

Değişikliklerin hangi aşamaya kadar yapılabileceğini değerlendir. Örneğin öğrencilerin notları girildikten veya değerlendirme tamamlandıktan sonra yapılan değişiklikler ek kontroller gerektirebilir.

Ağırlıkların toplamı yüzdeyle hesaplanıyorsa doğrulama uygulanmalıdır. Gerekli olduğu durumda ağırlık toplamı %100 olmalıdır. Ağırlık hesabının kullanılmadığı değerlendirme yöntemleri varsa bunları da gereksiz biçimde zorlamamalısın.

Şablon varsayılanı ile dersin gerçekleşen değerlendirme planını birbirinden ayır.

## 12. Çan eğrisi ve notlandırma

Öğretmen, görevlendirildiği ders için çan eğrisi gibi sınırlı notlandırma seçeneklerini açıp kapatabilmelidir.

Örnek permission:

`course.grading.bell-curve.manage`

Bu permission yalnızca atandığı dersin kapsamı içinde geçerli olmalıdır.

Kurallar:

- Öğretmen yalnızca kendi dersinin ilgili notlandırma ayarını değiştirebilmelidir.
- Çan eğrisinin açık veya kapalı olduğu saklanmalıdır.
- Değişiklik yapan kullanıcı ve zaman bilgisi kaydedilmelidir.
- Notlandırma tamamlanmışsa değişiklik için ek kısıtlar uygulanmalıdır.
- Çan eğrisinin açılması veya kapatılması başka dersleri etkilememelidir.
- Not hesaplama algoritması ile bu algoritmanın kullanımını etkinleştiren ayarı birbirinden ayır.
- Çan eğrisi algoritmasını ihtiyaç duyulmadan sabit aralıklara veya belirli bir üniversitenin politikasına bağlama.
- Gerçek not hesaplama implementasyonu geliştiriliyorsa algoritmanın akademik kuralları ayrıca açıkça tanımlanmalıdır.

Bu aşamadaki hedef, ilgili ayarı güvenli biçimde yönetebilen ve gelecekte hesaplama mantığının eklenmesine uygun bir domain tasarlamaktır. İş gereksinimi netleşmeden eksiksiz bir harf notu dönüşüm algoritması uydurma.

## 13. Ders değerlendirme planı ve geçmiş veriler

Bu noktada farklı veri seviyelerini karıştırma:

1. Fakültenin varsayılan değerlendirme şablonu.
2. Belirli bir ders için geçerli değerlendirme planı.
3. Belirli bir dönem için açılmış dersin değerlendirme yapılandırması.
4. Öğrencilere ait not ve değerlendirme sonuçları.

Bu seviyeler arasında açık ilişkiler bulunmalıdır.

Varsayılan şablon değiştirildiğinde geçmiş dönem sonuçları veya tamamlanmış derslerin değerlendirme yapısı bozulmamalıdır.

Bir öğretim elemanının yeni dönem için değerlendirme koşullarını güncellemesi, başka bir dönemde tamamlanan dersin sonuçlarını geriye dönük olarak değiştirmemelidir.

Yayınlanmış değerlendirme planı değiştirilmek istendiğinde uygulanacak geçiş ve audit kurallarını tasarla.

## 14. Öğrenci işleri işlemleri

Öğrenci işleri yalnızca rapor görüntüleyen bir kullanıcı değildir. Sistemdeki idari süreçleri yürütür.

Yetkilendirmeye bağlı olarak aşağıdaki kullanım senaryoları desteklenmelidir:

- Öğrenci oluşturma ve öğrenci bilgilerini düzenleme.
- Öğrenciyi bir bölüm veya akademik programa bağlama.
- Öğrencinin müfredat ilişkisini yönetme.
- Fakülte ve bölüm oluşturma/düzenleme.
- Öğrencinin ders kayıtlarını yönetme.
- Gerekli durumda ders seçimini manuel yapma.
- AKTS limitini gerekçeli bir istisnayla aşma.
- İlgili akademik kayıt ve durumları görüntüleme.

Öğrenci işleri yetkisi ile sistem yöneticisi yetkisini otomatik olarak eşitleme. Öğrenci işleri bir kurumun akademik operasyonlarını yürütebilir; ancak kullanıcı rollerini veya tüm permission tanımlarını yönetmek zorunda değildir.

## 15. Audit ve izlenebilirlik

Aşağıdaki işlemler en azından audit bakımından değerlendirilmelidir:

- Fakülte ve bölüm oluşturma/düzenleme.
- Müfredat oluşturma, yayınlama ve sürüm değiştirme.
- Öğrenci oluşturma ve program ilişkisini değiştirme.
- Manuel ders kaydı.
- AKTS istisnası verme.
- Ders programında değişiklik.
- Öğretmen ataması değişikliği.
- Fakülte değerlendirme şablonu değişikliği.
- Ders değerlendirme planı değişikliği.
- Çan eğrisi ayarı değişikliği.
- Kritik rol ve permission değişiklikleri.

Mümkün olduğunda aşağıdaki bilgileri kaydet:

- İşlemi gerçekleştiren kullanıcı.
- İşlem tarihi.
- İşlemin türü.
- Etkilenen kaynak.
- Önceki ve yeni değerler veya anlamlı değişiklik özeti.
- Gerekçe gerektiren işlemlerde gerekçe.
- İlgili akademik dönem ve kapsam.

Audit içeriğine parola, erişim token'ı veya başka hassas sırları yazma.

Projede evrensel Auditing modülü varsa onu inceleyip kullan. Böyle bir modül yoksa bütün audit altyapısını tek seferde bağımsız bir sistem olarak icat etme; önce mevcut mimarideki entegrasyon noktasını belirle.

## 16. API tasarımı

Mevcut API sözleşmelerimizle uyumlu, tutarlı ve dokümante edilebilir endpoint'ler geliştir.

Gerekli kullanım senaryolarını aşağıdaki kategorilerde ele al:

- Faculty management
- Department management
- Academic year and term management
- Curriculum and curriculum version management
- Student management
- Course catalog management
- Course offering management
- Instructor assignment management
- Student course selection
- Manual course registration
- Student and instructor timetable queries
- Assessment template management
- Course assessment configuration
- Grading configuration
- Permission ve kapsam yönetimi için mevcut Security entegrasyonu

Bu kategorilerdeki her CRUD işleminin mutlaka aynı controller içinde bulunması gerekmez. Mevcut API yapısına göre tasarla.

API kuralları:

- API versioning yaklaşımını koru.
- Request DTO, response DTO ve domain modelini birbirine karıştırma.
- EF entity'lerini doğrudan dışarı açma.
- Listeleme endpoint'lerinde uygun pagination, filtering ve sorting kullan.
- Beklenmeyen null, invalid ID ve ilişki hatalarını kontrollü yönet.
- Yetkisiz kaynak erişimini doğru HTTP semantiğiyle ele al.
- Var olan `ServiceResult<T>` ve `Error` yapısını kullan.
- Hata kodlarını UI'dan bağımsız, kararlı kodlar şeklinde üret. Örneğin `Department.NotFound`, `Curriculum.InvalidAcademicYear` veya `CourseSelection.EctsLimitExceeded`.
- Hata yanıtlarında mevcut standarttaki hata dizisini koru.
- RFC 9457 Problem Details entegrasyonu varsa mevcut yaklaşımıyla uyumlu kal.
- HTTP statü kodlarını başarılı ve başarısız işlemlere uygun şekilde ata.
- Hata ayrıntılarında stack trace veya hassas iç sistem bilgisini dışarı verme.

Örnek hata kodları yalnızca başlangıç önerisidir. Projenin mevcut ErrorType ve ServiceResult sözleşmesine göre gerçek kodları belirle.

## 17. Domain doğrulamaları ve işlem bütünlüğü

Aşağıdaki kontrollerin yalnızca istemci tarafında yapılmasına güvenme:

- Fakülte ve bölüm ilişkileri.
- Akademik yıl ve dönem ilişkileri.
- Program ve müfredat uyumluluğu.
- Öğrenci numarası tekilliği.
- Ders görevlendirmeleri.
- Öğrenci ders seçimi.
- AKTS hesaplaması.
- Ders programı çakışmaları.
- Sınav ağırlıkları.
- Permission ve kaynak kapsamı.
- Şablon veya müfredat sürümü değişiklikleri.

Validasyonun doğru seviyesini seç:

- İstek formatı ve basit input kontrolleri: request validation.
- Kullanım senaryosuna özgü koşullar: application katmanı.
- Domain bütünlüğü ve geçersiz durumların engellenmesi: domain katmanı.
- Unique constraint, foreign key ve concurrency gibi kalıcı veri garantileri: veritabanı ve persistence katmanı.

Birden fazla veritabanı yazımı gerektiren operasyonlarda transaction ihtiyacını değerlendir. İlişkili kayıtları oluştururken yarım kalan işlemlere karşı koruma sağla.

Eşzamanlı iki isteğin aynı kontenjanı doldurması veya aynı öğrencinin limitini birlikte aşması gibi yarış durumlarını göz ardı etme.

## 18. Veritabanı ve migration yaklaşımı

Mevcut EF Core DbContext ve entity configuration düzenini incele.

Yeni model ve ilişkileri mevcut persistence yapısına göre ekle.

- Foreign key ve unique index gereksinimlerini belirle.
- Uygun ilişkilerde delete behavior'ı açıkça tanımla.
- Geçmiş akademik verilerin yanlışlıkla silinmesini engelle.
- Müfredat ve değerlendirme şablonu sürümlerini veri tutarlılığını koruyacak şekilde modelle.
- Gerekli migration'ları oluştur.
- Mevcut migration'ları kontrol et; sırf yeni tasarıma uymuyor diye geçmiş migration geçmişini gelişigüzel değiştirme.
- Veri kaybına sebep olabilecek migration'ları özellikle incele.
- Varsayılan ve seed verilerini idempotent tasarla.
- Test ortamı için gerekli başlangıç verileri ile production verilerini birbirinden ayır.

Her entity için gereksiz yere repository üretme. Gerçek sorgu ve aggregate ihtiyaçlarına göre mevcut persistence yaklaşımını kullan.

## 19. Test stratejisi

Yeni özellikleri yalnızca derleme başarılı olduğu için tamamlanmış kabul etme.

En azından domain, application ve API/persistence düzeylerinde ihtiyaç duyulan testleri tasarla.

Özellikle şu senaryoları test et:

### Yetkilendirme

- Bölüm sorumlusu kendi bölümünü düzenleyebilir.
- Bölüm sorumlusu başka bir bölümü düzenleyemez.
- Fakülte sorumlusu kendi fakültesine bağlı bölümleri yönetebilir.
- Fakülte sorumlusu başka bir fakültenin kayıtlarını yönetemez.
- Öğrenci işleri, kendisine tanınan yetkilerle fakülte ve bölüm işlemlerini gerçekleştirebilir.
- Öğretmen kendi dersini yönetebilir, başka bir öğretmenin dersini değiştiremez.
- Sadece okuma izni olan kullanıcı kayıt değiştiremez.
- Yetkisi iptal edilmiş kullanıcı eski bir kapsam üzerinden işlem yapamaz.

### Müfredat

- Farklı akademik yıllara ait müfredatlar birlikte saklanabilir.
- Yeni müfredat sürümü oluşturmak eski öğrenci kayıtlarını değiştirmez.
- Yetkisiz kullanıcı müfredatı düzenleyemez veya yayınlayamaz.
- Müfredatın bölüm/program ilişkileri doğrulanır.

### Öğrenci ve ders seçimi

- Yetkili kullanıcı öğrenci oluşturabilir.
- Yetkisiz kullanıcı oluşturamaz.
- Mükerrer öğrenci numarası engellenir.
- AKTS limitinin altındaki kayıt başarılı olur.
- Limit aşıldığında yetkisiz ders seçimi reddedilir.
- Yetkili istisna, gerekçe ve audit bilgisiyle uygulanır.
- Ders kontenjanı ve ön koşullar kontrol edilir.
- Ders programı çakışmaları kontrol edilir.
- İptal edilmiş ders kaydı AKTS hesabını yanlış etkilemez.

### Değerlendirme

- Fakülteye özel şablon oluşturulabilir.
- Şablonun değiştirilmesi tamamlanmış dönemlerin sonuçlarını bozmaz.
- Öğretmen yalnızca görevlendirildiği dersin koşullarını değiştirebilir.
- Geçersiz değerlendirme ağırlıkları reddedilir.
- Çan eğrisi ayarı yalnızca yetkili öğretmen tarafından ilgili derste değiştirilebilir.

Test isimleri ve framework'ü mevcut test yapısına göre seç. Gerekiyorsa unit test, integration test ve gerçek veritabanı davranışını sınayan testleri birbirinden ayır.

## 20. Geliştirme aşamaları

Bütün özellikleri tek bir devasa değişiklik halinde yazma. Önce mevcut solution'ı incele ve işleri dikey dilimler halinde uygula.

### Aşama 1 — Repository keşfi ve tasarım

- Solution ve proje yapısını incele.
- Mevcut Domain, Application, Infrastructure ve API katmanlarını belirle.
- Security, permission, auditing ve result altyapılarının hangi noktada olduğunu tespit et.
- İlgili entity, DbContext, configuration, handler, controller/endpoint ve test örneklerini bul.
- Mevcut işlevlerle çakışabilecek yeni yapıları belirle.
- Önerilen domain ilişkilerini ve yetki matrisini çıkar.
- Eksik gereksinimleri varsayımlardan ayır.

### Aşama 2 — Fakülte, bölüm ve akademik yapı

- Fakülte ve bölüm yönetimi.
- Permission ve kaynak kapsamı entegrasyonu.
- Akademik yıl ve dönem.
- Program ve müfredat sürümü.
- Gerekli API ve persistence yapıları.

### Aşama 3 — Öğrenci ve ders kataloğu

- Manuel öğrenci oluşturma.
- Öğrencinin program ve müfredat ilişkisi.
- Ders kataloğu.
- Dönemlik ders açımları.
- Öğretim elemanı atamaları.

### Aşama 4 — Ders seçimi

- Ders kayıtları.
- AKTS limit politikası.
- Öğrenci işleri istisnaları.
- Ön koşullar, kontenjan ve zaman çakışması kontrolleri.
- Manuel ders kaydı.
- Transaction ve concurrency kontrolleri.

### Aşama 5 — Ders programı ve değerlendirme

- Öğrenci/öğretmen ders programı sorguları.
- Fakülte varsayılan değerlendirme şablonları.
- Şablon sürümleme.
- Ders bazlı değerlendirme planı.
- Öğretmenlerin sınırlı değerlendirme ayarları.
- Çan eğrisi yapılandırması.

### Aşama 6 — Sağlamlaştırma

- Audit entegrasyonu.
- Yetkilendirme testleri.
- Domain ve application testleri.
- Migration incelemesi.
- API hata sözleşmelerinin kontrolü.
- Derleme, test ve regresyon kontrolü.

Bu aşamalar kesin klasör isimleri veya modül isimleri dayatmaz. Mevcut repository yapısına uygun biçimde uyarlanmalıdır.

## 21. Uygulama kuralları ve teslim beklentisi

Çalışma boyunca şu kurallara uy:

1. Var olan kodu okumadan yeni mimari kurma.
2. Gerçekten bulunmayan bir repository, abstraction veya servisin zaten var olduğunu varsayma.
3. Eksik Security veya Auditing işlevlerini tamamlanmış gibi kabul etme.
4. Tüm permission kontrollerini yalnızca controller katmanına bırakma.
5. Permission ile kaynak kapsamını birbirinden ayır.
6. İş kurallarını doğrudan controller'lara veya yalnızca FluentValidation kurallarına sıkıştırma.
7. Yıllara göre değişen akademik verileri tek bir güncel kayıt üzerine yazarak geçmişi kaybetme.
8. Varsayılan şablonları geçmişteki derslerin gerçek değerlendirme koşullarıyla karıştırma.
9. Gerçek gereksinim olmadan mikroservis, event bus, generic repository veya aşırı soyutlama ekleme.
10. Her işlemde tutarlı authorization ve validation uygula.
11. Kritik işlemlerde audit ve işlem bütünlüğünü göz ardı etme.
12. Mevcut kodla çelişen tasarım farklarını saklama; kısa bir teknik gerekçe sun.
13. Üzerinde anlaşılmamış akademik politika değerlerini kod içinde sabitleme. Gerekli yerlerde konfigürasyon veya açık iş kuralları öner.
14. Kod yazdıktan sonra ilgili projeleri derle ve mevcut testleri çalıştır.
15. Başarısız testleri veya derleme hatalarını gizleme.
16. Sadece derlenmesi için yüzeysel mock veya boş implementasyonlar kullanarak gerçek gereksinimleri tamamlanmış gösterme.

### İlk çıktı

Repository incelemesinden sonra şu bilgileri sun:

- Mevcut mimarinin özeti.
- OBS domain'inde hâlihazırda bulunan yapıların listesi.
- Kullanılabilecek Security ve permission altyapısı.
- Oluşturulması veya değiştirilmesi gereken domain kavramları.
- Önerilen temel entity ilişkileri.
- Yetki matrisi ve kapsam davranışları.
- Eksik veya belirsiz iş kuralları.
- Uygulama sırası ve olası migration riskleri.

Ardından en küçük anlamlı dikey dilimden başlayarak uygulamaya geç. Bir aşamayı tamamlanmış saymadan önce ilgili kodu ve testleri doğrula.

Nihai hedef, yalnızca çalışan endpoint'ler üretmek değil; gerçek üniversite operasyonlarını destekleyen, geçmiş akademik kayıtları koruyan, permission ve kaynak kapsamı güvenliği sağlam olan, mevcut universal backend mimarisiyle uyumlu bir OBS backend oluşturmaktır.

---

## 🚀 Tamamlanan Gelişmiş Kurumsal Modüller (2026 Güncellemesi)

Sol menü, dashboard ve kullanıcı talimatlarındaki tüm modüller modüler monolit mimarisinde Domain, Application, Infrastructure ve API katmanlarıyla eksiksiz inşa edilmiştir:

### 1. Çoklu Bölüm / Program & Çift Kadro Mimarisi
- **Öğrenci:** Bir öğrencinin aynı anda Ana Dal (Major), Çift Ana Dal (ÇAP / DoubleMajor), Yandal (Minor) veya Yatay Geçiş (Transfer) programlarında kayıtlı olabilmesi; üst bardaki `"Akademik Program Seçiniz"` seçicisi üzerinden aktif program bağlamını dinamik değiştirebilmesi sağlandı.
- **Öğretim Elemanı:** Öğretim elemanlarının birden fazla fakülte ve bölümde Asil Kadro (`PrimaryCadre`), Ek Görevlendirme (`SecondaryAssignment`) veya Misafir (`Adjunct`) statüsünde eşzamanlı atanabilmesi sağlandı.

### 2. Kampüs Yaşamı & Tesis Rezervasyonları (`CampusLife`)
- Slot tabanlı rezervasyon yönetimi: Spor Salonları, Halı Saha, Psikolojik Destek & Danışmanlık (PDR), Sosyal Market.
- Yetkili onay ve kontrol mekanizması (`ReviewFacilityReservation`: Onay / Red / Açıklama).
- Öğrenci Toplulukları ve Kulüp Etkinlik Yönetimi (`StudentClub`, `ClubEvent`).

### 3. Servis Destek Masası (`SupportDesk`)
- Öğrenci ve personelin ilgili kurumu/birimi (Öğrenci İşleri, Bilgi İşlem, SKS, Kütüphane, Dekanlık, Bölüm Başkanlığı, Rektörlük, Kariyer Merkezi) seçerek destek talebi açabilmesi.
- Bilet numaralandırması (`SRV-...`), öncelik ve durum takibi, mesajlaşma geçmişi ve birim yetkilisi atama mekanizması.

### 4. Sürekli Eğitim Merkezi (`ContinuingEducation` / SEM)
- Sertifika, kurs ve eğitim programı açma, kapasite ve ücret yönetimi.
- Kursiyer başvurusu, yetkili onayı, ders katılım/yoklama saati takibi.
- Notlandırma ve devam koşulu (`MinAttendancePercentage` & `MinPassingGrade`) sağlandığında otomatik benzersiz sertifika numarası (`SEM-YYYY-...`) üretimi.

### 5. Elektronik Belge Yönetim Sistemi (`DocumentManagement` / EBYS & Belge Doğrulama)
- Resmi yazı, Gelen/Giden evrak, Karar, Dilekçe ve Belge oluşturma.
- Paraf ve İmza iş akışı (`DocumentWorkflowStep`: İnceleme/Paraf -> İmza/Nihai Onay).
- Otomatik evrak sayısı (`E-DEPT-YYYY-...`) ve benzersiz doğrulama kodu (`DOC-...`).
- **Anonim / Harici Doğrulama API:** `GET /api/v1/documents/verify/{verificationCode}` üzerinden dış paydaşların, işverenlerin veya karekod okuyucuların oturum açmadan resmi belge geçerliliğini ve içeriğini doğrulayabilmesi.

### 6. Öğrenci Başvuru İşlemleri (`StudentApplications`)
- ÇAP (Çift Ana Dal) Başvurusu
- Yandal Başvurusu
- Yatay Geçiş Başvurusu
- Dış Kurum / Yaz Okulu Ders Onayı Başvurusu
- Belge Talebi Başvurusu (Transkript, Öğrenci Belgesi vb.)
- Erasmus / Değişim Programı Başvurusu
- Bütünleşik Yüksek Lisans Başvurusu
- İlişik Kesme Talebi
- Yatay Geçişte Engel Yoktur Süreci Başlatma Talebi
- İzin / Kayıt Dondurma Talebi
- Yetkili onay/red değerlendirme süreci ve gerekçelendirme.

### 7. Öğrenci Paneli & "Diğer" Menüsü (`StudentPortal`)
- **Üst Bar:** "Akademik Program Seçiniz" (Aktif program bağlamı), "Transkript" (Hızlı kümülatif transkript görüntüleme).
- **Dashboard Kartları:** Derslerim, Canlı Derslerim (Zoom/Teams), Takvim, Online Sınav, Ders Seçimi - Kayıt Yenileme, Mezun İletişim Sistemine Giriş, ÇAP/Yandal Başvurusu, Erasmus Başvurusu, Haftalık Ders Programı, Dış Kurum Yaz Okulu Ders Onay Başvurusu, Belge Talebi, Bütünleşik Yüksek Lisans Başvurusu.
- **"Diğer" Menüsü (14 Kalem):**
  1. Harç Bilgileri (Dönem harcı, ödeme, kalan borç, muafiyet)
  2. Danışman Bilgisi (Akademik danışman, unvan, ofis, görüşme saatleri)
  3. Tarihçe Bilgileri (Kayıt ve intibak tarihçesi)
  4. Kayıtlanma Onay Raporu (Ders kayıt ve danışman onay fişi)
  5. Aktiviteler (Etkinlik ve bildirim sayısı)
  6. İlişik Kesme Talebinde Bulun
  7. Yatay Geçişte Engel Yoktur Sürecini Başlat
  8. Askerlik Durumu (Tecil ve ASAL bildirimleri)
  9. Akademik Takvim (Dönem başlangıç, vize/final haftaları)
  10. Profil Resmi Yönetimi (Fotoğraf yükleme ve onay süreci)
  11. Ceza Bilgileri (Disiplin cezaları)
  12. İzin Bilgileri (Kayıt dondurma)
  13. IBAN Bilgileri (Banka hesap ve IBAN yönetimi)
  14. Ek Süre Bilgileri (Azami süre ve ek sınav hakları - örn. 9. Eğitim Dönemi)

### 8. Kişiselleştirilebilir Widget & Kısayol Sistemi (`Dashboard`)
- **Dinamik Widget Kataloğu (`WidgetType`):** Ders Listesi (`CourseList`), Haftalık Program (`WeeklySchedule`), Akademik Takvim, Duyurular, Anketler, Yaklaşan Sınavlar, Devamsızlık Durumu, Harç Bilgisi, Destek Biletleri, Rezervasyonlar ve Kısayollar.
- Rol bazlı widget filtreleme (Öğrenci, Akademisyen, İdari Personel).
- Panoya yeni widget ekleme (`POST /api/v1/dashboard/widgets`), kolon genişliği (1-12 span), sıralama ve yapılandırma JSON desteği, widget silme (`x` butonu).
- Kişisel kısayollar (Kişisel link, başlık, ikon, renk ve sıralama).

### 9. Toplumsal Katkı Koordinatörlüğü (`SocialContribution`)
- **Sosyal Sorumluluk & Gönüllülük Projeleri:** Sosyal sorumluluk, halka açık eğitim, çevre/sürdürülebilirlik, afet yardımı ve kültürel miras kategorileri.
- Proje teklif oluşturma, koordinatör atama, hedef kitle, iş birliği yapılan STK/Belediye kurumları, başlama/bitiş tarihleri ve gönüllü kontenjanı yönetimi.
- Koordinatörlük onay/ret mekanizması (`Approve`, `Reject`).
- Öğrenci ve personelin projeye gönüllü başvurusu yapabilmesi (`ApplyForVolunteering`), koordinatörün gönüllü onaylama/reddetme akışı ve kontenjan kontrolü.
- Proje tamamlama, ulaşılan yararlanıcı sayısı (`BeneficiaryCount`) ve sosyal etki özeti (`ImpactSummary`) raporlama altyapısı.

### 10. Herkese Açık Eğitim Kataloğu & Müfredat Portalı (`EducationCatalog`)
- **Akademik Hiyerarşi Ağacı:** Rektörlük -> Fakülte / Enstitü -> Bölüm -> Program (Eğitim Düzeyine göre: Ön Lisans, Lisans, Yüksek Lisans, Doktora filtreleme).
- **Program Tanımı (Profil):** Kuruluş, Kazanılan Derece, Derece Düzeyi, Kabul ve Kayıt Koşulları, Önceki Öğrenmenin Tanınması, Yeterlilik Koşulları (240 AKTS vb.), Programın Amacı, İstihdam Profilleri, Üst Dereceye Geçiş, Sınav/Ölçme/Değerlendirme, Mezuniyet Şartları, Çalışma Şekli, Bölüm Olanakları ve İletişim Bilgileri.
- **Program Çıktıları:** Bölümün yetkinlik ve kazanım maddeleri.
- **Programın Öğretim Planı (Müfredat):** Dönem bazlı (1. Yarıyıl, 2. Yarıyıl ... 8. Yarıyıl) ders tablosu: Ders Kodu, Ders Adı, Ders Tipi (Zorunlu / Üniversite Seçmeli / Bölüm Seçmeli), Dili, Teorik (T), Uygulama (U), Laboratuvar (L), Yerel Kredi ve AKTS toplamları.
- **Ders Bilgi Paketi / Ders İzlencesi (Syllabus):** Dersin Seviyesi, Dili, Ön Koşulları, Amacı, Haftalık İçeriği, Ders Kitabı / Kaynaklar, Planlanan Öğrenme Etkinlikleri, Dersi Veren Öğretim Elemanları, Ders Öğrenme Çıktıları.
- **Erişim:** Herkes tarafından (oturum açmaya gerek olmadan) görüntülenebilir (`AllowAnonymous`).

### 11. Kurumsal Değerlendirme & Üniversite İstatistikleri (`InstitutionalEvaluation`)
Paylaşılan kurumsal değerlendirme menü ağacının tüm kalemlerini karşılayan şeffaf istatistik ve kalite göstergeleri sistemi:
- **Öğrenci İstatistikleri:** Toplam Öğrenci Sayıları, Programa Göre, Birimlere Göre, Ön Lisans, Lisans, Lisansüstü, Uluslararası, Hazırlık, İl/Bölge Bazında ve Sınıf Bazında Öğrenci Sayıları.
- **Mezun Öğrenci İstatistikleri:** Mezun Öğrenci Sayıları ve Birimlere Göre Mezun Sayıları (Yıl ve birim dağılımı).
- **Personel İstatistikleri:** Akademik Personel Sayıları, Görev Yerine Göre ve Kadro Yerine Göre Dağılımlar, İdari Personel Sayıları.
- **Yayınlar:** Akademik Yayınlar (SCI-Expanded, Scopus, Ulusal Hakemli, Kitaplar).
- **Birim İstatistikleri:** Türlerine Göre Birim Sayıları (Fakülte, Enstitü, MYO, Araştırma Merkezi) ve Akademik Program Sayıları.
- **Göstergeler (KPI & YÖKAK):** Öğrenci/Öğretim Üyesi Oranı, Fiziki Alanlar ($m^2$), Fiziki Alan Oranları (Yeşil Alan/Öğrenci, Laboratuvar/Öğrenci), Bologna Göstergeleri uyumu.
- **BAP İstatistikleri:** Proje Sayıları (Kapsamlı, Hızlı Destek, Lisansüstü Tez, Sanayi İşbirliği) ve Hibe Bütçe Dağılımı.
- **Uzaktan Eğitim İstatistikleri:** Aktif Online Dersler, Senkron Canlı Oturumlar, Asenkron Videolar, Online Sınav İstatistikleri ve Katılım Oranları.
- **Erişim:** Öğrenciler, personeller ve dış kamuoyu tarafından şeffaflıkla incelenebilmesi için `.AllowAnonymous()` olarak açık sunulmuştur.

> **Test Durumu:**  
> - `dotnet test ../tests/UniversalSystem.UnitTests` -> **130 test başarılı (%100 Pass)** *(Core, Security, Files, Notifications, Translations, Messaging, Auditing, Scheduling, Webhooks, ImportExport, Architecture)*.  
> - `dotnet test ../tests/Obs.UnitTests` -> **74 test başarılı (%100 Pass)** *(Academic, Dashboard, CampusLife, SupportDesk, SEM, EBYS, StudentApplications, StudentPortal, SocialContribution, EducationCatalog, InstitutionalEvaluation)*.  
> - **Toplam:** **204 testin tamamı başarılı (%100 Pass, 0 Hata, 0 Uyarı)**.