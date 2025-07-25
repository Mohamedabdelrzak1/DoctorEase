# 🩺 دليل استخدام API - نظام حجز العيادة

## 🔗 Endpoints المتاحة

### 1️⃣ Authentication

#### تسجيل دخول الأدمن
```
POST /api/auth/login
```
**الوصف:** تسجيل دخول الأدمن
**المعاملات (JSON Body):**
```json
{
  "email": "admin@doctorease.com",
  "password": "admin123"
}
```
**الاستجابة:** JWT Token للوصول للصفحات المحمية

### 2️⃣ المرضى (Patients)

#### الحصول على قائمة المرضى
```
GET /api/patient
```
**الوصف:** الحصول على قائمة جميع المرضى المتاحين
**الاستجابة:** قائمة من المرضى مع الـ IDs الخاصة بهم

#### الحصول على مريض محدد
```
GET /api/patient/{id}
```
**الوصف:** الحصول على معلومات مريض محدد بواسطة الـ ID
**المعاملات:** `id` - معرف المريض

### 3️⃣ الحجوزات (Bookings)

#### إنشاء حجز جديد (مع إنشاء مريض)
```
POST /api/bookings
```
**الوصف:** إنشاء حجز جديد لمريض (يُنشئ المريض تلقائياً إذا لم يكن موجوداً)
**المعاملات (JSON Body):**
```json
{
  "patientName": "أحمد محمد",
  "phone": "+966501234567",
  "date": "2025-01-20",
  "time": "10:30"
}
```

#### الحصول على قائمة الحجوزات
```
GET /api/bookings
```
**الوصف:** الحصول على قائمة جميع الحجوزات (للمدير)

#### تحديث حالة الحجز
```
PUT /api/bookings/{id}?status={status}
```
**الوصف:** تحديث حالة الحجز (للمدير)
**المعاملات:** 
- `id` - معرف الحجز
- `status` - الحالة الجديدة (Confirmed, Cancelled, Pending)

### 4️⃣ المقالات (Articles)

#### الحصول على قائمة المقالات
```
GET /api/articles
```
**الوصف:** الحصول على قائمة جميع المقالات الطبية (للزوار)

#### الحصول على مقال محدد
```
GET /api/articles/{id}
```
**الوصف:** الحصول على مقال محدد
**المعاملات:** `id` - معرف المقال

#### إنشاء مقال جديد
```
POST /api/articles
```
**الوصف:** إضافة مقال طبي جديد (للمدير)
**المعاملات (JSON Body):**
```json
{
  "title": "فوائد التمارين الرياضية",
  "category": "الصحة العامة",
  "content": "محتوى المقال...",
  "pictureUrl": "https://example.com/image.jpg"
}
```

#### تحديث مقال
```
PUT /api/articles/{id}
```
**الوصف:** تعديل مقال موجود (للمدير)
**المعاملات:** `id` - معرف المقال

#### حذف مقال
```
DELETE /api/articles/{id}
```
**الوصف:** حذف مقال (للمدير)
**المعاملات:** `id` - معرف المقال

### 5️⃣ التقييمات (Testimonials)

#### الحصول على قائمة التقييمات
```
GET /api/testimonials
```
**الوصف:** الحصول على قائمة جميع تقييمات المرضى (للزوار)

#### الحصول على تقييم محدد
```
GET /api/testimonials/{id}
```
**الوصف:** الحصول على تقييم محدد
**المعاملات:** `id` - معرف التقييم

#### إنشاء تقييم جديد
```
POST /api/testimonials
```
**الوصف:** إضافة تقييم جديد (للمرضى)
**المعاملات (JSON Body):**
```json
{
  "patientId": 1,
  "rating": 5,
  "message": "المعاملة ممتازة والخدمة رائعة"
}
```

#### حذف تقييم
```
DELETE /api/testimonials/{id}
```
**الوصف:** حذف تقييم غير مناسب (للمدير)
**المعاملات:** `id` - معرف التقييم

## 📝 أمثلة للاستخدام

### تسجيل دخول الأدمن
```bash
POST https://localhost:44371/api/auth/login
Content-Type: application/json

{
  "email": "admin@doctorease.com",
  "password": "admin123"
}
```

### إنشاء حجز جديد
```bash
POST https://localhost:44371/api/bookings
Content-Type: application/json

{
  "patientName": "أحمد محمد",
  "phone": "+966501234567",
  "date": "2025-01-20",
  "time": "10:30"
}
```

### إضافة مقال جديد
```bash
POST https://localhost:44371/api/articles
Content-Type: application/json

{
  "title": "فوائد التمارين الرياضية",
  "category": "الصحة العامة",
  "content": "التمارين الرياضية لها فوائد عديدة...",
  "pictureUrl": "https://example.com/exercise.jpg"
}
```

### إضافة تقييم
```bash
POST https://localhost:44371/api/testimonials
Content-Type: application/json

{
  "patientId": 1,
  "rating": 5,
  "message": "المعاملة ممتازة والخدمة رائعة"
}
```

## ⚠️ ملاحظات مهمة

1. **بيانات تسجيل الدخول:**
   - Email: `admin@doctorease.com`
   - Password: `admin123`

2. **التاريخ:** يجب أن يكون التاريخ في صيغة `YYYY-MM-DD`

3. **الوقت:** يجب أن يكون الوقت في صيغة `HH:MM`

4. **حالات الحجز:** القيم المتاحة هي `Pending`, `Confirmed`, `Cancelled`

5. **التقييم:** يجب أن يكون الرقم من 1 إلى 5

## 🔧 استكشاف الأخطاء

### خطأ 401 Unauthorized
- تأكد من تسجيل الدخول بشكل صحيح
- تحقق من صحة بيانات الاعتماد

### خطأ 400 Bad Request
- تحقق من صحة البيانات المرسلة
- تأكد من وجود جميع الحقول المطلوبة

### خطأ 404 Not Found
- تحقق من صحة الـ ID المستخدم
- تأكد من وجود البيانات المطلوبة

### خطأ 500 Internal Server Error
- راجع الـ logs للحصول على تفاصيل الخطأ
- تأكد من أن قاعدة البيانات تعمل بشكل صحيح

## 🎯 ملخص عملي سريع للحفظ

### ✅ POST = إضافة شيء جديد
- `/api/auth/login` - تسجيل دخول
- `/api/bookings` - حجز جديد
- `/api/articles` - مقال جديد
- `/api/testimonials` - تقييم جديد

### ✅ GET = عرض البيانات
- `/api/patient` - قائمة المرضى
- `/api/bookings` - قائمة الحجوزات
- `/api/articles` - قائمة المقالات
- `/api/testimonials` - قائمة التقييمات

### ✅ PUT = تعديل البيانات
- `/api/bookings/{id}` - تحديث حالة الحجز
- `/api/articles/{id}` - تعديل مقال

### ✅ DELETE = حذف البيانات
- `/api/articles/{id}` - حذف مقال
- `/api/testimonials/{id}` - حذف تقييم

### ✅ Public = أي حد يقدر يستخدمه
### ✅ Admin only = الأدمن فقط بعد تسجيل الدخول 