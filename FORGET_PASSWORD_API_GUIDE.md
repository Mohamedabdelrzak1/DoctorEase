# 🔑 دليل استخدام ميزة Forget Password - DoctorEase

## 📋 نظرة عامة
تم إضافة ميزة نسيان كلمة المرور للمشرفين في نظام DoctorEase. هذه الميزة تسمح للمشرفين بإعادة تعيين كلمة المرور عبر البريد الإلكتروني.

## 🚀 الميزات المضافة

### 1. ForgetPasswordDto
```csharp
public class ForgetPasswordDto
{
    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Invalid email address")]
    public string Email { get; set; } = string.Empty;
}
```

### 2. AdminAuthController - ForgetPassword Endpoint
- **Endpoint**: `POST /api/AdminAuth/forget-password`
- **الوظيفة**: إرسال رابط إعادة تعيين كلمة المرور عبر البريد الإلكتروني
- **الموقع**: `#region Forget Password`

### 3. AuthService - SendResetPasswordUrlAsync
- **الوظيفة**: إنشاء رابط إعادة تعيين كلمة المرور
- **الإرجاع**: رابط إعادة التعيين أو null إذا لم يتم العثور على المستخدم

## 📧 إعداد البريد الإلكتروني

### إعدادات البريد في appsettings.json
```json
{
  "MailSettings": {
    "Email": "your-email@gmail.com",
    "DisplayName": "DoctorEase",
    "Password": "your-app-password",
    "Host": "smtp.gmail.com",
    "Port": 587
  }
}
```

## 📡 استخدام API

### طلب إرسال رابط إعادة التعيين
```http
POST /api/AdminAuth/forget-password
Content-Type: application/json

{
  "email": "admin@doctorease.com"
}
```

### الاستجابة الناجحة
```json
{
  "message": "Reset password link has been sent to your email successfully.",
  "resetUrl": "http://doctorease.runasp.net/api/AdminAuth/reset-password?email=admin@doctorease.com&token=guid"
}
```

### الاستجابة عند عدم وجود المستخدم
```json
{
  "message": "Admin with this email does not exist."
}
```

## 📧 محتوى البريد الإلكتروني

سيتم إرسال بريد إلكتروني يحتوي على:
- **الموضوع**: "🔑 Reset Password Request - DoctorEase"
- **المحتوى**: رابط إعادة تعيين كلمة المرور مع تعليمات واضحة

## 🔧 التسجيل في DI Container

تم تسجيل الخدمات التالية:

```csharp
// في ApplicationServicesRegistration.cs
services.AddScoped<IMailingService, MailingService>();

// في InfrastructureServicesRegistration.cs
var mailSettings = configuration.GetSection("MailSettings").Get<MailSettings>();
services.AddSingleton(mailSettings);
```

## 📝 ملاحظات التطوير

### الملفات المضافة:
- `Shared/Dtos/Auth/ForgetPasswordDto.cs`

### الملفات المعدلة:
- `Core/Service/Service/AuthService.cs` - إضافة SendResetPasswordUrlAsync
- `Core/ServiceAbstraction/IAuthService.cs` - إضافة الدالة الجديدة
- `Infrastructure/Presentation/Controllers/AdminAuthController.cs` - إضافة #region Forget Password
- `Core/Service/ApplicationServicesRegistration.cs` - تسجيل IMailingService
- `Infrastructure/Persistence/InfrastructureServicesRegistration.cs` - إعداد MailSettings

## 🎯 الخطوات التالية

1. **اختبار الميزة** مع بريد إلكتروني صحيح
2. **تحسين الأمان** بإضافة Token management
3. **إضافة Rate Limiting** لمنع الإساءة
4. **تحسين UI** لعرض رسائل النجاح/الخطأ

## ✅ حالة البناء
```
dotnet build
✅ Build succeeded
```

---

**تم تطوير ميزة Forget Password بنجاح! 🎉** 